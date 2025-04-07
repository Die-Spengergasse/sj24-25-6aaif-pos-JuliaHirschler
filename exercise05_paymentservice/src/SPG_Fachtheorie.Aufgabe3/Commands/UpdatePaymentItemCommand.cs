using SPG_Fachtheorie.Aufgabe1.Model;
using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe3.Commands
{
    public record UpdatePaymentItemCommand(
    [Range(1, int.MaxValue, ErrorMessage = "Invalid ID")]
    int Id,
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Invalid Article Name")]
    string ArticleName,
    [Range(1, 999999, ErrorMessage = "Invalid Amount")]
    int Amount,
    [Range(1, 1_000_000, ErrorMessage = "Invalid Price")]
    decimal Price,
    [Range(1, 999999, ErrorMessage = "Invalid Payment ID")]
    Payment Payment,
    DateTime? LastUpdated
        ) : IValidatableObject

    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        // Validate: Method called to validate the object
        // ValidationContext: Provides context about the object being validated
        {
            if (LastUpdated > Payment.PaymentDateTime)
            {
                yield return new ValidationResult("Date for update cannot be more than inital Payment", new[] { nameof(LastUpdated) });
            }
        }
    }
}
