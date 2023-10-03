using budget_tracker.Models;
using budget_tracker.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Transactions;
using Telegram.Bot.Types;
using static Org.BouncyCastle.Math.EC.ECCurve;

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

        private readonly AppSetting settings;
        private readonly BulkSms bulkSms;
        private readonly TelegramBot telegramBot;
        private readonly Logging logging;
        private readonly PollyPolicy pollyPolicy;

        public TransactionsController(ILogger<TransactionsController> logger, ITransactionsService _transactionService, IFeePaymentService _feePaymentService, IOptionsMonitor<AppSetting> _settings, BulkSms _bulkSms, TelegramBot _telegramBot, Logging logging, PollyPolicy pollyPolicy)
        {
            _logger = logger;
            transactionService = _transactionService;
            feePaymentService = _feePaymentService;
            settings = _settings.CurrentValue;
            bulkSms = _bulkSms;
            telegramBot = _telegramBot;
            this.logging = logging;
            this.pollyPolicy = pollyPolicy;
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

        [HttpGet("Test")]
        public bool Test()
        {
            //pollyPolicy.AfricasTalkingRetry.Execute(bulkSms.SendSms("+254712490863", "test"));
            telegramBot.SendMessage("test");
            return true;
        }

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
            Console.WriteLine($"RefNo: {context.TransID} Name: {context.FirstName} {context.MiddleName} {context.LastName} Amount:  {context.TransAmount} Acc:  {context.BillRefNumber}");

            logging.WriteToLog($"Name: {context.FirstName} {context.MiddleName} {context.LastName} Amount:  {context.TransAmount} Acc:  {context.BillRefNumber}", "Information");

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
            string messageAdmin = $"Payment of Ksh. {transaction.Amount} for {context.BillRefNumber} has been recieved. RefNo: {context.TransID} . Current balance is Ksh. {context.OrgAccountBalance}";
            string messageAdminTelegram = $"Recieved: Ksh. {transaction.Amount} Balance: Ksh. {context.OrgAccountBalance} Account: {context.BillRefNumber} RefNo: {context.TransID}";
            //BulkSms.SendSms(context.MSISDN, message); //Send to parent/ whoever initialized the payment

            if (!context.BillRefNumber.Equals("test"))
            {
                foreach (var contact in settings.AdminContacts)
                {
                    bulkSms.SendSms(contact, messageAdmin);
                    //int statusCode = pollyPolicy.AfricasTalkingRetry.Execute(bulkSms.SendSms(contact, messageAdmin));

                    //if (statusCode == 101)
                    //{
                    //logging.WriteToLog($"Sms Sent: {res}", "Debug");
                    //}
                    //else if (statusCode == 405)
                    //{
                    //telegramBot.SendMessage("Insufficient funds.");
                    //}
                }
            }

            telegramBot.SendMessage(messageAdminTelegram); //for me to save on sms cost
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
