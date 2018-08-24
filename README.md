# VRCTools
This is a mod of VRChat to add some of the requested features to VRChat

**<span style="color:red">
This version is deprecated !<br>
The new version is automatically installed by [VRCModLoader](https://github.com/Slaynash/VRCModLoader), and can be found [here](https://github.com/Slaynash/VRCTools).<br>
Also the "Favoite avatar" part is now the [AvatarFav Mod](https://github.com/Slaynash/AvatarFav).
</span>**

**Don't forget to take a look at [the website](https://vrchat.survival-machines.fr) ! :wink:**

Current features:
---

Discord RichPresence:
  - username
  - current world (or "private")
  - world type (public/friends+/friends/invite/invite+)
  - users and max users in instance

Favorite avatars list:
  - you can add the weared avatar to favorite by pressing CTRL+O
  - favorited avatars appear in the "Personal" list
  - you can remove your avatars using [the website](https://vrchat.survival-machines.fr) (top left corner of the page)

Change cache folder:
  - you now have a file named `vrctools_datapath.txt` in your game directory containing the cache path
  - you can change it by hand or using [the installer](https://vrchat.survival-machines.fr/vrctools_updater.jar)

Planned features:
---
- Remove favorited avatars by pressing CTRL+P
- Separated favorite list

Maybe future features:
---
- VR-friendly way to add avatars to favorites (check boxes on avatar image ?)
- Mod injector (to avoid files modification)
- Auto-Update
- In-game log reporting
- Modding API
- Dedicated servers for multiplayer tools
- In-game log viewer

Installation
---

Before install:
- **Tupper (from VRChat Team) said that any modification of the game can lead to a ban, as with this mod**
- You need to know that this program has not been validated by the VRChat team
- Make sure VRCTools is running the same version as the game (0.12.0p12 Build 507)
- DLL_DIR directory is located at YOUR_GAME_FOLDER/VRChat_Data/Managed

Installation (easy):
- Download the mod installer [here](https://vrchat.survival-machines.fr/vrctools_updater.jar) and run it with Java ([download Java](https://java.com/download))

Installation (advanced):
1. Make a backup of DLL_DIR/Assembly-CSharp.dll and DLL_DIR/VRCCore-Standalone.dll
2. Copy the files [Assembly-CSharp.dll](https://github.com/Slaynash/VRCTools/raw/master/bin/Release/Assembly-CSharp.dll), [VRCCore-Standalone.dll](https://github.com/Slaynash/VRCTools/raw/master/bin/Release/VRCCore-Standalone.dll), [VRCTools.dll](https://github.com/Slaynash/VRCTools/raw/master/bin/Release/VRCTools.dll) and [discord-rpc.dll](https://github.com/Slaynash/VRCTools/raw/master/bin/Release/discord-rpc.dll) located in bin/Release/ to DLL_DIR/ (replace files if asked)

Disclaimer:
---
'I' stand for Hugo Flores (Slaynash).

I am not affiliated with VRChat.
This content is for entertainment purpose only, and is provided "AS IS".
I am not responsible of any legal prejudice agaist VRChat, the VRChat team, VRChat community or legals prejudice done with an edited version of this code.

Note for the VRChat Team
---

If you have any request, please send a mail at [slaynash@survival-machines.fr](mailto:slaynash@survival-machines.fr)
