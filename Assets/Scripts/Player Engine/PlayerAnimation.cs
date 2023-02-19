using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class PlayerAnimation : MonoBehaviour
{
    public Animator animator = null;

    public PlayerFeatureAnimation playerFeatureAnimation = null;

    public SpriteRenderer spriteRenderer = null;
    public Transform sprite = null;       // performing rotation on object to so Transform instead of GameObject

    public GameObject headGameObject = null;

    public float walkSpeedModifier = 0.25f;

    public Texture2D idleSpriteSheet = null;
    public Texture2D walkSpriteSheet = null;
    public Texture2D runSpriteSheet = null;
    public Texture2D jumpSpriteSheet = null;
    public Texture2D slideSpriteSheet = null;

    public Texture2D idleNormalMapSpriteSheet = null;
    public Texture2D walkNormalMapSpriteSheet = null;
    public Texture2D runNormalMapSpriteSheet = null;
    public Texture2D jumpNormalMapSpriteSheet = null;
    public Texture2D slideNormalMapSpriteSheet = null;

    public AnimatorClipInfo[] animatorClipInfo = new AnimatorClipInfo[0];
    public string clipName = "";

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        animator = null;
        spriteRenderer = null;
    }

    [BurstCompile]
    private void Awake()
    {
        sprite = GameObject.Find("Player/Camera Rig/Player Sprite").transform;
        spriteRenderer = sprite.GetComponent<SpriteRenderer>();

        //Make sure to enable the Keywords
        spriteRenderer.material.EnableKeyword("_NORMALMAP");
    }

    // Designed to sync the sprite sheets in the "material/shader" 
    // so that it contains the sprite displayed in the "Sprite Renderer"
    [BurstCompile]
    private void LateUpdate()
    {
        //Fetch the current Animation clip information for the base layer
        animatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        //Access the current length of the clip
        //m_CurrentClipLength = m_CurrentClipInfo[0].clip.length;
        //Access the Animation clip name
        clipName = animatorClipInfo[0].clip.name;            

        switch (clipName)
        {
            case "Human_Idle_Side_Animation":
            case "Human_Idle_Front_Animation":
            case "Human_Idle_Back_Animation":

                spriteRenderer.material.SetTexture("_BaseMap", idleSpriteSheet);
                spriteRenderer.material.SetTexture("_BumpMap", idleNormalMapSpriteSheet);

                break;

            case "Human_Walk_Side_Animation":
            case "Human_Walk_Front_Animation":
            case "Human_Walk_Back_Animation":

                spriteRenderer.material.SetTexture("_BaseMap", walkSpriteSheet);
                spriteRenderer.material.SetTexture("_BumpMap", walkNormalMapSpriteSheet);

                break;

            case "Human_Run_Side_Animation":
            case "Human_Run_Front_Animation":
            case "Human_Run_Back_Animation":

                spriteRenderer.material.SetTexture("_BaseMap", runSpriteSheet);
                spriteRenderer.material.SetTexture("_BumpMap", runNormalMapSpriteSheet);

                break;

            case "Human_Jump_Side_Animation":
            case "Human_Jump_Front_Animation":
            case "Human_Jump_Back_Animation":

                spriteRenderer.material.SetTexture("_BaseMap", jumpSpriteSheet);
                spriteRenderer.material.SetTexture("_BumpMap", jumpNormalMapSpriteSheet);

                break;

            case "Human_Slide_Side_Animation":
            case "Human_Slide_Front_Animation":
            case "Human_Slide_Back_Animation":

                spriteRenderer.material.SetTexture("_BaseMap", slideSpriteSheet);
                spriteRenderer.material.SetTexture("_BumpMap", slideNormalMapSpriteSheet);

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
            playerFeatureAnimation.FlipFeatures(false);
        }
        if (speed < 0)
        {
            spriteRenderer.flipX = true;
            playerFeatureAnimation.FlipFeatures(true);
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
}
