# Gloomhaven Fast Load
This mod loads the main menu faster by improving parallelism during startup.

Parallelism is achieved by loading the main scene while the intro videos are playing, which can cause significant lag. The amount of improvement is going to vary between machines, and will depend a lot on your hardware. On my machine, the time to get to the main menu went from 80 seconds to about 45 seconds.

## Installation
1. Install BepInEx ([Windows](https://docs.bepinex.dev/articles/user_guide/installation/index.html?tabs=tabid-win) | [Mac/Linux](https://docs.bepinex.dev/articles/user_guide/installation/index.html?tabs=tabid-nix))
2. Download `FastLoad.dll` from the [latest release](https://github.com/gummyboars/gloomhaven-fastload/releases)
3. Copy `FastLoad.dll` to the `BepInEx/plugins/` folder
4. Proton/Wine users only: [Configure wine](https://docs.bepinex.dev/articles/advanced/proton_wine.html) for BepInEx

## Compatibility
This mod is client-side only. You may safely play with others that do not have the mod.
This mod does not affect save files.
