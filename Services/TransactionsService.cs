using Microsoft.AspNetCore.Identity;
using budget_tracker.Models;
using Microsoft.AspNet.Identity;

namespace budget_tracker.Services
{
    public class TransactionsService : ITransactionsService
    {
        private readonly BudgetTrackerDbContext budgetTrackerDbContext;
        private readonly IConfiguration _configuration;
        PasswordHasher passwordHasher = new PasswordHasher();

        public TransactionsService(BudgetTrackerDbContext _budgetTrackerDbContext, IConfiguration configuration)
        {
            budgetTrackerDbContext = _budgetTrackerDbContext;
            _configuration = configuration;
        }

        //Transactions
        public bool SaveTransaction(general_ledger transactions)
        {
            general_ledger _trasaction = new general_ledger()
            {
            };

            _trasaction = transactions;

            try
            {
                var res = budgetTrackerDbContext.Add<general_ledger>(transactions);
                budgetTrackerDbContext.SaveChanges();

                Console.WriteLine("Transaction Saved ->  Account: "+transactions.Account_fk.ToString()+ "Amount: " + transactions.Amount.ToString());    

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
