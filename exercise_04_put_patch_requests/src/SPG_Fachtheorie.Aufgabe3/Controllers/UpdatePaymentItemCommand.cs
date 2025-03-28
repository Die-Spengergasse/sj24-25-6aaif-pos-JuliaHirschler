namespace SPG_Fachtheorie.Aufgabe3.Controllers
{
    using System.ComponentModel.DataAnnotations;

    public class UpdatePaymentItemCommand
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string ArticleName { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Amount { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public double Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int PaymentId { get; set; }

        public DateTime? LastUpdated { get; set; }
    }


}