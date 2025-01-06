using JumpKing.PauseMenu.BT.Actions;

namespace TeleportBugFix.Menu;
public class ToggleFix : ITextToggle
{
    public ToggleFix() : base(TeleportBugFix.isFix)
    {
    }

    protected override string GetName() => "Fix Teleport Bug";

    protected override void OnToggle()
    {
        TeleportBugFix.isFix = toggle;
    }
}