using budget_tracker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace budget_tracker
{
    public class StartupService : BackgroundService
    {
        private readonly ILogger _logger;
        private Timer? _timer;
        private readonly TelegramBot telegramBot;        

        private static Logging logging;

        public StartupService(ILogger<StartupService> logger, Logging _logging)
        {
            _logger = logger;
            logging = _logging;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logging.WriteToLog("✅ Lifeway service starting...", "Information");
        }
    }
}