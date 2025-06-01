using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Address
    {

        public Address(int id, string street, string city, string zip) 
        {
            Id = id;
            Street = street;
            Zip = zip;
            City = city;
        }
        public int Id { get; set; }
        [MaxLength(255)]
        public string Street { get; set; }
        [MaxLength(255)]
        public string Zip { get; set; }
        [MaxLength(255)]
        public string City { get; set; }
    }
}