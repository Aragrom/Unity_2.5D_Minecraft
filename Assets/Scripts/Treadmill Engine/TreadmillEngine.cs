using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using UnityEngine;

// Edit > Project Settings > Physics - "Auto Sync Transforms" - Enable 
// - Required to correctly move the "chunks" around the world and have there positions updated internally by unity 

[BurstCompile]
public class TreadmillEngine : MonoBehaviour
{
    private BlockEngine blockEngine = null;
    private TerrainEngine terrainEngine = null;
    private PlayerEngine playerEngine = null;
    private NetworkedActorMovement networkActorMovement = null;

    public bool manualUpdate = false;

    public bool moveForward = false;
    public bool moveBackward = false;
    public bool moveRight = false;
    public bool moveLeft = false;

    public float defaultMapSize = 512;                              // size of the height map (limit = 46,336 / ?2,147,483,647 - 46,340.9500010519853390887900102 )
    public int SUB_MAP_SIZE = 16;                            // grid - size * size

    public Dictionary<Vector3, Transform> tempPool = new Dictionary<Vector3, Transform>();

    public Transform viewer = null;

    public Vector3 moveDirection = Vector3.zero;

    public Vector3 truePosition = Vector3.zero;     // ViewerPosition + TruePosition = actual position.

    public Vector3 chunkPosition = Vector3.zero;

    public Vector3 forwardDirection = new Vector3(0, 0, 16);
    public Vector3 backwardDirection = new Vector3(0, 0, -16);
    public Vector3 leftDirection = new Vector3(-16, 0, 0);
    public Vector3 rightDirection = new Vector3(16, 0, 0);
    //public Vector3 upDirection = new Vector3(0, 16, 0);
    //public Vector3 downDirection = new Vector3(0, -16, 0);

    public int gridSize = 32;
    public int gridHeight = 32;

    // Size in which to update and get new maps.

    public int CHUNK_SIZE = 16;        // x/z the player can move before everything moves.
    public int CHUNK_HEIGHT = 16;       // y the player can move before everything moves.

    // Size in which to move all objects back inside the origin.

    public int MAX_SIZE = 32;
    public int MIN_SIZE = 16;
    public int size = 32;

    public bool hasMoved = false;

    public TMP_Text tmpTextTruePosition = null;

    public Rect boundary = new Rect(-256, -256, 240, 240);

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        blockEngine = null;
        terrainEngine = null;
        //crossSectionEngine = null;
        playerEngine = null;

        tempPool.Clear();
        tempPool = null;

        viewer = null;

        tmpTextTruePosition = null;
    }

    [BurstCompile]
    public void Awake()
    {
        blockEngine = GameObject.Find("Block Engine").GetComponent<BlockEngine>();
        terrainEngine = GameObject.Find("Terrain Engine").GetComponent<TerrainEngine>();
        //crossSectionEngine = GameObject.Find("Cross Section Engine").GetComponent<CrossSectionEngine>();
        playerEngine = GameObject.Find("Player Engine").GetComponent<PlayerEngine>();

        networkActorMovement = GameObject.Find("Networked Actor Engine").GetComponent<NetworkedActorMovement>();

        tmpTextTruePosition = GameObject.Find("Canvas Stats").transform.Find("Text True Position").GetComponent<TMP_Text>();
    }

    [BurstCompile]
    public void Resize(int size, bool isBigger, int differenceBetweenOldSizeAndNewSize)
    {
        // is the same size return;
        //if (this.size == size) { return; }

        this.size = size;

        if (isBigger)
        {
            // Resize boundary
            boundary.x -= differenceBetweenOldSizeAndNewSize * SUB_MAP_SIZE;
            boundary.y -= differenceBetweenOldSizeAndNewSize * SUB_MAP_SIZE;
            boundary.width += differenceBetweenOldSizeAndNewSize * SUB_MAP_SIZE;
            boundary.height += differenceBetweenOldSizeAndNewSize * SUB_MAP_SIZE;
        }
        else
        {
            // Resize boundary
            boundary.x += differenceBetweenOldSizeAndNewSize * SUB_MAP_SIZE;
            boundary.y += differenceBetweenOldSizeAndNewSize * SUB_MAP_SIZE;
            boundary.width -= differenceBetweenOldSizeAndNewSize * SUB_MAP_SIZE;
            boundary.height -= differenceBetweenOldSizeAndNewSize * SUB_MAP_SIZE;
        }

        //if (blockEngine.main.optionEngine.inGame == false) { return; }
    }

    public void Update()
    {
        ManualUpdate();
    }


    // Called from PlayerMovement.CharacterControllerMovement()
    // FixedUpdate
    [BurstCompile]
    public void ManualUpdate()
    {
        // Stop processing until game is ready.

        //if (terrainEngine.setUp == false)
        //{
            //return;
        //}

        // Wait until new terrain is done until you generate more.

        /*if (terrainEngine.newTerrainQueue.hasNewTerrain
                || terrainEngine.newTerrainQueue.newSortedTerrain)
        {
            return;
        }*/

        if (CheckForNewChunkDirection()
            || manualUpdate)    // manual update here is for debugging only.
        {
            hasMoved = true;

            blockEngine.main.loadingBar.loadingVisualizer.SetActive(true);

            playerEngine.playerMovement.characterController.enabled = false;

            if (moveForward)
            {
                moveForward = false;

                moveDirection = forwardDirection;

                MoveForward();

                networkActorMovement.MoveActorsByAmount(forwardDirection);
            }
            else
            {
                if (moveBackward)
                {
                    moveBackward = false;

                    moveDirection = backwardDirection;

                    MoveBackward();

                    networkActorMovement.MoveActorsByAmount(backwardDirection);
                }
                else
                {
                    if (moveRight)
                    {
                        moveRight = false;

                        moveDirection = rightDirection;

                        MoveRight();

                        networkActorMovement.MoveActorsByAmount(rightDirection);
                    }
                    else
                    {
                        if (moveLeft)
                        {
                            moveLeft = false;

                            moveDirection = leftDirection;

                            MoveLeft();

                            networkActorMovement.MoveActorsByAmount(leftDirection);
                        }
                    }
                }
            }

            Physics.SyncTransforms();

            playerEngine.playerMovement.characterController.enabled = true;

            tmpTextTruePosition.text = truePosition.ToString();
        }
    }

    [BurstCompile]
    public void MoveForward()
    {
        truePosition += backwardDirection;

        blockEngine.terrain.position += forwardDirection;

        // tell terrain system to reassign heights ====== bad relationship <<<<< to many dots .. .

        Vector2 tempPosition = new Vector2(truePosition.x - 2048, truePosition.z - 2048);

        terrainEngine.newTerrainQueue.AddToQueue(tempPosition, terrainEngine.terrainHeightMap.SUB_MAP_SIZE + 2, 1);

        playerEngine.playerMovement.player.position += forwardDirection;

        // ==============================================
        // MOVE BLOCK POOL
        // ==============================================

        // Needs to be new Dictionary. .Clear() broke the system - investigate carefully.
        tempPool = new Dictionary<Vector3, Transform>();

        Vector3 newPosition = Vector3.zero;

        foreach (Vector3 key in blockEngine.blockPool.chunkPool.Keys)
        {
            // Make position is currently inside the treamill boundary. 
            // More chunks could be getting added outside of the boundary.
            if (key.x > boundary.width
                || key.x < boundary.x
                || key.z > boundary.height
                || key.z < boundary.y)
            {
                tempPool.Add(key, blockEngine.blockPool.chunkPool[key]);
            }
            else
            {
                // Detect edges to wrap around.
                // When edge chunks are discovered - Mark them for needing updated.

                // Wrap around

                //if (key.z == (defaultMapSize / 2) - SUB_MAP_SIZE)   // 32 = 240
                if (key.z == boundary.height)
                {
                    newPosition = new Vector3(blockEngine.blockPool.chunkPool[key].transform.position.x,
                        blockEngine.blockPool.chunkPool[key].transform.position.y,
                        boundary.y);
                    //-(defaultMapSize / 2));

                    /*newPosition.x = blockEngine.blockPool.chunkPool[key].transform.position.x;
                    newPosition.y = blockEngine.blockPool.chunkPool[key].transform.position.y;
                    newPosition.z = boundary.y;*/

                    blockEngine.blockPool.chunkPool[key].transform.Translate(backwardDirection * (size - 1));
                    //blockEngine.blockPool.chunkPool[key].transform.position = newPosition;

                    tempPool.Add(newPosition, blockEngine.blockPool.chunkPool[key]);

                    // Is the chunk already in the queue to be updated?

                    if (blockEngine.newBlockQueue.CheckIsAlreadyInQueue(blockEngine.blockPool.chunkPool[key].name) == false)
                    {
                        // NOT already in queue
                        blockEngine.newBlockQueue.AddToQueue(blockEngine.blockPool.chunkPool[key], truePosition + newPosition);

                        // Also make the current "Sub Chunk"s active = false
                        blockEngine.blockPool.SetSubChunksThatAreActiveFalseForChunk(blockEngine.blockPool.chunkPool[key].name);
                    }
                    else
                    {
                        // Already in queue
                        blockEngine.newBlockQueue.ChangeQueue(blockEngine.blockPool.chunkPool[key], truePosition + newPosition);

                        // Also make the current "Sub Chunk"s active = false
                        blockEngine.blockPool.SetSubChunksThatAreActiveFalseForChunk(blockEngine.blockPool.chunkPool[key].name);
                    }
                }
                else
                {
                    // No edge detected move normally

                    newPosition = blockEngine.blockPool.chunkPool[key].transform.position + forwardDirection;

                    //blockEngine.blockPool.chunkPool[key].transform.position = newPosition;
                    blockEngine.blockPool.chunkPool[key].transform.Translate(forwardDirection);

                    tempPool.Add(newPosition, blockEngine.blockPool.chunkPool[key]);
                } 
            }
        }

        blockEngine.blockPool.chunkPool = tempPool;
    }

    [BurstCompile]
    public void MoveBackward()
    {
        truePosition += forwardDirection;

        blockEngine.terrain.position += backwardDirection;

        // tell terrain system to reassign heights ====== bad relationship <<<<< to many dots .. .

        Vector2 tempPosition = new Vector2(truePosition.x - 2048, truePosition.z + 2048);

        terrainEngine.newTerrainQueue.AddToQueue(tempPosition, terrainEngine.terrainHeightMap.SUB_MAP_SIZE + 2, 1);

        playerEngine.playerMovement.player.position += backwardDirection;

        // ==============================================

        // Needs to be new Dictionary. .Clear() broke the system - investigate carefully.
        tempPool = new Dictionary<Vector3, Transform>();

        Vector3 newPosition = Vector3.zero;

        foreach (Vector3 key in blockEngine.blockPool.chunkPool.Keys)
        {
            // Make position is currently inside the treamill boundary. 
            // More chunks could be getting added outside of the boundary.
            if (key.x > boundary.width
                || key.x < boundary.x
                || key.z > boundary.height
                || key.z < boundary.y)
            {
                tempPool.Add(key, blockEngine.blockPool.chunkPool[key]);
            }
            else
            {
                // Detect edges to wrap around.
                // When edge chunks are discovered - Mark them for needing updated.

                //if (key.z == -(defaultMapSize / 2))     // 32 = -256
                if (key.z == boundary.y)
                {
                    newPosition = new Vector3(blockEngine.blockPool.chunkPool[key].transform.position.x,
                        blockEngine.blockPool.chunkPool[key].transform.position.y,
                        boundary.height);
                    //(defaultMapSize / 2) - SUB_MAP_SIZE);

                    blockEngine.blockPool.chunkPool[key].transform.Translate(forwardDirection * (size - 1));
                    //blockEngine.blockPool.chunkPool[key].transform.position = newPosition;

                    tempPool.Add(newPosition, blockEngine.blockPool.chunkPool[key]);

                    // Is the chunk already in the queue to be updated?

                    if (blockEngine.newBlockQueue.CheckIsAlreadyInQueue(blockEngine.blockPool.chunkPool[key].name) == false)
                    {
                        // NOT already in queue
                        blockEngine.newBlockQueue.AddToQueue(blockEngine.blockPool.chunkPool[key], truePosition + newPosition);

                        // Also make the current "Sub Chunk"s active = false
                        blockEngine.blockPool.SetSubChunksThatAreActiveFalseForChunk(blockEngine.blockPool.chunkPool[key].name);
                    }
                    else
                    {
                        // Already in queue
                        blockEngine.newBlockQueue.ChangeQueue(blockEngine.blockPool.chunkPool[key], truePosition + newPosition);

                        // Also make the current "Sub Chunk"s active = false
                        blockEngine.blockPool.SetSubChunksThatAreActiveFalseForChunk(blockEngine.blockPool.chunkPool[key].name);
                    }
                }
                else
                {
                    // No edge detected move normally

                    newPosition = blockEngine.blockPool.chunkPool[key].transform.position + backwardDirection;

                    //blockEngine.blockPool.chunkPool[key].transform.position = newPosition;
                    blockEngine.blockPool.chunkPool[key].transform.Translate(backwardDirection);

                    tempPool.Add(newPosition, blockEngine.blockPool.chunkPool[key]);
                } 
            }
        }

        blockEngine.blockPool.chunkPool = tempPool;
    }

    [BurstCompile]
    public void MoveLeft()
    {
        truePosition += rightDirection;

        blockEngine.terrain.position += leftDirection;

        // tell terrain system to reassign heights ====== bad relationship <<<<< to many dots .. .

        Vector2 tempPosition = new Vector2(truePosition.x + 2048, truePosition.z - 2048);

        terrainEngine.newTerrainQueue.AddToQueue(tempPosition, 1, terrainEngine.terrainHeightMap.SUB_MAP_SIZE + 2);

        playerEngine.playerMovement.player.position += leftDirection;

        // ==============================================

        // Needs to be new Dictionary. .Clear() broke the system - investigate carefully.
        tempPool = new Dictionary<Vector3, Transform>();

        Vector3 newPosition = Vector3.zero;

        foreach (Vector3 key in blockEngine.blockPool.chunkPool.Keys)
        {
            // Make position is currently inside the treamill boundary. 
            // More chunks could be getting added outside of the boundary.
            if (key.x > boundary.width
                || key.x < boundary.x
                || key.z > boundary.height
                || key.z < boundary.y)
            {
                tempPool.Add(key, blockEngine.blockPool.chunkPool[key]);
            }
            else
            {
                // Detect edges to wrap around.
                // When edge chunks are discovered - Mark them for needing updated.

                //if (key.x == -(defaultMapSize / 2))     // 32 = -256
                if (key.x == boundary.x)
                {
                    //newPosition = new Vector3((defaultMapSize / 2) - SUB_MAP_SIZE,      // 32 = 240
                    newPosition = new Vector3(boundary.width,
                        blockEngine.blockPool.chunkPool[key].transform.position.y,
                        blockEngine.blockPool.chunkPool[key].transform.position.z);

                    blockEngine.blockPool.chunkPool[key].transform.Translate(rightDirection * (size - 1));
                    //blockEngine.blockPool.chunkPool[key].transform.position = newPosition;

                    tempPool.Add(newPosition, blockEngine.blockPool.chunkPool[key]);

                    // Is the chunk already in the queue to be updated?

                    if (blockEngine.newBlockQueue.CheckIsAlreadyInQueue(blockEngine.blockPool.chunkPool[key].name) == false)
                    {
                        // NOT already in queue
                        blockEngine.newBlockQueue.AddToQueue(blockEngine.blockPool.chunkPool[key], truePosition + newPosition);

                        // Also make the current "Sub Chunk"s active = false
                        blockEngine.blockPool.SetSubChunksThatAreActiveFalseForChunk(blockEngine.blockPool.chunkPool[key].name);
                    }
                    else
                    {
                        // Already in queue
                        blockEngine.newBlockQueue.ChangeQueue(blockEngine.blockPool.chunkPool[key], truePosition + newPosition);

                        // Also make the current "Sub Chunk"s active = false
                        blockEngine.blockPool.SetSubChunksThatAreActiveFalseForChunk(blockEngine.blockPool.chunkPool[key].name);
                    }
                }
                else
                {
                    // No edge detected move normally

                    newPosition = blockEngine.blockPool.chunkPool[key].transform.position + leftDirection;

                    //blockEngine.blockPool.chunkPool[key].transform.position = newPosition;
                    blockEngine.blockPool.chunkPool[key].transform.Translate(leftDirection);

                    tempPool.Add(newPosition, blockEngine.blockPool.chunkPool[key]);
                } 
            }
        }

        blockEngine.blockPool.chunkPool = tempPool;
    }

    [BurstCompile]
    public void MoveRight()
    {
        truePosition += leftDirection;

        blockEngine.terrain.position += rightDirection;

        // tell terrain system to reassign heights ====== bad relationship <<<<< to many dots .. .

        Vector2 tempPosition = new Vector2(truePosition.x - 2048, truePosition.z - 2048);

        terrainEngine.newTerrainQueue.AddToQueue(tempPosition, 1, terrainEngine.terrainHeightMap.SUB_MAP_SIZE + 2);

        playerEngine.playerMovement.player.position += rightDirection;

        // ==============================================

        // Needs to be new Dictionary. .Clear() broke the system - investigate carefully.
        tempPool = new Dictionary<Vector3, Transform>();

        Vector3 newPosition = Vector3.zero;

        foreach (Vector3 key in blockEngine.blockPool.chunkPool.Keys)
        {
            // Make position is currently inside the treamill boundary. 
            // More chunks could be getting added outside of the boundary.
            if (key.x > boundary.width
                || key.x < boundary.x
                || key.z > boundary.height
                || key.z < boundary.y)
            {
                tempPool.Add(key, blockEngine.blockPool.chunkPool[key]);
            }
            else 
            {
                // Detect edges to wrap around.
                // When edge chunks are discovered - Mark them for needing updated.

                //if (key.x == (defaultMapSize / 2) - SUB_MAP_SIZE)   // 32 = 240
                if (key.x == boundary.width)
                {
                    //newPosition = new Vector3(-(defaultMapSize / 2),                    // 32 = -256
                    newPosition = new Vector3(boundary.x,
                        blockEngine.blockPool.chunkPool[key].transform.position.y,
                        blockEngine.blockPool.chunkPool[key].transform.position.z);

                    blockEngine.blockPool.chunkPool[key].transform.Translate(leftDirection * (size - 1));
                    //blockEngine.blockPool.chunkPool[key].transform.position = newPosition;

                    tempPool.Add(newPosition, blockEngine.blockPool.chunkPool[key]);

                    // Is the chunk already in the queue to be updated?

                    if (blockEngine.newBlockQueue.CheckIsAlreadyInQueue(blockEngine.blockPool.chunkPool[key].name) == false)
                    {
                        // NOT already in queue
                        blockEngine.newBlockQueue.AddToQueue(blockEngine.blockPool.chunkPool[key], truePosition + newPosition);

                        // Also make the current "Sub Chunk"s active = false
                        blockEngine.blockPool.SetSubChunksThatAreActiveFalseForChunk(blockEngine.blockPool.chunkPool[key].name);
                    }
                    else
                    {
                        // Already in queue
                        blockEngine.newBlockQueue.ChangeQueue(blockEngine.blockPool.chunkPool[key], truePosition + newPosition);

                        // Also make the current "Sub Chunk"s active = false
                        blockEngine.blockPool.SetSubChunksThatAreActiveFalseForChunk(blockEngine.blockPool.chunkPool[key].name);
                    }
                }
                else
                {
                    // No edge detected move normally

                    newPosition = blockEngine.blockPool.chunkPool[key].transform.position + rightDirection;

                    //blockEngine.blockPool.chunkPool[key].transform.position = newPosition;
                    blockEngine.blockPool.chunkPool[key].transform.Translate(rightDirection);

                    tempPool.Add(newPosition, blockEngine.blockPool.chunkPool[key]);
                } 
            }
        }

        blockEngine.blockPool.chunkPool = tempPool;
    }

    [BurstCompile]
    public bool CheckForNewChunkDirection()
    {
        Vector3 worldPositon = playerEngine.playerMovement.player.position;

        //if (playerEngine.playerMovement.player.parent != null) worldPositon = playerEngine.playerMovement.player.parent.transform.TransformPoint(playerEngine.playerMovement.player.localPosition);

        if (worldPositon.x < -CHUNK_SIZE)
        {
            // Going east

            moveRight = true;

            return true;
        }
        else
        {
            if (worldPositon.x > CHUNK_SIZE)
            {
                // Going west

                moveLeft = true;

                return true;
            }
            else
            {
                if (worldPositon.z < -CHUNK_SIZE)
                {
                    // Going north

                    moveForward = true;

                    return true;
                }
                else
                {
                    if (worldPositon.z > CHUNK_SIZE)
                    {
                        // Going south

                        moveBackward = true;

                        return true;
                    }
                    else
                    {
                        // Add later to allow to go much deeper and higher.

                        /*if (this.transform.position.y < chunkPosition.y + CHUNK_HEIGHT)
                        {
                            // Going south

                            moveUp = true;

                            return true;
                        }
                        else
                        {
                            if (this.transform.position.y < chunkPosition.y - CHUNK_SIZE)
                            {
                                // Going south

                                moveDown = true;

                                return true;
                            }
                        }*/
                    }
                }
            }
        }

        return false;
    }
}


