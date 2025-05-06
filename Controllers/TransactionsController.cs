using budget_tracker.Models;
using budget_tracker.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
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
        // private readonly BulkSms bulkSms;
        private readonly MobileSasaBulkSms bulkSms;
        private readonly TelegramBot telegramBot;
        private readonly Logging logging;
        private readonly PollyPolicy pollyPolicy;
        private readonly RevenueApiClient revenueApiClient;

        public TransactionsController
            (ILogger<TransactionsController> logger, 
            ITransactionsService _transactionService, 
            IFeePaymentService _feePaymentService, 
            IOptionsMonitor<AppSetting> _settings,
            MobileSasaBulkSms _bulkSms, 
            TelegramBot _telegramBot, 
            Logging logging,
            PollyPolicy pollyPolicy,
            RevenueApiClient revenueApiClient)
        {
            _logger = logger;
            transactionService = _transactionService;
            feePaymentService = _feePaymentService;
            settings = _settings.CurrentValue;
            bulkSms = _bulkSms;
            telegramBot = _telegramBot;
            this.logging = logging;
            this.pollyPolicy = pollyPolicy;
            this.revenueApiClient = revenueApiClient;
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

        [HttpPost("TestTelegram")]
        public bool TestTelegram()
        {
            //pollyPolicy.AfricasTalkingRetry.Execute(bulkSms.SendSms("+254712490863", "test"));
            telegramBot.SendMessage("telegram test message");
            return true;
        }

        [HttpPost("TestBulkSms")]
        public async Task<bool> TestBulkSms([FromBody] string phoneNumberDto)
        {
            await bulkSms.SendSms(phoneNumberDto, "test text message");
            return true;
        }

        [HttpPost("callback")]
        public ActionResult CallBack([FromBody] Object body)
        {
            //pollyPolicy.AfricasTalkingRetry.Execute(bulkSms.SendSms("+254712490863", "test"));
            telegramBot.SendMessage("call back recieved");
            return Ok(body);
        }


        [HttpGet("subscribe")]
        public ActionResult Subscribe()
        {
            var subscription = new
            {
                shipmentId = "UTIDC7GLFZQO0NXA",
                scac = "MAEU",
                transportDocumentId = "ABCD421911263977",
                transportDocumentType = "BL"
            };

            return Ok(subscription);

        }

        [HttpGet("shipment")]
        public ActionResult GetShipmentDetails()
        {
            var shipment = new
            {
                shipmentId = "E07PWLP1PN2C5IPI",
                scac = "MAEU",
                carrier = "Maersk",
                transportDocumentId = "G9O2KF9IC9",
                transportDocumentType = "BL",
                containers = new List<object>
            {
                new { id = "BMOU6909XXX", isoCode = "42V0" },
                new { id = "CAAU5517XXX", isoCode = "45G1" },
                new { id = "CARU5172XXX", isoCode = "45G1" }
            },
                tags = new List<string> { "exampleTag", "anotherTag" },
                shipmentStatus = 5,
                plannedEta = DateTime.UtcNow.AddMonths(9).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                originPortId = 1010,
                originPortName = "TANJUNG PELEPAS",
                originPortUnlocode = "MYTPP",
                originPortCountry = "MALAYSIA",
                originPortLocalTimeOffset = 8,
                originPortPredictiveDepartureUtc = "2022-02-01T04:18:00Z",
                originPortActualDepartureUtc = "2022-02-01T10:32:00Z",
                finalPortId = 87,
                finalPortName = "LOS ANGELES",
                finalPortUnlocode = "USLAX",
                finalPortCountry = "USA",
                finalPortLocalTimeOffset = -7,
                finalPortPredictiveArrivalUtc = "2022-03-27T13:00:00Z",
                finalPortPredictiveArrivalUtcLast = "2022-03-27T13:00:00Z",
                finalPortActualArrivalUtc = "",
                finalPortAnchorageActualArrivalUtc = "",
                finalPortTerminalActualArrivalUtc = "2022-11-29T11:21:00Z",
                finalPortTerminalActualArrivalName = "Halterm Container Terminal",
                finalPortTerminalActualArrivalSMDG = "HAFXT",
                vesselName = "MAERSK ENSENADA",
                vesselIMONumber = 9502958,
                vesselShipId = 199849,
                vesselVoyageStatus = "22",
                vesselArea = "WMED - West Mediterranean",
                currentPortId = 4324,
                currentPortName = "CARDIFF",
                currentPortUnlocode = "GBCDF",
                currentCarrierVoyageNumber = "206N",
                nextPortId = 87,
                nextPortName = "LOS ANGELES",
                nextPortUnlocode = "USLAX",
                nextPortDistanceToGo = 2643,
                nextPortPredictiveArrivalUtc = "2022-03-27T13:00:00Z",
                webViewUrl = "https://www.marinetraffic.com/track-shipment?id=ΧΧΧΧΧΧ",
                lastCarrierUpdatedTimestamp = "2023-03-03T01:12:06Z",
                transportationPlan = new
                {
                    routes = new List<object>
                {
                    new
                    {
                        carrierVoyageNumber = "152E",
                        departure = new { eventId = 567382 },
                        arrival = new { eventId = 567383 }
                    },
                    new
                    {
                        carrierVoyageNumber = "206N",
                        departure = new { eventId = 567388 },
                        arrival = new { eventId = 567389 }
                    }
                },
                    events = new List<object>
                {
                    new
                    {
                        eventId = 567380,
                        eventTypeCode = "CEP",
                        eventTypeDescription = "Container empty to shipper",
                        eventCategory = "LAND",
                        eventDatetime = "2022-01-26T15:40:00Z",
                        eventStatus = "ACT",
                        carrierVoyageNumber = "",
                        vessel = new { shipId = "" },
                        location = new { portId = 1634 }
                    },
                    new
                    {
                        eventId = 567381,
                        eventTypeCode = "CGI",
                        eventTypeDescription = "Container gate in at POL (Port of Load)",
                        eventCategory = "LAND",
                        eventDatetime = "2022-01-27T19:26:00Z",
                        eventStatus = "ACT",
                        carrierVoyageNumber = "",
                        vessel = new { shipId = "" },
                        location = new { portId = 1010 }
                    },
                    new
                    {
                        eventId = 567382,
                        eventTypeCode = "CLL",
                        eventTypeDescription = "Container loaded at first POL (Port of Load)",
                        eventCategory = "SEA",
                        eventDatetime = "2022-02-01T04:18:00Z",
                        eventStatus = "ACT",
                        carrierVoyageNumber = "152E",
                        vessel = new { shipId = 733029 },
                        location = new { portId = 1010 }
                    },
                    new
                    {
                        eventId = 567383,
                        eventTypeCode = "CDT",
                        eventTypeDescription = "Container discharge at T/S port (Transhipment Port)",
                        eventCategory = "SEA",
                        eventDatetime = "2022-02-09T11:46:00Z",
                        eventStatus = "ACT",
                        carrierVoyageNumber = "152E",
                        vessel = new { shipId = 733029 },
                        location = new { portId = 959 }
                    },
                    new
                    {
                        eventId = 567384,
                        eventTypeCode = "LTS",
                        eventTypeDescription = "Land transshipment",
                        eventCategory = "LAND",
                        eventDatetime = "2022-02-14T14:19:00Z",
                        eventStatus = "ACT",
                        carrierVoyageNumber = "",
                        vessel = new { shipId = "" },
                        location = new { portId = 959 }
                    },
                    new
                    {
                        eventId = 567385,
                        eventTypeCode = "LTS",
                        eventTypeDescription = "Land transshipment",
                        eventCategory = "LAND",
                        eventDatetime = "2022-02-14T14:32:00Z",
                        eventStatus = "ACT",
                        carrierVoyageNumber = "",
                        vessel = new { shipId = "" },
                        location = new { portId = 959 }
                    },
                    new
                    {
                        eventId = 567386,
                        eventTypeCode = "LTS",
                        eventTypeDescription = "Land transshipment",
                        eventCategory = "LAND",
                        eventDatetime = "2022-02-15T04:20:00Z",
                        eventStatus = "ACT",
                        carrierVoyageNumber = "",
                        vessel = new { shipId = "" },
                        location = new { portId = 959 }
                    },
                    new
                    {
                        eventId = 567387,
                        eventTypeCode = "LTS",
                        eventTypeDescription = "Land transshipment",
                        eventCategory = "LAND",
                        eventDatetime = "2022-02-15T13:03:00Z",
                        eventStatus = "ACT",
                        carrierVoyageNumber = "",
                        vessel = new { shipId = "" },
                        location = new { portId = 959 }
                    },
                    new
                    {
                        eventId = 567388,
                        eventTypeCode = "CLT",
                        eventTypeDescription = "Container loaded at T/S port (Transhipment Port)",
                        eventCategory = "SEA",
                        eventDatetime = "2022-02-18T23:39:00Z",
                        eventStatus = "ACT",
                        carrierVoyageNumber = "206N",
                        vessel = new { shipId = 199849 },
                        location = new { portId = 959 }
                    },
                    new
                    {
                        eventId = 567389,
                        eventTypeCode = "CDD",
                        eventTypeDescription = "Container discharge at final POD (Port of Discharge)",
                        eventCategory = "SEA",
                        eventDatetime = "2022-03-25T07:00:00Z",
                        eventStatus = "PLN",
                        carrierVoyageNumber = "206N",
                        vessel = new { shipId = 199849 },
                        location = new { portId = 87 }
                    },
                    new
                    {
                        eventId = 567390,
                        eventTypeCode = "CGO",
                        eventTypeDescription = "Container gate out at POD (Port of Discharge)",
                        eventCategory = "LAND",
                        eventDatetime = "2022-03-28T16:00:00Z",
                        eventStatus = "PLN",
                        carrierVoyageNumber = "",
                        vessel = new { shipId = "" },
                        location = new { portId = 87 }
                    }
                },
                    transportLegs = new List<object>
                {
                    new
                    {
                        transportLegId = "9dbe6d964fa84c6761a0cd4710dd40ad",
                        carrierVoyageNumber = "08WAUE1MA",
                        vessel = new { shipId = 733029 },
                        departureLocation = new { portId = 1010 },
                        portPredictiveDepartureUtc = "",
                        portActualDepartureUtc = "2022-02-01T10:32:00Z",
                        arrivalLocation = new { portId = 959 },
                        portPredictiveArrivalUtc = "",
                        portActualArrivalUtc = "2022-02-07T10:27:00Z",
                        anchorageActualArrivalUtc = "",
                        terminalActualArrivalUtc = "2022-02-07T10:33:00Z"
                    },
                    new
                    {
                        transportLegId = "273e992a4ced437d66f3c13a681b472a",
                        carrierVoyageNumber = "147S",
                        vessel = new { shipId = 199849 },
                        departureLocation = new { portId = 959 },
                        portPredictiveDepartureUtc = "",
                        portActualDepartureUtc = "2022-02-04T05:13:00Z",
                        arrivalLocation = new { portId = 2999 },
                        portPredictiveArrivalUtc = "",
                        portActualArrivalUtc = "2022-02-08T04:11:00Z",
                        anchorageActualArrivalUtc = "",
                        terminalActualArrivalUtc = "2022-02-08T04:43:00Z"
                    },
                    new
                    {
                        transportLegId = "273e992a4ced437d83cefa8fe208463a",
                        carrierVoyageNumber = "147S",
                        vessel = new { shipId = 199849 },
                        departureLocation = new { portId = 2999 },
                        portPredictiveDepartureUtc = "",
                        portActualDepartureUtc = "2022-02-09T21:39:00Z",
                        arrivalLocation = new { portId = 2429 },
                        portPredictiveArrivalUtc = "",
                        portActualArrivalUtc = "2022-02-12T06:32:00Z",
                        anchorageActualArrivalUtc = "",
                        terminalActualArrivalUtc = "2022-02-12T07:09:00Z"
                    }
                },
                    vessels = new List<object>
                {
                    new
                    {
                        shipId = 199849,
                        vesselName= "MAERSK ENSENADA",
                        vesselIMONumber = 9502958,
                        vesselMMSINumber = 371274000
                    },
                    new
                    {
                        shipId = 199849,
                        vesselName= "MAERSK ENSENADA",
                        vesselIMONumber = 9502958,
                        vesselMMSINumber = 371274000
                    }
                },
                    locations = new List<Object> {
                        new
                      {
                        portId = 87,
                        locationName = "LOS ANGELES",
                        locationCountry = "USA",
                        lat = 33.74021,
                        lon = -118.265,
                        UNLocationCode = "USLAX",
                        localTimeOffset = -7
                      },
                        new
                      {
                        portId = 87,
                        locationName = "LOS ANGELES",
                        locationCountry = "USA",
                        lat = 33.74021,
                        lon = -118.265,
                        UNLocationCode = "USLAX",
                        localTimeOffset = -7
                      }
                    }
            }

        };

            return Ok(shipment);
        }


        // POST api/<MembersController>
        [HttpPost("SaveTransaction")]
        public bool Post([FromBody] general_ledger transaction)
        {
            return transactionService.SaveTransaction(transaction).Item1;
        }

        [HttpPost("Confirmation")]
        public async Task<IResult> ProcessRequest([FromBody] mpesa_c2b_result context)
        {
            // Log the response
            Console.WriteLine($"RefNo: {context.TransID} Phone: {context.MSISDN} Name: {context.FirstName} {context.MiddleName} {context.LastName} Amount:  {context.TransAmount} Acc:  {context.BillRefNumber}");

            logging.WriteToLog($"Name: {context.FirstName} {context.MiddleName} {context.LastName}\nAcc:  {context.BillRefNumber} Amount:  {context.TransAmount} Balance: {context.OrgAccountBalance}", "Information");

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

                await feePaymentService.SaveFeePayment(fee_payment, context.BillRefNumber, context.TransAmount);
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
                    await bulkSms.SendSms(contact, messageAdmin);

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

            //telegramBot.SendMessage(messageAdminTelegram); //for me to save on sms cost

            //send to new app Prema.ShuleOne
            try
            {
                await revenueApiClient.PostRevenueAsync(new Revenue
                {
                    amount = transaction.Amount,
                    paid_by = $"{context.FirstName} {context.MiddleName} {context.LastName}",
                    payment_reference = context.TransID,
                    account_number = context.BillRefNumber,
                    status = RevenueStatus.Unallocated,
                    payment_date = DateTime.ParseExact(context.TransTime, "yyyyMMddHHmmss", provider),
                    payment_method = PaymentMethod.Mpesa,
                    recorded_by = "0"
                });
            }
            catch (Exception ex)
            {
                logging.WriteToLog($"Error posting payment to new app: {ex.Message}", "Error");
            }

            return TypedResults.Ok();
        }
        
        [HttpGet("TransactionStatus")]
        public IActionResult CheckTransaction([FromQuery] string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
            {
                return BadRequest(new TransactionResult
                {
                    TransactionId = transactionId,
                    Status = Models.TransactionStatus.NotFound,
                    Message = "Transaction ID cannot be null or empty."
                });
            }

            var result = transactionService.CheckTransaction(transactionId);


            if (result.Status == Models.TransactionStatus.NotFound)
            {
                return NotFound(result);
            }

            return Ok(result);
        }


        [HttpGet("UpdateAdmissionStatus")]
        public async Task<IActionResult> UpdateAdmissionStatus([FromQuery] int admissionNumber)
        {
            if (admissionNumber < 1000)
            {
                return BadRequest(new AdmissionUpdateStatus
                {
                    AdmissionNumber = admissionNumber,
                    AdmissionStatus = "Invalid admission number."
                });
            }

            var result = await feePaymentService.UpdateAdmissionStatus(admissionNumber);

            if(result)
            {
                return Ok(result);
            } else
            {
                return BadRequest(new AdmissionUpdateStatus
                {
                    AdmissionNumber = admissionNumber,
                    AdmissionStatus = "Error updating admission status."
                });
            }

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
