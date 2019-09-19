namespace PhilipDaubmeier.GraphIoT.App.Model.Config
{
    public class TelegramConfig
    {
        public string BotId { get; set; }
        public string ChatId { get; set; }

        public string WebhookBaseUri { get; set; }

        public string WhitelistFirstname { get; set; }
        public string WhitelistLastname { get; set; }
    }
}