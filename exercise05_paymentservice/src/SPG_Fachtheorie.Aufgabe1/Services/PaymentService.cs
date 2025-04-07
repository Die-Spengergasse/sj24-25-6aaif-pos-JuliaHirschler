using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Commands;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPG_Fachtheorie.Aufgabe1.Services
{
    public class PaymentService
    {
        private readonly AppointmentContext _db;

        public PaymentService(AppointmentContext db)
        {
            _db = db;
        }

        public IQueryable<PaymentItem> PaymentItems => _db.PaymentItems.AsQueryable();
        public IQueryable<Payment> Payments => _db.Payments.AsQueryable();

        public Payment CreatePayment(NewPaymentCommand cmd)
        {
            DateTime paymentDateTime = DateTime.UtcNow;
            var cashDesk = _db.CashDesks.FirstOrDefault(c => c.Number == cmd.CashDeskNumber);
            if (cashDesk is null) throw new ArgumentException("Invalid cash desk", nameof(cmd.CashDeskNumber));
            var employee = _db.Employees.FirstOrDefault(e => e.RegistrationNumber == cmd.EmployeeRegistrationNumber);
            if (employee is null) throw new ArgumentException("Invalid employee", nameof(cmd.EmployeeRegistrationNumber));
            // Prüfung, ob offene Zahlung bereits existiert
            var existingPayment = _db.Payments.FirstOrDefault(p => p.CashDesk.Number == cmd.CashDeskNumber && p.Confirmed == null);
            if (existingPayment != null) throw new PaymentServiceException("Open payment for cashdesk.");
            
            if (!Enum.TryParse<PaymentType>(cmd.PaymentType, out var paymentType))
            {
                throw new ArgumentException("Invalid payment type", nameof(cmd.PaymentType));
            }
            // Überprüfung auf Zahlmethode und ob Angestellter Manager ist
            var checkManager = _db.Managers.FirstOrDefault(m => m.RegistrationNumber == cmd.EmployeeRegistrationNumber)
                ;
            if (paymentType == PaymentType.CreditCard && checkManager == null)
            {
                throw new PaymentServiceException("Insufficent rights to create a credit card payment");
            }
            var payment = new Payment(
                cashDesk, paymentDateTime, employee, paymentType);
            _db.Payments.Add(payment);
            SaveOrThrow();
            return payment;
        }

        public void ConfirmPayment(int paymentId)
        {
            var payment = _db.Payments.FirstOrDefault(p => p.Id == paymentId);
            if (payment is null)
                throw new ArgumentException("Payment not found");
            if (payment.Confirmed.HasValue)
                throw new ArgumentException("Payment already confirmed");
            payment.Confirmed = DateTime.UtcNow;
            SaveOrThrow();
        }

        public void AddPaymentItem (NewPaymentItemCommand cmd)
        {
            var payment = _db.Payments.FirstOrDefault(p => p.Id == cmd.Payment.Id);
            if (payment is null) throw new PaymentServiceException("Payment not found.");
            if (payment.Confirmed.HasValue) throw new PaymentServiceException("Payment already confirmed.");
            var paymentItem = new PaymentItem(cmd.ArticleName, cmd.Amount, cmd.Price, cmd.Payment);
            _db.PaymentItems.Add(paymentItem);
            SaveOrThrow();
        }

        public void DeletePayment (int paymentId, bool deleteItems)
        {
            var payment = _db.Payments.FirstOrDefault(p => p.Id == paymentId);
            if (payment is null) return;
            var paymentItems = _db.PaymentItems.Where(p => p.Payment.Id == paymentId).ToList();
            if (paymentItems.Any() && deleteItems)
            {
                try
                {
                    _db.PaymentItems.RemoveRange(paymentItems);
                    _db.SaveChanges();
                }
                catch (DbUpdateException e)
                {
                    throw new PaymentServiceException(e.InnerException?.Message ?? e.Message);
                }
                catch (InvalidOperationException e)
                {
                    throw new PaymentServiceException(
                        e.InnerException?.Message ?? e.Message);
                }
            }
            try
            {
                _db.Payments.Remove(payment);
                _db.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new PaymentServiceException(e.InnerException?.Message ?? e.Message);
            }
            catch (InvalidOperationException e)
            {
                throw new PaymentServiceException(
                    e.InnerException?.Message ?? e.Message);
            }
        }

        private void SaveOrThrow()
        {
            try
            {
                _db.SaveChanges();  // INSERT INTO
            }
            catch (DbUpdateException e)
            {
                throw new PaymentServiceException(e.InnerException?.Message ?? e.Message);
            }
        }
    }
}
