using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BingoGoalPackSpleengy
{
    internal static class DreamTreesExtension
    {
        public static void TrackDreamTrees(ILContext il)
        {
            FieldInfo completed = typeof(DreamPlant).GetField("completed", BindingFlags.NonPublic | BindingFlags.Instance);
            ILCursor cursor = new ILCursor(il).Goto(0);
            while (cursor.TryGotoNext(i => i.MatchStfld(completed)))
            {
                cursor.GotoNext();
                cursor.EmitDelegate<Action>(() => {
                    var zone = GameManager.instance.sm.mapZone;
                    if (zone.ToString() != "HIVE")
                        return;
                    string regionVariableName = $"dreamTreeCompleted_{zone}";
                    BingoSync.Variables.UpdateBoolean(regionVariableName, true);
                });
            }
        }
    }
}
