using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class ActorPool : MonoBehaviour
{
    private ActorEngine actorEngine = null;

    public GameObject prefab = null;

    public GameObject[] gameObjects = null;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        prefab = null;
        gameObjects = null;
    }

    public void Start()
    {
        actorEngine = GetComponent<ActorEngine>();

        gameObjects = new GameObject[actorEngine.MAX_NUMBER_OF_ACTORS];

        // Removed as not currently being used
        //SetUp();
    }

    [BurstCompile]
    public void SetUp()
    {
        for (int i = 0; i < actorEngine.MAX_NUMBER_OF_ACTORS; i++)
        {
            CreateActor(i);
        }
    }

    [BurstCompile]
    public void CreateActor(int index)
    {
        GameObject actor = Instantiate(prefab);

        // Add to pool
        gameObjects[index] = actor;

        actorEngine.actorMovement.transforms[index] = actor.transform;
        actorEngine.actorMovement.characterControllers[index] = actor.GetComponent<CharacterController>();

        actorEngine.actorAnimation.animators[index] = actor.transform.Find("Actor Sprite").GetComponent<Animator>();

        actor.SetActive(false);
    }
}
