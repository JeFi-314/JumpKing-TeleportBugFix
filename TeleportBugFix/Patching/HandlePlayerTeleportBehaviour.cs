using HarmonyLib;
using JumpKing;
using JumpKing.Level;
using JK = JumpKing.BodyCompBehaviours;

using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Diagnostics;

using System;
using JumpKing.GameManager;

namespace TeleportBugFix.Patching;
public class HandlePlayerTeleportBehaviour
{
    public HandlePlayerTeleportBehaviour (Harmony harmony)
    {
        Type type = typeof(JK.HandlePlayerTeleportBehaviour);
        MethodInfo ExecuteBehaviour = type.GetMethod(nameof(JK.HandlePlayerTeleportBehaviour.ExecuteBehaviour));
        harmony.Patch(
            ExecuteBehaviour,
            transpiler: new HarmonyMethod(AccessTools.Method(typeof(HandlePlayerTeleportBehaviour), nameof(transpileExecuteBehaviour)))
        );
    }

    private static IEnumerable<CodeInstruction> transpileExecuteBehaviour(IEnumerable<CodeInstruction> instructions , ILGenerator generator) {
        CodeMatcher matcher = new CodeMatcher(instructions , generator);

        try {
            // Find local variable then replace it
            matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Ldloc_0),
                    new CodeMatch(OpCodes.Ldflda, AccessTools.Field("JumpKing.Player.BodyComp:Position")),
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field("Microsoft.Xna.Framework.Vector2:Y")),
                    new CodeMatch(OpCodes.Ldc_R4, 360f),
                    new CodeMatch(OpCodes.Sub),
                    new CodeMatch(OpCodes.Neg),
                    new CodeMatch(OpCodes.Ldc_R4, 360f),
                    new CodeMatch(OpCodes.Div),
                    new CodeMatch(OpCodes.Conv_I4),
                    new CodeMatch(OpCodes.Stloc_S)
                )
                .ThrowIfInvalid("Cant find code");
            var labels = new List<Label>(matcher.Instruction.labels);
            matcher.RemoveInstructions(10);
            matcher.Insert(
                    // new CodeInstruction(OpCodes.Ldloc_1),
                    // new CodeInstruction(OpCodes.Callvirt, AccessTools.Method("TeleportBugFix.Patching.LevelScreen:GetIndex0")),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HandlePlayerTeleportBehaviour), nameof(GetIndex0))),
                    new CodeInstruction(OpCodes.Stloc_S, 7)
                );
            matcher.Instruction.labels = labels;
        } catch (Exception e) {
            Debug.WriteLine($"[ERROR] {e.Message}");
            return instructions;
        }
        return matcher.Instructions();
    }

    public static int GetIndex0() {
        if (Game1.instance.contentManager?.level != null && TeleportBugFix.isFix) {
        // if (true) {
            return Camera.CurrentScreen;
        }
        else {
            return (int)((0f - (GameLoop.m_player.m_body.Position.Y - 360f)) / 360f);
        }
    }
}