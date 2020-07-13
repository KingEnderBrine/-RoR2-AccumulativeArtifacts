using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using RoR2;
using System.Linq;
using UnityEngine;

namespace ArtifactsRandomizer
{
    [R2APISubmoduleDependency(nameof(CommandHelper))]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.KingEnderBrine.AccumulativeArtifacts", "Accumulative Artifacts", "1.0.0")]
    public class ArtifactsRandomizerPlugin : BaseUnityPlugin
    {
        private static ConfigWrapper<bool> isEnabled { get; set; }

        public void Awake()
        {
            CommandHelper.AddToConsoleWhenReady();

            isEnabled = Config.Wrap("Main", "enabled", "Is mod should accumulate artifacts or not", true);

            On.RoR2.ArtifactTrialMissionController.OnCurrentArtifactDiscovered += (orig, self, artifactDef) =>
            {
                orig(self, artifactDef);
                if (isEnabled.Value)
                {
                    self.SetFieldValue("artifactWasEnabled", true);
                }
            };
        }

        [ConCommand(commandName = "aa_enable", flags = ConVarFlags.None, helpText = "Enable artifacts accumulation")]
        private static void CCEnable(ConCommandArgs args)
        {
            if (isEnabled.Value)
            {
                return;
            }
            isEnabled.Value = true;
            isEnabled.ConfigFile.Save();
            Debug.Log($"[AccumulativeArtifacts] is enabled");
        }

        [ConCommand(commandName = "aa_disable", flags = ConVarFlags.None, helpText = "Disable artifacts accumulation")]
        private static void CCDisable(ConCommandArgs args)
        {
            if (!isEnabled.Value)
            {
                return;
            }
            isEnabled.Value = false;
            isEnabled.ConfigFile.Save();
            Debug.Log($"[AccumulativeArtifacts] is disabled");
        }

        [ConCommand(commandName = "aa_status", flags = ConVarFlags.None, helpText = "Shows is artifacts accumulation enabled or disabled")]
        private static void CCStatus(ConCommandArgs args)
        {
            Debug.Log($"[AccumulativeArtifacts] is {(isEnabled.Value ? "enabled" : "disabled")}");
        }
    }
}