using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace budget_tracker
{
    public class TelegramBot
    {
        private readonly AppSetting settings;

        private static TelegramBotClient bot;

        public TelegramBot(IOptionsMonitor<AppSetting> _settings)
        {
            settings = _settings.CurrentValue;
        }

        public TelegramBot()
        {
        }

        public void SendMessage(string message)
        {
            bot = new TelegramBotClient(settings.TelegramToken);
            bot.SendTextMessageAsync(settings.TelegramChatId, message);
        }
    }
}