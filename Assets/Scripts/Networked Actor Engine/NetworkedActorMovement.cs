using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class NetworkedActorMovement : MonoBehaviour
{
    public NetworkedActorPool networkedActorPool = null;
    public TreadmillEngine treadmillEngine = null;          // need to true position for networked player positions

    public Vector3[] startPositions = new Vector3[100];     // need to lerp smoothly over time
    public Vector3[] targets = new Vector3[100];            // target position that the networked actor can move towards. (lerp)
    public float TIME_TO_REACH = 0.05f;
    public float time = 0f;

    public Vector3[] velocity = new Vector3[100];

    public float smoothTime = 0.3f;

    [BurstCompile]
    public void Awake()
    {
        networkedActorPool = GetComponent<NetworkedActorPool>();
        treadmillEngine = GameObject.Find("Treadmill Engine").GetComponent<TreadmillEngine>();
    }

    [BurstCompile]
    public void FixedUpdate()
    {
        // Lerp each 

        //time += Time.deltaTime/TIME_TO_REACH;

        for (int i = 0; i < networkedActorPool.active; i++)
        {
            networkedActorPool.transforms[i].gameObject.SetActive(true);

            // animate the position of the game object...
            //networkedActorPool.transforms[i].position = Vector3.Lerp(startPositions[i],
                //targets[i] - treadmillEngine.truePosition,
                //time * 20);

            // Smoothly move
            networkedActorPool.transforms[i].position = Vector3.SmoothDamp(networkedActorPool.transforms[i].position, targets[i] - treadmillEngine.truePosition, ref velocity[i], smoothTime);
        }
    }

    // used by treadmill engine to move everything by a chunk amount
    public void MoveActorsByAmount(Vector3 amount)
    {
        for (int i = 0; i < networkedActorPool.active; i++)
        {
            startPositions[i] += amount;
            networkedActorPool.transforms[i].position += amount;
        }
    }

    [BurstCompile]
    public void MoveActors(List<Vector3> positions)
    {
        if (networkedActorPool.active != positions.Count)
        {
            // length of online players on servers has changed.

            networkedActorPool.AdjustActiveGameObjects(positions.Count);
        }

        networkedActorPool.active = positions.Count;

        for (int i = 0; i < positions.Count; i++)
        {
            networkedActorPool.transforms[i].gameObject.SetActive(true);
            networkedActorPool.transforms[i].position = positions[i] - treadmillEngine.truePosition;
        }
    }

    [BurstCompile]
    public void SetTargets(List<Vector3> positions)
    {
        if (networkedActorPool.active < positions.Count)
        {
            // length of online players on servers has changed.

            networkedActorPool.AdjustActiveGameObjects(positions.Count);
        }

        for (int i = 0; i < positions.Count; i++)
        {
            startPositions[i] = networkedActorPool.transforms[i].position;
        }

        time = 0;

        networkedActorPool.active = positions.Count;

        targets = positions.ToArray();
    }
}
