using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using VirtualAgent.Models;
using VirtualAgent.Repository.AccountRepositories;
using VirtualAgent.Repository.PersonRepositories;
using VirtualAgent.Repository.TransactionRepositories;
using VirtualAgent.ViewModels;

namespace VirtualAgent.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IPersonRepository _personRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountRepository accountRepository, IPersonRepository personRepository, ITransactionRepository transactionRepository, ILogger<AccountController> logger)
        {
            _accountRepository = accountRepository;
            _personRepository = personRepository;
            _transactionRepository = transactionRepository;
            _logger = logger;
        }


        // GET: Account/Details/5
        public ActionResult Details(int? code)
        {
            if (code == null)
            {
                return BadRequest();
            }

            var account = _accountRepository.GetAccountByCode(code);

            if (account == null)
            {
                return NotFound();
            }

            var mymodel = new AccountTranctionsViewModel();
            mymodel.Account = account;
            mymodel.Transactions = _transactionRepository.GetTransactions().Where(t => t.AccountCode == account.Code);

            return View(mymodel);
        }

        // GET: Account/Create
        public ActionResult Create(int code)
        {
            var account = new Account
            {
                PersonCode = code
            };

            return View(account);
        }

        // POST: Account/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Account account)
        {
            var checkAccount = _accountRepository.GetAccounts().Any(a => a.AccountNumber == account.AccountNumber);
            var person = _personRepository.GetPersons().Any(a => a.Code == account.PersonCode);

            if (checkAccount)
            {
                ViewBag.ErrorMessage = "Account number already exsists";
                return RedirectToAction("Create", new { code = account.PersonCode });
            }

            if (!person)
            {
                ViewBag.ErrorMessage = "Person does not exsists";
                return RedirectToAction("Create", new { code = account.PersonCode });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    account.Code = 0;
                    _accountRepository.InsertAccount(account);
                    _accountRepository.Save();
                    return RedirectToAction("Details", "Persons", new { code = account.PersonCode });
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(account);
        }

        // GET: Account/Edit/5
        public ActionResult Edit(int? code)
        {
            if (code == null)
            {
                return BadRequest();
            }

            var account = _accountRepository.GetAccountByCode(code);

            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Account/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? code)
        {
            if (code == null)
            {
                return BadRequest();
            }

            var account = _accountRepository.GetAccountByCode(code);

            if (TryUpdateModelAsync(account).Result)
            {
                try
                {
                    _accountRepository.Save();

                    return RedirectToAction("Details", "Persons", new { code = account.PersonCode });
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            return View(account);
        }
    }
}