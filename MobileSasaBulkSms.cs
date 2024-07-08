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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PhoneNumbers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace budget_tracker
{
    public class MobileSasaBulkSms
    {
        private readonly AppSetting settings;
        private readonly Logging logging;
        private readonly TelegramBot telegram;
        private readonly HttpClient _httpClient;
        public MobileSasaBulkSms(IOptionsMonitor<AppSetting> settings, Logging logging, TelegramBot telegram, HttpClient httpClient)
        {
            this.settings = settings.CurrentValue;
            this.logging = logging;
            this.telegram = telegram;
            _httpClient = httpClient;
        }

        private const string ApiUrl = "https://api.mobilesasa.com/v1/send/bulk";
        private const string ApiToken = "jz2Le0d4yt2TpJhfLrlf8elSY3KwXXynl9sdTLmgutjpBgywBxM0ny4r4VQR";

        public async Task<bool> SendSms(string contact, string message)
        {
            try
            {
                var requestContent = new
                {
                    senderID = "MOBILESASA",
                    message = message,
                    phones = contact
                };

                var jsonContent = new StringContent(
                    Newtonsoft.Json.JsonConvert.SerializeObject(requestContent),
                    Encoding.UTF8,
                    "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiToken}");

                var response = await _httpClient.PostAsync(ApiUrl, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    logging.WriteToLog($"Failed to send message. Status code: {response.StatusCode}", "Error: {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                logging.WriteToLog($"SendSms: {ex}", "Error");
                return false;
            }
        }

    }
}