# Another-Archery-Patcher
Customizable Archery physics for any load order.

# Installation
  Extract the latest release archive to your `zEdit/modules/` directory.

# Usage
  
  Launch zEdit and click the "Manage Patchers" puzzle icon in the top-right, you can configure the patch by clicking the "AnotherArcheryTweak" tab.
  
# Configuration Settings

## Arrow / Bolt Tweaks

#### `Toggle Section`
You can use the checkbox next to the section header to disable modification of stats for a particular projectile type.

#### `Speed`
Changes the speed of in-flight projectiles.

*Defaults: (5000 for arrows, 5800 for bolts)*

#### `Gravity`
Changes the amount of gravity applied to in-flight projectiles, combined with the Speed value these control projectile drop.

*Defaults: (0.34 for arrows/bolts)*

#### `Impact Force`
The amount of force imparted to objects/actors that the projectile collides with. 

*Defaults: (0.44 for arrows/bolts)*

#### `Noise Level`
The amount of detectable noise a projectile makes when fired / passing by an NPC.

*Defaults: (Silent for arrows, Normal for bolts.)*

## Other Tweaks

#### `Remove "Supersonic" Flag`
The supersonic flag removes sound from in-flight projectiles.

#### `Disable Auto-Aim`
Removes the redirection effect that happens when you fire an arrow with your cursor close to an enemy.

#### `Disable Gravity on Bloodcursed Arrows`
Removes gravity from Bloodcursed Arrows to make it easier to hit the sun when modifying projectile gravity.

#### `(Experimental) Fix NPC Ninja Dodge Bug`
This modifies the dodge chance for actors, however this may interact with some combat mods.
