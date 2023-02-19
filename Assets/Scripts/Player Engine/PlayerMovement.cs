using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using UnityEngine;

// Edit > Project Settings > Physics - "Auto Sync Transforms" - Enable 
// Required for moving platform character controller movement

[BurstCompile]
public class PlayerMovement : MonoBehaviour
{
    public PlayerEngine playerEngine = null;

    public Transform player = null;
    public CharacterController characterController = null;

	public Vector3 chunkPosition = Vector3.zero;
	public Vector3 positionInChunk = Vector3.zero;

	// =====================================================================================

	public CollisionFlags collisionFlags = CollisionFlags.None;

	public bool isGrounded = false;		// only used for detecting the change in characterController.isGrounded for landing behaviour
	public bool wasGrounded = false;	// used to stop falling when running off an edge - reset height

	public bool hasJustJumped = true;   // used to control jump acceleration
	public bool delayedJumpPossible = false;    // Used to allow a delayed jump when falling off the edge. Done with PlayerEngine.Fixed() for "timing queue".
	public float delayedJumpTimer = 0.0f;		// Actual time left to delay jump
	public float DELAYED_JUMP_DURATION = 1f / 15f;      // Delay jump window.

	public float defaultSpeed = 10f;
	public float walkSpeed = 10f;
	public float runSpeed = 20f;
	public float maxSpeed = 30f;
	public float defaultAirSpeed = 2.5f;
	public Vector2 velocity = Vector2.zero;
	public Vector3 rotatedVelocity = Vector3.zero;

	public float jump = 12f;
	public float jumpAcceleration = 20f;       // when jumping velocity.y becomes positive. While it is you can accelerate the jump (Hold the button)
	public float gravity = 32f;               // m/s ^2 / gravity = 9.81f;
	public float stayGroundedForce = 20.0f;

	public float friction = 3.948f;              // Multiplied by the velocity per frame to reduce speed
	public float stopThreshold = 0.05f;        // Used to become stationary along with friction

	public float pixelsPerUnit = 16;

	public float boost = 0.125f;

	public float lastHeight = 0f;	// Used to reset position if player walked of edge (wasGrounded)

	public float pixelSize = 0.125f;    // the amount moved to move a pixel

	[BurstCompile]
	private void OnDestroy()
	{
		// Free memory

		playerEngine = null;
		player = null;
		characterController = null;
	}

	[BurstCompile]
	public void Awake()
	{
		playerEngine = GetComponent<PlayerEngine>();

		characterController = player.GetComponent<CharacterController>();
	}

	[BurstCompile]
	public void Move(Vector3 displacement)
	{
		// ===============================

		displacement *= Time.deltaTime;

		// Perform move ======

		if (displacement != Vector3.zero)
		{
			if (characterController.isGrounded) playerEngine.playerMovement.lastHeight = playerEngine.playerMovement.player.position.y;

			collisionFlags = characterController.Move(displacement);
		}
	}

	[BurstCompile]
	public void Climb(Vector3 displacement)
	{

	}

	[BurstCompile]
	public void CheckForRunToggle(bool run)
	{
		if (run) defaultSpeed = runSpeed;
		else
		{
			if (velocity.x < walkSpeed / 2
				&& velocity.x > -walkSpeed / 2)
			{
				defaultSpeed = walkSpeed;
			}
		}
	}

	[BurstCompile]
	public Vector2 ApplyJump(Vector2 velocity)
	{
		wasGrounded = false;    // forcing falling off edge velocity stuff
		velocity.y = jump;
		hasJustJumped = true;
		delayedJumpPossible = false;
		delayedJumpTimer = 0;
		playerEngine.playerAnimation.SetIsJumping(true);

		return velocity;
	}

	[BurstCompile]
	public Vector2 ApplyJumpAcceleration(Vector2 velocity)
	{
		velocity.y += jumpAcceleration * Time.deltaTime;

		return velocity;
	}



	[BurstCompile]
	public void IncreaseHorizontalVelocity()
	{
		if (characterController.isGrounded)
		{
			velocity.x += defaultSpeed * Time.deltaTime;

			if (velocity.x < 0) playerEngine.playerAnimation.SetIsSliding(true);
			else playerEngine.playerAnimation.SetIsSliding(false);
		}
		else    // in the air
		{       // Make the change in velocity less when your going with it.
				// But slightly more when your going against. To allow fast turn around.

			if (velocity.x > 0) velocity.x += defaultAirSpeed * Time.deltaTime;
			else velocity.x += (defaultAirSpeed * 2f) * Time.deltaTime;
		}
	}

	[BurstCompile]
	public void DecreaseHorizontalVelocity()
	{
		if (characterController.isGrounded)
		{
			velocity.x -= defaultSpeed * Time.deltaTime;

			if (velocity.x > 0) playerEngine.playerAnimation.SetIsSliding(true);
			else playerEngine.playerAnimation.SetIsSliding(false);
		}
		else    // in the air
		{
			if (velocity.x < 0) velocity.x -= defaultAirSpeed * Time.deltaTime;
			else velocity.x -= (defaultAirSpeed * 2f) * Time.deltaTime;
		}
	}

	[BurstCompile]
	public Vector2 ApplyGravity(Vector2 velocity)
	{
		velocity.y -= gravity * Time.deltaTime;

		return velocity;
	}

	[BurstCompile]
	public Vector2 ApplyFriction(Vector2 velocity)
	{
		// create friction

		if (velocity.x != 0)
		{
			if (velocity.x > 0) velocity.x -= friction * Time.deltaTime;
			else velocity.x += friction * Time.deltaTime;
		}

		// check for almost stopped
		// Stop when movement is really small.

		if (velocity.x < stopThreshold
			&& velocity.x > -stopThreshold)
		{
			velocity.x = 0;

			// force stop sliding

			playerEngine.playerAnimation.SetIsSliding(false);
		}

		return velocity;
	}

	[BurstCompile]
	public Vector3 LimitVelocity(Vector3 velocity)
	{
		// Limit magnitude of velocity

		/*if (velocity.magnitude > maxSpeed)
		{
			velocity = velocity.normalized * maxSpeed;
		}*/

		if (velocity.y > maxSpeed)
		{
			velocity.y = maxSpeed;
		}

		if (velocity.y < -maxSpeed)
		{
			velocity.y = -maxSpeed;
		}

		if (velocity.x > defaultSpeed / 2)
		{
			velocity.x = defaultSpeed / 2;
		}

		if (velocity.x < -defaultSpeed / 2)
		{
			velocity.x = -defaultSpeed / 2;
		}

		return velocity;
	}

	[BurstCompile]
	public void ResetVelocityX()
	{
		velocity.x = 0;
	}

	[BurstCompile]
	public void ResetVelocityY()
	{
		velocity.y = 0;
	}

	[BurstCompile]
	public bool IsFalling(Vector2 velocity)
	{
		if (velocity.y < 0) return true;
		else return false;
	}

	[BurstCompile]
	private Vector3 PixelPerfectClamp(Vector3 displacement, float pixelsPerUnit)
	{
		Vector3 displacementInPixels = new Vector3(
			Mathf.RoundToInt(displacement.x * pixelsPerUnit),
			Mathf.RoundToInt(displacement.y * pixelsPerUnit),
			Mathf.RoundToInt(displacement.z * pixelsPerUnit)
			);

		return displacementInPixels / pixelsPerUnit;
	}

	/// <summary>
	/// Move the actors position to be in the center of the block in the x and z axis.
	/// </summary>
	[BurstCompile]
	public void CenterToBlock()
	{
		float x = Mathf.FloorToInt(player.transform.position.x) + 0.5f;  // 0.5f = half a block width
		float z = Mathf.FloorToInt(player.transform.position.z) + 0.5f;   //  0.5f = half a block width

		characterController.enabled = false;
		player.transform.position = new Vector3(x, player.transform.position.y, z);
		characterController.enabled = true;
	}

	[BurstCompile]
	public Vector3 SwitchVelocityDirection(bool controllerToRigidbody, PlayerRotation.Direction type, Vector3 velocity)
	{
		switch (type)
		{
			case PlayerRotation.Direction.North:

				// Do nothing velocity is fine.

				return velocity;

			case PlayerRotation.Direction.East:

				// Do Nothing

				if (controllerToRigidbody) return velocity = new Vector3(velocity.z, velocity.y, -velocity.x);
				else return velocity = new Vector3(-velocity.z, velocity.y, velocity.x);

			case PlayerRotation.Direction.South:

				velocity = new Vector3(-velocity.x, velocity.y, velocity.z);

				return velocity;

			case PlayerRotation.Direction.West:

				velocity = new Vector3(velocity.z, velocity.y, velocity.x);

				return velocity;
		}

		Debug.Log("Unreachable code reached! returned velocity = Vector3.zero");

		return Vector3.zero;
	}
}
