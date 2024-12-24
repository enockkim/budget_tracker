using budget_tracker.Controllers;
using budget_tracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using Prema.ShuleOne.Web.Server.Database;

namespace budget_tracker.Services
{
    public class FeePaymentService : IFeePaymentService
    {
        private readonly BudgetTrackerDbContext budgetTrackerDbContext;
        private readonly ShuleOneDatabaseContext shuleOneDatabaseContext;
        private readonly IConfiguration _configuration;
        PasswordHasher passwordHasher = new PasswordHasher();
        private readonly MobileSasaBulkSms bulkSms;
        private readonly ILogger _logger;

        public FeePaymentService(ILogger<FeePaymentService> logger, BudgetTrackerDbContext _budgetTrackerDbContext, ShuleOneDatabaseContext _shuleOneDatabaseContext, IConfiguration configuration, MobileSasaBulkSms _bulkSms)
        {
            _logger = logger;
            budgetTrackerDbContext = _budgetTrackerDbContext;
            shuleOneDatabaseContext = _shuleOneDatabaseContext;
            _configuration = configuration;
            bulkSms = _bulkSms;
        }

        //Transactions
        public async Task<bool> SaveFeePayment(fee_payment fee_payment, string account, string amount)
        {   
            try
            {
                var res = budgetTrackerDbContext.Add<fee_payment>(fee_payment);

                if (int.TryParse(account, out int admissionNumber))
                {
                    var student = await shuleOneDatabaseContext.Student.AsNoTracking()
                        .FirstOrDefaultAsync(student => student.id == admissionNumber);

                    if(student != null)
                    {
                        var studentContact = await shuleOneDatabaseContext.StudentContact.AsNoTracking()
                            .FirstOrDefaultAsync(studentContact => studentContact.fk_student_id == student.id && studentContact.contact_priority == 1);
                        
                        string message = $"Hello {studentContact.surname}, your fee payment of Ksh. {amount} for {student.other_names} has been recieved successfully. Thank you! For fee inquires, please contact the school bursar.";
                        await bulkSms.SendSms(studentContact.phone_number, message);

                        _logger.LogWarning($"Message sent: {message}");
                    }
                    else
                    {
                        _logger.LogWarning($"Missing student record or invalid admission number: {account}");
                    }
                } else
                {
                    _logger.LogWarning($"Invalid account/admission number: {account}");
                }


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

        public static bool IsValidAdmissionNumber(string account)
        {
            return int.TryParse(account, out _);
        }

    }
}
