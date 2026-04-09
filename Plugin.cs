using LabApi.Loader.Features; // Often where PluginEntryPoint lives now
using LabApi.Events.CustomHandlers;
using LabApi.Loader.Features.Plugins;
using LabApi.Features;
using CapybaraCafePlugin.EventListeners;
using System;
using LabApi.Features.Console;

namespace CapybaraCafePlugin
{
    public class CapybaraPlugin : Plugin
    {
        public Config Config;

        public override string Name { get; } = "Capybara Cafe Plugin";
        public override string Description { get; } = "The CapyBara Cafe WIP Plugin";
        public override string Author { get; } = "SolarBlackHole";
        public override Version Version { get; } = new Version(1, 0, 0, 0);
        public override Version RequiredApiVersion { get; } = new Version(LabApiProperties.CompiledVersion);
        public SCPEventListener Events { get;  } = new();
        public override void Enable()
        {
            CustomHandlersManager.RegisterEventsHandler(Events);
            Logger.Info("Capybara Cafe Plugin has been enabled!");
        }

        public override void Disable()
        {
            CustomHandlersManager.UnregisterEventsHandler(Events);
            Logger.Info("Capybara Cafe Plugin has been disabled!");
        }
    }
}