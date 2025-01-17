# Jump King Teleport Block Mechanic Patch

## Introduction
This mod provides a temporary patch for an issue with the teleport block mechanic in Jump King. The issue occurs when a player crosses the screen boundary and is teleported to a linked screen, but their position is incorrectly calculated, leading to an unintended vertical shift. The mod fixes this problem by modifying the relevant logic using Harmony transpilers.

---

## Problem Description
When the player's horizontal middle position exceeds the screen boundaries (i.e., `x < 0 || x > 480`), the game attempts to teleport the player to the linked screen (if a teleport link exists) by calling `ExecuteBehaviour()` (from `JumpKing.BodyCompBehaviours.HandlePlayerTeleportBehaviour`). However, the method has inconsistencies between its first and second halves:

1. **Determining the Target Screen:**
   - The first half of `ExecuteBehaviour` determines the target screen using `JumpKing.Camera.CurrentScreen`.

2. **Calculating the Vertical Position Shift:**
   - The second half calculates the player's vertical position shift using `bodyComp.Position.Y`, which represents the top-left vertex of the player's hitbox. The calculated shift is then added to the new position.

### Issues with the Current Implementation
- **Hitbox Reference Discrepancy:**
  - The camera uses the middle-bottom of the player's hitbox (`JumpKing.Camera.UpdateCameraWithVelocity`) to calculate the current screen, whereas the teleportation logic references the top-left vertex of the hitbox.
- **Additional Conditions in Camera Update:**
  - The camera logic includes additional conditions (e.g., related to the y-component of the player's velocity) to determine the current screen.
- **Ceiling/Grounding Inconsistencies:**
  - The teleportation logic applies a ceiling (or possibly grounding) operation to `bodyComp.Position.Y` with 360 pixels (screen height) when shifting positions. However, the camera logic allows `bodyComp.Position.Y` to exceed 360 due to the aforementioned conditions.

### Resulting Bug
When the player crosses the edge of the screen at a high vertical position (e.g., >334 pixels), they are teleported to the correct target screen, but their vertical position is shifted downward.

---

## Solution
This mod addresses the issue by:

1. **Modifying the Local Variable:**
   - Using Harmony transpilers, the mod replaces the usage of `bodyComp.Position.Y` in the teleportation logic with `JumpKing.Camera.CurrentScreen` to ensure consistency.

2. **Relying on CameraFollowComp for Position Adjustment:**
   - The mod does not modify the camera logic (`JumpKing.Camera.UpdateCameraWithVelocity`) since it uses the center of the hitbox, which is ultimately corrected by `CameraFollowComp` after teleportation.

### Testing and Edge Cases
The patch has undergone limited testing and has successfully resolved the original issue in observed cases. However, potential edge cases may still exist and require further exploration.

---

## Implementation Notes
The patch utilizes Harmony transpilers to locate and adjust the affected variables dynamically, ensuring minimal disruption to the game's existing logic.

