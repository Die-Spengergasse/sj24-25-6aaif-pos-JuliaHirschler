
namespace SPG_Fachtheorie.Aufgabe3.Controllers
{
    public class NewPaymentCommand
    {
        public int EmployeeRegistrationNumber { get; internal set; }
        public int CashDeskNumber { get; internal set; }
        public DateTime PaymentDateTime { get; internal set; }
        public ReadOnlySpan<char> PaymentType { get; internal set; }
    }
}