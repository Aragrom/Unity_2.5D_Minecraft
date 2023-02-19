using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;

// features should be attached to main sprite and be placed and moved as a child.
public struct FeatureLocations
{
    public Vector3 head;
    public Vector3 weaponHand;
    public Vector3 offHand;
}

public struct FaceFeatureLocations
{
    public Vector3 eyes;
    public Vector3 eyeBrows;
    public Vector3 eyesBackground;
}

public struct RenderOrder
{
    // should be used to set the layer of each sprite being rendered
    public int hair;
    public int beard;
    public int moustache;
}

[BurstCompile]
public class CreatedCharacterAnimation : MonoBehaviour
{
    public CharacterCreationEngine characterCreationEngine = null;

    public Animator animator = null;

    public AnimatorOverrideController sideOverride = null;
    public AnimatorOverrideController frontOverride = null;
    public AnimatorOverrideController backOverride = null;

    public PlayerAnimation playerAnimation = null;

    public int facing = 1;                          // 0 side - 1 front - 2 back
    public SpriteRenderer spriteRenderer = null;
    public Transform sprite = null;                 // performing rotation on sprite so Transform instead of GameObject
    public Transform head = null;
    public Transform eyes = null;
    public Transform eyeBrows = null;
    public Transform eyeBackground = null;
    public FeatureLocations currentFeatureLocations = new FeatureLocations();

    public AnimatorStateInfo animationStateInfo = new AnimatorStateInfo();
    public AnimatorClipInfo[] animatorClipInfo = new AnimatorClipInfo[0];
    public string clipName = "";
    public int currentFrameNumber = 0;
    public float totalTime = 0;             // Time animation has been running/loop for. required when using jump/fall to know when we are no longer cycling frames
    public float time = 0;

    public Dictionary<string, FeatureLocations[]> animationFeatureLocations = new Dictionary<string, FeatureLocations[]>();

    // key - facing
    public Dictionary<int, FaceFeatureLocations> faceFeatureLocations = new Dictionary<int, FaceFeatureLocations>();
    public Dictionary<int, RenderOrder> renderOrders = new Dictionary<int, RenderOrder>();

    [BurstCompile]
    public void Start()
    {
        SetUpFeatureLocations();
        SetUpFaceFeatureLocations();
        SetUpRenderOrder();
    }

    [BurstCompile]
    public void SetUpRenderOrder()
    {
        // Side -----------------------

        RenderOrder renderOrder = new RenderOrder
        {
            hair = 0,
            beard = 1,
            moustache = 2
        };

        renderOrders.Add(0, renderOrder);

        // Front -----------------------

        renderOrder = new RenderOrder
        {
            hair = 0,
            beard = 1,
            moustache = 2
        };

        renderOrders.Add(1, renderOrder);

        // Back -----------------------

        renderOrder = new RenderOrder
        {
            hair = 2,
            beard = 1,
            moustache = 0
        };

        renderOrders.Add(2, renderOrder);
    }

    [BurstCompile]
    public void SetUpFaceFeatureLocations()
    {
        // Side

        FaceFeatureLocations ffl = new FaceFeatureLocations
        {
            eyes = new Vector3(0.093f, -0.03f, 0),
            eyeBrows = new Vector3(0.062f, 0.032f, 0),
            eyesBackground = new Vector3(0.062f, -0.031f, 0)
        };

        faceFeatureLocations.Add(0, ffl);

        // Front

        ffl = new FaceFeatureLocations
        {
            eyes = new Vector3(0, -0.03f, 0),
            eyeBrows = new Vector3(0, 0.032f, 0),
            eyesBackground = new Vector3(0, -0.031f, 0)
        };

        faceFeatureLocations.Add(1, ffl);

        // Back

        ffl = new FaceFeatureLocations
        {
            eyes = new Vector3(0, -0.03f, 0),
            eyeBrows = new Vector3(0, 0.032f, 0),
            eyesBackground = new Vector3(0, -0.031f, 0)
        };

        faceFeatureLocations.Add(2, ffl);
    }

    [BurstCompile]
    public void SetUpFeatureLocations()
    {
        // IDLE --------------------

        // Side, front, Back

        FeatureLocations[] featureLocations = new FeatureLocations[8];

        featureLocations[0].head = new Vector3(0.0f, 0.75f, 0.0f);
        featureLocations[1].head = new Vector3(0.0f, 0.75f, 0.0f);
        featureLocations[2].head = new Vector3(0.0f, 0.75f, 0.0f);
        featureLocations[3].head = new Vector3(0.0f, 0.75f, 0.0f);
        featureLocations[4].head = new Vector3(0.0f, 0.75f, 0.0f);
        featureLocations[5].head = new Vector3(0.0f, 0.75f, 0.0f);
        featureLocations[6].head = new Vector3(0.0f, 0.75f, 0.0f);
        featureLocations[7].head = new Vector3(0.0f, 0.75f, 0.0f);

        animationFeatureLocations.Add("Human_Idle_Side_Animation", featureLocations);   // shared locations
        animationFeatureLocations.Add("Human_Idle_Front_Animation", featureLocations);
        animationFeatureLocations.Add("Human_Idle_Back_Animation", featureLocations);

        // WALK --------------------

        // Side, front, Back

        featureLocations = new FeatureLocations[8];

        featureLocations[0].head = new Vector3(0.0f, 0.811f, 0.0f);
        featureLocations[1].head = new Vector3(0.0f, 0.75f, 0.0f);
        featureLocations[2].head = new Vector3(0.0f, 0.75f, 0.0f);
        featureLocations[3].head = new Vector3(0.0f, 0.811f, 0.0f);
        featureLocations[4].head = new Vector3(0.0f, 0.811f, 0.0f);
        featureLocations[5].head = new Vector3(0.0f, 0.75f, 0.0f);
        featureLocations[6].head = new Vector3(0.0f, 0.75f, 0.0f);
        featureLocations[7].head = new Vector3(0.0f, 0.811f, 0.0f);

        animationFeatureLocations.Add("Human_Walk_Front_Animation", featureLocations);  // Shared locations
        animationFeatureLocations.Add("Human_Walk_Back_Animation", featureLocations);

        featureLocations = new FeatureLocations[8];

        featureLocations[0].head = new Vector3(0.064f, 0.811f, 0.0f);
        featureLocations[1].head = new Vector3(0.064f, 0.75f, 0.0f);
        featureLocations[2].head = new Vector3(0.064f, 0.75f, 0.0f);
        featureLocations[3].head = new Vector3(0.064f, 0.811f, 0.0f);
        featureLocations[4].head = new Vector3(0.064f, 0.811f, 0.0f);
        featureLocations[5].head = new Vector3(0.064f, 0.75f, 0.0f);
        featureLocations[6].head = new Vector3(0.064f, 0.75f, 0.0f);
        featureLocations[7].head = new Vector3(0.064f, 0.811f, 0.0f);

        animationFeatureLocations.Add("Human_Walk_Side_Animation", featureLocations);

        // RUN --------------------

        // Side, front, Back

        featureLocations = new FeatureLocations[8];

        featureLocations[0].head = new Vector3(0.0f, 0.812f, 0.0f);
        featureLocations[1].head = new Vector3(0.0f, 0.812f, 0.0f);
        featureLocations[2].head = new Vector3(0.0f, 0.875f, 0.0f);
        featureLocations[3].head = new Vector3(0.0f, 0.875f, 0.0f);
        featureLocations[4].head = new Vector3(0.0f, 0.812f, 0.0f);
        featureLocations[5].head = new Vector3(0.0f, 0.812f, 0.0f);
        featureLocations[6].head = new Vector3(0.0f, 0.875f, 0.0f);
        featureLocations[7].head = new Vector3(0.0f, 0.875f, 0.0f);

        animationFeatureLocations.Add("Human_Run_Front_Animation", featureLocations);     // Shared locations
        animationFeatureLocations.Add("Human_Run_Back_Animation", featureLocations);

        featureLocations = new FeatureLocations[8];

        featureLocations[0].head = new Vector3(0.064f, 0.812f, 0.0f);
        featureLocations[1].head = new Vector3(0.064f, 0.812f, 0.0f);
        featureLocations[2].head = new Vector3(0.064f, 0.875f, 0.0f);
        featureLocations[3].head = new Vector3(0.064f, 0.875f, 0.0f);
        featureLocations[4].head = new Vector3(0.064f, 0.812f, 0.0f);
        featureLocations[5].head = new Vector3(0.064f, 0.812f, 0.0f);
        featureLocations[6].head = new Vector3(0.064f, 0.875f, 0.0f);
        featureLocations[7].head = new Vector3(0.064f, 0.875f, 0.0f);

        animationFeatureLocations.Add("Human_Run_Side_Animation", featureLocations);

        // SLIDE --------------------

        // Side, front, Back

        featureLocations = new FeatureLocations[8];

        featureLocations[0].head = new Vector3(0.0f, 0.626f, 0.0f);
        featureLocations[1].head = new Vector3(0.0f, 0.626f, 0.0f);
        featureLocations[2].head = new Vector3(0.0f, 0.626f, 0.0f);
        featureLocations[3].head = new Vector3(0.0f, 0.626f, 0.0f);
        featureLocations[4].head = new Vector3(0.0f, 0.626f, 0.0f);
        featureLocations[5].head = new Vector3(0.0f, 0.626f, 0.0f);
        featureLocations[6].head = new Vector3(0.0f, 0.626f, 0.0f);
        featureLocations[7].head = new Vector3(0.0f, 0.626f, 0.0f);

        animationFeatureLocations.Add("Human_Slide_Front_Animation", featureLocations);     // Shared locations
        animationFeatureLocations.Add("Human_Slide_Back_Animation", featureLocations);

        featureLocations = new FeatureLocations[8];

        featureLocations[0].head = new Vector3(-0.124f, 0.626f, 0.0f);
        featureLocations[1].head = new Vector3(-0.124f, 0.626f, 0.0f);
        featureLocations[2].head = new Vector3(-0.124f, 0.626f, 0.0f);
        featureLocations[3].head = new Vector3(-0.124f, 0.626f, 0.0f);
        featureLocations[4].head = new Vector3(-0.124f, 0.626f, 0.0f);
        featureLocations[5].head = new Vector3(-0.124f, 0.626f, 0.0f);
        featureLocations[6].head = new Vector3(-0.124f, 0.626f, 0.0f);
        featureLocations[7].head = new Vector3(-0.124f, 0.626f, 0.0f);

        animationFeatureLocations.Add("Human_Slide_Side_Animation", featureLocations);


        // JUMP --------------------

        // Side, front, Back

        featureLocations = new FeatureLocations[8];

        featureLocations[0].head = new Vector3(0.0f, 0.312f, 0.0f);
        featureLocations[1].head = new Vector3(0.0f, 0.436f, 0.0f);
        featureLocations[2].head = new Vector3(0.0f, 0.436f, 0.0f);
        featureLocations[3].head = new Vector3(0.0f, 0.436f, 0.0f);

        featureLocations[4].head = new Vector3(0.0f, 0.436f, 0.0f);
        featureLocations[5].head = new Vector3(0.0f, 0.436f, 0.0f);
        featureLocations[6].head = new Vector3(0.0f, 0.436f, 0.0f);
        featureLocations[7].head = new Vector3(0.0f, 0.436f, 0.0f);

        animationFeatureLocations.Add("Human_Jump_Front_Animation", featureLocations);     // Shared locations
        animationFeatureLocations.Add("Human_Jump_Back_Animation", featureLocations);

        featureLocations = new FeatureLocations[8];

        featureLocations[0].head = new Vector3(0.064f, 0.312f, 0.0f);
        featureLocations[1].head = new Vector3(0.064f, 0.436f, 0.0f);
        featureLocations[2].head = new Vector3(0.064f, 0.436f, 0.0f);
        featureLocations[3].head = new Vector3(0.064f, 0.436f, 0.0f);

        featureLocations[4].head = new Vector3(0.064f, 0.436f, 0.0f);
        featureLocations[5].head = new Vector3(0.064f, 0.436f, 0.0f);
        featureLocations[6].head = new Vector3(0.064f, 0.436f, 0.0f);
        featureLocations[7].head = new Vector3(0.064f, 0.436f, 0.0f);

        animationFeatureLocations.Add("Human_Jump_Side_Animation", featureLocations);
    }

    // Designed to sync the sprite sheets in the "material/shader" 
    // so that it contains the sprite displayed in the "Sprite Renderer"
    [BurstCompile]
    private void LateUpdate()
    {
        if (characterCreationEngine.isCreatingCharacter == false) return;

        //Fetch the current Animation clip information for the base layer
        animatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        animationStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        //Access the current length of the clip
        //m_CurrentClipLength = m_CurrentClipInfo[0].clip.length;
        //Access the Animation clip name
        clipName = animatorClipInfo[0].clip.name;

        totalTime = animationStateInfo.normalizedTime;
        time = totalTime;
        time = time - (int)time;

        currentFrameNumber = (int)(8 * time);

        switch (clipName)
        {
            case "Human_Idle_Side_Animation":
            case "Human_Idle_Front_Animation":
            case "Human_Idle_Back_Animation":

                spriteRenderer.material.SetTexture("_BaseMap", playerAnimation.idleSpriteSheet);
                spriteRenderer.material.SetTexture("_BumpMap", playerAnimation.idleNormalMapSpriteSheet);

                // Set position of features

                switch (facing)
                {
                    case 0:
                        currentFeatureLocations = animationFeatureLocations["Human_Idle_Side_Animation"][currentFrameNumber];
                        head.localPosition = currentFeatureLocations.head;
                        break;
                    case 1:
                        currentFeatureLocations = animationFeatureLocations["Human_Idle_Front_Animation"][currentFrameNumber];
                        head.localPosition = currentFeatureLocations.head;
                        break;
                    case 2:
                        currentFeatureLocations = animationFeatureLocations["Human_Idle_Back_Animation"][currentFrameNumber];
                        head.localPosition = currentFeatureLocations.head;
                        break;
                }

                break;

            case "Human_Walk_Side_Animation":
            case "Human_Walk_Front_Animation":
            case "Human_Walk_Back_Animation":

                spriteRenderer.material.SetTexture("_BaseMap", playerAnimation.walkSpriteSheet);
                spriteRenderer.material.SetTexture("_BumpMap", playerAnimation.walkNormalMapSpriteSheet);

                switch (facing)
                {
                    case 0:
                        currentFeatureLocations = animationFeatureLocations["Human_Walk_Side_Animation"][currentFrameNumber];
                        head.localPosition = currentFeatureLocations.head;
                        break;
                    case 1:
                        currentFeatureLocations = animationFeatureLocations["Human_Walk_Front_Animation"][currentFrameNumber];
                        head.localPosition = currentFeatureLocations.head;
                        break;
                    case 2:
                        currentFeatureLocations = animationFeatureLocations["Human_Walk_Back_Animation"][currentFrameNumber];
                        head.localPosition = currentFeatureLocations.head;
                        break;
                }

                break;

            case "Human_Run_Side_Animation":
            case "Human_Run_Front_Animation":
            case "Human_Run_Back_Animation":

                spriteRenderer.material.SetTexture("_BaseMap", playerAnimation.runSpriteSheet);
                spriteRenderer.material.SetTexture("_BumpMap", playerAnimation.runNormalMapSpriteSheet);

                switch (facing)
                {
                    case 0:
                        currentFeatureLocations = animationFeatureLocations["Human_Run_Side_Animation"][currentFrameNumber];
                        head.localPosition = currentFeatureLocations.head;
                        break;
                    case 1:
                        currentFeatureLocations = animationFeatureLocations["Human_Run_Front_Animation"][currentFrameNumber];
                        head.localPosition = currentFeatureLocations.head;
                        break;
                    case 2:
                        currentFeatureLocations = animationFeatureLocations["Human_Run_Back_Animation"][currentFrameNumber];
                        head.localPosition = currentFeatureLocations.head;
                        break;
                }

                break;

            case "Human_Slide_Side_Animation":
            case "Human_Slide_Front_Animation":
            case "Human_Slide_Back_Animation":

                spriteRenderer.material.SetTexture("_BaseMap", playerAnimation.slideSpriteSheet);
                spriteRenderer.material.SetTexture("_BumpMap", playerAnimation.slideNormalMapSpriteSheet);

                switch (facing)
                {
                    case 0:
                        currentFeatureLocations = animationFeatureLocations["Human_Slide_Side_Animation"][currentFrameNumber];
                        head.localPosition = currentFeatureLocations.head;
                        break;
                    case 1:
                        currentFeatureLocations = animationFeatureLocations["Human_Slide_Front_Animation"][currentFrameNumber];
                        head.localPosition = currentFeatureLocations.head;
                        break;
                    case 2:
                        currentFeatureLocations = animationFeatureLocations["Human_Slide_Back_Animation"][currentFrameNumber];
                        head.localPosition = currentFeatureLocations.head;
                        break;
                }

                break;

            case "Human_Jump_Side_Animation":
            case "Human_Jump_Front_Animation":
            case "Human_Jump_Back_Animation":

                // Jump doesn't loop. So need to use total time and only when less than 1.0 (first animation cycle 0f - 1.0f)
                if (totalTime < 1.0f)
                {
                    switch (facing)
                    {
                        case 0:
                            currentFeatureLocations = animationFeatureLocations["Human_Jump_Side_Animation"][currentFrameNumber/2]; // divide by 2 as jump animtion is only 4 frames
                            head.localPosition = currentFeatureLocations.head;
                            break;
                        case 1:
                            currentFeatureLocations = animationFeatureLocations["Human_Jump_Front_Animation"][currentFrameNumber/2];
                            head.localPosition = currentFeatureLocations.head;
                            break;
                        case 2:
                            currentFeatureLocations = animationFeatureLocations["Human_Jump_Back_Animation"][currentFrameNumber/2];
                            head.localPosition = currentFeatureLocations.head;
                            break;
                    }
                }

                spriteRenderer.material.SetTexture("_BaseMap", playerAnimation.jumpSpriteSheet);
                spriteRenderer.material.SetTexture("_BumpMap", playerAnimation.jumpNormalMapSpriteSheet);

                break;
        }
    }

    /// <summary>
    /// Passing a speed determine if the sprite renderer animation/sprite should be flipped. 
    /// Positive speed false, Negative speed true.
    /// </summary>
    /// <param name="speed"> velocity.x </param>
    [BurstCompile]
    public void Flip(float speed)
    {
        if (speed > 0)
        {
            spriteRenderer.flipX = false;
        }
        if (speed < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    [BurstCompile]
    public void SetSpeed(float value)
    {
        animator.SetFloat("Speed", Mathf.Abs(value));
    }

    [BurstCompile]
    public void SetAnimationSpeed(float value)
    {
        value /= 5;

        if (Mathf.Abs(value) < 0.2f) value = 0.2f;

        animator.SetFloat("AnimationSpeed", Mathf.Abs(value));
    }

    [BurstCompile]
    public void SetVelocityY(float value)
    {
        animator.SetFloat("VelocityY", value);
    }

    [BurstCompile]
    public void SetIsJumping(bool value)
    {
        animator.SetBool("IsJumping", value);
    }

    [BurstCompile]
    public void SetIsSliding(bool value)
    {
        animator.SetBool("IsSliding", value);
    }

    [BurstCompile]
    public void OnClickSide()
    {
        int oldFacing = facing;
        int difference;

        facing = 0;

        difference = facing - oldFacing;
        animator.runtimeAnimatorController = sideOverride;

        // assign correct sprites to features.

        characterCreationEngine.hairType += difference;
        characterCreationEngine.beardType += difference;
        characterCreationEngine.moustacheType += difference;

        characterCreationEngine.hairImage.sprite = characterCreationEngine.humanFeatures.hairs[characterCreationEngine.hairType];
        characterCreationEngine.beardImage.sprite = characterCreationEngine.humanFeatures.beards[characterCreationEngine.beardType];
        characterCreationEngine.moustacheImage.sprite = characterCreationEngine.humanFeatures.moustache[characterCreationEngine.moustacheType];

        characterCreationEngine.eyeImage.sprite = characterCreationEngine.humanFeatures.eyesSide;
        characterCreationEngine.eyeBrowImage.sprite = characterCreationEngine.humanFeatures.eyeBrowsSide;
        characterCreationEngine.eyeBackgroundImage.sprite = characterCreationEngine.humanFeatures.eyesBackgroundSide;

        // move eyes, brows and background to be in correct position for new rotation.

        eyes.localPosition = faceFeatureLocations[facing].eyes;
        eyeBrows.localPosition = faceFeatureLocations[facing].eyeBrows;
        eyeBackground.localPosition = faceFeatureLocations[facing].eyesBackground;

        // back disables so must reenable them.
        characterCreationEngine.eyeImage.gameObject.SetActive(true);
        characterCreationEngine.eyeBrowImage.gameObject.SetActive(true);
        characterCreationEngine.eyeBackgroundImage.gameObject.SetActive(true);

        // Set render order so hair, beard and moustache is layered properly.
        characterCreationEngine.hairImage.sortingOrder = renderOrders[facing].hair;
        characterCreationEngine.beardImage.sortingOrder = renderOrders[facing].beard;
        characterCreationEngine.moustacheImage.sortingOrder = renderOrders[facing].moustache;
    }

    [BurstCompile]
    public void OnClickFront()
    {
        int oldFacing = facing;
        int difference;

        facing = 1;

        difference = facing - oldFacing;
        animator.runtimeAnimatorController = frontOverride;

        // assign correct sprites to features.

        characterCreationEngine.hairType += difference;
        characterCreationEngine.beardType += difference;
        characterCreationEngine.moustacheType += difference;

        characterCreationEngine.hairImage.sprite = characterCreationEngine.humanFeatures.hairs[characterCreationEngine.hairType];
        characterCreationEngine.beardImage.sprite = characterCreationEngine.humanFeatures.beards[characterCreationEngine.beardType];
        characterCreationEngine.moustacheImage.sprite = characterCreationEngine.humanFeatures.moustache[characterCreationEngine.moustacheType];

        characterCreationEngine.eyeImage.sprite = characterCreationEngine.humanFeatures.eyesFront;
        characterCreationEngine.eyeBrowImage.sprite = characterCreationEngine.humanFeatures.eyeBrowsFront;
        characterCreationEngine.eyeBackgroundImage.sprite = characterCreationEngine.humanFeatures.eyesBackgroundFront;

        // move eyes, brows and background to be in correct position for new rotation.

        eyes.localPosition = faceFeatureLocations[facing].eyes;
        eyeBrows.localPosition = faceFeatureLocations[facing].eyeBrows;
        eyeBackground.localPosition = faceFeatureLocations[facing].eyesBackground;

        // back disables so must reenable them.
        characterCreationEngine.eyeImage.gameObject.SetActive(true);
        characterCreationEngine.eyeBrowImage.gameObject.SetActive(true);
        characterCreationEngine.eyeBackgroundImage.gameObject.SetActive(true);

        // Set render order so hair, beard and moustache is layered properly.
        characterCreationEngine.hairImage.sortingOrder = renderOrders[facing].hair;
        characterCreationEngine.beardImage.sortingOrder = renderOrders[facing].beard;
        characterCreationEngine.moustacheImage.sortingOrder = renderOrders[facing].moustache;
    }

    [BurstCompile]
    public void OnClickBack()
    {
        int oldFacing = facing;
        int difference;

        facing = 2;

        difference = facing - oldFacing;
        animator.runtimeAnimatorController = backOverride;

        // assign correct sprites to features.

        characterCreationEngine.hairType += difference;
        characterCreationEngine.beardType += difference;
        characterCreationEngine.moustacheType += difference;

        characterCreationEngine.hairImage.sprite = characterCreationEngine.humanFeatures.hairs[characterCreationEngine.hairType];
        characterCreationEngine.beardImage.sprite = characterCreationEngine.humanFeatures.beards[characterCreationEngine.beardType];
        characterCreationEngine.moustacheImage.sprite = characterCreationEngine.humanFeatures.moustache[characterCreationEngine.moustacheType];

        characterCreationEngine.eyeImage.gameObject.SetActive(false);
        characterCreationEngine.eyeBrowImage.gameObject.SetActive(false);
        characterCreationEngine.eyeBackgroundImage.gameObject.SetActive(false);

        // Set render order so hair, beard and moustache is layered properly.
        characterCreationEngine.hairImage.sortingOrder = renderOrders[facing].hair;
        characterCreationEngine.beardImage.sortingOrder = renderOrders[facing].beard;
        characterCreationEngine.moustacheImage.sortingOrder = renderOrders[facing].moustache;
    }

    [BurstCompile]
    public void OnClickJumping()
    {
        SetIsSliding(false);
        SetIsJumping(true);
    }

    [BurstCompile]
    public void OnClickIdle()
    {
        SetIsSliding(false);
        SetIsJumping(false);
        SetSpeed(0);
    }

    [BurstCompile]
    public void OnClickWalking()
    {
        SetIsSliding(false);
        SetIsJumping(false);
        SetSpeed(5.0f);
    }

    [BurstCompile]
    public void OnClickRunning()
    {
        SetIsSliding(false);
        SetIsJumping(false);
        SetSpeed(10.0f);
    }

    [BurstCompile]
    public void OnClickSliding()
    {
        SetIsJumping(false);
        SetSpeed(10.0f);
        SetIsSliding(true);
    }
}
