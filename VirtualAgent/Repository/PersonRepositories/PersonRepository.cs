using System.Collections.Generic;
using System.Linq;
using VirtualAgent.Data;
using VirtualAgent.Models;

namespace VirtualAgent.Repository.PersonRepositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly DBStoreContext _context;

        public PersonRepository(DBStoreContext context)
        {
            _context = context;
        }

        public IEnumerable<Person> GetPersons()
        {
            return _context.Persons.ToList();
        }

        public Person GetPersonByCode(int? code)
        {
            return _context.Persons.Find(code);
        }

        public void InsertPerson(Person person)
        {
            _context.Persons.Add(person);
        }

        public void DeletePerson(int code)
        {
            var person = _context.Persons.Find(code);
            _context.Persons.Remove(person);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}