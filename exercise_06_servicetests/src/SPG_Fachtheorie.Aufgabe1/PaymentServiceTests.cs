using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using SPG_Fachtheorie.Aufgabe1.Services;
using SPG_Fachtheorie.Aufgabe1.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SPG_Fachtheorie.Aufgabe1.Test
{
    [Collection("Sequential")]
    public class PaymentServiceTests
    {
        private AppointmentContext GetEmptyDbContext()
        {
            var options = new DbContextOptionsBuilder<AppointmentContext>()
                .UseSqlite("Data Source=cash.db")
                .Options;
            var db = new AppointmentContext(options);
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            return db;
        }

        [Theory]
        [InlineData(0, "Cash", 1, "Cash desk not found")]
        [InlineData(9999, "Cash", 1, "Cash desk not found")]
        [InlineData(1, "Invalid Type", 1, "Invalid payment type")]
        [InlineData(1, "Cash", 0, "Employee not found")]
        [InlineData(1, "Cash", 999, "Employee not found")]
        [InlineData(1, "CreditCard", 1, "Insufficent rights to create a credit card payment")]
        public void CreatePaymentExceptionsTest(int cashDeskNum, string paymentType, int employeeNum, string errorMessage)
        {
            using var db = GetEmptyDbContext();
            db.CashDesks.Add(new CashDesk(1));
            db.Employees.Add(new Cashier(1, "D1", "J1", new DateOnly(1990, 1, 1), 5000, null, "General"));
            db.Employees.Add(new Manager(2, "D2", "J2", new DateOnly(2000, 1, 1), 4000, null, "Manager"));
            db.SaveChanges();

            var service = new PaymentService(db);

            var ex = Assert.Throws<PaymentServiceException>(() =>
                service.CreatePayment(new NewPaymentCommand(cashDeskNum, paymentType, employeeNum)));
            Assert.Equal(errorMessage, ex.Message);
        }

        [Fact]
        public void CreatePaymentSuccessTest()
        {
            using var db = GetEmptyDbContext();
            db.Employees.Add(new Cashier(1001, "fn", "ln", new DateOnly(2000, 1, 1), 3000, null, "Kassier"));
            db.CashDesks.Add(new CashDesk(1));
            db.SaveChanges();

            var service = new PaymentService(db);

            var payment = service.CreatePayment(new NewPaymentCommand(1, PaymentType.Cash.ToString(), 1001));
            Assert.Equal(1001, payment.Employee.RegistrationNumber);
            Assert.Equal(1, payment.CashDesk.Number);
            Assert.Equal(PaymentType.Cash, payment.PaymentType);
        }

        [Theory]
        [InlineData(9999, "Payment not found")]
        [InlineData(1, "Payment already confirmed")]
        public void ConfirmPaymentExceptionsTest(int paymentId, string errorMessage)
        {
            using var db = GetEmptyDbContext();
            db.CashDesks.Add(new CashDesk(1));
            db.Employees.Add(new Cashier(1, "D1", "J1", new DateOnly(1990, 1, 1), 5000, null, "General"));
            db.Payments.Add(new Payment(db.CashDesks.First(), DateTime.UtcNow, db.Employees.First(), PaymentType.Cash)
            {
                Id = 1,
                Confirmed = paymentId == 1 ? DateTime.UtcNow : null
            });
            db.SaveChanges();

            var service = new PaymentService(db);

            var ex = Assert.Throws<PaymentServiceException>(() => service.ConfirmPayment(paymentId));
            Assert.Equal(errorMessage, ex.Message);
        }

        [Fact]
        public void ConfirmPaymentSuccessTest()
        {
            using var db = GetEmptyDbContext();
            db.Employees.Add(new Cashier(1001, "fn", "ln", new DateOnly(2000, 1, 1), 3000, null, "Kassier"));
            db.CashDesks.Add(new CashDesk(1));
            db.Payments.Add(new Payment(db.CashDesks.First(), DateTime.UtcNow, db.Employees.First(), PaymentType.Cash)
            {
                Id = 1,
                Confirmed = null
            });
            db.SaveChanges();

            var service = new PaymentService(db);

            service.ConfirmPayment(1);

            var updated = db.Payments.First(p => p.Id == 1);
            Assert.NotNull(updated.Confirmed);
            Assert.True(updated.Confirmed <= DateTime.UtcNow);
        }

        [Theory]
        [InlineData("name", 5, 30.0, 9999, "Payment not found")]
        [InlineData("name", 5, 30.0, 1, "Payment not confirmed")]
        public void AddPaymentItemExceptionsTest(string articleName, int amount, decimal price, int paymentId, string errorMessage)
        {
            using var db = GetEmptyDbContext();
            db.CashDesks.Add(new CashDesk(1));
            db.Employees.Add(new Cashier(1001, "fn", "ln", new DateOnly(2000, 1, 1), 3000, null, "Kassier"));
            db.Payments.Add(new Payment(db.CashDesks.First(), DateTime.UtcNow, db.Employees.First(), PaymentType.Cash)
            {
                Id = 1
            });
            db.SaveChanges();

            var service = new PaymentService(db);

            var ex = Assert.Throws<PaymentServiceException>(() =>
                service.AddPaymentItem(new NewPaymentItemCommand(articleName, amount, price, paymentId)));
            Assert.Equal(errorMessage, ex.Message);
        }

        [Fact]
        public void AddPaymentItemSuccessTest()
        {
            using var db = GetEmptyDbContext();
            db.CashDesks.Add(new CashDesk(1));
            db.Employees.Add(new Cashier(1001, "fn", "ln", new DateOnly(2000, 1, 1), 3000, null, "Kassier"));
            db.Payments.Add(new Payment(db.CashDesks.First(), DateTime.UtcNow, db.Employees.First(), PaymentType.Cash)
            {
                Id = 1,
                Confirmed = DateTime.UtcNow
            });
            db.SaveChanges();

            var service = new PaymentService(db);

            var cmd = new NewPaymentItemCommand("name", 20, 30.0m, 1);
            service.AddPaymentItem(cmd);

            var item = db.PaymentItems.FirstOrDefault();
            Assert.NotNull(item);
            Assert.Equal("name", item.ArticleName);
            Assert.Equal(20, item.Amount);
            Assert.Equal(30.0m, item.Price);
            Assert.Equal(1, item.Payment.Id);
        }

        [Theory]
        [InlineData(999, "Payment not found")]
        public void DeletePaymentExceptionsTest(int paymentId, string errorMessage)
        {
            using var db = GetEmptyDbContext();
            db.CashDesks.Add(new CashDesk(1));
            db.Employees.Add(new Cashier(1001, "fn", "ln", new DateOnly(2000, 1, 1), 3000, null, "Kassier"));
            db.Payments.Add(new Payment(db.CashDesks.First(), DateTime.UtcNow, db.Employees.First(), PaymentType.Cash)
            {
                Id = 1
            });
            db.SaveChanges();

            var service = new PaymentService(db);

            var ex = Assert.Throws<PaymentServiceException>(() => service.DeletePayment(paymentId, true));
            Assert.Equal(errorMessage, ex.Message);
        }

        [Fact]
        public void DeletePaymentSuccessTest()
        {
            using var db = GetEmptyDbContext();
            db.CashDesks.Add(new CashDesk(1));
            db.Employees.Add(new Cashier(1001, "fn", "ln", new DateOnly(2000, 1, 1), 3000, null, "Kassier"));
            db.Payments.Add(new Payment(db.CashDesks.First(), DateTime.UtcNow, db.Employees.First(), PaymentType.Cash)
            {
                Id = 1001
            });
            db.SaveChanges();

            var service = new PaymentService(db);

            service.DeletePayment(1001, true);

            Assert.Null(db.Payments.FirstOrDefault(p => p.Id == 1001));
            Assert.Null(db.PaymentItems.FirstOrDefault(p => p.Payment.Id == 1001));
        }
    }
}
