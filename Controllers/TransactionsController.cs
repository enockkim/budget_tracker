using budget_tracker.Models;
using budget_tracker.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Transactions;

namespace budget_tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase//, IHttpHandler
    {
        private readonly ILogger<TransactionsController> _logger;
        private ITransactionsService transactionService;
        private IFeePaymentService feePaymentService;
        CultureInfo provider = CultureInfo.InvariantCulture;

        public TransactionsController(ILogger<TransactionsController> logger, ITransactionsService _transactionService, IFeePaymentService _feePaymentService)
        {
            _logger = logger;
            transactionService = _transactionService;
            feePaymentService = _feePaymentService;
        }

        //// GET: api/<MembersController>
        //[HttpGet("GetAllMembers")]
        //public IEnumerable<members> GetAllMembers()
        //{
        //    return (IEnumerable<members>)groupService.GetMembers();
        //}

        //// GET api/<MembersController>/5
        //[HttpGet("GetMemberById")]
        //public members Get(int memberId)
        //{
        //    return groupService.GetMemberById(memberId);
        //}

        // POST api/<MembersController>
        [HttpPost("SaveTransaction")]
        public bool Post([FromBody] general_ledger transaction)
        {
            return transactionService.SaveTransaction(transaction).Item1;
        }

        [HttpPost("Confirmation")]
        public void ProcessRequest([FromBody] mpesa_c2b_result context)
        {
            // Log the response
            Console.WriteLine($"Name: {context.FirstName} {context.MiddleName} {context.LastName} Amount:  {context.TransAmount} Acc:  {context.BillRefNumber}");

            Logging.WriteToLog($"Name: {context.FirstName} {context.MiddleName} {context.LastName} Amount:  {context.TransAmount} Acc:  {context.BillRefNumber}", "Information");

            general_ledger transaction = new general_ledger()
            {
                Account_fk = 1,
                TransactionType_fk = 1,
                Amount = Convert.ToDecimal(context.TransAmount),
                Refrence = context.TransID,
                Description = context.BillRefNumber,
                AddedBy = 0,
                DateAdded = DateTime.Now,
                DateOfTransaction = DateTime.ParseExact(context.TransTime, "yyyyMMddHHmmss", provider),
            };

            var res = transactionService.SaveTransaction(transaction);

            if (res.Item1) {

                fee_payment fee_payment = new fee_payment()
                {
                    AccountNo = context.BillRefNumber,
                    PhoneNumber = context.MSISDN,
                    FirstName = context.FirstName,
                    LastName = context.LastName,
                    MiddleName = context.MiddleName,
                    TransactionId_fk = res.Item2
                };

                var SaveFeePayment = feePaymentService.SaveFeePayment(fee_payment);
            }

            //Send sms
            //TODO need to change since msisdn will phased
            string message = $"Your payment of Ksh. {transaction.Amount} for {transaction.Account_fk} has been recieved. Thank you.";
            string messageAdmin = $"Payment of Ksh. {transaction.Amount} for {transaction.Account_fk} has been recieved. Current balance is Ksh. {context.OrgAccountBalance}";
            //BulkSms.SendSms(context.MSISDN, message); //Send to parent/ whoever initialized the payment
            //BulkSms.SendSms("+254712345678", message); //Send to bursar need to get school simcard
            BulkSms.SendSms("+254712490863", messageAdmin); //Send to me for testing
            //BulkSms.SendSms("+254797303073", messageAdmin); //Send to me for testing
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        //// PUT api/<MembersController>/5
        //[HttpPut("UpdateMemberProfile")]
        //public result Put([FromBody] members member)
        //{
        //    return groupService.UpdateMemberProfile(member);
        //}

        //// DELETE api/<MembersController>/5
        //[HttpPost("ChangeMemberStatus")]
        //public result Delete(int memberId)
        //{
        //    return groupService.ChangeMemberStatus(memberId);
        //}
    }
}
