using System.Collections.Generic;
using VirtualAgent.Models;

namespace VirtualAgent.Repository.PersonRepositories
{
    public interface IPersonRepository
    {
        IEnumerable<Person> GetPersons();
        Person GetPersonByCode(int? code);
        void DeletePerson(int code);
        void InsertPerson(Person person);
        void Save();   
    }
}