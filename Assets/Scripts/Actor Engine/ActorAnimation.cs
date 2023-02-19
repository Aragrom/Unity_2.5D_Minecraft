using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class ActorAnimation : MonoBehaviour
{
    public ActorEngine actorEngine = null;

    public AnimatorOverrideController frontAnimationOverride = null;
    public AnimatorOverrideController backAnimationOverride = null;
    public AnimatorOverrideController sideAnimationOverride = null;

    public Animator[] animators = null;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        actorEngine = null;

        frontAnimationOverride = null;
        backAnimationOverride = null;
        sideAnimationOverride = null;

        animators = null;
    }

    [BurstCompile]
    public void Awake()
    {
        actorEngine = GetComponent<ActorEngine>();

        animators = new Animator[actorEngine.MAX_NUMBER_OF_ACTORS];
    }
}
