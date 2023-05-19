using System.ComponentModel.DataAnnotations;

namespace budget_tracker.Models
{
    public class general_ledger
    {
        #region Properties
        [Key]
        public int TransactionId { get; set; }
        public int Account_fk { get; set; }
        public int TransactionType_fk { get; set; }
        public decimal Amount { get; set; }
        public string Refrence { get; set; }
        public string Description { get; set; }
        public int AddedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateOfTransaction { get; set; }
        #endregion    
    }

    public class mpesa_c2b_result   
    {
        #region Properties
        [Key]
        public string TransactionType { get; set; }
        public string TransID { get; set; }
        public string TransTime { get; set; }
        public string TransAmount { get; set; }
        public string BusinessShortCode { get; set; }
        public string BillRefNumber { get; set; }
        public string OrgAccountBalance { get; set; }
        public string ThirdPartyTransID { get; set; }
        public string MSISDN { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        #endregion    
    }
}
