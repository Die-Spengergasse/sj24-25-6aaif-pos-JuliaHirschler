namespace SPG_Fachtheorie.Aufgabe3.Controllers
{
    internal class PaymentItemDto
    {
        private string articleName;
        private int amount;
        private decimal price;

        public PaymentItemDto(string articleName, int amount, decimal price)
        {
            this.articleName = articleName;
            this.amount = amount;
            this.price = price;
        }
    }
}