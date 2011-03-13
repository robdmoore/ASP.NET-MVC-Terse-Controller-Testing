using System.Collections.Generic;
using TerseControllerTesting.Models;

namespace TerseControllerTesting.Data
{
    public interface IPersonRepository
    {
        Person GetById(int id);
        IEnumerable<Person> GetAll();
        void Save(Person person);
        bool EmailExists(string emailAddress);
        bool EmailExists(string emailAddress, int idOfExistingPerson);
    }
}