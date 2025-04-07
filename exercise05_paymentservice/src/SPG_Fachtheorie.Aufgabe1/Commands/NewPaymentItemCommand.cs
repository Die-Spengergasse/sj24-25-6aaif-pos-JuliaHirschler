using SPG_Fachtheorie.Aufgabe1.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPG_Fachtheorie.Aufgabe1.Commands
{
    public record NewPaymentItemCommand(
    [Required(ErrorMessage = "ArticleName is required.")]
    [StringLength(100, ErrorMessage = "ArticleName cannot exceed 100 characters.")]
    string ArticleName,

    [Range(1, int.MaxValue, ErrorMessage = "Amount must be at least 1.")]
    int Amount,

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    decimal Price,

    [Range(1, int.MaxValue, ErrorMessage = "PaymentId must be a positive integer.")]
    Payment Payment
    );
}
