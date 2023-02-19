using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class PlayerCollision : MonoBehaviour
{
    public PlayerEngine playerEngine = null;

    // List used to detect when contact made is grounded.
    public List<Transform> detectedObjects = new List<Transform>();

    public Transform ground = null;
    //public bool hasNewGround = false;

    public bool isGroundedRigidbody = false;

    public Vector3 lastNormalDetected = Vector3.zero;

    public bool isRightWall = false;
    public bool isLeftWall = false;
    public bool isFloor = false;
    public bool isCeiling = false;
    public bool isSlope = false;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory
        playerEngine = null;

        detectedObjects.Clear();
        detectedObjects = null;
        ground = null;
    }

    // Start is called before the first frame update
    [BurstCompile]
    void Awake()
    {
        playerEngine = GameObject.Find("Player Engine").GetComponent<PlayerEngine>();
    }

    [BurstCompile]
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        lastNormalDetected = RoundVectorTwoDecimal(hit.normal);

        ground = hit.transform;

        SetSurfaceDetected(lastNormalDetected, playerEngine.playerRotation.direction);

        if (isLeftWall
            || isRightWall)
        {
            playerEngine.playerMovement.ResetVelocityX();
        }

        if (isCeiling)
        {
            //playerEngine.playerMovement.ResetVelocityY();
        }

        if (isFloor)
        {
            // Make sure grounded animations trigger asap.
            playerEngine.playerAnimation.SetIsJumping(false);

            ground = hit.transform;
        }
    }

    [BurstCompile]
    private void ResetHorizontalVelocity(PlayerRotation.Direction type)
    {
        switch ((int)type)
        {
            case (int)PlayerRotation.Direction.North:
            case (int)PlayerRotation.Direction.South:

                break;

            case (int)PlayerRotation.Direction.East:
            case (int)PlayerRotation.Direction.West:

                break;
        }
    }

    [BurstCompile]
    public Vector3 RoundVectorTwoDecimal(Vector3 vector)
    {
        // ROUND IS USING SYSTEM. Might not be the most efficent. Look for unity solution?

        return new Vector3((float)Math.Round(vector.x, 1), (float)Math.Round(vector.y, 1), (float)Math.Round(vector.z, 1));
    }

    [BurstCompile]
    public void SetSurfaceDetected(Vector3 normal, PlayerRotation.Direction type)
    {
        isFloor = false;
        isCeiling = false;
        isLeftWall = false;
        isRightWall = false;
        isSlope = false;

        switch (normal.ToString())
        {
            case "(1.0, 0.0, 0.0)":

                if (type == PlayerRotation.Direction.North) isLeftWall = true;
                if (type == PlayerRotation.Direction.South) isRightWall = true;

                break;

            case "(-1.0, 0.0, 0.0)":

                if (type == PlayerRotation.Direction.North) isRightWall = true;
                if (type == PlayerRotation.Direction.South) isLeftWall = true;

                break;

            case "(0.0, 0.0, 1.0)":

                if (type == PlayerRotation.Direction.East) isRightWall = true;
                if (type == PlayerRotation.Direction.West) isLeftWall = true;

                break;

            case "(0.0, 0.0, -1.0)":

                if (type == PlayerRotation.Direction.East) isLeftWall = true;
                if (type == PlayerRotation.Direction.West) isRightWall = true;

                break;

            case "(0.0, 1.0, 0.0)":

                isFloor = true;

                break;

            case "(0.0, -1.0, 0.0)":

                isCeiling = true;

                break;
        }
    }
}
