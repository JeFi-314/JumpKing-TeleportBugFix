# Jump King Teleport Bug Patch

This mod provides a temporary patch for an issue with the teleport block mechanic in Jump King. The issue occurs when a player crosses the screen boundary and is teleported to a linked screen, but their position is incorrectly calculated, leading to an unintended vertical shift. The mod fixes this problem by modifying the relevant logic using Harmony transpilers.  

You can find this mod on [Steam Workshop Page](https://steamcommunity.com/sharedfiles/filedetails/?id=3402005276).

## Problem Description

When the player's horizontal middle position exceeds the screen boundaries (i.e., `x < 0 || x > 480`), the game attempts to teleport the player to the linked screen (if a teleport link exists) by calling `ExecuteBehaviour()` (from `JumpKing.BodyCompBehaviours.HandlePlayerTeleportBehaviour`). However, inconsistencies in the logic cause an incorrect vertical position shift:

1. **Determining the Target Screen:**

   - The first half of `ExecuteBehaviour` determines the target screen using `JumpKing.Camera.CurrentScreen`.

2. **Calculating the Vertical Position Shift:**

   - The second half calculates the player's global vertical position shift using `bodyComp.Position.Y` (top-left vertex of the player's hitbox), which is then added to the new position.

### Issues

- **Hitbox Reference Discrepancy:**
  - The camera calculates the current screen using the middle-bottom of the player's hitbox (`JumpKing.Camera.UpdateCameraWithVelocity`), but the teleportation logic references the top-left vertex.
- **Additional Camera Conditions:**
  - The camera logic includes conditions (e.g., based on the y-component of velocity) to determine the current screen, which are not accounted for in the teleportation logic.
- **Ceiling/Grounding Operation:**
  - The teleportation logic applies a ceiling (or possibly grounding) operation to `bodyComp.Position.Y` with 360 pixels (screen height) when shifting positions. This results in positions originally above the screen's visible range being incorrectly mapped to the bottom of the screen after teleportation. However, the camera allows `bodyComp.Position.Y` to exceed 360 due to the aforementioned conditions, creating a mismatch between the intended and actual positions.

### Resulting Bug

When the player crosses the edge of the screen at a high vertical position (e.g., >334 pixels), they are teleported to the correct target screen but shifted downward.

---

## Solution

The mod resolves the issue by using Harmony transpilers to replace the IL code snippet `((int)((-(bodyComp.Position.Y - 360f)) / 360f))` with `JumpKing.Camera.CurrentScreen`, ensuring consistent behavior. This modification does not affect Nexile's original maps to preserve their integrity.

### Testing and Edge Cases

The patch has been tested and successfully resolves the issue in observed cases. However, potential edge cases may still exist and require further testing.

---

## Notes on UpdateCamera

A similar issue exists within the same method (`Camera.UpdateCamera(center);`), where `center` represents the middle point of the hitbox. However, this should not cause significant problems due to how Jump King handles collision and because the camera position will be corrected by `CameraFollowComp` afterward.
