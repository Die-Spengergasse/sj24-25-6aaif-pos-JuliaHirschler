using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public abstract class Employee
    {
#pragma warning disable CS8618
        protected Employee() { }
#pragma warning disable CS8618
        public Employee(
            int registrationNumber, string firstName, string lastName,
            Address? address)
        {
            RegistrationNumber = registrationNumber;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RegistrationNumber { get; set; } 
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public Address? Address { get; set; }

        public string Type { get; set; } = null!;
    }
}