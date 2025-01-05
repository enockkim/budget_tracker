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
        private readonly MobileSasaBulkSms bulkSms;
        private readonly Logging logging;

        public FeePaymentService(Logging logging, BudgetTrackerDbContext _budgetTrackerDbContext, ShuleOneDatabaseContext _shuleOneDatabaseContext, IConfiguration configuration, MobileSasaBulkSms _bulkSms)
        {
            this.logging = logging;
            budgetTrackerDbContext = _budgetTrackerDbContext;
            shuleOneDatabaseContext = _shuleOneDatabaseContext;
            _configuration = configuration;
            bulkSms = _bulkSms;
        }

        public async Task<bool> SaveFeePayment(fee_payment fee_payment, string account, string amount)
        {
            try
            {
                // Add the fee payment to the BudgetTrackerDbContext
                budgetTrackerDbContext.Add(fee_payment);

                if (int.TryParse(account, out int admissionNumber))
                {
                    // Fetch the student record from the ShuleOneDatabaseContext
                    var student = await shuleOneDatabaseContext.Student
                        .FirstOrDefaultAsync(s => s.id == admissionNumber);

                    if (student != null)
                    {
                        // Fetch the primary contact for the student
                        var studentContact = await shuleOneDatabaseContext.StudentContact
                            .FirstOrDefaultAsync(c => c.fk_student_id == student.id && c.contact_priority == 1);

                        if (studentContact != null)
                        {
                            // Send SMS notification
                            string message = $"Hello {studentContact.surname}, your fee payment of Ksh. {amount} for {student.other_names} has been received successfully. Thank you! For fee inquiries, please contact the school bursar.";
                            await bulkSms.SendSms(studentContact.phone_number, message);

                            logging.WriteToLog($"Message sent: {message}", "Information");
                        }
                        else
                        {
                            logging.WriteToLog($"No primary contact found for student ID: {student.id}", "Information");
                        }                    
                    }
                    else
                    {
                        logging.WriteToLog($"No student found with admission number: {account}", "Warning");
                    }
                }
                else
                {
                    logging.WriteToLog($"Invalid account/admission number: {account}", "Warning");
                }

                // Save changes to the BudgetTrackerDbContext
                await budgetTrackerDbContext.SaveChangesAsync();
                Console.WriteLine("Fee payment saved ->  Account/Index: " + fee_payment.AccountNo);

                return true;
            }
            catch (Exception ex)
            {
                logging.WriteToLog($"Error saving fee payment: {ex.Message}", "Error");
                return false;
            }
        }



        public static bool IsValidAdmissionNumber(string account)
        {
            return int.TryParse(account, out _);
        }
    }
}
