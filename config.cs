namespace CapybaraCafePlugin
{
    public class Config
    {
        public bool IsEnabled { get; set; } = true;
        public string WebhookUrl { get; set; } = "";
        public string normalBotPort { get; set; } = "";
        public string moderationBotPort { get; set; } = "";
        public int HeartbeatInterval { get; set; } = 15;
    }
}