using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BingoSync.Interfaces;
using UObject = UnityEngine.Object;
using BingoSync.CustomGoals;
using System.IO;
using System.Reflection;
using System.Linq;
using BingoAdvancedCustomGeneration;
using BingoSync;
using BingoSync.Settings;
using MonoMod.Utils;
using MonoMod.RuntimeDetour;

namespace BingoGoalPackSpleengy
{
    public class BingoGoalPackSpleengy : Mod
    {
        new public string GetName() => "BingoGoalPackSpleengy";
        public override string GetVersion() => "1.0";

        internal static BingoGoalPackSpleengy Instance;

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            OrderedLoader.OnStandaloneGoalsGameModesLoaded += SetupGoalsGameModes;
            var _hook = new ILHook
                (
                typeof(DreamPlant).GetMethod("CheckOrbs", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(),
                DreamTreesExtension.TrackDreamTrees
                );
        }

        private void SetupGoalsGameModes(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            

            Dictionary<string, BingoGoal> spleenGoals = processEmbeddedJson(assembly, "spleengy");
            addExistingGoals(spleenGoals);
            BingoSync.Goals.AddGameMode(new AdvancedGameMode("spleengy", setupGoalsDict(spleenGoals)));
            BingoSync.Goals.RegisterGoalsForCustom("spleengy", spleenGoals);

        }

        private Dictionary<string, AdvancedGoal> setupGoalsDict(Dictionary<string, BingoGoal> basicGoals)
        {
                Dictionary<string, AdvancedGoal> advancedGoals = [];
                foreach (BingoGoal basicGoal in basicGoals.Values)
                {
                    advancedGoals.Add(basicGoal.name, new AdvancedGoal()
                    {
                        Name = basicGoal.name,
                        FullExclusions = [.. basicGoal.exclusions.Select(s => string.Copy(s))],
                    });
                }
                SetupCustomExclusions(advancedGoals);
            return advancedGoals;
        }

        public static void SetupCustomExclusions(Dictionary<string, AdvancedGoal> goals)
        {
            bool lineExclusion = true;
            bool fullExclusion = false;

            Exclude(goals, "Shade Soul", "Kill 2 Soul Warriors", fullExclusion);
            Exclude(goals, "Read Bretta's diary", "Sprintmaster + Dashmaster", lineExclusion);
            Exclude(goals, "Crystal Guardian 1", "Crystal Heart", lineExclusion);

            Exclude(goals, "Lumafly Lantern", "Crystal Heart", lineExclusion);
            Exclude(goals, "Lumafly Lantern", "Crystal Guardian 1", lineExclusion);

            Exclude(goals, "Descending Dark", "Desolate Dive", lineExclusion);
            Exclude(goals, "Descending Dark", "Soul Master", lineExclusion);

            Exclude(goals, "Slash Zote's corpse in Greenpath", "Defeat Colosseum Zote", fullExclusion);
            Exclude(goals, "Slash Zote's corpse in Greenpath", "Rescue Zote in Deepnest", fullExclusion);
            Exclude(goals, "Slash Zote's corpse in Greenpath", "Vengefly King + Massive Moss Charger", fullExclusion);

            Exclude(goals, "Idol: Deepnest Zote", "Rescue Zote in Deepnest", fullExclusion);

            Exclude(goals, "Dream Nail Willoh's Meal", "Seal: Queen's Station", fullExclusion);

            Unexclude(goals, "Save the 2 grubs in Hive", "Mask Shard  in the Hive");
            Exclude(goals, "Save the 2 grubs in Hive", "Mask Shard  in the Hive", lineExclusion);
            Exclude(goals, "Save the 2 grubs in Hive", "Hive Knight", lineExclusion);
            Exclude(goals, "Save the 2 grubs in Hive", "Hiveblood", lineExclusion);
            Exclude(goals, "Save the 2 grubs in Hive", "Complete the Hive Root", lineExclusion);

            Exclude(goals, "Complete the Hive Root", "Mask Shard  in the Hive", lineExclusion);
            Exclude(goals, "Complete the Hive Root", "Hive Knight", lineExclusion);
            Exclude(goals, "Complete the Hive Root", "Hiveblood", lineExclusion);

            Unexclude(goals, "Tram Pass + Visit all 5 Tram Stations", "Hive Knight");
            Unexclude(goals, "Tram Pass + Visit all 5 Tram Stations", "Hiveblood");
            Unexclude(goals, "Tram Pass + Visit all 5 Tram Stations", "Mask Shard  in the Hive");

            Exclude(goals, "Unlock Deepnest Stag", "Talk to Midwife", fullExclusion);
            Exclude(goals, "Unlock Deepnest Stag", "Talk to Mask Maker", lineExclusion);
            Exclude(goals, "Unlock Deepnest Stag", "Herrah", lineExclusion);

            Exclude(goals, "Charged Lumafly Journal Entry", "Uumuu", fullExclusion);

            Exclude(goals, "Open the Dirtmouth / Crystal Peak elevator", "Kill 4 Mimics", fullExclusion);

            Exclude(goals, "2 nail arts", "Cyclone Slash", fullExclusion);
            Exclude(goals, "2 nail arts", "Dash Slash", fullExclusion);
            Exclude(goals, "2 nail arts", "Great Slash", fullExclusion);

            Exclude(goals, "Read both lore tablets in Soul Sanctum", "Soul Master", fullExclusion);
            Exclude(goals, "Read both lore tablets in Soul Sanctum", "Desolate Dive", fullExclusion);

            Exclude(goals, "King's Brand", "Hornet 2", lineExclusion);

            Exclude(goals, "Defeat 3 dream warriors", "Xero", lineExclusion);
            Exclude(goals, "Defeat 3 dream warriors", "Galien", lineExclusion);
            Exclude(goals, "Defeat 3 dream warriors", "No Eyes", lineExclusion);
            Exclude(goals, "Defeat 3 dream warriors", "Elder Hu", lineExclusion);
            Exclude(goals, "Defeat 3 dream warriors", "Gorb", lineExclusion);
            Exclude(goals, "Defeat 3 dream warriors", "Marmu", lineExclusion);

            Exclude(goals, "Read the Kingdom's Edge lore tablet", "Break the 420 geo rock in Kingdom's Edge", fullExclusion);

            Exclude(goals, "Complete the Crystal Peak Root", "Hallownest Crown Pale Ore", lineExclusion);

            Exclude(goals, "Broken Vessel", "Unlock Hidden Stag Station", lineExclusion);
        }
        public static void Exclude(Dictionary<string, AdvancedGoal> goals, string goal1, string goal2, bool line = false)
        {
            if (line)
            {
                goals[goal1].LineExclusions.Add(goal2);
                goals[goal2].LineExclusions.Add(goal1);
            }
            else
            {
                goals[goal1].FullExclusions.Add(goal2);
                goals[goal2].FullExclusions.Add(goal1);
            }
        }

        public static void Unexclude(Dictionary<string, AdvancedGoal> goals, string goal1, string goal2)
        {
            goals[goal1].LineExclusions.Remove(goal2);
            goals[goal2].LineExclusions.Remove(goal1);
            goals[goal1].FullExclusions.Remove(goal2);
            goals[goal2].FullExclusions.Remove(goal1);
        }

        private Dictionary<string, BingoGoal> processEmbeddedJson(Assembly assembly, string jsonName)
        {
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("Resources." + jsonName + ".json"));
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            return BingoSync.Goals.ProcessGoalsStream(stream);
        }

        private void addExistingGoals(Dictionary<string, BingoGoal> spleenGoals)
        {
            Dictionary<string, BingoGoal> vanillaGoals = BingoSync.Goals.GetVanillaGoals();
            Dictionary<string, BingoGoal> extendedGoals = Goals.GetGoalsByGroupName("Extended");
            Dictionary<string, BingoGoal> extendedPlusGoals = Goals.GetGoalsByGroupName("Extended+");
            Dictionary<string, BingoGoal> relicGoals = Goals.GetGoalsByGroupName("Relics");
            string[] addGoals = [
                "Charged Lumafly Journal Entry",
                "Defeat 3 dream warriors",
                "Dream Nail Willoh's meal",
                "Kill a Lightseed",
                "Kill two different Alubas",
                "Nothing? (junk pit chest)",
                "Open the Dirtmouth / Crystal Peak elevator"
            ];
            spleenGoals.AddRange(vanillaGoals);
            spleenGoals.Remove("Kill Myla");
            spleenGoals.Remove("Have 4 Rancid Eggs");
            spleenGoals.Remove("Unlock Queen's Stag + King's Stag Stations");
            spleenGoals.AddRange(extendedGoals);
            spleenGoals.Remove("Slash Millibelle in Pleasure House");
            spleenGoals.Remove("Open 6 geo chests (not in junk pit)");
            spleenGoals.Remove("Decipher Hunter's Notes: Maskfly + Shrumeling");
            spleenGoals.Remove("Collect 4 Simple Keys");
            foreach (string goal in addGoals)
            {
                spleenGoals.Add(goal, extendedPlusGoals[goal]);
            }
            spleenGoals.Add("Seal: Deepnest near Mantis Lords", relicGoals["Seal: Deepnest near Mantis Lords"]);
            spleenGoals.Add("Seal: Queen's Station", relicGoals["Seal: Queen's Station"]);
            spleenGoals.Add("Idol: Deepnest Zote", relicGoals["Idol: Deepnest Zote"]);
            spleenGoals.Add("Idol: Dung Defender", relicGoals["Idol: Dung Defender"]);
        }
    }
}