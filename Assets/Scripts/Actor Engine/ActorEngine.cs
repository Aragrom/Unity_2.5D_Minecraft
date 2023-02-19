using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class ActorEngine : MonoBehaviour
{
    public ActorPool actorPool = null;
    public ActorMovement actorMovement = null;
    public ActorAnimation actorAnimation = null;
    public ActorRotation actorRotation = null;
    public ActorAI actorAI = null;
    public ActorPathing actorPathing = null;

    public int MAX_NUMBER_OF_ACTORS = 100;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        actorPool = null;
        actorMovement = null;
        actorAnimation = null;
        actorRotation = null;
        actorAI = null;
        actorPathing = null;
    }

    [BurstCompile]
    private void Awake()
    {
        actorPool = GetComponent<ActorPool>();
        actorMovement = GetComponent<ActorMovement>();
        actorAnimation = GetComponent<ActorAnimation>();
        actorRotation = GetComponent<ActorRotation>();
        actorAI = GetComponent<ActorAI>();
        actorPathing = GetComponent<ActorPathing>();
    }

    [BurstCompile]
    private void Update()
    {
        // using AI system decide what each actor should do.
        // - mimic player input?

        // Go through each active actor and move them.
        /*for (int i = 0; i < MAX_NUMBER_OF_ACTORS; i++)
        {
            if (actorPool.gameObjects[i].activeSelf)
            { 
                
            }
        }*/
    }
}
