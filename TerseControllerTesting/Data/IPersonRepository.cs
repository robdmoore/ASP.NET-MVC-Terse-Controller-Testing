using System.Collections.Generic;
using TerseControllerTesting.Models;

namespace TerseControllerTesting.Data
{
    public interface IPersonRepository
    {
        Person GetById(int id);
        IEnumerable<Person> GetAll();
        void Save(Person person);
        bool EmailBelongsToSomeoneElse(string emailAddress);
        bool EmailBelongsToSomeoneElse(string emailAddress, int idOfExistingPerson);
    }
}