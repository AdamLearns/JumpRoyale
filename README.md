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
  - Note to self: include the 18 images directly from the sprites folder. If you're reading this text, then please remind Adam to do this! 🙏
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
