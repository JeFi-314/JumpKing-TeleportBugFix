using HarmonyLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using JumpKing.Mods;
using JumpKing.PauseMenu;
using TeleportBugFix.Menu;

namespace TeleportBugFix;
[JumpKingMod(IDENTIFIER)]
public static class TeleportBugFix
{
    const string IDENTIFIER = "JeFi.TeleportBugFix";
    const string HARMONY_IDENTIFIER = "JeFi.TeleportBugFix.Harmony";

    public static string AssemblyPath { get; set; }
    public static bool isFix { get; set; }

    [BeforeLevelLoad]
    public static void BeforeLevelLoad()
    {
        AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#if DEBUG
        Debugger.Launch();
        Harmony.DEBUG = true;
        Environment.SetEnvironmentVariable("HARMONY_LOG_FILE", $@"{AssemblyPath}\harmony.log.txt");
#endif
        Harmony harmony = new Harmony(HARMONY_IDENTIFIER);

        new Patching.HandlePlayerTeleportBehaviour(harmony);
#if DEBUG
        Environment.SetEnvironmentVariable("HARMONY_LOG_FILE", null);
#endif
    }

    [PauseMenuItemSetting]
    public static ToggleFix ToggleCurrentScreen(object factory, GuiFormat format)
    {
        return new ToggleFix();
    }
}
