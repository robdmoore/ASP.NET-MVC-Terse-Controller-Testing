using System;
using System.Collections.Generic;
using System.Linq;
using TerseControllerTesting.Models;

namespace TerseControllerTesting.Data
{
    public class InMemoryPersonRepository : IPersonRepository
    {
        // Instead of actually connecting to a persistent database, this class will use
        //  a static variable to create an in memory database
        private static List<Person> _database;

        public InMemoryPersonRepository(List<Person> personDatabase)
        {
            _database = personDatabase;
        }

        public Person GetById(int id)
        {
            return _database.Where(p => p.Id == id).FirstOrDefault();
        }

        public IEnumerable<Person> GetAll()
        {
            return _database;
        }

        public void Save(Person person)
        {
            if (person.Id != 0)
            {
                var existingPerson = GetById(person.Id);

                if (existingPerson == null)
                    throw new ApplicationException(string.Format("Attempt to save person {0} who didn't exist.", person.Id));

                existingPerson.FirstName = person.FirstName;
                existingPerson.LastName = person.LastName;
                existingPerson.EmailAddress = person.EmailAddress;
            }
            else
            {
                lock (_database)
                {
                    person.Id = _database.Count + 1;
                    _database.Add(person);
                }
            }

        }

        public bool EmailExists(string emailAddress)
        {
            return _database.Any(p => p.EmailAddress == emailAddress);
        }

        public bool EmailExists(string emailAddress, int idOfExistingPerson)
        {
            return _database.Any(p => p.EmailAddress == emailAddress && p.Id != idOfExistingPerson);
        }
    }
}