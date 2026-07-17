using LabApi.Events.CustomHandlers;
using LabApi.Loader.Features.Plugins;
using LabApi.Features;
using CapybaraCafePlugin.EventListeners;
using CapybaraCafePlugin.Discord;
using System;
using LabApi.Features.Console;

namespace CapybaraCafePlugin
{
    public class CapybaraPlugin : Plugin<Config>
    {
        public override string Name { get; } = "Capybara Cafe Plugin";
        public override string Description { get; } = "The CapyBara Cafe WIP Plugin";
        public override string Author { get; } = "SolarBlackHole";
        public override Version Version { get; } = new Version(1, 0, 2, 0);
        public override Version RequiredApiVersion { get; } = new Version(LabApiProperties.CompiledVersion);
        public SCPEventListener Events { get;  } = new();
        public override void Enable()
        {

            DiscordBridge.WebhookUrl = Config.WebhookUrl;
            DiscordBridge.normalBotPort = Config.normalBotPort;
            DiscordBridge.moderationBotPort = Config.moderationBotPort;
            HeartbeatManager.heartbeatInterval = Config.HeartbeatInterval;
            if (string.IsNullOrEmpty(DiscordBridge.WebhookUrl) || 
                string.IsNullOrEmpty(DiscordBridge.normalBotPort) || 
                string.IsNullOrEmpty(DiscordBridge.moderationBotPort))
            {
                Logger.Error("Webhook URL or one of the bot ports are not set in the configuration. Please set it before enabling the plugin.");
                return;
            }

            CustomHandlersManager.RegisterEventsHandler(Events);
            HeartbeatManager.Start();
            Logger.Info("Capybara Cafe Plugin has been enabled!");

        }

        public override void Disable()
        {
            CustomHandlersManager.UnregisterEventsHandler(Events);
            HeartbeatManager.Stop();
            Logger.Info("Capybara Cafe Plugin has been disabled!");
        }
    }
}