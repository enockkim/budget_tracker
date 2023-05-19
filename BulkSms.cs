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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PhoneNumbers;

namespace budget_tracker
{

    /// <summary>
    /// The africas talking gateway class. Accepting sandbox as an environment
    /// </summary>
    public class BulkSms
    {

        public static void SendSms(string contact, string message)
        {
            var username = "lifeway";
            var apiKey = "0397ce461d588c159c9f337785fc49b15e87a78306e4120bcf7505bf5bf0814e";
            var recep = contact;
            var msg = message;


            var gateway = new AfricasTalkingGateway(username, apiKey);
            try
            {
                dynamic res = gateway.SendMessage(recep, msg);
                Console.WriteLine(res);

            }
            catch (AfricasTalkingGatewayException exception)
            {
                Console.WriteLine(exception);
            }
        }

    }
}