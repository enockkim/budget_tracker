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

        public BulkSms(IOptionsMonitor<AppSetting> _settings)
        {
            settings = _settings.CurrentValue;
        }

        public void SendSms(string contact, string message)
        {
            var recep = contact;
            var msg = message;


            var gateway = new AfricasTalkingGateway(settings.Username, settings.ApiKey);
            try
            {
                dynamic res = gateway.SendMessage(recep, msg);
                Console.WriteLine(res);

                int statusCode = res.SMSMessageData.Recipients[0].statusCode;

                if(statusCode == 101)
                {
                    Logging.WriteToLog($"Sms Sent: {res}", "Information");
                } else
                {

                }

                
            }
            catch (AfricasTalkingGatewayException exception)
            {
                Console.WriteLine(exception);
                Logging.WriteToLog($"SendSms: {exception}", "Error");
            }
        }

    }
}