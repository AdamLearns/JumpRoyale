# JumpRoyale

A platformer controlled through Twitch chat to be played on my stream during short breaks.

- [JumpRoyale](#jumproyale)
  - [How to play](#how-to-play)
  - [Customizing your character](#customizing-your-character)
  - [Extras](#extras)
  - [Background](#background)
  - [Building, running, testing](#building-running-testing)
  - [Credits](#credits)

---

## How to play

The only way to play is for Adam to be streaming the game. At that point, you can use the following commands in his [Twitch chat](https://twitch.tv/AdamLearnsLive):

- `join`: joins the game. Available at any time, although you'll only really stand a chance at winning if you join in the first ~45 seconds. 😉
- `jump [angle=0] [power=100]`: jumps in the given angle with the specified power.
  - `angle` can be from `-90` to `90`.
    - `-90`: ←
    - `-45`: ↖
    - `0`: ↑
    - `45`: ↗︎
    - `90`: →
  - `power` can be from `1` to `100`.

Jumping is so important that there are lots of shortcuts for how to jump!

- `j` and `r` also jump, e.g. `j 45` or `r 45` to jump ↗︎.
- `l` jumps to the left, e.g. `l 45` to jump ↖.
- `u` jumps straight up.

To bypass the same-message limitation on Twitch, add some garbage letters after commands that you want to repeat:

- `u`: jump up
- `u a`: jump up again
- `u bbbb`: jump up again

---

## Customizing your character

`char <choice>` - change your character graphic. This is only for cosmetics and will be saved between the sessions.

> [!Note]
> Character customization is not saved during the Result Screen

- `choice` is a number from `1` to `18`

| choice  | preview  |
|:-:|:-:|
| **1**  | ![char1](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%201/Clothes%201/Character1M_1_idle_0.png?raw=true)  |
| **2**  | ![char2](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%201/Clothes%202/Character1M_2_idle_0.png?raw=true)  |
| **3**  | ![char3](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%201/Clothes%203/Character1M_3_idle_0.png?raw=true)  |
| **4**  | ![char4](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%202/Clothes%201/Character2M_1_idle_0.png?raw=true)  |
| **5**  | ![char5](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%202/Clothes%202/Character2M_2_idle_0.png?raw=true)  |
| **6**  | ![char6](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%202/Clothes%203/Character2M_3_idle_0.png?raw=true)  |
| **7**  | ![char7](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%203/Clothes%201/Character3M_1_idle_0.png?raw=true)  |
| **8**  | ![char8](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%203/Clothes%202/Character3M_2_idle_0.png?raw=true)  |
| **9**  | ![char9](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%203/Clothes%203/Character3M_3_idle_0.png?raw=true)  |
| **10**  | ![char10](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%201/Clothes%201/Character1F_1_idle_0.png?raw=true)  |
| **11**  | ![char11](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%201/Clothes%202/Character1F_2_idle_0.png?raw=true)  |
| **12**  | ![char12](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%201/Clothes%203/Character1F_3_idle_0.png?raw=true)  |
| **13**  | ![char13](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%202/Clothes%201/Character2F_1_idle_0.png?raw=true)  |
| **14**  | ![char14](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%202/Clothes%202/Character2F_2_idle_0.png?raw=true)  |
| **15**  | ![char15](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%202/Clothes%203/Character2F_3_idle_0.png?raw=true)  |
| **16**  | ![char16](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%203/Clothes%201/Character3F_1_idle_0.png?raw=true)  |
| **17**  | ![char17](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%203/Clothes%202/Character3F_2_idle_0.png?raw=true)  |
| **18**  | ![char18](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%203/Clothes%203/Character3F_3_idle_0.png?raw=true)  |

- `glow [color]`: change your glow color. This is for subscribers, VIPs, and moderators only.
  - If `color` isn't specified, it'll take the color of your Twitch name.
  - `color` is in the form `RGB` or `RRGGBB`, e.g. `color f00` to set it to red, `color f0f` to set it to pink.

---

## Extras

- You can **revive** yourself using channel points. This isn't guaranteed to work, nor should you feel good about winning if you use this feature. 😛

---

## Background

- Designed live (see [the design document](https://docs.google.com/document/d/1YoMtmxC9b5bVoKzm7LxIQ2DxAr3bq84uruarrXFOgQQ/edit))
- Developed hastily in three days (although there'll probably be minor improvements made in the future... 👀)

---

## Building, running, testing

- Prerequisites:
  - Install [DotNet](https://dotnet.microsoft.com/en-us/download)
  - Install [Godot ≥4.2](https://godotengine.org/download/windows/)
  - Clone this repo
  - Generate a twitch token for your twitch account: <https://twitchtokengenerator.com/>. You only need the scopes "chat:read" and "chat:edit" for now. Copy the **access token** and set it:
    - `cd JumpRoyale`
    - `dotnet user-secrets set twitch_access_token <your access token>`
    - `dotnet user-secrets set twitch_channel_name <your channel name>`
  - Ensure that you have a `GODOT4` environment variable:
    - Windows: modify system properties to set the environment variable to something like `C:\myPath\Godot_vx.y.z-stable_mono_win64.exe`
    - macOS: modify your shell's start-up script to add: `export GODOT4="/Applications/Godot_mono.app/Contents/MacOS/Godot"`
- Building:
  - Run `dotnet restore`
  - Open in Godot
  - Click "Build" in Godot itself
- Running:
  - Same as "building", but click "Run" in Godot
- Testing
  - The project was made in 3 days, which already felt like a breakneck pace. You think there were time for tests? 😓

---

## Credits

- Characters: <https://muchopixels.itch.io/character-animation-asset-pack>
- World: <https://pixelfrog-assets.itch.io/pixel-adventure-1>
