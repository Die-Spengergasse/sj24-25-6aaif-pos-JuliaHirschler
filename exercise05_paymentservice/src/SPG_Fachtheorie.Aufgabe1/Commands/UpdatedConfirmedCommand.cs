using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Commands
{
    public record UpdatedConfirmedCommand
    (
            [Range(1, 999999, ErrorMessage = "Invalid cash desk numbner")]
                int CashDeskNumber,
                DateTime PaymentDateTime,
            [StringLength(255, MinimumLength = 1, ErrorMessage = "Invalid payment type")]
                string PaymentType,
            [Range(1, 999999, ErrorMessage = "Invalid registration number")]
                int EmployeeRegistrationNumber,
                DateTime Confirmed
        ) : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        // Validate: Method called to validate the object
        // ValidationContext: Provides context about the object being validated
        {
            if (Confirmed > DateTime.Now.Date)
            {
                yield return new ValidationResult("Payment date cannot be in the future", new[] { nameof(Confirmed) });
            }
            if (Confirmed > PaymentDateTime)
            {
                yield return new ValidationResult("Date for update cannot be more than inital Payment", new[] { nameof(Confirmed) });
            }
        }

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
