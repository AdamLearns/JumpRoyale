using System.Collections.Generic;
using Godot;

public class SpriteFrameCreator
{
  public static SpriteFrameCreator Instance = new SpriteFrameCreator();

  public static SpriteFrameCreator getInstance()
  {
    return Instance;
  }

  // Map of string to SpriteFrames
  // Keys are animation names
  private Dictionary<string, SpriteFrames> allSpriteFrames = new Dictionary<string, SpriteFrames>();

  // Map of animation name to number of frames
  private Dictionary<string, int> numFramesPerAnimation = new Dictionary<string, int>()
  {
    { JumperAnimations.AnimationIdle, 8 },
    { JumperAnimations.AnimationJump, 2 },
    { JumperAnimations.AnimationFall, 2 },
    { JumperAnimations.AnimationLand, 2 },
  };

  private Dictionary<string, int> frameratePerAnimation = new Dictionary<string, int>()
  {
    { JumperAnimations.AnimationIdle, 5 },
    { JumperAnimations.AnimationJump, 10 },
    { JumperAnimations.AnimationFall, 10 },
    { JumperAnimations.AnimationLand, 10 },
  };

  private Dictionary<string, bool> doesAnimationLoop = new Dictionary<string, bool>()
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
          foreach (var animName in numFramesPerAnimation.Keys)
          {
            Create(gender, charNumber, clothingNumber);
          }
        }
      }
    }
  }

  public SpriteFrames GetSpriteFrames(string gender, int charNumber, int clothingNumber)
  {
    return allSpriteFrames[getAnimationHash(gender, charNumber, clothingNumber)];
  }

  private string getAnimationHash(string gender, int charNumber, int clothingNumber)
  {
    return $"{gender}_{charNumber}_{clothingNumber}";
  }

  public void Create(string gender, int charNumber, int clothingNumber)
  {
    var spriteFrames = new SpriteFrames();
    foreach (var animName in numFramesPerAnimation.Keys)
    {

      spriteFrames.AddAnimation(animName);
      var fullGender = gender == "m" ? "Male" : "Female";
      var pathToFolder = $"res://assets/sprites/characters/{fullGender}/Character {charNumber}/Clothes {clothingNumber}/";

      var numFrames = numFramesPerAnimation[animName];
      var framerate = frameratePerAnimation[animName];

      for (int frameNumber = 0; frameNumber < numFrames; frameNumber++)
      {
        var fileName = $"Character{charNumber}{fullGender[0]}_{clothingNumber}_{animName}_{frameNumber}.png";

        // res://assets/sprites/characters/Female/Character 2/Clothes 2/Character2F_2_fall_0.png
        var pathToImage = $"{pathToFolder}{fileName}";
        var texture = ResourceLoader.Load<Texture2D>(pathToImage);

        spriteFrames.AddFrame(animName, texture, 1);
      }

      spriteFrames.SetAnimationSpeed(animName, framerate);
      spriteFrames.SetAnimationLoop(animName, doesAnimationLoop[animName]);

    }

    allSpriteFrames[getAnimationHash(gender, charNumber, clothingNumber)] = spriteFrames;

  }
}
