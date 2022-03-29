using BepInEx.Bootstrap;
using InLobbyConfig;
using System.Runtime.CompilerServices;

namespace AccumulativeArtifacts
{
    public class InLobbyConfigIntegration
    {
        public const string GUID = "com.KingEnderBrine.InLobbyConfig";
        private static bool Enabled => Chainloader.PluginInfos.ContainsKey(GUID);
        private static object ModConfig { get; set; }

        public static void OnStart()
        {
            if (Enabled)
            {
                OnStartInternal();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void OnStartInternal()
        {
            var modConfig = new ModConfigEntry
            {
                DisplayName = "Accumulative Artifacts",
                EnableField = new InLobbyConfig.Fields.BooleanConfigField("", () => AccumulativeArtifactsPlugin.IsEnabled.Value, (newValue) => AccumulativeArtifactsPlugin.IsEnabled.Value = newValue)
            };

            ModConfigCatalog.Add(modConfig);
            ModConfig = modConfig;
        }

        public static void OnDestroy()
        {
            if (Enabled)
            {
                OnDestroyInternal();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void OnDestroyInternal()
        {
            ModConfigCatalog.Remove(ModConfig as ModConfigEntry);
        }
    }
}
