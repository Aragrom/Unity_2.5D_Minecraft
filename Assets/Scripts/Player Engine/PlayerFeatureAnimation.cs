using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class PlayerFeatureAnimation : MonoBehaviour
{
    public bool flipped = false;    // Are all feature sprites flipped?

    public Animator animator = null;

    public SpriteRenderer characterSprite = null;
    public SpriteRenderer eyeSprite = null;
    public SpriteRenderer hairSprite = null;
    public SpriteRenderer eyeBrowSprite = null;
    public SpriteRenderer eyeBackgroundSprite = null;

    public SpriteRenderer beardSprite = null;
    public SpriteRenderer moustacheSprite = null;

    public AnimatorOverrideController sideOverride = null;
    public AnimatorOverrideController frontOverride = null;
    public AnimatorOverrideController backOverride = null;

    public PlayerAnimation playerAnimation = null;

    public int facing = 0;                          // 0 side - 1 front - 2 back
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
    public string lastClipName = "";        // used to track and trigger events when animation first changes. (like in slide to make the head and other sprites flip)
    public int currentFrameNumber = 0;
    public float totalTime = 0;             // Time animation has been running/loop for. required when using jump/fall to know when we are no longer cycling frames
    public float time = 0;

    public Dictionary<string, FeatureLocations[]> animationFeatureLocations = new Dictionary<string, FeatureLocations[]>();
    public Dictionary<string, FeatureLocations[]> inverseAnimationFeatureLocations = new Dictionary<string, FeatureLocations[]>();

    // key - facing
    public Dictionary<int, FaceFeatureLocations> faceFeatureLocations = new Dictionary<int, FaceFeatureLocations>();
    public Dictionary<int, FaceFeatureLocations> inverseFaceFeatureLocations = new Dictionary<int, FaceFeatureLocations>();

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

        //===============================================
        // Inverse ----------------------------------
        //===============================================

        // Side

        ffl = new FaceFeatureLocations
        {
            eyes = new Vector3(-0.093f, -0.03f, 0),
            eyeBrows = new Vector3(-0.062f, 0.032f, 0),
            eyesBackground = new Vector3(-0.062f, -0.031f, 0)
        };

        inverseFaceFeatureLocations.Add(0, ffl);

        // Front

        ffl = new FaceFeatureLocations
        {
            eyes = new Vector3(0, -0.03f, 0),
            eyeBrows = new Vector3(0, 0.032f, 0),
            eyesBackground = new Vector3(0, -0.031f, 0)
        };

        inverseFaceFeatureLocations.Add(1, ffl);

        // Back

        ffl = new FaceFeatureLocations
        {
            eyes = new Vector3(0, -0.03f, 0),
            eyeBrows = new Vector3(0, 0.032f, 0),
            eyesBackground = new Vector3(0, -0.031f, 0)
        };

        inverseFaceFeatureLocations.Add(2, ffl);
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

        // FALLING --------------------

        // Side, front, Back

        featureLocations = new FeatureLocations[8];

        featureLocations[0].head = new Vector3(0.0f, 0.436f, 0.0f);
        featureLocations[1].head = new Vector3(0.0f, 0.436f, 0.0f);
        featureLocations[2].head = new Vector3(0.0f, 0.436f, 0.0f);
        featureLocations[3].head = new Vector3(0.0f, 0.436f, 0.0f);

        featureLocations[4].head = new Vector3(0.0f, 0.436f, 0.0f);
        featureLocations[5].head = new Vector3(0.0f, 0.436f, 0.0f);
        featureLocations[6].head = new Vector3(0.0f, 0.436f, 0.0f);
        featureLocations[7].head = new Vector3(0.0f, 0.436f, 0.0f);

        animationFeatureLocations.Add("Human_Fall_Front_Animation", featureLocations);     // Shared locations
        animationFeatureLocations.Add("Human_Fall_Back_Animation", featureLocations);

        featureLocations = new FeatureLocations[8];

        featureLocations[0].head = new Vector3(0.064f, 0.436f, 0.0f);
        featureLocations[1].head = new Vector3(0.064f, 0.436f, 0.0f);
        featureLocations[2].head = new Vector3(0.064f, 0.436f, 0.0f);
        featureLocations[3].head = new Vector3(0.064f, 0.436f, 0.0f);

        featureLocations[4].head = new Vector3(0.064f, 0.436f, 0.0f);
        featureLocations[5].head = new Vector3(0.064f, 0.436f, 0.0f);
        featureLocations[6].head = new Vector3(0.064f, 0.436f, 0.0f);
        featureLocations[7].head = new Vector3(0.064f, 0.436f, 0.0f);

        animationFeatureLocations.Add("Human_Fall_Side_Animation", featureLocations);


        //========================================================
        // INVERSE
        //========================================================

        // IDLE --------------------

        // Side, front, Back

        featureLocations = new FeatureLocations[8];

        featureLocations[0].head = new Vector3(0.0f, 0.75f, 0.0f);
        featureLocations[1].head = new Vector3(0.0f, 0.75f, 0.0f);
        featureLocations[2].head = new Vector3(0.0f, 0.75f, 0.0f);
        featureLocations[3].head = new Vector3(0.0f, 0.75f, 0.0f);
        featureLocations[4].head = new Vector3(0.0f, 0.75f, 0.0f);
        featureLocations[5].head = new Vector3(0.0f, 0.75f, 0.0f);
        featureLocations[6].head = new Vector3(0.0f, 0.75f, 0.0f);
        featureLocations[7].head = new Vector3(0.0f, 0.75f, 0.0f);

        inverseAnimationFeatureLocations.Add("Human_Idle_Side_Animation", featureLocations);   // shared locations
        inverseAnimationFeatureLocations.Add("Human_Idle_Front_Animation", featureLocations);
        inverseAnimationFeatureLocations.Add("Human_Idle_Back_Animation", featureLocations);

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

        inverseAnimationFeatureLocations.Add("Human_Walk_Front_Animation", featureLocations);  // Shared locations
        inverseAnimationFeatureLocations.Add("Human_Walk_Back_Animation", featureLocations);

        featureLocations = new FeatureLocations[8];

        featureLocations[0].head = new Vector3(-0.064f, 0.811f, 0.0f);
        featureLocations[1].head = new Vector3(-0.064f, 0.75f, 0.0f);
        featureLocations[2].head = new Vector3(-0.064f, 0.75f, 0.0f);
        featureLocations[3].head = new Vector3(-0.064f, 0.811f, 0.0f);
        featureLocations[4].head = new Vector3(-0.064f, 0.811f, 0.0f);
        featureLocations[5].head = new Vector3(-0.064f, 0.75f, 0.0f);
        featureLocations[6].head = new Vector3(-0.064f, 0.75f, 0.0f);
        featureLocations[7].head = new Vector3(-0.064f, 0.811f, 0.0f);

        inverseAnimationFeatureLocations.Add("Human_Walk_Side_Animation", featureLocations);

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

        inverseAnimationFeatureLocations.Add("Human_Run_Front_Animation", featureLocations);     // Shared locations
        inverseAnimationFeatureLocations.Add("Human_Run_Back_Animation", featureLocations);

        featureLocations = new FeatureLocations[8];

        featureLocations[0].head = new Vector3(-0.064f, 0.812f, 0.0f);
        featureLocations[1].head = new Vector3(-0.064f, 0.812f, 0.0f);
        featureLocations[2].head = new Vector3(-0.064f, 0.875f, 0.0f);
        featureLocations[3].head = new Vector3(-0.064f, 0.875f, 0.0f);
        featureLocations[4].head = new Vector3(-0.064f, 0.812f, 0.0f);
        featureLocations[5].head = new Vector3(-0.064f, 0.812f, 0.0f);
        featureLocations[6].head = new Vector3(-0.064f, 0.875f, 0.0f);
        featureLocations[7].head = new Vector3(-0.064f, 0.875f, 0.0f);

        inverseAnimationFeatureLocations.Add("Human_Run_Side_Animation", featureLocations);

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

        inverseAnimationFeatureLocations.Add("Human_Slide_Front_Animation", featureLocations);     // Shared locations
        inverseAnimationFeatureLocations.Add("Human_Slide_Back_Animation", featureLocations);

        featureLocations = new FeatureLocations[8];

        featureLocations[0].head = new Vector3(0.124f, 0.626f, 0.0f);
        featureLocations[1].head = new Vector3(0.124f, 0.626f, 0.0f);
        featureLocations[2].head = new Vector3(0.124f, 0.626f, 0.0f);
        featureLocations[3].head = new Vector3(0.124f, 0.626f, 0.0f);
        featureLocations[4].head = new Vector3(0.124f, 0.626f, 0.0f);
        featureLocations[5].head = new Vector3(0.124f, 0.626f, 0.0f);
        featureLocations[6].head = new Vector3(0.124f, 0.626f, 0.0f);
        featureLocations[7].head = new Vector3(0.124f, 0.626f, 0.0f);

        inverseAnimationFeatureLocations.Add("Human_Slide_Side_Animation", featureLocations);


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

        inverseAnimationFeatureLocations.Add("Human_Jump_Front_Animation", featureLocations);     // Shared locations
        inverseAnimationFeatureLocations.Add("Human_Jump_Back_Animation", featureLocations);

        featureLocations = new FeatureLocations[8];

        featureLocations[0].head = new Vector3(-0.064f, 0.312f, 0.0f);
        featureLocations[1].head = new Vector3(-0.064f, 0.436f, 0.0f);
        featureLocations[2].head = new Vector3(-0.064f, 0.436f, 0.0f);
        featureLocations[3].head = new Vector3(-0.064f, 0.436f, 0.0f);

        featureLocations[4].head = new Vector3(-0.064f, 0.436f, 0.0f);
        featureLocations[5].head = new Vector3(-0.064f, 0.436f, 0.0f);
        featureLocations[6].head = new Vector3(-0.064f, 0.436f, 0.0f);
        featureLocations[7].head = new Vector3(-0.064f, 0.436f, 0.0f);

        inverseAnimationFeatureLocations.Add("Human_Jump_Side_Animation", featureLocations);

        // FALLING --------------------

        // Side, front, Back

        featureLocations = new FeatureLocations[8];

        featureLocations[0].head = new Vector3(0.0f, 0.436f, 0.0f);
        featureLocations[1].head = new Vector3(0.0f, 0.436f, 0.0f);
        featureLocations[2].head = new Vector3(0.0f, 0.436f, 0.0f);
        featureLocations[3].head = new Vector3(0.0f, 0.436f, 0.0f);

        featureLocations[4].head = new Vector3(0.0f, 0.436f, 0.0f);
        featureLocations[5].head = new Vector3(0.0f, 0.436f, 0.0f);
        featureLocations[6].head = new Vector3(0.0f, 0.436f, 0.0f);
        featureLocations[7].head = new Vector3(0.0f, 0.436f, 0.0f);

        inverseAnimationFeatureLocations.Add("Human_Fall_Front_Animation", featureLocations);     // Shared locations
        inverseAnimationFeatureLocations.Add("Human_Fall_Back_Animation", featureLocations);

        featureLocations = new FeatureLocations[8];

        featureLocations[0].head = new Vector3(-0.064f, 0.436f, 0.0f);
        featureLocations[1].head = new Vector3(-0.064f, 0.436f, 0.0f);
        featureLocations[2].head = new Vector3(-0.064f, 0.436f, 0.0f);
        featureLocations[3].head = new Vector3(-0.064f, 0.436f, 0.0f);

        featureLocations[4].head = new Vector3(-0.064f, 0.436f, 0.0f);
        featureLocations[5].head = new Vector3(-0.064f, 0.436f, 0.0f);
        featureLocations[6].head = new Vector3(-0.064f, 0.436f, 0.0f);
        featureLocations[7].head = new Vector3(-0.064f, 0.436f, 0.0f);

        inverseAnimationFeatureLocations.Add("Human_Fall_Side_Animation", featureLocations);
    }

    // Designed to sync the sprite sheets in the "material/shader" 
    // so that it contains the sprite displayed in the "Sprite Renderer"
    [BurstCompile]
    private void LateUpdate()
    {
        //if (characterCreationEngine.isCreatingCharacter == false) return; // SHOULD PROBABLY CHECK GAME HAS STARTED!!!!

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
                        if (flipped) currentFeatureLocations = inverseAnimationFeatureLocations["Human_Idle_Side_Animation"][currentFrameNumber];
                        else currentFeatureLocations = animationFeatureLocations["Human_Idle_Side_Animation"][currentFrameNumber];
                        break;
                    case 1:
                        if (flipped) currentFeatureLocations = inverseAnimationFeatureLocations["Human_Idle_Front_Animation"][currentFrameNumber];
                        else currentFeatureLocations = animationFeatureLocations["Human_Idle_Front_Animation"][currentFrameNumber];
                        break;
                    case 2:
                        if (flipped) currentFeatureLocations = inverseAnimationFeatureLocations["Human_Idle_Back_Animation"][currentFrameNumber];
                        else currentFeatureLocations = animationFeatureLocations["Human_Idle_Back_Animation"][currentFrameNumber];
                        break;
                }

                // flip head sprites to match animation. slide unique
                ForceFlipFeatures(playerAnimation.spriteRenderer.flipX);    // <<<<<<<<<<<<<<<<<<<<<<<<<

                break;

            case "Human_Walk_Side_Animation":
            case "Human_Walk_Front_Animation":
            case "Human_Walk_Back_Animation":

                spriteRenderer.material.SetTexture("_BaseMap", playerAnimation.walkSpriteSheet);
                spriteRenderer.material.SetTexture("_BumpMap", playerAnimation.walkNormalMapSpriteSheet);

                switch (facing)
                {
                    case 0:
                        if(flipped) currentFeatureLocations = inverseAnimationFeatureLocations["Human_Walk_Side_Animation"][currentFrameNumber];
                        else currentFeatureLocations = animationFeatureLocations["Human_Walk_Side_Animation"][currentFrameNumber];
                        break;
                    case 1:
                        if (flipped) currentFeatureLocations = inverseAnimationFeatureLocations["Human_Walk_Front_Animation"][currentFrameNumber];
                        else currentFeatureLocations = animationFeatureLocations["Human_Walk_Front_Animation"][currentFrameNumber];
                        break;
                    case 2:
                        if (flipped) currentFeatureLocations = inverseAnimationFeatureLocations["Human_Walk_Back_Animation"][currentFrameNumber];
                        else currentFeatureLocations = animationFeatureLocations["Human_Walk_Back_Animation"][currentFrameNumber];
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
                        if(flipped) currentFeatureLocations = inverseAnimationFeatureLocations["Human_Run_Side_Animation"][currentFrameNumber];
                        else currentFeatureLocations = animationFeatureLocations["Human_Run_Side_Animation"][currentFrameNumber];
                        break;
                    case 1:
                        if(flipped) currentFeatureLocations = inverseAnimationFeatureLocations["Human_Run_Front_Animation"][currentFrameNumber];
                        else currentFeatureLocations = animationFeatureLocations["Human_Run_Front_Animation"][currentFrameNumber];
                        break;
                    case 2:
                        if (flipped) currentFeatureLocations = inverseAnimationFeatureLocations["Human_Run_Back_Animation"][currentFrameNumber];
                        else currentFeatureLocations = animationFeatureLocations["Human_Run_Back_Animation"][currentFrameNumber];
                        break;
                }

                break;

            case "Human_Slide_Side_Animation":
            case "Human_Slide_Front_Animation":
            case "Human_Slide_Back_Animation":

                spriteRenderer.material.SetTexture("_BaseMap", playerAnimation.slideSpriteSheet);
                spriteRenderer.material.SetTexture("_BumpMap", playerAnimation.slideNormalMapSpriteSheet);

                // flip head sprites to match animation. slide unique
                if (lastClipName != clipName) { ForceFlipFeatures(!flipped); }  // <<<<<<<<<<<<<<<<

                switch (facing)
                {
                    case 0:
                        if (flipped) currentFeatureLocations = inverseAnimationFeatureLocations["Human_Slide_Side_Animation"][currentFrameNumber];
                        else currentFeatureLocations = animationFeatureLocations["Human_Slide_Side_Animation"][currentFrameNumber];
                        break;
                    case 1:
                        if (flipped) currentFeatureLocations = inverseAnimationFeatureLocations["Human_Slide_Front_Animation"][currentFrameNumber];
                        else currentFeatureLocations = animationFeatureLocations["Human_Slide_Front_Animation"][currentFrameNumber];
                        break;
                    case 2:
                        if (flipped) currentFeatureLocations = inverseAnimationFeatureLocations["Human_Slide_Back_Animation"][currentFrameNumber];
                        else currentFeatureLocations = animationFeatureLocations["Human_Slide_Back_Animation"][currentFrameNumber];
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
                            if (flipped) currentFeatureLocations = inverseAnimationFeatureLocations["Human_Jump_Side_Animation"][currentFrameNumber / 2]; // divide by 2 as jump animtion is only 4 frames
                            else currentFeatureLocations = animationFeatureLocations["Human_Jump_Side_Animation"][currentFrameNumber / 2];
                            break;
                        case 1:
                            if (flipped) currentFeatureLocations = inverseAnimationFeatureLocations["Human_Jump_Front_Animation"][currentFrameNumber / 2];
                            else currentFeatureLocations = animationFeatureLocations["Human_Jump_Front_Animation"][currentFrameNumber / 2];
                            break;
                        case 2:
                            if (flipped) currentFeatureLocations = inverseAnimationFeatureLocations["Human_Jump_Back_Animation"][currentFrameNumber / 2];
                            else currentFeatureLocations = animationFeatureLocations["Human_Jump_Back_Animation"][currentFrameNumber / 2];
                            break;
                    }
                }
                else
                {
                    switch (facing)
                    {
                        case 0:
                            if (flipped) currentFeatureLocations = inverseAnimationFeatureLocations["Human_Jump_Side_Animation"][1]; // any frame other than '0'
                            else currentFeatureLocations = animationFeatureLocations["Human_Jump_Side_Animation"][1];
                            break;
                        case 1:
                            if (flipped) currentFeatureLocations = inverseAnimationFeatureLocations["Human_Jump_Front_Animation"][1];
                            else currentFeatureLocations = animationFeatureLocations["Human_Jump_Front_Animation"][1];
                            break;
                        case 2:
                            if (flipped) currentFeatureLocations = inverseAnimationFeatureLocations["Human_Jump_Back_Animation"][1];
                            else currentFeatureLocations = animationFeatureLocations["Human_Jump_Back_Animation"][1];
                            break;
                    }
                }

                spriteRenderer.material.SetTexture("_BaseMap", playerAnimation.jumpSpriteSheet);
                spriteRenderer.material.SetTexture("_BumpMap", playerAnimation.jumpNormalMapSpriteSheet);

                break;

            case "Human_Fall_Side_Animation":
            case "Human_Fall_Front_Animation":
            case "Human_Fall_Back_Animation":

                // Falling doesn't loop. So need to use total time and only when less than 1.0 (first animation cycle 0f - 1.0f)
                //if (totalTime < 1.0f)
                //{
                    switch (facing)
                    {
                        case 0:
                            if (flipped) currentFeatureLocations = inverseAnimationFeatureLocations["Human_Fall_Side_Animation"][currentFrameNumber]; // divide by 2 as jump animtion is only 4 frames
                            else currentFeatureLocations = animationFeatureLocations["Human_Fall_Side_Animation"][currentFrameNumber];
                            break;
                        case 1:
                            if (flipped) currentFeatureLocations = inverseAnimationFeatureLocations["Human_Fall_Front_Animation"][currentFrameNumber];
                            else currentFeatureLocations = animationFeatureLocations["Human_Fall_Front_Animation"][currentFrameNumber];
                            break;
                        case 2:
                            if (flipped) currentFeatureLocations = inverseAnimationFeatureLocations["Human_Fall_Back_Animation"][currentFrameNumber];
                            else currentFeatureLocations = animationFeatureLocations["Human_Fall_Back_Animation"][currentFrameNumber];
                            break;
                    }
                //}

                spriteRenderer.material.SetTexture("_BaseMap", playerAnimation.jumpSpriteSheet);
                spriteRenderer.material.SetTexture("_BumpMap", playerAnimation.jumpNormalMapSpriteSheet);

                break;
        }

        // Regardless of selection move features

        head.localPosition = currentFeatureLocations.head;

        lastClipName = clipName;
    }

    [BurstCompile]
    public void FlipFeatures(bool flip)
    {
        // Fetch the current Animation clip information for the base layer
        animatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        // Access the Animation clip name
        clipName = animatorClipInfo[0].clip.name;

        if (clipName == "Human_Slide_Side_Animation"
            || clipName == "Human_Slide_Front_Animation"
            || clipName == "Human_Slide_Back_Animation")
        {
            return;
        }

        flipped = flip;

        hairSprite.flipX = flip;
        eyeSprite.flipX = flip;
        eyeBrowSprite.flipX = flip;
        eyeBackgroundSprite.flipX = flip;
        beardSprite.flipX = flip;
        moustacheSprite.flipX = flip;

        if (flip)
        {
            eyes.localPosition = inverseFaceFeatureLocations[facing].eyes;
            eyeBrows.localPosition = inverseFaceFeatureLocations[facing].eyeBrows;
            eyeBackground.localPosition = inverseFaceFeatureLocations[facing].eyesBackground;
        }
        else 
        {
            eyes.localPosition = faceFeatureLocations[facing].eyes;
            eyeBrows.localPosition = faceFeatureLocations[facing].eyeBrows;
            eyeBackground.localPosition = faceFeatureLocations[facing].eyesBackground;
        }
    }

    // Used by slide which faces the other way and must be handled differently
    [BurstCompile]
    public void ForceFlipFeatures(bool flip)
    {
        //flipped = flip;

        hairSprite.flipX = flip;
        eyeSprite.flipX = flip;
        eyeBrowSprite.flipX = flip;
        eyeBackgroundSprite.flipX = flip;
        beardSprite.flipX = flip;
        moustacheSprite.flipX = flip;

        if (flip)
        {
            eyes.localPosition = inverseFaceFeatureLocations[facing].eyes;
            eyeBrows.localPosition = inverseFaceFeatureLocations[facing].eyeBrows;
            eyeBackground.localPosition = inverseFaceFeatureLocations[facing].eyesBackground;
        }
        else
        {
            eyes.localPosition = faceFeatureLocations[facing].eyes;
            eyeBrows.localPosition = faceFeatureLocations[facing].eyeBrows;
            eyeBackground.localPosition = faceFeatureLocations[facing].eyesBackground;
        }
    }
}
