using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class ActorRotation : MonoBehaviour
{
    public ActorEngine actorEngine = null;

    public Direction[] directions = null;
    public Facing[] facing = null;

    public enum Direction { North = 0, East = 1, South = 2, West = 3 }

    public enum Facing { Front, Back, Side }

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory
        directions = null;

        facing = null;
    }

    [BurstCompile]
    private void Awake()
    {
        actorEngine = GetComponent<ActorEngine>();

        directions = new Direction[actorEngine.MAX_NUMBER_OF_ACTORS];
        facing = new Facing[actorEngine.MAX_NUMBER_OF_ACTORS];
    }

    [BurstCompile]
    public Facing CalculateFacingToCamera(Direction camera, Vector3 velocity, Facing currentActorFacing)
    {
        switch (camera) 
        {
            case Direction.North:

                if (velocity.x > 0) 
                {
                    // Going East
                    // side
                    return Facing.Side;
                }
                else
                {
                    if (velocity.x < 0)
                    {
                        // Going West
                        // side
                        return Facing.Side;
                    }
                    else 
                    {
                        if (velocity.z > 0)
                        {
                            // Going North
                            // Back
                            return Facing.Back;
                        }
                        else
                        {
                            if (velocity.z < 0)
                            {
                                // Going South
                                // Front
                                return Facing.Front;
                            }
                        }
                    }
                }

                break;

            case Direction.South:

                if (velocity.x > 0)
                {
                    // Going East
                    // side
                    return Facing.Side;
                }
                else
                {
                    if (velocity.x < 0)
                    {
                        // Going West
                        // side
                        return Facing.Side;
                    }
                    else
                    {
                        if (velocity.z > 0)
                        {
                            // Going South
                            // Back
                            return Facing.Back;
                        }
                        else
                        {
                            if (velocity.z < 0)
                            {
                                // Going North
                                // Front
                                return Facing.Front;
                            }
                        }
                    }
                }

                break;

            case Direction.East:

                if (velocity.x > 0)
                {
                    // Going East
                    // Back
                    return Facing.Back;
                }
                else
                {
                    if (velocity.x < 0)
                    {
                        // Going West
                        // Front
                        return Facing.Front;
                    }
                    else
                    {
                        if (velocity.z > 0)
                        {
                            // Going North
                            // Side
                            return Facing.Side;
                        }
                        else
                        {
                            if (velocity.z < 0)
                            {
                                // Going South
                                // Side
                                return Facing.Side;
                            }
                        }
                    }
                }

                break;

            case Direction.West:

                if (velocity.x > 0)
                {
                    // Going East
                    // Back
                    return Facing.Back;
                }
                else
                {
                    if (velocity.x < 0)
                    {
                        // Going West
                        // Front
                        return Facing.Front;
                    }
                    else
                    {
                        if (velocity.z > 0)
                        {
                            // Going North
                            // Side
                            return Facing.Side;
                        }
                        else
                        {
                            if (velocity.z < 0)
                            {
                                // Going South
                                // Side
                                return Facing.Side;
                            }
                        }
                    }
                }

                break;
        }

        return currentActorFacing;
    }
}
