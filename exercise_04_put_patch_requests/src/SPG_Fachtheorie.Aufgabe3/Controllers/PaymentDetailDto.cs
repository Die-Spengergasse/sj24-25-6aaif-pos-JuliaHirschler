
namespace SPG_Fachtheorie.Aufgabe3.Controllers
{
    public class PaymentDetailDto
    {
        private int id;
        private string firstName;
        private string lastName;
        private int number;
        private string v;
        private List<PaymentItemDto> paymentItemDtos;

        public PaymentDetailDto(int id, string firstName, string lastName, int number, string v, List<PaymentItemDto> paymentItemDtos)
        {
            this.id = id;
            this.firstName = firstName;
            this.lastName = lastName;
            this.number = number;
            this.v = v;
            this.paymentItemDtos = paymentItemDtos;
        }
    }
}