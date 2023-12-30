# JumpRoyale

A platformer controlled through Twitch chat to be played on my stream during short breaks.

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

## Customizing your character

- `char <choice>`: change your character graphic. This is only for cosmetics.
  - `choice` is a number from `1` to `18`
  - `char 1` ↓
    - ![](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%201/Clothes%201/Character1M_1_idle_0.png?raw=true)
  - `char 2` ↓
    - ![](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%201/Clothes%202/Character1M_2_idle_0.png?raw=true)
  - `char 3` ↓
    - ![](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%201/Clothes%203/Character1M_3_idle_0.png?raw=true)
  - `char 4` ↓
    - ![](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%202/Clothes%201/Character2M_1_idle_0.png?raw=true)
  - `char 5` ↓
    - ![](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%202/Clothes%202/Character2M_2_idle_0.png?raw=true)
  - `char 6` ↓
    - ![](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%202/Clothes%203/Character2M_3_idle_0.png?raw=true)
  - `char 7` ↓
    - ![](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%203/Clothes%201/Character3M_1_idle_0.png?raw=true)
  - `char 8` ↓
    - ![](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%203/Clothes%202/Character3M_2_idle_0.png?raw=true)
  - `char 9` ↓
    - ![](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%203/Clothes%203/Character3M_3_idle_0.png?raw=true)
  - `char 10` ↓
    - ![](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%201/Clothes%201/Character1F_1_idle_0.png?raw=true)
  - `char 11` ↓
    - ![](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%201/Clothes%202/Character1F_2_idle_0.png?raw=true)
  - `char 12` ↓
    - ![](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%201/Clothes%203/Character1F_3_idle_0.png?raw=true)
  - `char 13` ↓
    - ![](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%202/Clothes%201/Character2F_1_idle_0.png?raw=true)
  - `char 14` ↓
    - ![](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%202/Clothes%202/Character2F_2_idle_0.png?raw=true)
  - `char 15` ↓
    - ![](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%202/Clothes%203/Character2F_3_idle_0.png?raw=true)
  - `char 16` ↓
    - ![](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%203/Clothes%201/Character3F_1_idle_0.png?raw=true)
  - `char 17` ↓
    - ![](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%203/Clothes%202/Character3F_2_idle_0.png?raw=true)
  - `char 18` ↓
    - ![](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%203/Clothes%203/Character3F_3_idle_0.png?raw=true)
- `glow [color]`: change your glow color. This is for subscribers, VIPs, and moderators only.
  - If `color` isn't specified, it'll take the color of your Twitch name.
  - `color` is in the form `RGB` or `RRGGBB`, e.g. `color f00` to set it to red, `color f0f` to set it to pink.

## Extras

- You can **revive** yourself using channel points. This isn't guaranteed to work, nor should you feel good about winning if you use this feature. 😛

## Background

- Designed live (see [the design document](https://docs.google.com/document/d/1YoMtmxC9b5bVoKzm7LxIQ2DxAr3bq84uruarrXFOgQQ/edit))
- Developed hastily in three days (although there'll probably be minor improvements made in the future... 👀)

## Building, running, testing

- Building:
  - Clone the repo
  - Run `dotnet restore`
  - Open in Godot ≥4.2
  - Click "Build" in Godot itself
- Running:
  - Same as "building", but click "Run" in Godot
- Testing
  - The project was made in 3 days, which already felt like a breakneck pace. You think there were time for tests? 😓

## Credits

- Characters: https://muchopixels.itch.io/character-animation-asset-pack
- World: https://pixelfrog-assets.itch.io/pixel-adventure-1
