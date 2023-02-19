using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class ActorAI : MonoBehaviour
{
    public enum ActorState
    {
        Off,
        On, 
        Idle,
        Walking,
        Running,
        Sliding,
        Jumping,
        Falling,
        Attacking,
        JumpAttacking,
        Parrying,
        Blocking,

        Sleeping,
        Eating,
        Drinking,

        Sitting,
        LyingDown,
        Stretching,
        Dancing,

        Mining,
        LumberJack,
        Smithing,
        Tinkering,
        WashingLaundry,

    }

    public enum ActorBehaviour 
    {
        Aggressive,
        Defensive,
        Passive,
    }

    public enum ActorEmotion
    { 
        Sadness,
        Happiness,
        Fear,
        Anger,
        Suprise,
        Disgust,
    }

    public enum ActorDesire
    {
        Power,
        Independence,
        Curiosity,
        Order,
        Saving,
        Honor,         
        SocialContact,
        Family,
        Status, 
        Vengeance, 
        Romance, 
        Eating,
        PhysicalExercise, 
        Tranquility,

        Idealism,
        Acceptance,
    }

    public enum ActorJob
    { 
        Fighter,
        MountedFighter,
        Civilian,
        Farmer,
        Miner,
        BlackSmith,
        Engineer,
    }

    [BurstCompile]
    public void Awake()
    {
        
    }

    // Start is called before the first frame update
    [BurstCompile]
    void Start()
    {
        
    }

    // Update is called once per frame
    [BurstCompile]
    void Update()
    {
        
    }
}
