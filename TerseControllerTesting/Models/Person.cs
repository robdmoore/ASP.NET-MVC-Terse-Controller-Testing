using System.ComponentModel.DataAnnotations;
using TerseControllerTesting.Utils;

namespace TerseControllerTesting.Models
{
    public class Person
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required, Email]
        public string EmailAddress { get; set; }
    }
}