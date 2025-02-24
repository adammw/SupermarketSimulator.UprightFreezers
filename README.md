# UprightFreezers

Mod for Supermarket Simulator to add Upright Freezers to the game (based on the built-in fridge model).

## Requirements

- [SupermarketSimulator](https://store.steampowered.com/app/2670630/Supermarket_Simulator/)
- [Tobey's BepInEx x MelonLoader Pack for Supermarket Simulator](https://www.nexusmods.com/supermarketsimulator/mods/9)

## Installation

Compile (or fetch a binary release) and install into the `BepInEx/plugins` folder.

When launching the game, two new furniture items will be available in the market: `FreezerA` (representing a single-wide Freezer) and `FreezerB` (representing a double-wide Freezer).

## Configuration

The mod has a configuration file located in `BepInEx/config/UprightFreezers.cfg` that allows you to change the following settings:

- `Cost.FreezerAPrice`: The price of the FreezerA.
- `Cost.FreezerBPrice`: The price of the FreezerB.
- `Products.ScaleMultiplier`: The scale multiplier to apply to the products placed in the Freezers.
- `Visuals.FreezerACustomVisuals`: Enable/disable the custom visuals (Frozen sign) for the FreezerA.
- `Visuals.FreezerBCustomVisuals`: Enable/disable the custom visuals (Frozen sign) for the FreezerB.
- `Visuals.FreezerASignTexture`: Path for a custom texture to use for the FreezerA sign.
- `Visuals.FreezerBSignTexture`: Path for a custom texture to use for the FreezerB sign.