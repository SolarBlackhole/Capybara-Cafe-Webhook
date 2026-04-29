using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LabApi.Features.Console;

namespace CapybaraCafePlugin.Discord
{
    public static class DiscordBridge
    {
        private static readonly HttpClient _client = new HttpClient();
        public static string WebhookUrl;
        public static async void SendEvent(string eventType, object data)
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
                await _client.PostAsync(WebhookUrl, content);
            }
            catch (Exception ex)
            {
                Logger.Error($"[DiscordBridge] Send Error: {ex.Message}");
            }
        }

        
    }
}