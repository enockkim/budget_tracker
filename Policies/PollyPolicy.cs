using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AfricasTalkingCS;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace budget_tracker
{
    public class PollyPolicy
    {

        private readonly IConfiguration configuration;
        private readonly Logging logging;

        public PollyPolicy(IConfiguration configuration, Logging logging)
        {
            this.configuration = configuration;
            this.logging = logging;
        }

        public AsyncRetryPolicy<HttpResponseMessage> ImmediateHttpRetry { get; }
        public AsyncRetryPolicy<HttpResponseMessage> LinearHttpRetry { get; }
        public AsyncRetryPolicy<HttpResponseMessage> ExponentialHttpRetry { get; }
        public AsyncRetryPolicy<int> AfricasTalkingRetry { get; }

        public PollyPolicy()
        {
            try
            {
                ImmediateHttpRetry = Policy.HandleResult<HttpResponseMessage>(
                    res => !res.IsSuccessStatusCode)
                    .RetryAsync(10);

                LinearHttpRetry = Policy.HandleResult<HttpResponseMessage>(
                    res => !res.IsSuccessStatusCode)
                    .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(3));

                ExponentialHttpRetry = Policy.HandleResult<HttpResponseMessage>(
                    res => !res.IsSuccessStatusCode)
                    .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

                //AfricasTalkingRetry = Policy.HandleResult<int>(
                //    res => res == 105) 
                //    .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                //    () => 
                //    {
                //        logging.WriteToLog("Insufficient funds", "Information");
                //    });

            }
            catch (Exception ex)
            {
                //Helper.WriteMessageToConsoleOrLog("Policy Error: "+ex.Message, MessageType.Information, false, true, true);
            }

        }
    }
}
