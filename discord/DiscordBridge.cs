using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using LabApi.Features.Console;

namespace CapybaraCafePlugin.Discord
{
    public static class DiscordBridge
    {
        private static readonly HttpClient _client = new HttpClient();
        public static string WebhookUrl;
        public static string normalBotPort;
        public static string moderationBotPort;
        public static async void SendEvent(string eventType, bool isModeration, object data)
        {
            if (string.IsNullOrEmpty(WebhookUrl)) return;

            try
            {
                var payload = new
                {
                    type = eventType,
                    timestamp = DateTime.UtcNow.ToString("o"),
                    content = data
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Fire and forget so the game doesn't lag
                if (!isModeration)
                {
                    string url = WebhookUrl + ":" + normalBotPort + "/webhook";
                    await _client.PostAsync(url, content);
                }
                else if (isModeration)
                {
                    string url = WebhookUrl + ":" + moderationBotPort + "/webhook";
                    await _client.PostAsync(url, content);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[DiscordBridge] Send Error: {ex.Message}");
            }
        }

        
    }
}