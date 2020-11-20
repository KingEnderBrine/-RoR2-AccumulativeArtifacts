using BepInEx;
using BepInEx.Configuration;
using InLobbyConfig;
using R2API.Utils;
using RoR2;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
namespace AccumulativeArtifacts
{
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync)]
    [BepInDependency("com.KingEnderBrine.InLobbyConfig")]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.KingEnderBrine.AccumulativeArtifacts", "Accumulative Artifacts", "1.1.0")]
    public class AccumulativeArtifactsPlugin : BaseUnityPlugin
    {
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

            On.RoR2.ArtifactTrialMissionController.OnCurrentArtifactDiscovered += OnCurrentArtifactDiscovered;
        }

        public void Destroy()
        {
            Instance = null;

            ModConfigCatalog.Remove(ModConfig);

            On.RoR2.ArtifactTrialMissionController.OnCurrentArtifactDiscovered -= OnCurrentArtifactDiscovered;
        }

        private static void OnCurrentArtifactDiscovered(On.RoR2.ArtifactTrialMissionController.orig_OnCurrentArtifactDiscovered orig, ArtifactTrialMissionController self, ArtifactDef artifactDef)
        {
            orig(self, artifactDef);

            if (IsEnabled.Value)
            {
                self.artifactWasEnabled = true;
            }
        }
    }
}