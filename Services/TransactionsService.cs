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
        public Tuple<bool, int> SaveTransaction(general_ledger transactions)
        {   
            try
            {
                var res = budgetTrackerDbContext.Add<general_ledger>(transactions);
                var result = budgetTrackerDbContext.SaveChanges();

                Console.WriteLine("Transaction Saved ->  Account: "+transactions.Account_fk.ToString()+ "Amount: " + transactions.Amount.ToString());    

                return Tuple.Create(true, transactions.TransactionId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Tuple.Create(false, 0);
            }
        }

        public TransactionResult CheckTransaction(string transactionId)
        {
            // Fetch transaction from the database
            var transaction = budgetTrackerDbContext.general_ledger
                .FirstOrDefault(t => t.Refrence == transactionId);

            if (transaction == null)
            {
                return new TransactionResult
                {
                    TransactionId = transactionId,
                    Status = TransactionStatus.NotFound,
                    Message = "Transaction not found."
                };
            }

            return new TransactionResult
            {
                TransactionId = transaction.Refrence,
                Status = TransactionStatus.Success,
                Message = $"Transaction is successful."
            };
        }
    }
}
