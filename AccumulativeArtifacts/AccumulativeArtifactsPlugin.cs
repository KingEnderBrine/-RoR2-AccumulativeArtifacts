using BepInEx;
using BepInEx.Configuration;
using MonoMod.RuntimeDetour.HookGen;
using RoR2;
using System;
using System.Reflection;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[assembly: AssemblyVersion(AccumulativeArtifacts.AccumulativeArtifactsPlugin.Version)]
namespace AccumulativeArtifacts
{
    [BepInDependency(InLobbyConfigIntegration.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(GUID, Name, Version)]
    public class AccumulativeArtifactsPlugin : BaseUnityPlugin
    {
        public const string GUID = "com.KingEnderBrine.AccumulativeArtifacts";
        public const string Name = "Accumulative Artifacts";
        public const string Version = "1.3.0";

        private static readonly MethodInfo onCurrentArtifactDiscovered = typeof(ArtifactTrialMissionController).GetMethod(nameof(ArtifactTrialMissionController.OnCurrentArtifactDiscovered), BindingFlags.NonPublic | BindingFlags.Instance);

        internal static AccumulativeArtifactsPlugin Instance { get; private set; }
        internal static ConfigEntry<bool> IsEnabled { get; set; }

        public void Start()
        {
            Instance = this;

            IsEnabled = Config.Bind("Main", "enabled", true, "Should accumulate artifacts or not?");
            InLobbyConfigIntegration.OnStart();
            HookEndpointManager.Add(onCurrentArtifactDiscovered, (Action<Action<ArtifactTrialMissionController, ArtifactDef>, ArtifactTrialMissionController, ArtifactDef>)OnCurrentArtifactDiscovered);
        }

        public void Destroy()
        {
            Instance = null;

            InLobbyConfigIntegration.OnDestroy();
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