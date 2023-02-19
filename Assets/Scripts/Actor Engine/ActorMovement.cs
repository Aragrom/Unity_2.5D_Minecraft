using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class ActorMovement : MonoBehaviour
{
	public ActorEngine actorEngine = null;

	//--------------------------------------------------

	public Transform[] transforms = null;
	public CharacterController[] characterControllers = null;

	public Vector3[] chunkPosition = null;
	public Vector3[] positionInChunk = null;

	public CollisionFlags[] collisionFlags = null;

	public bool[] isGrounded = null;     // only used for detecting the change in characterController.isGrounded for landing behaviour
	public bool[] wasGrounded = null;    // used to stop falling when running off an edge - reset height

	public float[] lastHeight = null;   // Used to reset position if player walked off edge (wasGrounded)

	public Vector2[] velocity = null;
	public Vector3[] rotatedVelocity = null;

	//---------------------------------------------------

	public float defaultSpeed = 10f;
	public float walkSpeed = 10f;
	public float runSpeed = 20f;
	public float maxSpeed = 30f;
	public float defaultAirSpeed = 2.5f;

	public float jump = 12f;
	public float jumpAcceleration = 20f;       // when jumping velocity.y becomes positive. While it is you can accelerate the jump (Hold the button)
	public float gravity = 32f;               // m/s ^2 / gravity = 9.81f;
	public float stayGroundedForce = 20.0f;

	public float friction = 3.948f;              // Multiplied by the velocity per frame to reduce speed
	public float stopThreshold = 0.05f;        // Used to become stationary along with friction

	public float boost = 0.125f;

	[BurstCompile]
	private void OnDestroy()
	{
		// Free memory

		actorEngine = null;

		transforms = null;
		characterControllers = null;
		chunkPosition = null;
		positionInChunk = null;
		collisionFlags = null;
		isGrounded = null;
		wasGrounded = null;
		lastHeight = null;
		velocity = null;
		rotatedVelocity = null;
	}

	[BurstCompile]
	private void Awake()
    {
		actorEngine = GetComponent<ActorEngine>();

		transforms = new Transform[actorEngine.MAX_NUMBER_OF_ACTORS];
		characterControllers = new CharacterController[actorEngine.MAX_NUMBER_OF_ACTORS];

		chunkPosition = new Vector3[actorEngine.MAX_NUMBER_OF_ACTORS];
		positionInChunk = new Vector3[actorEngine.MAX_NUMBER_OF_ACTORS];

		collisionFlags = new CollisionFlags[actorEngine.MAX_NUMBER_OF_ACTORS];

		isGrounded = new bool[actorEngine.MAX_NUMBER_OF_ACTORS];     // only used for detecting the change in characterController.isGrounded for landing behaviour
		wasGrounded = new bool[actorEngine.MAX_NUMBER_OF_ACTORS];    // used to stop falling when running off an edge - reset height

		lastHeight = new float[actorEngine.MAX_NUMBER_OF_ACTORS];   // Used to reset position if player walked off edge (wasGrounded)

		velocity = new Vector2[actorEngine.MAX_NUMBER_OF_ACTORS];
		rotatedVelocity = new Vector3[actorEngine.MAX_NUMBER_OF_ACTORS];
	}
}


