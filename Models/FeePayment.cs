using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;

namespace budget_tracker.Models
{
    public class fee_payment
    {
        #region Properties
        [Key]
        public int PaymentId { get; set; }
        public int TransactionId_fk { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string AccountNo { get; set; }
        #endregion    
    }
}
