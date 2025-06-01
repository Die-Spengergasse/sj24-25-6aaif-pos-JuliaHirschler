using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Cashier : Employee
    {
#pragma warning disable CS8618
        protected Cashier() { }
#pragma warning disable CS8618

        public Cashier(int registrationNumber, string firstName, string lastName,
            Address? address, string jobSpezialisation) : base(registrationNumber, firstName, lastName, address)
        {
            JobSpezialisation = jobSpezialisation;
        }

        [MaxLength(255)]
        public string JobSpezialisation { get; set; }
    }
}