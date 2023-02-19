using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class PlayerEngine : MonoBehaviour
{
	public Main main = null;

	public InputEngine inputEngine = null;
    public PlayerMovement playerMovement = null;
    public PlayerAnimation playerAnimation = null;
    public PlayerRotation playerRotation = null;
	public PlayerAddRemoveBlocks playerAddRemoveBlocks = null;
	public PlayerCombat playerCombat = null;

	// On Player GameObject
	public PlayerCollision playerCollision = null;

	public LayerMask Ground;

	[BurstCompile]
	private void OnDestroy()
	{
		// Free memory

		main = null;
		inputEngine = null;

		playerMovement = null;
		playerAnimation = null;
		playerRotation = null;
		playerAddRemoveBlocks = null;
		playerCombat = null;

		playerCollision = null;
	}

	// Start is called before the first frame update
	[BurstCompile]
    void Awake()
    {
		main = GameObject.Find("Main").GetComponent<Main>();

		inputEngine = GameObject.Find("Input Engine").GetComponent<InputEngine>();

        playerMovement = GetComponent<PlayerMovement>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerRotation = GetComponent<PlayerRotation>();
		playerAddRemoveBlocks = GetComponent<PlayerAddRemoveBlocks>();
		playerCombat = GetComponent<PlayerCombat>();

		playerCollision = GameObject.Find("Player").GetComponent<PlayerCollision>();
	}

	// Start is called before the first frame update
	[BurstCompile]
	void Start()
	{
		this.gameObject.SetActive(false);
	}

    // Update is called once per frame
    [BurstCompile]
	void Update()
	{
		CharacterControllerMovement();
		AddRemoveBlocks();
	}

	[BurstCompile]
	public void CharacterControllerMovement()
	{
		if (playerMovement.characterController.isGrounded) playerAnimation.SetIsJumping(false);

		// Allow delayed jump after falling off edge.
		// In update to be connect to when rendering is done. visual queue.

		if (playerMovement.delayedJumpTimer > 0)
		{
			playerMovement.delayedJumpTimer -= Time.deltaTime;
		}
		else
		{
			playerMovement.delayedJumpPossible = false;
		}

		// Detect player has landed
		if (playerMovement.isGrounded == false
			&& playerMovement.characterController.isGrounded == true)
		{
			playerMovement.wasGrounded = true;

			// Stop delayed jump being possible

			playerMovement.delayedJumpTimer = 0f;
			playerMovement.delayedJumpPossible = false;
		}

		// used to detect landing only. the change from false to true shows this - landing.
		// everything else should refer to characterController.isGrounded directly.
		playerMovement.isGrounded = playerMovement.characterController.isGrounded;

		// Reset height if just fallen off an edge. 
		// Stop fast falling off edge by removing the large downward velocity on y axis
		// Move the players position up to the last height detect when on the platform

		if (playerMovement.characterController.isGrounded == false
			&& playerMovement.wasGrounded == true)
		{
			// wasGrounded set false when jumping and when falling off edge
			// used to stop this effect from happening every frame.
			playerMovement.wasGrounded = false;

			// Reset y velocity
			playerMovement.velocity.y = 0;

			//playerMovement.player.position = new Vector3(playerMovement.player.position.x, playerMovement.lastHeight, playerMovement.player.position.z);

			playerMovement.delayedJumpTimer = playerMovement.DELAYED_JUMP_DURATION;
			playerMovement.delayedJumpPossible = true;
		}

		if (playerMovement.characterController.isGrounded) playerMovement.CheckForRunToggle(inputEngine.run);

		// Apply horizontal movement based on player input

		if (inputEngine.move.x < 0)	playerMovement.DecreaseHorizontalVelocity();
		if (inputEngine.move.x > 0) playerMovement.IncreaseHorizontalVelocity();

		// Apply gravity or (Friction and stay grounded force)

		if (playerMovement.characterController.isGrounded == false) playerMovement.velocity = playerMovement.ApplyGravity(playerMovement.velocity);
		else
		{
			playerMovement.velocity.y = -playerMovement.stayGroundedForce; // Keep grounded
			playerMovement.velocity = playerMovement.ApplyFriction(playerMovement.velocity);    // apply friction force
		}

		// Once player is descending do not allow jump acceleration

		if (playerMovement.IsFalling(playerMovement.velocity))
		{
			playerMovement.hasJustJumped = false;
		}

		// Add Jump force to velocity

		if (inputEngine.jump
			&& playerMovement.delayedJumpPossible == false
			&& playerMovement.hasJustJumped == false
			&& playerMovement.characterController.isGrounded)
		{
			playerMovement.velocity = playerMovement.ApplyJump(playerMovement.velocity);

			playerAnimation.SetIsSliding(false);
		}
		else
		{
			if (inputEngine.jump
			&& playerMovement.delayedJumpPossible)
			{
				playerMovement.velocity = playerMovement.ApplyJump(playerMovement.velocity);
				playerAnimation.SetIsSliding(false);
			}
			else
			{
				if (playerMovement.characterController.isGrounded) playerAnimation.SetIsJumping(false);
			}
		}

		// Add Jump Acceleration to velocity

		if (inputEngine.jump
			&& playerMovement.hasJustJumped == true)
		{
			playerMovement.velocity = playerMovement.ApplyJumpAcceleration(playerMovement.velocity);
		}

		// Limit the size of speed/velocity

		playerMovement.velocity = playerMovement.LimitVelocity(playerMovement.velocity);

		// Ready a vector3 to hold the actual movement (vector2 -> vector3)

		Vector3 displacement = playerMovement.velocity;

		// Rotate the vector to move in the correct axis for the direction (North, east..)

		displacement = playerRotation.RotateVelocity(displacement, playerRotation.direction);
		playerMovement.rotatedVelocity = displacement;

		// Using the character controller peform the move

		playerMovement.Move(displacement);

		// Control the speed/transitions of the animator/animations by assigning values to the animator

		playerAnimation.SetAnimationSpeed(playerMovement.velocity.x);
		playerAnimation.SetSpeed(playerMovement.velocity.x);
		playerAnimation.SetVelocityY(playerMovement.velocity.y);

		// Using the sprite renderer flip the sprite/animation

		playerAnimation.Flip(playerMovement.velocity.x);

		// Rotate the player

		if (inputEngine.rotateLeft)
		{
			playerRotation.Rotate(90);
		}

		if (inputEngine.rotateRight)
		{
			playerRotation.Rotate(-90);
		}

		// Add/Remove blocks and wedges from chunks

		//actor.actorEditTerrain.Shoot();

		// Cancel input - playerInput should re trigger it again

		inputEngine.rotateRight = false;
		inputEngine.rotateLeft = false;

		// Need to say we are not jumping so boosting/acceleration (holding A/jump) stops being applied
		if(playerMovement.velocity.y < 0) inputEngine.jump = false;

		// Trigger treadmill engine to process

		//main.treadmillEngine.ManualUpdate(); // <<<< take note this is where treadmill is called
	}

	[BurstCompile]
	public void AddRemoveBlocks()
	{
		if (inputEngine.dig)
		{
			inputEngine.dig = false;

            if (playerAddRemoveBlocks.hasBlock)
            {
                playerAddRemoveBlocks.AddBlock(); 
            }
            else 
			{
				playerAddRemoveBlocks.RemoveBlock();
			}
		}
	}
}