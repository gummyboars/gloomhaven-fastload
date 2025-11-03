# Gloomhaven Fast Load
This mod loads the main menu faster by improving parallelism during startup.

Parallelism is achieved by loading the main scene while the intro videos are playing, which can cause significant lag. The amount of improvement is going to vary between machines, and will depend a lot on your hardware. On my machine, the time to get to the main menu went from 80 seconds to about 45 seconds.

## Installation
1. Install BepInEx 5 (Stable) ([Windows/SteamDeck/Linux](https://docs.bepinex.dev/articles/user_guide/installation/index.html?tabs=tabid-win#where-to-download-bepinex) | [Mac](https://docs.bepinex.dev/articles/user_guide/installation/index.html?tabs=tabid-nix#where-to-download-bepinex))
2. Download `FastLoad.dll` from the [latest release](https://github.com/gummyboars/gloomhaven-fastload/releases)
3. Copy `FastLoad.dll` to the `BepInEx/plugins/` folder
4. SteamDeck/Proton/Wine users only: [Configure wine](https://docs.bepinex.dev/articles/advanced/proton_wine.html) for BepInEx

## Compatibility
This mod is client-side only. You may safely play with others that do not have the mod.
This mod does not affect save files.

## Other Mods
You may be interested in other mods for Gloomhaven:
- [Bug Fixes](https://github.com/gummyboars/gloomhaven-bugfixes)
- [Camera Tweaks](https://github.com/gummyboars/gloomhaven-camera)
- [UI Tweaks](https://github.com/gummyboars/gloomhaven-uitweaks)
