# NukesFusionServerUtilities

This mod will contain small additions to make navigating and managing Fusion servers a bit easier.

The mod currently contains a Panic Button feature described below, as well as a spawn limiting system that only the host
is required to have (also described below).

## Features

### Prop Limits & Spawner Identification

Attaches a metadata property to every player server-side to track how many network entities they've spawned. If this
property exceeds a certain configurable value, it will prevent the player from spawning anything else.

**If this mod is also installed client-side, it will notify the player when the server considers this limit reached.**

It also has a setting to disallow people from despawning others' spawnables.

### Panic Button

Meant to prevent chaotically laggy servers or stuck players in an emergency.

Spam the menu button on your LEFT controller (or B on your keyboard) 10 times (by default; configurable) to activate.

Upon activation, it will teleport you back to spawn and change your avatar to Polyblank (toggleable). If you are the
host of a Fusion server, you get the option of using SceneJanitor, a complete scene reload, or nothing to clean up the
level on trigger.

---

*You can find all config options for each feature in the NukesFusionServerUtilities tab in the BoneMenu.*

## Footnotes

- Icon credit: https://www.flaticon.com/free-icon/fusion_4708047?term=nuclear+fusion&page=1&position=1&origin=search&related_id=4708047
- Made by `the_ultimatenuke` on Discord, or `The_UltimateNuke` on Fusion.