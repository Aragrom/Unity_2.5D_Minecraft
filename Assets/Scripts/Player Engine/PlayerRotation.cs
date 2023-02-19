using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class PlayerRotation : MonoBehaviour
{
	public PlayerEngine playerEngine = null;

	public Transform player = null;

	public float rotationInY = 0;

	public enum Direction { North, East, South, West }

	public Direction direction = Direction.North;

	// =================

	public float speed = 10.0f;

	public bool isRotating = false;
	public bool isRotatingClockWise = false;

	public Vector3 targetAngle = new Vector3(0f, 0f, 0f);
	public Vector3 currentAngle;

	public TMP_Text tmpTextCardinalDirection = null;
	public string north = "north";
	public string south = "south";
	public string east = "east";
	public string west = "west";

	[BurstCompile]
	private void OnDestroy()
	{
		// Free memory

		playerEngine = null;
		player = null;
	}

	[BurstCompile]
	public void Awake()
    {
		playerEngine = GetComponent<PlayerEngine>();
    }

	[BurstCompile]
	public void Update()
    {
        // Rotate overtime.

        if (isRotating)
        {
			if (isRotatingClockWise)
			{
				currentAngle = new Vector3(
						 Mathf.LerpAngle(currentAngle.x, targetAngle.x, Time.deltaTime * speed),
						 Mathf.LerpAngle(currentAngle.y, targetAngle.y, Time.deltaTime * speed),
						 Mathf.LerpAngle(currentAngle.z, targetAngle.z, Time.deltaTime * speed));
			}
			else
			{
				currentAngle = new Vector3(
				 Mathf.LerpAngle(currentAngle.x, targetAngle.x, Time.deltaTime * speed),
				 Mathf.LerpAngle(currentAngle.y, targetAngle.y, Time.deltaTime * speed),
				 Mathf.LerpAngle(currentAngle.z, targetAngle.z, Time.deltaTime * speed));
			}

			// rotate player by
			//player.transform.eulerAngles = currentAngle;

			// rotate camera rig by
			playerEngine.main.zoom.cameraRig.eulerAngles = currentAngle;

			if (currentAngle.y > rotationInY - 0.005f
				&& currentAngle.y < rotationInY + 0.005f)
			{
				// rotate player to complete rotation
				//player.transform.eulerAngles = new Vector3(0, rotationInY, 0);

				// rotate camera rig to the complete rotation
				playerEngine.main.zoom.cameraRig.eulerAngles = new Vector3(0, rotationInY, 0);

				isRotating = false;

				// NOTE ANGLE NEEDS CHANGED. EVENTUALLY NEVER REACHES DESTINATION? BUG? DOES NOT RESET BACK FACE COLOUR <<<<<
				playerEngine.main.blockEngine.subChunkMaterial.SetInt("Boolean_f0cac22185be4c50ac0b6f802997cfc6", 0);

				// Enable cross section
				//playerEngine.main.crossSectionEngine.crossSectionPool.root.gameObject.SetActive(true);
			}
		}
	}

    [BurstCompile]
	public Vector3 RotateVelocity(Vector3 velocity, Direction type)
	{
		switch (type)
		{
			case Direction.North:
				return velocity;

			case Direction.South:
				return new Vector3(-velocity.x, velocity.y, 0);

			case Direction.East:
				return new Vector3(0, velocity.y, -velocity.x);

			case Direction.West:
				return new Vector3(0, velocity.y, velocity.x);
		}

		// code should be unreachable

		Debug.Log("Unreachable code reached");
		return Vector3.zero;
	}

	[BurstCompile]
	// Rotates the Player and all attached child gameobjects around the Y-Axis
	// also rotates the cross section (only has two ways it can face)
	public void Rotate(float angle)
	{
		// Change material property for back face colour (Grey > black)
		// Back faces are grey to avoid gaps in block maps leaving black artifacts on screen.
		playerEngine.main.blockEngine.subChunkMaterial.SetInt("Boolean_f0cac22185be4c50ac0b6f802997cfc6", 1);

		// Increment and assign variables for target rotation
		// NOTE ANGLE NEEDS CHANGED. EVENTUALLY NEVER REACHES DESTINATION? BUG? DOES NOT RESET TEXTURE <<<<<
		rotationInY += angle;
		targetAngle = new Vector3(0, rotationInY, 0);

		// Limit and set direction
		// Wraps around.
		if (angle > 0)
		{
			isRotatingClockWise = true;
			if (direction != Direction.West) direction++;
			else { direction = Direction.North; }
		}
		else
		{
			isRotatingClockWise = false;
			if (direction != Direction.North) direction--;
			else { direction = Direction.West; }
		}

		// keep the player in the center of the lane.
		playerEngine.playerMovement.CenterToBlock();

		// Rotate cross section.
		//if (playerEngine.main.crossSectionEngine.crossSectionPool.root.rotation == Quaternion.Euler(0, 90, 0)) playerEngine.main.crossSectionEngine.crossSectionPool.root.rotation = Quaternion.Euler(0, 0, 0);
		//else playerEngine.main.crossSectionEngine.crossSectionPool.root.rotation = Quaternion.Euler(0, 90, 0);

		// Disable cross section.
		//playerEngine.main.crossSectionEngine.crossSectionPool.root.gameObject.SetActive(false);

		// Tell cross section engine to update

		// Set so rotation will happen in fixed update
		isRotating = true;

		// set direction in look class so it can move correctly

		playerEngine.main.look.ResetCameraRigLocalPositon();

		// Sync up the "look" camera move direction with correct rotation

		playerEngine.main.look.direction = (Look.Direction)direction;

		//playerEngine.playerAnimation.sprite.eulerAngles = targetAngle;

		SetCardinalDirectionOnUI();
	}

	public void SetCardinalDirectionOnUI()
	{
		switch (direction)
		{
			case Direction.North:

				tmpTextCardinalDirection.text = north;

				break;

			case Direction.South:

				tmpTextCardinalDirection.text = south;

				break;

			case Direction.East:

				tmpTextCardinalDirection.text = east;

				break;

			case Direction.West:

				tmpTextCardinalDirection.text = west;

				break;
		}
	}
}
