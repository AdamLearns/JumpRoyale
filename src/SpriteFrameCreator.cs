using System.Collections.Generic;
using Godot;

public class SpriteFrameCreator
{
    private static readonly object _lock = new();

    private static SpriteFrameCreator? _instance;

    // Map of string to SpriteFrames
    // Keys are animation names
    private readonly Dictionary<string, SpriteFrames> _allSpriteFrames = new();

    // Map of animation name to number of frames
    private readonly Dictionary<string, int> _numFramesPerAnimation =
        new()
        {
            { JumperAnimations.AnimationIdle, 8 },
            { JumperAnimations.AnimationJump, 2 },
            { JumperAnimations.AnimationFall, 2 },
            { JumperAnimations.AnimationLand, 2 },
        };

    private readonly Dictionary<string, int> _frameratePerAnimation =
        new()
        {
            { JumperAnimations.AnimationIdle, 5 },
            { JumperAnimations.AnimationJump, 10 },
            { JumperAnimations.AnimationFall, 10 },
            { JumperAnimations.AnimationLand, 10 },
        };

    private readonly Dictionary<string, bool> _doesAnimationLoop =
        new()
        {
            { JumperAnimations.AnimationIdle, true },
            { JumperAnimations.AnimationJump, true },
            { JumperAnimations.AnimationFall, true },
            { JumperAnimations.AnimationLand, false },
        };

    public SpriteFrameCreator()
    {
        int numCharacters = 3;
        int numClothings = 3;
        string[] genders = new[] { "m", "f" };

        foreach (string gender in genders)
        {
            for (int charNumber = 1; charNumber <= numCharacters; charNumber++)
            {
                for (int clothingNumber = 1; clothingNumber <= numClothings; clothingNumber++)
                {
                    foreach (string animName in _numFramesPerAnimation.Keys)
                    {
                        Create(gender, charNumber, clothingNumber);
                    }
                }
            }
        }
    }

    // TODO: test this later, thread-safe singleton
    public static SpriteFrameCreator Instance
    {
        get
        {
            lock (_lock)
            {
                _instance ??= new SpriteFrameCreator();
            }

            return _instance;
        }
    }

    public SpriteFrames GetSpriteFrames(string gender, int charNumber, int clothingNumber)
    {
        return _allSpriteFrames[GetAnimationHash(gender, charNumber, clothingNumber)];
    }

    public void Create(string gender, int charNumber, int clothingNumber)
    {
        var spriteFrames = new SpriteFrames();
        foreach (string animName in _numFramesPerAnimation.Keys)
        {
            spriteFrames.AddAnimation(animName);
            string fullGender = gender == "m" ? "Male" : "Female";
            string pathToFolder =
                $"res://assets/sprites/characters/{fullGender}/Character {charNumber}/Clothes {clothingNumber}/";

            int numFrames = _numFramesPerAnimation[animName];
            int framerate = _frameratePerAnimation[animName];

            for (int frameNumber = 0; frameNumber < numFrames; frameNumber++)
            {
                string fileName = $"Character{charNumber}{fullGender[0]}_{clothingNumber}_{animName}_{frameNumber}.png";

                // E.g. "res://assets/sprites/characters/Female/Character 2/Clothes 2/Character2F_2_fall_0.png"
                string pathToImage = $"{pathToFolder}{fileName}";
                Texture2D texture = ResourceLoader.Load<Texture2D>(pathToImage);

                spriteFrames.AddFrame(animName, texture, 1);
            }

            spriteFrames.SetAnimationSpeed(animName, framerate);
            spriteFrames.SetAnimationLoop(animName, _doesAnimationLoop[animName]);
        }

        _allSpriteFrames[GetAnimationHash(gender, charNumber, clothingNumber)] = spriteFrames;
    }

    private string GetAnimationHash(string gender, int charNumber, int clothingNumber)
    {
        return $"{gender}_{charNumber}_{clothingNumber}";
    }
}
