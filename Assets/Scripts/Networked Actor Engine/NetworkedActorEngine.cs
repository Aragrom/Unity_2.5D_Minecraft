using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class NetworkedActorEngine : MonoBehaviour
{
    public NetworkedActorPool networkedActorPool = null;
	public NetworkedActorMovement networkedActorMovement = null;

	[BurstCompile]
	private void OnDestroy()
	{
		// Free memory

		networkedActorPool = null;
		networkedActorMovement = null;
	}

	[BurstCompile]
    void Awake()
    {
        networkedActorPool = GetComponent<NetworkedActorPool>();
		networkedActorMovement = GetComponent<NetworkedActorMovement>();
	}
}
