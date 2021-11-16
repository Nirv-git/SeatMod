# SeatMod


SeatMod allows you to parent yourself to another player's head, or other bones in a variety of methods.  In order to sit on others you will need [*consent*](#Consent-and-Risky-Functions) from them.   

Note: All the movements are local to you and take time to be networked to others. If the person you are parented/sitting on isn't moving much, it will look fine. If they are running around it will be noticeable. 

*Modding VRC is against their TOS and this mod visually and obviously does something that isn't possible with the native game. Only use around those you trust.*

**Requires: [UIX](https://api.vrcmg.com/v0/mods/55/UIExpansionKit.dll)**  
**Supports: [AMAPI](https://api.vrcmg.com/v0/mods/201/ActionMenuApi.dll) (Highly recommended)**  

* How to interact with this mod 
	* UIX QuickMenu button "Sit Menu" for Settings
	* UIX _User_ QuickMenu button "Sit On..." to open the sit menu (Select someone with your QM open) 
	* AMAPI - Has Unsit and Setting adjustments implemented in your radial ActionMenu


## **Sit Menu/Options**
Why the different modes? Because I like making this overly complicated! 
**In short:**
|Mode| Desc |
|--|--|
| Parent to Head | Simple, just toss on head. Only position adjust, no rotation |
| Sit with Chair | Puts you in a chair, to any bone, position adjust and full rotation support |
|Parent to - | Like 'Parent to Head' but to any bone and has limited rotation support |

**In long:**
* **Parent to Head**
	* This is the simplest of the modes and simply will move you to follow the target avatar's head / IK effector
* Sit/Parent to Bone
	* This menu lets you select any bone on an avatar for you to sit/parent to. It will default to the Head bone, or remember your last position in the menu. The 'Humanoid Shortcuts' button can be useful for quickly moving around the hierarchy. 
		* **Sit with Chair  - _bone_**  ***(Works best)***
			* This will seat you in a chair that is parented to the specific bone. 
			 * You can toggle between 3 rotation options: Adjustments Only, Yaw Rotation, Full Rotation
			* There are a selection of chair animations you can use   
				* If you are using FBT with IKTweaks: 'Animations mode in FBT' set to 'Ignore all', your pose should be your normal FBT one as IKT with that setting overrides chair animations
		* **Parent to - _bone_**
			* This will make your motions follow a specific bone, similar to "Parent to Head"
			* You can toggle between 3 rotation options: None,  Adjustments Only, Yaw Rotation 
  
  
* **Options** - This menu can be reached from your QuickMenu UIX **Sit Menu** button
	* Unsit **|** Does what it says. You can also press "**9**" on your keyboard break out of a sit
	* Position Adjust **|**  All modes allow you to use this menu to fine tune to your location
	* Rotation Adjust **|** Will adjust your rotation on the bone depending on the rotation option selected
		* Parent to Head: No Adjustments
		* Sit with Chair - _bone_: All rotation options
		* Parent to - _bone_: 'Adjustments Only', 'Yaw Rotation'
	* Settings 
		* Stored Positions/Rotations **|** Allows you to save and load 6 presets each
		* Chair Rotation **|** Changes the Rotation Setting for "Sit with Chair"
		* Chair Animations **|** Changes the Chair animation for "Sit with Chair" - If you are currently sitting and change this it will reload the chair
		* No Falling Animations **|** Suppresses the falling animation you can get with "Parent to Head" and "Parent to - _bone_"
		* Parent Rotation **|** Changes the Rotation Setting for "Parent to - _bone_"
		* Personal Pin **|** Shows your personal consent code that others can add to their Status/Bio


## Consent and Risky Functions
_**Aka: Can this be used maliciously!?**_  
   
SeatMod has a few safety checks in it
* Consent
	* SeatMod requires the target player to have a consent code/phrase in their Status or Bio.
		* The following are the currently used consent codes. _(Not case sensitive)_
			* The individual's private code
			* "siton"
			* "sit with me"
			* "seats together"
		* **Note: A user's Status updates much quicker then a user's Bio. If you want to use this mod immediately with someone, have them place one of the codes in their Status**
  * If the user is on 'Do Not Distrub', or they have the phrase 'nosit' in their status, the mod will not allow you to parent to them.
* Risky Function Check
	* Much like other mods that allow you to teleport or move in ways VRC did not intend, SeatMod implements a risky function check. 
		* The mod can not be used in a Public/Friends+ instance that is a Game or Club world, or world is blacklisted in emmVRC's database or it has a blacklist GameObject in the world.

## Screenshots! 
### AMAPI Support 
![image](https://user-images.githubusercontent.com/81605232/136749353-3ff92683-6e13-4024-907c-2319f17e54aa.png)
### Example of sitting on a user's head with a chair
![image](https://user-images.githubusercontent.com/81605232/137252900-e24f10ca-af15-480e-8ae7-1171524e4675.png)



