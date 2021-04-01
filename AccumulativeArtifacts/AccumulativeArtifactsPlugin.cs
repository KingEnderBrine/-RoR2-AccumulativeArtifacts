using BepInEx;
using BepInEx.Configuration;
using InLobbyConfig;
using MonoMod.RuntimeDetour.HookGen;
using RoR2;
using System;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[assembly: R2API.Utils.ManualNetworkRegistration]
[assembly: EnigmaticThunder.Util.ManualNetworkRegistration]
namespace AccumulativeArtifacts
{
    [BepInDependency("com.KingEnderBrine.InLobbyConfig")]
    [BepInPlugin("com.KingEnderBrine.AccumulativeArtifacts", "Accumulative Artifacts", "1.2.0")]
    public class AccumulativeArtifactsPlugin : BaseUnityPlugin
    {
        private static readonly MethodInfo onCurrentArtifactDiscovered = typeof(ArtifactTrialMissionController).GetMethod(nameof(ArtifactTrialMissionController.OnCurrentArtifactDiscovered), BindingFlags.NonPublic | BindingFlags.Instance);

        internal static AccumulativeArtifactsPlugin Instance { get; private set; }
        internal static ConfigEntry<bool> IsEnabled { get; set; }
        private ModConfigEntry ModConfig { get; set; }

        public void Awake()
        {
            Instance = this;

            IsEnabled = Config.Bind("Main", "enabled", true, "Should accumulate artifacts or not?");
            ModConfig = new ModConfigEntry
            {
                DisplayName = "Accumulative Artifacts",
                EnableField = new InLobbyConfig.Fields.BooleanConfigField("", () => IsEnabled.Value, (newValue) => IsEnabled.Value = newValue)
            };

            ModConfigCatalog.Add(ModConfig);
            HookEndpointManager.Add(onCurrentArtifactDiscovered, (Action<Action<ArtifactTrialMissionController, ArtifactDef>, ArtifactTrialMissionController, ArtifactDef>)OnCurrentArtifactDiscovered);
        }

        public void Destroy()
        {
            Instance = null;

            ModConfigCatalog.Remove(ModConfig);

            HookEndpointManager.Remove(onCurrentArtifactDiscovered, (Action<Action<ArtifactTrialMissionController, ArtifactDef>, ArtifactTrialMissionController, ArtifactDef>)OnCurrentArtifactDiscovered);
        }

        private static void OnCurrentArtifactDiscovered(Action<ArtifactTrialMissionController, ArtifactDef> orig, ArtifactTrialMissionController self, ArtifactDef artifactDef)
        {
            orig(self, artifactDef);

            if (IsEnabled.Value)
            {
                self.artifactWasEnabled = true;
            }
        }
    }
}

namespace R2API.Utils
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ManualNetworkRegistrationAttribute : Attribute { }
}

namespace EnigmaticThunder.Util
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ManualNetworkRegistrationAttribute : Attribute { }
}