using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VirtualAgent.Models;
using VirtualAgent.ViewModels;
using X.PagedList;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using VirtualAgent.Repository.PersonRepositories;
using VirtualAgent.Repository.AccountRepositories;

namespace VirtualAgent.Controllers
{
    public class PersonsController : Controller
    {

        private readonly IAccountRepository _accountRepository;
        private readonly IPersonRepository _personRepository;
        private readonly ILogger<PersonsController> _logger;

        public PersonsController(IAccountRepository accountRepository, IPersonRepository personRepository, ILogger<PersonsController> logger)
        {
            _accountRepository = accountRepository;
            _personRepository = personRepository;
            _logger = logger;
        }

        public IActionResult Index(string currentFilter, string searchString, int? page)
        {
            var persons = _personRepository.GetPersons();

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                persons = persons.Where(s => s.Name.ToLower().Contains(searchString.ToLower())
                                        || s.Surname.ToLower().Contains(searchString.ToLower())).ToList();
            }

            ViewBag.CurrentFilter = searchString;

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(persons.ToPagedList(pageNumber, pageSize));
        }

        // GET: Persons/Details/5
        public ActionResult Details(int? code)
        {
            if (code == null)
            {
                return BadRequest();
            }

            var person = _personRepository.GetPersonByCode(code);

            if (person == null)
            {
                return NotFound();
            }

            var accounts = _accountRepository.GetAccounts().Where(a => a.PersonCode == person.Code);
            var personAccount = new PersonAccountsViewModel
            {
                Person = person,
                Accounts = accounts
            };

            return View(personAccount);
        }

        // GET: Person/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Person/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Person person)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _personRepository.InsertPerson(person);
                    _personRepository.Save();

                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(person);
        }

        // GET: Persons/Edit/5
        public ActionResult Edit(int? code)
        {
            if (code == null)
            {
                return BadRequest();
            }

            var person = _personRepository.GetPersonByCode(code);

            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }

        // POST: Persons/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? code)
        {
            if (code == null)
            {
                return BadRequest();
            }

            var person = _personRepository.GetPersonByCode(code);

            if (TryUpdateModelAsync(person, "").Result)
            {
                try
                {
                    _personRepository.Save();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(person);
        }

        // GET: Persons/Delete/5
        public ActionResult Delete(int? code, bool? saveChangesError = false)
        {
            if (code == null)
            {
                return BadRequest();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }

            var person = _personRepository.GetPersonByCode(code);

            if (person == null)
            {
                return NotFound();
            }

            var accounts = _accountRepository.GetAccounts().Where(a => a.PersonCode == person.Code);

            if (accounts.Count() > 0)
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, person still has active accounts.";
            }

            return View(person);
        }

        // POST: Persons/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int code)
        {
            try
            {
                var person = _personRepository.GetPersonByCode(code);
                var accounts = _accountRepository.GetAccounts().Where(a => a.PersonCode == person.Code);

                if (accounts.Count() > 0)
                {
                    ViewBag.ErrorMessage = "Delete failed. Try again, person still has active accounts.";
                    return RedirectToAction("Delete", new { code = code, saveChangesError = true });
                }

                _personRepository.DeletePerson(person.Code);
                _personRepository.Save();
            }
            catch (RetryLimitExceededException/* dex */)
            {
                //Log the error
                return RedirectToAction("Delete", new { code = code, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }
    }
}