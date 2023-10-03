using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using AfricasTalkingCS;
using budget_tracker.Controllers;
using budget_tracker.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PhoneNumbers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace budget_tracker
{
    public class BulkSms
    {
        private readonly AppSetting settings;
        private readonly Logging logging;
        private readonly TelegramBot telegram;
        public BulkSms(IOptionsMonitor<AppSetting> settings, Logging logging, TelegramBot telegram)
        {
            this.settings = settings.CurrentValue;
            this.logging = logging;
            this.telegram = telegram;
        }

        public Task<int> SendSms(string contact, string message)
        {
            var recep = contact;
            var msg = message;
            


            var gateway = new AfricasTalkingGateway(settings.Username, settings.ApiKey);
            try
            {
                var res = gateway.SendMessage(recep, msg);

                return res.SMSMessageData.Recipients[0].statusCode;
            }
            catch (AfricasTalkingGatewayException exception)
            {
                Console.WriteLine(exception);
                logging.WriteToLog($"SendSms: {exception}", "Error");
                return (dynamic)exception;
            }
        }

    }
}