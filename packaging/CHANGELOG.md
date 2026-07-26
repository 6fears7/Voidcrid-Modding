1.7.0 - All Ends...
- Rebuilt against Risk of Rain 2 1.4.1
- Updated BepInEx, HookGenPatcher and R2API dependencies to current versions
- Fixed the README screenshots which had been pointing at expired Discord links
- Updated Flamebreath coloring to match Voidcrid Glow config setting; Now won't break when attack rate scales
- Fixed skins not appearing after SotS
- Some Skin unlocks are now linked to the skill achievments, which was intended from the beginning
- Fixed void sound effects


1.6.0 - Not Dead Yet
- Updated Voidcrid to be compatible with SotS
- Disabled death effects until I can determine a fix.
- ping @UO with any issues you might have until then. Thanks for playing!

1.5.3 - Dust off
- Updated README, Added Wiki section
- Thank you to everyone who has enjoyed or given feedback on this mod


1.5.2 - King of Rats
- Fixed Entropy, Umbral Entropy, and Nullbeam breaking when using the MadVeteran_Skinpack

*Dev notes: Acrid's skin behind the scenes is made up of two parts: body, and spine. When the player skin didn't have a spine index, the Void glow the Void abilities exhibit freaked out due to bad error handling and broke the skill. This has been fixed.* 

1.5.1 - House Divided (hotfix)
- Fixed Umbral Entropy (Ancient Scepter) not working. Thanks @Cryptid Candle for letting me know!
- Fixed corruption achievement item requirement to *actually* require 7 items.
- Adjusted Void death potentials to 3 and 7, respectively.

1.5.0 - House Divided
- Removed keypress requirement for Flamebreath. Holding M1 or RT will apply the attack automatically
- Jailing mechanic overhaul: Jailing an enemy now performs as expected: you apply a tether onto them rather than instantly killing them 
- "Right To Jail" achievement has been changed to acccommodate this
- Nullbeam's jailing proc change: 0.1f => 0.4f
- New death state: On death, you will release acid when touching the ground + perform a small antimatter explosion. If you carry more than 3 Void items, you'll release a devastating Void explosion.
- The death state can be disabled in the config under the "Voidcrid: Death" section
- Lots of changes, let me know if something broke in the process @Unanimate Objec#3176


1.4.0 - New Beginnings
- Added an alternate ending flavor text if Voidcrid's Passive is set to True

1.3.8 - Networking #2
- After many months, fixed major client issues with Voidcrid
- (Don't ever forget to add your modded EntityStates with ContentAddition).

1.3.7 - Fan the flame
- Fixed Flamebreath's tracking. You can now point your mouth skyward to burn enemies!
- Added config for fog damage
- NOTICE: Chef Mod creates a downstream interoperability issue with Entropy's fog. I've reached out to the creator to determine a fix. As long as the Chef Mod is active, Entropy's void fog from the black hole *will not work*. 

1.3.6 - Networking #1
- Fixed Entropy's / Umbral Entropy's networking client-side. No more orbs staying around, and void fog will now finally kill you and all your friends.
- This is the first in a series of updates planned to fix client-side issues that went undetected for quite some time. 
- Added a config option for NullBeam proc
- Added Ko-Fi support for new skins

1.3.5 - Rest In Peace
- Hotfix to black hole visualization

1.3.4 - Rest In Peace
- Added DoT effect to Entropy while the black hole is active. Due to this change, everyone will take damage inside but there is no more damage-from-health requirement to summon the black hole. 
- Fixed a grammatical issue within Entropy's description.
- "Aisle 12" update additionally added invincibility while Ethereally Drifting. That changelog has been updated.
- NullBeam and Entropy's recharges have been decreased and increased, respectively. If you're using the config for custom values, you can ignore this change.

*Dev notes: Entropy's original vision was to choke and damage enemies, and this update realizes that vision and provides the most satisfying feedback loop since it's original inception. Taking such high damage for activating the skill plus ensarement was no longer a utility, but a burden to use. This has been rectified. The longer cooldown and DoT ensures it's utilized as a powerful, short-range hit-and-run attack compared to Disease's range.*

1.3.3 - Aisle 12

- Moved all vars loading Addressables into ES constructors
- Fixed Ethereal Drift cancelling other skills
- Fixed a bug where users would become Glacial after using Entropy
- Added config options for Entropy / Umbral Entropy Damage Heal & Hurt values
- Fixed Entropy's spine glow not accepting user input
- Fixed Entropy and Ethereal Drift ~~notworking~~ networking
- Added intial code for Skills++, but is inactive at this time
- Slightly tweaked Grandfather Paradox description for less abstraction
- Reduced Entropy self-damage to 15%
- Added invincibility to Ethereal Drift. No more dying while traveling between realms.
- Voidcrid skin will be comissioned, eventually.


1.3.2 - Voidzilla

- Added emissions to Voidcrid's spines when performing Void abilities
- Added config to change the color for preference or color-blinded purposes

*Dev notes: Always wanted to do this, figured out how.*

1.3.1 - Deeprot Synergy

- Cleaned up some pretty awful code
- Added support for Spikestrip
- Added new Deeprot support: All Void abilites will apply Deeprot if selected. (Thanks @DΞMONBØY.exe for the idea!)

*Dev notes: Running a fully Void loadout will provide syngery with Deeprot that was not possible before.* 

*Other passives will not apply to Void skills in an effort to provide reasons for using base skills.*

1.3.0 - Fresh Paint

- Added **three** new achievements to unlock Void skills
- Updated project to mirror split assembly R2API transformation
- Updated Entropy's base damage (4)
- Renamed Deeprotted Entroy => Umbral Entropy
- Updated Umbral Entropy's base damage (5)
- Updated Voidcrid Icon
- Publicized Voidcrid Github repo
- Updated Entropy to include vortex attack when skill is held

*Dev notes: New skins (hopefully) coming soon, once I learn Blender more.*

*Repo has been made public for anyone who wants to assist with troubleshooting the networking for Ethereal Drift and Entropy. Multiple other mod creators have not been able to identify why the simple network check is not working.*

1.2.5 - Home Field Advantage
- Identified issue where non-host players online cannot take damage or healing from Entropy, and do not enter stealth from Ethereal Drift (thanks @Dolos!)
- Registered Entropy for online play. You will no longer receieve a random skill when playing with your friends
- Added seasonal effect: Freezing
- Added a goofy-looking Acrid skin
- Freezing will occur for Ethereal Drift instead of stunning
- Freezing will occur for Entropy / Deeprotted Entropy instead of your Passive
- This can be disabled by changing "Seasonal" to False in the Config
- Delete your config and populate the new one by restarting the game

*Dev notes: I really wanted to make a santa hat item, but I simply don't have time. Merry Christmas, and hope you enjoy the rest of your year!>

1.2.4 - Fixes and Figs
- Increased Entropy's BlastAttackRadius (10 => 12)
- Increased Deeprotted Entropy's BlastAttackRadius (10 => 12)
- Added both as config file options
- Added Flamebreath totalDamageCoefficient to Flamebreath's Damage config. 
- Fixed a bug where all stacks of Flamebreath were consumed on use (thanks @MrPokemon11)
- This same bugfix *should* prevent Purity from disabling Flamebreath (thanks @Syrax)

*Dev notes: Flamebreath has a lot going on in terms of math formulas. totalDamageCoefficient is one major component that users can change at will.*

*As a general reference: Bumping totalDamageCoefficient from 10 => 20 will kill a stone Golem in 2 hits instead of 3 on Normal difficulty*

*As an additional reminder: You must delete your config and start the game again for the mod to create the updated config file.*

1.2.3 - Ancient Scepter
- Added Ancient Scepter support for StandaloneAncientScepter
- Increased Ethereal Drift's Y-axis velocity (base => 1.7)
- Added new skill: Deeprotted Entropy (Ancient Scepter)
- Added new icon to accompany skill
- Added more config file options (Recharge intervals for all skills)
- Voidcrid's fake Passive is now disabled by default. Config file allows this to be changed.

*Dev notes: The new jump height with Ethereal Drift is less than Acrid's standard leap but more than a standard bump that it was before. This ensures Acrid is still a useful mobile character for scaling the environment but maintains Ethereal Drift's original purpose: escape*

1.2.2 - Config File Update
- Added Config File support. You can find it under "\BepInEx\config\com.6fears7.Voidcrid"

*Dev notes: Added Config file to enable players to control the experience. If you want more options, @me (Unanimate Objec) in Discord*

1.2.1 - All About *H(X) = -sum(each k in K p(k) * log(p(k)))*
- Adjusted Entropy to fire in a three-round burst at 0.3 sec intervals (0.3, 0.6, 0.9)
- Increased Entropy blastAttackForce [knockback] (20f => 1,000f)
- Increased Entropy base damage (baseDamageStat * 0.3 => 3f)
- Increased Entropy's cooldown (4s => 6s)
- Fixed Entropy's health action => damage type. Healing will apply Passive, Hurt will apply Generic.
- Fixed a bug where firing Null Beam consumed all held stacks from Backup Magazine
- Fixed a bug where firing Ethereal Drift consumed all held stacks from Hardlight Afterburners

*Dev notes: Entropy's audio and vfx felt chaotic and disjointed. This update encourages users to activate it to buy a little breathing room.*
*Happy Thanksgiving!*

1.2.0 - Gambler
- Added Passive functionality to Entropy to support Blight or Poison
- Decreased Entropy duration (0.9 sec => 0.6 sec)
- Increased Entropy base damage (baseDamageStat * 0.1 => 0.3)
- Slightly decreased damagecoefficientPerSecond for Null Beam (0.5 => 0.3)
- Decreased Flamebreath recharge interval (1 sec => 0.5 sec)

*Dev notes: Flamebreath needed to be faster to prevent Lemurians constantly trying to play tag.*

*Entropy's gameplay hook needs to reward players for taking a risk and punish them for greed*


1.0.1 - Nerf Gun

- Reduced Special damage output to not obliterate everything. 
- Swapped out Special VFX with new VFX more in line with the Void.
- Added damage percentages to skills in lobby.
- Reduced jail chance on Utility
- Added better icons thanks to @Hifu*

1.0.0 - Initial release

*Dev notes: Best experienced with Artifact of Vengeance to experience true terror. Bring some backup mags.*