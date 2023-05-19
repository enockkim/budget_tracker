using budget_tracker.Models;
using Microsoft.AspNet.Identity;

namespace budget_tracker.Services
{
    public class FeePaymentService : IFeePaymentService
    {
        private readonly BudgetTrackerDbContext budgetTrackerDbContext;
        private readonly IConfiguration _configuration;
        PasswordHasher passwordHasher = new PasswordHasher();

        public FeePaymentService(BudgetTrackerDbContext _budgetTrackerDbContext, IConfiguration configuration)
        {
            budgetTrackerDbContext = _budgetTrackerDbContext;
            _configuration = configuration;
        }

        //Transactions
        public bool SaveFeePayment(fee_payment fee_payment)
        {   
            try
            {
                var res = budgetTrackerDbContext.Add<fee_payment>(fee_payment);
                var result = budgetTrackerDbContext.SaveChanges();
                                
                Console.WriteLine("Fee payment saved ->  Account/Index: "+fee_payment.AccountNo);    


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
