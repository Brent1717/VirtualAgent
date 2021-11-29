using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using VirtualAgent.Models;
using VirtualAgent.Repository.AccountRepositories;
using VirtualAgent.Repository.PersonRepositories;
using VirtualAgent.Repository.TransactionRepositories;

namespace VirtualAgent.Controllers
{
    public class TransactionController : Controller
    {

        private readonly IAccountRepository _accountRepository;
        private readonly IPersonRepository _personRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(IAccountRepository accountRepository, IPersonRepository personRepository, ITransactionRepository transactionRepository, ILogger<TransactionController> logger)
        {
            _accountRepository = accountRepository;
            _personRepository = personRepository;
            _transactionRepository = transactionRepository;
            _logger = logger;
        }


        // GET: Transaction/Details/5
        public ActionResult Details(int? code)
        {
            if (code == null)
            {
                return BadRequest();
            }

            var transaction = _transactionRepository.GetTransactionByCode(code);

            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transaction/Create
        public ActionResult Create(int code)
        {
            var transaction = new Transaction
            {
                AccountCode = code
            };

            return View(transaction);
        }

        // POST: Transaction/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Transaction transaction)
        {
            if (transaction.Amount == 0)
            {
                ViewBag.ErrorMessage = "Cannot enter a transaction amount of 0";
                return RedirectToAction("Create", new { code = transaction.AccountCode, saveChangesError = true });
            }

            transaction.Code = 0;
            transaction.CaptureDate = DateTime.Now;

            var account = _accountRepository.GetAccountByCode(transaction.AccountCode);
            account.OutstandingBalance += transaction.Amount;

            if (TryUpdateModelAsync(account, "", a => a.OutstandingBalance).Result)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        _transactionRepository.InsertTransaction(transaction);
                        _transactionRepository.Save();

                        return RedirectToAction("Details", "Account", new { code = transaction.AccountCode });
                    }
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error
                    ViewBag.ErrorMessage = "test error";
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }

            ViewBag.ErrorMessage = "Invalid entry";

            return RedirectToAction("Create", "Transaction", new { code = transaction.AccountCode });
        }

        // GET: Transaction/Edit/5
        public ActionResult Edit(int? code, decimal amount)
        {
            if (code == null)
            {
                return BadRequest();
            }

            var transaction = _transactionRepository.GetTransactionByCode(code);

            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transaction/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? code, decimal amount)
        {
            if (code == null)
            {
                return BadRequest();
            }

            var transaction = _transactionRepository.GetTransactionByCode(code);

            transaction.CaptureDate = DateTime.Now;

            if (transaction.Amount == 0)
            {
                ViewBag.ErrorMessage = "Cannot enter a transaction amount of 0";
                return RedirectToAction("Create", new { code = transaction.AccountCode });
            }

            var account = _accountRepository.GetAccountByCode(transaction.AccountCode);


            if (TryUpdateModelAsync(account, "", a => a.OutstandingBalance).Result && TryUpdateModelAsync(transaction).Result)
            {
                try
                {
                    account.OutstandingBalance = _transactionRepository.GetTransactions()
                        .Where(t => t.AccountCode == transaction.AccountCode)
                        .Sum(t => t.Amount);

                    _transactionRepository.Save();

                    return RedirectToAction("Details", "Account", new { code = transaction.AccountCode });
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(transaction);
        }
    }
}