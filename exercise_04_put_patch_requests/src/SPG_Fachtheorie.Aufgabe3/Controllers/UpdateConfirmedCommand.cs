namespace SPG_Fachtheorie.Aufgabe3.Controllers
{
    using System.ComponentModel.DataAnnotations;
    using System;

    public class UpdateConfirmedCommand : IValidatableObject
    {
        [Required]
        public DateTime Confirmed { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Confirmed > DateTime.Now.AddMinutes(1))
            {
                yield return new ValidationResult("Das Bestätigungsdatum darf maximal 1 Minute in der Zukunft liegen.");
            }
        }
    }

}