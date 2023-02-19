using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// NEED TO SHOW NIGHTIME APPEARANCE ON SPRITE. SETTING COLOUR OF SKIN SHOULD SET NIGHTIME COLOUR OF SKIN AND THE SPRITE SHOULD BE ADDED TO DAY NIGHT FEATURE

[BurstCompile]
public class CharacterCreationEngine : MonoBehaviour
{
    public bool isCreatingCharacter = false;

    public GameObject firstSelected = null;

    public TitleScreenEngine titleScreenEngine = null;
    public OptionEngine optionEngine = null;
    public PlayerEngine playerEngine = null;

    public BlockEngine blockEngine = null;

    public SunOrbit sunOrbit = null;

    public Canvas canvas = null;

    public CreatedCharacterAnimation createdCharacterAnimation = null;
    public HumanFeatures humanFeatures = null;

    public PlayerFeatureAnimation playerFeatureAnimation = null;

    public int raceType = 0;
    public int genderType = 0;
    public int hairType = 1;
    public int beardType = 1;
    public int moustacheType = 1;
    public int hornType = 1;

    public Color eyeColor = Color.white;
    public Color skinColor = Color.white;
    public Color hairColor = Color.white;
    public Color eyeBrowColor = Color.white;

    public Color beardColor = Color.white;
    public Color moustacheColor = Color.white;

    public SpriteRenderer characterImage = null;
    public SpriteRenderer eyeImage = null;
    public SpriteRenderer hairImage = null;
    public SpriteRenderer eyeBrowImage = null;
    public SpriteRenderer eyeBackgroundImage = null;

    public SpriteRenderer beardImage = null;
    public SpriteRenderer moustacheImage = null;

    public float MAX_DIFFERENCE_ALLOWED = 0.25f;    // amount of difference allowed between Color range values (0-1 Red-green-blue)

    public Slider redEyeSlider = null;
    public Slider greenEyeSlider = null;
    public Slider blueEyeSlider = null;

    public Slider redSkinSlider = null;
    public Slider greenSkinSlider = null;
    public Slider blueSkinSlider = null;

    public Slider redHairSlider = null;
    public Slider greenHairSlider = null;
    public Slider blueHairSlider = null;

    public Slider redEyeBrowSlider = null;
    public Slider greenEyeBrowSlider = null;
    public Slider blueEyeBrowSlider = null;

    public Slider redBeardSlider = null;
    public Slider greenBeardSlider = null;
    public Slider blueBeardSlider = null;

    public Slider redMoustacheSlider = null;
    public Slider greenMoustacheSlider = null;
    public Slider blueMoustacheSlider = null;

    [BurstCompile]
    public void OnClickNewGame()
    {
        // Title screen has update function.
        // Need to stop gameobject from updating
        titleScreenEngine.gameObject.SetActive(false);

        // User interface management - when its be clicked deselect it so it does not stay "lit" (visuals)
        EventSystem.current.SetSelectedGameObject(null);

        //main.titleScreenEngine.titleCanvas.SetActive(false);
        titleScreenEngine.characterCreationEngine.DisableCanvas();

        titleScreenEngine.onTitleScreen = false;
        optionEngine.inGame = true;

        // Set colour of sprite in game
        playerEngine.playerAnimation.spriteRenderer.material.color = skinColor;

        playerFeatureAnimation.hairSprite.color = hairColor;
        playerFeatureAnimation.eyeSprite.color = eyeColor;
        playerFeatureAnimation.eyeBrowSprite.color = eyeBrowColor;
        playerFeatureAnimation.beardSprite.color = beardColor;
        playerFeatureAnimation.moustacheSprite.color = moustacheColor;

        // change actual sprites

        playerFeatureAnimation.hairSprite.sprite = humanFeatures.hairs[hairType - createdCharacterAnimation.facing];
        playerFeatureAnimation.beardSprite.sprite = humanFeatures.beards[beardType - createdCharacterAnimation.facing];
        playerFeatureAnimation.moustacheSprite.sprite = humanFeatures.moustache[moustacheType - createdCharacterAnimation.facing];

        sunOrbit.playerDayTimeColour = skinColor;
        sunOrbit.playerNightTimeColour = new Color(skinColor.r - 0.1f, skinColor.g - 0.1f, skinColor.b - 0.1f);

        // disable sprite being editted
        characterImage.gameObject.SetActive(false);

        // Start the world generation

        blockEngine.StartCoroutineFresh();

        // disable the character creation behaviour (LateUpdate)
        gameObject.SetActive(false);
    }

    [BurstCompile]
    public void OnClickBack()
    {
        isCreatingCharacter = false;
        DisableCanvas();
        titleScreenEngine.titleCanvas.SetActive(true);

        // disable sprite being editted
        characterImage.gameObject.SetActive(false);

        EventSystem.current.firstSelectedGameObject = titleScreenEngine.newButton;
        EventSystem.current.SetSelectedGameObject(titleScreenEngine.newButton);

        titleScreenEngine.currentMenu = TitleScreenEngine.Menu.Title;
    }

    [BurstCompile]
    public void OnClickIterateBeard()
    {
        if (beardType != humanFeatures.beards.Length - 1 - (2 - createdCharacterAnimation.facing)) // note extra -1 to be front
        {
            beardType += 3;     // adjust by 3 as sprite array hold side-front-back
        }
        else
        {
            beardType = createdCharacterAnimation.facing;
        }

        beardImage.sprite = humanFeatures.beards[beardType];
    }

    [BurstCompile]
    public void OnClickDecrementBeard()
    {
        if (beardType != createdCharacterAnimation.facing)
        {
            beardType -= 3;     // adjust by 3 as sprite array hold side-front-back
        }
        else
        {
            beardType = humanFeatures.beards.Length - 1 - (2 - createdCharacterAnimation.facing);    // note extra -1 to be front
        }

        beardImage.sprite = humanFeatures.beards[beardType];
    }

    // ------------------------------

    [BurstCompile]
    public void OnClickIterateHair()
    {
        if (hairType != humanFeatures.hairs.Length - 1 - (2 - createdCharacterAnimation.facing)) // note extra -1 to be front
        {
            hairType += 3;     // adjust by 3 as sprite array hold side-front-back
        }
        else
        {
            hairType = createdCharacterAnimation.facing;
        }

        hairImage.sprite = humanFeatures.hairs[hairType];
    }

    [BurstCompile]
    public void OnClickDecrementHair()
    {
        if (hairType != createdCharacterAnimation.facing)
        {
            hairType -= 3;     // adjust by 3 as sprite array hold side-front-back
        }
        else
        {
            hairType = humanFeatures.hairs.Length - 1 - (2 - createdCharacterAnimation.facing);    // note extra -1 to be front
        }

        hairImage.sprite = humanFeatures.hairs[hairType];
    }

    // -------------------------------------

    [BurstCompile]
    public void OnClickIterateMoustache()
    {
        if (moustacheType != humanFeatures.moustache.Length - 1 - (2 - createdCharacterAnimation.facing)) // note extra -1 to be front
        {
            moustacheType += 3;     // adjust by 3 as sprite array hold side-front-back
        }
        else
        {
            moustacheType = createdCharacterAnimation.facing;
        }

        moustacheImage.sprite = humanFeatures.moustache[moustacheType];
    }

    [BurstCompile]
    public void OnClickDecrementMoustache()
    {
        if (moustacheType != createdCharacterAnimation.facing)
        {
            moustacheType -= 3;     // adjust by 3 as sprite array hold side-front-back
        }
        else
        {
            moustacheType = humanFeatures.moustache.Length - 1 - createdCharacterAnimation.facing;    // note extra -1 to be front
        }

        moustacheImage.sprite = humanFeatures.moustache[moustacheType];
    }

    // -----------------------------------

    [BurstCompile]
    public void EnableCanvas()
    {
        canvas.gameObject.SetActive(true);
    }

    [BurstCompile]
    public void DisableCanvas()
    {
        canvas.gameObject.SetActive(false);
    }

    // ------------------------------

    [BurstCompile]
    public void AdjustRedSkinColour(float sliderValue)
    {
        // Keep R-G-B value close together to not allow overly bright colours
        ForceAdjustGreenSkinColour(sliderValue);
        ForceAdjustBlueSkinColour(sliderValue);

        skinColor.r = sliderValue;

        characterImage.material.color = skinColor;
    }

    [BurstCompile]
    public void AdjustGreenSkinColour(float sliderValue)
    {
        // Keep R-G-B value close together to not allow overly bright colours
        ForceAdjustRedSkinColour(sliderValue);
        ForceAdjustBlueSkinColour(sliderValue);

        skinColor.g = sliderValue;

        characterImage.material.color = skinColor;
    }

    [BurstCompile]
    public void AdjustBlueSkinColour(float sliderValue)
    {
        // Keep R-G-B value close together to not allow overly bright colours
        ForceAdjustRedSkinColour(sliderValue);
        ForceAdjustGreenSkinColour(sliderValue);

        skinColor.b = sliderValue;

        characterImage.material.color = skinColor;
    }

    // ---------------------------

    [BurstCompile]
    public void ForceAdjustRedSkinColour(float sliderValue)
    {
        // Move towards the slider value if required.

        if (skinColor.r > sliderValue + MAX_DIFFERENCE_ALLOWED)
        {
            skinColor.r = sliderValue + MAX_DIFFERENCE_ALLOWED;
        }
        else
        {
            if (skinColor.r < sliderValue - MAX_DIFFERENCE_ALLOWED)
            {
                skinColor.r = sliderValue - MAX_DIFFERENCE_ALLOWED;
            }
        }

        // Set the value adjusted here on the slider bar to help make more 
        // sense to the user that colours are being forceably changed to limit selection

        redSkinSlider.value = skinColor.r;
    }

    [BurstCompile]
    public void ForceAdjustGreenSkinColour(float sliderValue)
    {
        // Move towards the slider value if required.

        if (skinColor.g > sliderValue + MAX_DIFFERENCE_ALLOWED)
        {
            skinColor.g = sliderValue + MAX_DIFFERENCE_ALLOWED;
        }
        else
        {
            if (skinColor.g < sliderValue - MAX_DIFFERENCE_ALLOWED)
            {
                skinColor.g = sliderValue - MAX_DIFFERENCE_ALLOWED;
            }
        }

        // Set the value adjusted here on the slider bar to help make more 
        // sense to the user that colours are being forceably changed to limit selection

        greenSkinSlider.value = skinColor.g;
    }

    [BurstCompile]
    public void ForceAdjustBlueSkinColour(float sliderValue)
    {
        // Move towards the slider value if required.

        if (skinColor.b > sliderValue + MAX_DIFFERENCE_ALLOWED)
        {
            skinColor.b = sliderValue + MAX_DIFFERENCE_ALLOWED;
        }
        else
        {
            if (skinColor.b < sliderValue - MAX_DIFFERENCE_ALLOWED)
            {
                skinColor.b = sliderValue - MAX_DIFFERENCE_ALLOWED;
            }
        }

        // Set the value adjusted here on the slider bar to help make more 
        // sense to the user that colours are being forceably changed to limit selection

        blueSkinSlider.value = skinColor.b;
    }

    // ---------------------------------------

    [BurstCompile]
    public void AdjustRedEyeColour(float sliderValue)
    {
        // Keep R-G-B value close together to not allow overly bright colours
        ForceAdjustGreenEyeColour(sliderValue);
        ForceAdjustBlueEyeColour(sliderValue);

        eyeColor.r = sliderValue;

        eyeImage.material.color = eyeColor;
    }

    [BurstCompile]
    public void AdjustGreenEyeColour(float sliderValue)
    {
        // Keep R-G-B value close together to not allow overly bright colours
        ForceAdjustRedEyeColour(sliderValue);
        ForceAdjustBlueEyeColour(sliderValue);

        eyeColor.g = sliderValue;

        eyeImage.material.color = eyeColor;
    }

    [BurstCompile]
    public void AdjustBlueEyeColour(float sliderValue)
    {
        // Keep R-G-B value close together to not allow overly bright colours
        ForceAdjustRedEyeColour(sliderValue);
        ForceAdjustGreenEyeColour(sliderValue);

        eyeColor.b = sliderValue;

        eyeImage.material.color = eyeColor;
    }

    // ---------------------------

    [BurstCompile]
    public void ForceAdjustRedEyeColour(float sliderValue)
    {
        // Move towards the slider value if required.

        if (eyeColor.r > sliderValue + MAX_DIFFERENCE_ALLOWED)
        {
            eyeColor.r = sliderValue + MAX_DIFFERENCE_ALLOWED;
        }
        else
        {
            if (eyeColor.r < sliderValue - MAX_DIFFERENCE_ALLOWED)
            {
                eyeColor.r = sliderValue - MAX_DIFFERENCE_ALLOWED;
            }
        }

        // Set the value adjusted here on the slider bar to help make more 
        // sense to the user that colours are being forceably changed to limit selection

        redEyeSlider.value = eyeColor.r;
    }

    [BurstCompile]
    public void ForceAdjustGreenEyeColour(float sliderValue)
    {
        // Move towards the slider value if required.

        if (eyeColor.g > sliderValue + MAX_DIFFERENCE_ALLOWED)
        {
            eyeColor.g = sliderValue + MAX_DIFFERENCE_ALLOWED;
        }
        else
        {
            if (eyeColor.g < sliderValue - MAX_DIFFERENCE_ALLOWED)
            {
                eyeColor.g = sliderValue - MAX_DIFFERENCE_ALLOWED;
            }
        }

        // Set the value adjusted here on the slider bar to help make more 
        // sense to the user that colours are being forceably changed to limit selection

        greenEyeSlider.value = eyeColor.g;
    }

    [BurstCompile]
    public void ForceAdjustBlueEyeColour(float sliderValue)
    {
        // Move towards the slider value if required.

        if (eyeColor.b > sliderValue + MAX_DIFFERENCE_ALLOWED)
        {
            eyeColor.b = sliderValue + MAX_DIFFERENCE_ALLOWED;
        }
        else
        {
            if (eyeColor.b < sliderValue - MAX_DIFFERENCE_ALLOWED)
            {
                eyeColor.b = sliderValue - MAX_DIFFERENCE_ALLOWED;
            }
        }

        // Set the value adjusted here on the slider bar to help make more 
        // sense to the user that colours are being forceably changed to limit selection

        blueEyeSlider.value = eyeColor.b;
    }

    // ---------------------------------------

    [BurstCompile]
    public void AdjustRedHairColour(float sliderValue)
    {
        // Keep R-G-B value close together to not allow overly bright colours
        ForceAdjustGreenHairColour(sliderValue);
        ForceAdjustBlueHairColour(sliderValue);

        hairColor.r = sliderValue;

        hairImage.material.color = hairColor;
    }

    [BurstCompile]
    public void AdjustGreenHairColour(float sliderValue)
    {
        // Keep R-G-B value close together to not allow overly bright colours
        ForceAdjustRedHairColour(sliderValue);
        ForceAdjustBlueHairColour(sliderValue);

        hairColor.g = sliderValue;

        hairImage.material.color = hairColor;
    }

    [BurstCompile]
    public void AdjustBlueHairColour(float sliderValue)
    {
        // Keep R-G-B value close together to not allow overly bright colours
        ForceAdjustRedHairColour(sliderValue);
        ForceAdjustGreenHairColour(sliderValue);

        hairColor.b = sliderValue;

        hairImage.material.color = hairColor;
    }

    // ---------------------------

    [BurstCompile]
    public void ForceAdjustRedHairColour(float sliderValue)
    {
        // Move towards the slider value if required.

        if (hairColor.r > sliderValue + MAX_DIFFERENCE_ALLOWED)
        {
            hairColor.r = sliderValue + MAX_DIFFERENCE_ALLOWED;
        }
        else
        {
            if (hairColor.r < sliderValue - MAX_DIFFERENCE_ALLOWED)
            {
                hairColor.r = sliderValue - MAX_DIFFERENCE_ALLOWED;
            }
        }

        // Set the value adjusted here on the slider bar to help make more 
        // sense to the user that colours are being forceably changed to limit selection

        redHairSlider.value = hairColor.r;
    }

    [BurstCompile]
    public void ForceAdjustGreenHairColour(float sliderValue)
    {
        // Move towards the slider value if required.

        if (hairColor.g > sliderValue + MAX_DIFFERENCE_ALLOWED)
        {
            hairColor.g = sliderValue + MAX_DIFFERENCE_ALLOWED;
        }
        else
        {
            if (hairColor.g < sliderValue - MAX_DIFFERENCE_ALLOWED)
            {
                hairColor.g = sliderValue - MAX_DIFFERENCE_ALLOWED;
            }
        }

        // Set the value adjusted here on the slider bar to help make more 
        // sense to the user that colours are being forceably changed to limit selection

        greenHairSlider.value = hairColor.g;
    }

    [BurstCompile]
    public void ForceAdjustBlueHairColour(float sliderValue)
    {
        // Move towards the slider value if required.

        if (hairColor.b > sliderValue + MAX_DIFFERENCE_ALLOWED)
        {
            hairColor.b = sliderValue + MAX_DIFFERENCE_ALLOWED;
        }
        else
        {
            if (hairColor.b < sliderValue - MAX_DIFFERENCE_ALLOWED)
            {
                hairColor.b = sliderValue - MAX_DIFFERENCE_ALLOWED;
            }
        }

        // Set the value adjusted here on the slider bar to help make more 
        // sense to the user that colours are being forceably changed to limit selection

        blueHairSlider.value = hairColor.b;
    }

    // ---------------------------------------

    [BurstCompile]
    public void AdjustRedEyeBrowColour(float sliderValue)
    {
        // Keep R-G-B value close together to not allow overly bright colours
        ForceAdjustGreenEyeBrowColour(sliderValue);
        ForceAdjustBlueEyeBrowColour(sliderValue);

        eyeBrowColor.r = sliderValue;

        eyeBrowImage.material.color = eyeBrowColor;
    }

    [BurstCompile]
    public void AdjustGreenEyeBrowColour(float sliderValue)
    {
        // Keep R-G-B value close together to not allow overly bright colours
        ForceAdjustRedEyeBrowColour(sliderValue);
        ForceAdjustBlueEyeBrowColour(sliderValue);

        eyeBrowColor.g = sliderValue;

        eyeBrowImage.material.color = eyeBrowColor;
    }

    [BurstCompile]
    public void AdjustBlueEyeBrowColour(float sliderValue)
    {
        // Keep R-G-B value close together to not allow overly bright colours
        ForceAdjustRedEyeBrowColour(sliderValue);
        ForceAdjustGreenEyeBrowColour(sliderValue);

        eyeBrowColor.b = sliderValue;

        eyeBrowImage.material.color = eyeBrowColor;
    }

    // ---------------------------

    [BurstCompile]
    public void ForceAdjustRedEyeBrowColour(float sliderValue)
    {
        // Move towards the slider value if required.

        if (eyeBrowColor.r > sliderValue + MAX_DIFFERENCE_ALLOWED)
        {
            eyeBrowColor.r = sliderValue + MAX_DIFFERENCE_ALLOWED;
        }
        else
        {
            if (eyeBrowColor.r < sliderValue - MAX_DIFFERENCE_ALLOWED)
            {
                eyeBrowColor.r = sliderValue - MAX_DIFFERENCE_ALLOWED;
            }
        }

        // Set the value adjusted here on the slider bar to help make more 
        // sense to the user that colours are being forceably changed to limit selection

        redEyeBrowSlider.value = eyeBrowColor.r;
    }

    [BurstCompile]
    public void ForceAdjustGreenEyeBrowColour(float sliderValue)
    {
        // Move towards the slider value if required.

        if (eyeBrowColor.g > sliderValue + MAX_DIFFERENCE_ALLOWED)
        {
            eyeBrowColor.g = sliderValue + MAX_DIFFERENCE_ALLOWED;
        }
        else
        {
            if (eyeBrowColor.g < sliderValue - MAX_DIFFERENCE_ALLOWED)
            {
                eyeBrowColor.g = sliderValue - MAX_DIFFERENCE_ALLOWED;
            }
        }

        // Set the value adjusted here on the slider bar to help make more 
        // sense to the user that colours are being forceably changed to limit selection

        greenEyeBrowSlider.value = eyeBrowColor.g;
    }

    [BurstCompile]
    public void ForceAdjustBlueEyeBrowColour(float sliderValue)
    {
        // Move towards the slider value if required.

        if (eyeBrowColor.b > sliderValue + MAX_DIFFERENCE_ALLOWED)
        {
            eyeBrowColor.b = sliderValue + MAX_DIFFERENCE_ALLOWED;
        }
        else
        {
            if (eyeBrowColor.b < sliderValue - MAX_DIFFERENCE_ALLOWED)
            {
                eyeBrowColor.b = sliderValue - MAX_DIFFERENCE_ALLOWED;
            }
        }

        // Set the value adjusted here on the slider bar to help make more 
        // sense to the user that colours are being forceably changed to limit selection

        blueEyeBrowSlider.value = eyeBrowColor.b;
    }

    // ---------------------------------------

    [BurstCompile]
    public void AdjustRedBeardColour(float sliderValue)
    {
        // Keep R-G-B value close together to not allow overly bright colours
        ForceAdjustGreenBeardColour(sliderValue);
        ForceAdjustBlueBeardColour(sliderValue);

        beardColor.r = sliderValue;

        beardImage.material.color = beardColor;
    }

    [BurstCompile]
    public void AdjustGreenBeardColour(float sliderValue)
    {
        // Keep R-G-B value close together to not allow overly bright colours
        ForceAdjustRedBeardColour(sliderValue);
        ForceAdjustBlueBeardColour(sliderValue);

        beardColor.g = sliderValue;

        beardImage.material.color = beardColor;
    }

    [BurstCompile]
    public void AdjustBlueBeardColour(float sliderValue)
    {
        // Keep R-G-B value close together to not allow overly bright colours
        ForceAdjustRedBeardColour(sliderValue);
        ForceAdjustGreenBeardColour(sliderValue);

        beardColor.b = sliderValue;

        beardImage.material.color = beardColor;
    }

    // ---------------------------

    [BurstCompile]
    public void ForceAdjustRedBeardColour(float sliderValue)
    {
        // Move towards the slider value if required.

        if (beardColor.r > sliderValue + MAX_DIFFERENCE_ALLOWED)
        {
            beardColor.r = sliderValue + MAX_DIFFERENCE_ALLOWED;
        }
        else
        {
            if (beardColor.r < sliderValue - MAX_DIFFERENCE_ALLOWED)
            {
                beardColor.r = sliderValue - MAX_DIFFERENCE_ALLOWED;
            }
        }

        // Set the value adjusted here on the slider bar to help make more 
        // sense to the user that colours are being forceably changed to limit selection

        redBeardSlider.value = beardColor.r;
    }

    [BurstCompile]
    public void ForceAdjustGreenBeardColour(float sliderValue)
    {
        // Move towards the slider value if required.

        if (beardColor.g > sliderValue + MAX_DIFFERENCE_ALLOWED)
        {
            beardColor.g = sliderValue + MAX_DIFFERENCE_ALLOWED;
        }
        else
        {
            if (beardColor.g < sliderValue - MAX_DIFFERENCE_ALLOWED)
            {
                beardColor.g = sliderValue - MAX_DIFFERENCE_ALLOWED;
            }
        }

        // Set the value adjusted here on the slider bar to help make more 
        // sense to the user that colours are being forceably changed to limit selection

        greenBeardSlider.value = beardColor.g;
    }

    [BurstCompile]
    public void ForceAdjustBlueBeardColour(float sliderValue)
    {
        // Move towards the slider value if required.

        if (beardColor.b > sliderValue + MAX_DIFFERENCE_ALLOWED)
        {
            beardColor.b = sliderValue + MAX_DIFFERENCE_ALLOWED;
        }
        else
        {
            if (beardColor.b < sliderValue - MAX_DIFFERENCE_ALLOWED)
            {
                beardColor.b = sliderValue - MAX_DIFFERENCE_ALLOWED;
            }
        }

        // Set the value adjusted here on the slider bar to help make more 
        // sense to the user that colours are being forceably changed to limit selection

        blueBeardSlider.value = beardColor.b;
    }

    // ---------------------------------------

    [BurstCompile]
    public void AdjustRedMoustacheColour(float sliderValue)
    {
        // Keep R-G-B value close together to not allow overly bright colours
        ForceAdjustGreenMoustacheColour(sliderValue);
        ForceAdjustBlueMoustacheColour(sliderValue);

        moustacheColor.r = sliderValue;

        moustacheImage.material.color = moustacheColor;
    }

    [BurstCompile]
    public void AdjustGreenMoustacheColour(float sliderValue)
    {
        // Keep R-G-B value close together to not allow overly bright colours
        ForceAdjustRedMoustacheColour(sliderValue);
        ForceAdjustBlueMoustacheColour(sliderValue);

        moustacheColor.g = sliderValue;

        moustacheImage.material.color = moustacheColor;
    }

    [BurstCompile]
    public void AdjustBlueMoustacheColour(float sliderValue)
    {
        // Keep R-G-B value close together to not allow overly bright colours
        ForceAdjustRedMoustacheColour(sliderValue);
        ForceAdjustGreenMoustacheColour(sliderValue);

        moustacheColor.b = sliderValue;

        moustacheImage.material.color = moustacheColor;
    }

    // ---------------------------

    [BurstCompile]
    public void ForceAdjustRedMoustacheColour(float sliderValue)
    {
        // Move towards the slider value if required.

        if (moustacheColor.r > sliderValue + MAX_DIFFERENCE_ALLOWED)
        {
            moustacheColor.r = sliderValue + MAX_DIFFERENCE_ALLOWED;
        }
        else
        {
            if (moustacheColor.r < sliderValue - MAX_DIFFERENCE_ALLOWED)
            {
                moustacheColor.r = sliderValue - MAX_DIFFERENCE_ALLOWED;
            }
        }

        // Set the value adjusted here on the slider bar to help make more 
        // sense to the user that colours are being forceably changed to limit selection

        redMoustacheSlider.value = moustacheColor.r;
    }

    [BurstCompile]
    public void ForceAdjustGreenMoustacheColour(float sliderValue)
    {
        // Move towards the slider value if required.

        if (moustacheColor.g > sliderValue + MAX_DIFFERENCE_ALLOWED)
        {
            moustacheColor.g = sliderValue + MAX_DIFFERENCE_ALLOWED;
        }
        else
        {
            if (moustacheColor.g < sliderValue - MAX_DIFFERENCE_ALLOWED)
            {
                moustacheColor.g = sliderValue - MAX_DIFFERENCE_ALLOWED;
            }
        }

        // Set the value adjusted here on the slider bar to help make more 
        // sense to the user that colours are being forceably changed to limit selection

        greenMoustacheSlider.value = moustacheColor.g;
    }

    [BurstCompile]
    public void ForceAdjustBlueMoustacheColour(float sliderValue)
    {
        // Move towards the slider value if required.

        if (moustacheColor.b > sliderValue + MAX_DIFFERENCE_ALLOWED)
        {
            moustacheColor.b = sliderValue + MAX_DIFFERENCE_ALLOWED;
        }
        else
        {
            if (moustacheColor.b < sliderValue - MAX_DIFFERENCE_ALLOWED)
            {
                moustacheColor.b = sliderValue - MAX_DIFFERENCE_ALLOWED;
            }
        }

        // Set the value adjusted here on the slider bar to help make more 
        // sense to the user that colours are being forceably changed to limit selection

        blueMoustacheSlider.value = moustacheColor.b;
    }
}
