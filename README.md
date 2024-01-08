# JumpRoyale

A platformer controlled through Twitch chat to be played on my stream during short breaks.

- [JumpRoyale](#jumproyale)
  - [How to play](#how-to-play)
    - [Tips](#tips)
  - [Customizing your character](#customizing-your-character)
  - [Extras](#extras)
  - [Background](#background)
  - [Building, running, testing](#building-running-testing)
    - [Testing](#testing)
      - [Running Tests](#running-tests)
  - [Credits](#credits)

---

## How to play

The only way to play is for Adam to be streaming the game. At that point, you can use the following commands in his [Twitch chat](https://twitch.tv/AdamLearnsLive):

- `join`: joins the game. Available at any time, although you'll only really stand a chance at winning if you join in the first ~45 seconds. 😉

To jump, you have to send one of the following commands in the chat: `l`, `u`, `r` (also `j` as alias).

- `l` - jump left if `angle` is greater than `0`, e.g. `l 45` to jump ↖
- `r` or `j` - jump right, if `angle` is greater than `0`, e.g. `j 45` or `r 45` to jump ↗︎
- `u` - jump up, **does not require `angle`**, but allows specifying `power` for weaker jumps

Jump commands are currently accepted in the following format:

- `<direction> [angle] [power]` - where default values for `angle` and `power` are `0` and `100` respectively
  - `angle` can be from `-90` to `90`.
  - `power` can be from `1` to `100`.

| angle input    | -90 | -45 | 0   | 45  | 90  |
| -------------- | --- | --- | --- | --- | --- |
| jump direction | ←   | ↖   | ↑   | ↗︎   | →   |

> [!note]
> While `angle` is clamped between `-90` and `90`, that does not mean you have to put negative numbers in. This syntax was left in since `j` became an alias - you can use `j -30` to jump left or `j 30` to jump right! Sometimes it's more convenient to stay on the `j` key, so this might be a more preferred way to some players

To bypass the same-message limitation on Twitch, add some garbage letters after commands that you want to repeat:

- `u`: jump up
- `u a`: jump up again
- `u bbbb`: jump up again

Jumping in the same direction also works with garbage letters:

- `l 5`
- `l 5 aaaa`
- `l 5 bbb`

### Tips

> [!tip]
> Most important: you don't need to put `space` between the command and `angle`, you can simply send `l30` to jump 30 degrees to the left (mind the angle input above: `0` degrees = `up`)

Other tips:

- you can alternate between `l` and `r` commands to jump up in place, avoiding twitch duplicate message restriction. Jumping left or right with no angle specified allows you to jump up
- if you are using 7TV chat extension, it has its own duplicate message block prevention, so no additional garbage in the chat message is needed

---

## Customizing your character

Sending `char <choice>` in the chat allows you to change your character graphic. This is only for cosmetics and will be saved between the sessions.

> [!Note]
> Character customization is not saved during the Result Screen

- `choice` is a number from `1` to `18`

| choice |                                                                          preview                                                                           |
| :----: | :--------------------------------------------------------------------------------------------------------------------------------------------------------: |
| **1**  |  ![char1](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%201/Clothes%201/Character1M_1_idle_0.png?raw=true)   |
| **2**  |  ![char2](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%201/Clothes%202/Character1M_2_idle_0.png?raw=true)   |
| **3**  |  ![char3](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%201/Clothes%203/Character1M_3_idle_0.png?raw=true)   |
| **4**  |  ![char4](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%202/Clothes%201/Character2M_1_idle_0.png?raw=true)   |
| **5**  |  ![char5](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%202/Clothes%202/Character2M_2_idle_0.png?raw=true)   |
| **6**  |  ![char6](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%202/Clothes%203/Character2M_3_idle_0.png?raw=true)   |
| **7**  |  ![char7](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%203/Clothes%201/Character3M_1_idle_0.png?raw=true)   |
| **8**  |  ![char8](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%203/Clothes%202/Character3M_2_idle_0.png?raw=true)   |
| **9**  |  ![char9](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Male/Character%203/Clothes%203/Character3M_3_idle_0.png?raw=true)   |
| **10** | ![char10](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%201/Clothes%201/Character1F_1_idle_0.png?raw=true) |
| **11** | ![char11](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%201/Clothes%202/Character1F_2_idle_0.png?raw=true) |
| **12** | ![char12](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%201/Clothes%203/Character1F_3_idle_0.png?raw=true) |
| **13** | ![char13](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%202/Clothes%201/Character2F_1_idle_0.png?raw=true) |
| **14** | ![char14](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%202/Clothes%202/Character2F_2_idle_0.png?raw=true) |
| **15** | ![char15](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%202/Clothes%203/Character2F_3_idle_0.png?raw=true) |
| **16** | ![char16](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%203/Clothes%201/Character3F_1_idle_0.png?raw=true) |
| **17** | ![char17](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%203/Clothes%202/Character3F_2_idle_0.png?raw=true) |
| **18** | ![char18](https://github.com/AdamLearns/JumpRoyale/blob/main/assets/sprites/characters/Female/Character%203/Clothes%203/Character3F_3_idle_0.png?raw=true) |

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

### Testing

> [!note]
> This section will remain here until someone manages to get a proper testing framework running :wink:

For now, very limited and simple Unit Tests are available thanks to the `GDMUT` Godot Plugin until a good `nunit/xunit` solutions are implemented.

To define a new test, create a new test under `Tests` directory (they don't have to be there, it's just to keep them in one place, they are read from Assembly anyway), insert a simple class:

```csharp
namespace Test {

    public class SomeTest
    {
        [CSTestFunction]
        public static Result ThisTestsSomething()
        {
            // Do whatever here
        }
    }
}
```

To pass a test, methods must return one of the following:

- `new Result()` - overloads available, `bool`, `string`
- `Result.Success`

Tests fail, with the following returns:

- `new Result(false)`
- `Result.Failure`
- or when Exception was thrown somewhere

> [!caution]
> Unfortunately, if any of the tests throws an exception, it won't get caught and you will be left with a vague message (screenshot below). Mono exceptions are not logged, the suggestion is to print errors instead (thanks, Godot)

![exception](https://github.com/DarkStoorM/JumpRoyale/assets/7021295/28fc2adf-ca5f-46c5-80ff-197071591117)

#### Running Tests

---

## Credits

- Characters: <https://muchopixels.itch.io/character-animation-asset-pack>
- World: <https://pixelfrog-assets.itch.io/pixel-adventure-1>
