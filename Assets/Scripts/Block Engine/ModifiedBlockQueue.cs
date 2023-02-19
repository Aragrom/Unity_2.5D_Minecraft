using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// Uses the block map sub chunk data to generate new sub chunks.
// Uses the faces in sub chunk data to
// WARNING: HAS HARDCODED VALUES FOR INTERACTING WITH NEIGHBOURS SUB CHUNKS.
// CHANGING SUB CHUNK WIDTH OR HEIGHT WILL BREAK THIS SYSTEM
[BurstCompile]
public class ModifiedBlockQueue : MonoBehaviour
{/*
    public BlockEngine blockEngine = null;

    // Once faces have been adjusted to reflect the modified sub chunk they are placed here.
    // Uses these new faces to generate triangle data.
    public Dictionary<Vector3, SubChunkData> subChunksThatNeedsNewMeshDataGenerated = new Dictionary<Vector3, SubChunkData>();
    public Dictionary<Vector3, SubChunkData> subChunksThatNeedsMeshDataAssigned = new Dictionary<Vector3, SubChunkData>();
    public Dictionary<Vector3, string> subChunksActiveChunkName = new Dictionary<Vector3, string>();

    //-----------------------------------------------------------------------------

    // FOR DEBUGGING BLOCK INDEX ONLY
    public bool addBlock = false;
    public bool removeBlock = false;
    public Vector3 digPlaceDirection = Vector3.zero;
    public Vector3 actorPositionDebugging = Vector3.zero;
    public Vector3 subChunkPositionDebugging = Vector3.zero;
    public Transform selectedSubChunkForTesting = null;

    //-----------------------------------------------------------------------------

    // All required globally in order to dispose safely

    public Dictionary<Vector3, JobHandle> jobHandles = new Dictionary<Vector3, JobHandle>();
    Dictionary<Vector3, GenerateTriangleUsingFaceMapJob> jobs = new Dictionary<Vector3, GenerateTriangleUsingFaceMapJob>();

    Dictionary<Vector3, NativeArray<bool>> nativeFaceMaps = new Dictionary<Vector3, NativeArray<bool>>();
    Dictionary<Vector3, NativeList<int>> nativeTriangles = new Dictionary<Vector3, NativeList<int>>();

    //-----------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------
    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        blockEngine = null;

        subChunksThatNeedsNewMeshDataGenerated.Clear();
        subChunksThatNeedsNewMeshDataGenerated = null;
        subChunksThatNeedsMeshDataAssigned.Clear();
        subChunksThatNeedsMeshDataAssigned = null;
        subChunksActiveChunkName.Clear();
        subChunksActiveChunkName = null;

        selectedSubChunkForTesting = null;

        jobs.Clear();
        jobs = null;
        jobHandles.Clear();
        jobHandles = null;

        nativeFaceMaps.Clear();
        nativeFaceMaps = null;
        nativeTriangles.Clear();
        nativeTriangles = null;
    }

    //-----------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------
    [BurstCompile]
    private void Awake()
    {
        blockEngine = GetComponent<BlockEngine>();
    }

    //-----------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------
    [BurstCompile]
    public void LateUpdate()
    {
        if (jobHandles.Count == 0) return;

        List<Vector3> positionsToRemove = new List<Vector3>();

        foreach (Vector3 key in jobHandles.Keys)
        {
            if (jobHandles[key].IsCompleted)
            {
                jobHandles[key].Complete();

                SubChunkData selectedSubChunkData = blockEngine.blockMap.subChunkData[key];

                selectedSubChunkData.triangles = jobs[key].nativeTriangles.ToArray();

                blockEngine.blockMap.subChunkData[key] = selectedSubChunkData;

                subChunksThatNeedsMeshDataAssigned.Add(key, selectedSubChunkData);

                nativeFaceMaps[key].Dispose();
                nativeTriangles[key].Dispose();

                positionsToRemove.Add(key);
            }
        }

        for (int i = 0; i < positionsToRemove.Count; i++)
        {
            // Remove all field dictionary elements

            nativeFaceMaps.Remove(positionsToRemove[i]);
            nativeTriangles.Remove(positionsToRemove[i]);

            jobs.Remove(positionsToRemove[i]);
            jobHandles.Remove(positionsToRemove[i]);
        }
    }

    //-----------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------
    [BurstCompile]
    public void Update()
    {
        if (addBlock)
        {
            addBlock = false;
            int blockIndex = CalculateIndexForBlockInSubChunk(actorPositionDebugging, digPlaceDirection);
            AddBlock(selectedSubChunkForTesting, subChunkPositionDebugging, blockIndex, RawSubChunkData.BlockType.Stone, false);
        }

        if (removeBlock)
        {
            removeBlock = false;
            int blockIndex = CalculateIndexForBlockInSubChunk(actorPositionDebugging, digPlaceDirection);
            RemoveBlock(selectedSubChunkForTesting, subChunkPositionDebugging, blockIndex, false);
        }
    }

    //-----------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------
    [BurstCompile]
    struct GenerateTriangleUsingFaceMapJob : IJob
    {
        public int size;        // 16
        public int height;      // 16
        public NativeArray<bool> nativeFaceMap;
        public NativeList<int> nativeTriangles;

        [BurstCompile]
        public void Execute()
        {
            int faceIterator = 0;
            int trianglesIterator = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        // is the block solid?

                        if (nativeFaceMap[faceIterator + 0] == true)
                        {
                            // 6, 7, 8, top
                            // 7, 9 ,8,

                            nativeTriangles.Add(trianglesIterator + 6);   //top
                            nativeTriangles.Add(trianglesIterator + 7);
                            nativeTriangles.Add(trianglesIterator + 8);
                            nativeTriangles.Add(trianglesIterator + 7);
                            nativeTriangles.Add(trianglesIterator + 9);
                            nativeTriangles.Add(trianglesIterator + 8); 
                        }
                            
                        if (nativeFaceMap[faceIterator + 1] == true)
                        {
                            // 4, 5, 6,  back
                            // 5, 7, 6,

                            nativeTriangles.Add(trianglesIterator + 4);   // Back
                            nativeTriangles.Add(trianglesIterator + 5);
                            nativeTriangles.Add(trianglesIterator + 6);
                            nativeTriangles.Add(trianglesIterator + 5);
                            nativeTriangles.Add(trianglesIterator + 7);
                            nativeTriangles.Add(trianglesIterator + 6);
                        }
                            
                        if (nativeFaceMap[faceIterator + 2] == true)
                        {
                            // 0, 2, 1, // front
                            // 1, 2, 3,

                            nativeTriangles.Add(trianglesIterator + 0);  // front
                            nativeTriangles.Add(trianglesIterator + 2);
                            nativeTriangles.Add(trianglesIterator + 1);
                            nativeTriangles.Add(trianglesIterator + 1);
                            nativeTriangles.Add(trianglesIterator + 2);
                            nativeTriangles.Add(trianglesIterator + 3);
                        }

                        if (nativeFaceMap[faceIterator + 3] == true)
                        {
                            // 3, 12, 5, right
                            // 5, 12, 13

                            nativeTriangles.Add(trianglesIterator + 3);  // Right
                            nativeTriangles.Add(trianglesIterator + 12);
                            nativeTriangles.Add(trianglesIterator + 5);
                            nativeTriangles.Add(trianglesIterator + 5);
                            nativeTriangles.Add(trianglesIterator + 12);
                            nativeTriangles.Add(trianglesIterator + 13);
                        }

                        if (nativeFaceMap[faceIterator + 4] == true)
                        {
                            // 1, 11, 10, left
                            // 1, 4, 11,

                            nativeTriangles.Add(trianglesIterator + 1);  // Left
                            nativeTriangles.Add(trianglesIterator + 11);
                            nativeTriangles.Add(trianglesIterator + 10);
                            nativeTriangles.Add(trianglesIterator + 1);
                            nativeTriangles.Add(trianglesIterator + 4);
                            nativeTriangles.Add(trianglesIterator + 11);
                        }

                        if (nativeFaceMap[faceIterator + 5] == true)
                        {
                            // 1, 3, 4, Bottom
                            // 3, 5, 4,

                            nativeTriangles.Add(trianglesIterator + 1);  // Bottom
                            nativeTriangles.Add(trianglesIterator + 3);
                            nativeTriangles.Add(trianglesIterator + 4);
                            nativeTriangles.Add(trianglesIterator + 3);
                            nativeTriangles.Add(trianglesIterator + 5);
                            nativeTriangles.Add(trianglesIterator + 4);
                        }

                        faceIterator += 6;          // += number of faces
                        trianglesIterator += 14;    // += number of vertices

                    }// end of z

                }// end of x

            }// End of y
        }
    }

    //-----------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------
    public void GenerateTrianglesUsingFaceMap(Vector3 position, bool[] faceMap)
    {
        nativeFaceMaps.Add(position, new NativeArray<bool>(faceMap, Allocator.TempJob));
        nativeTriangles.Add(position, new NativeList<int>(Allocator.TempJob));

        jobHandles.Add(position, default);

        jobs.Add(position, new GenerateTriangleUsingFaceMapJob
        {
            nativeFaceMap = nativeFaceMaps[position],
            nativeTriangles = nativeTriangles[position],
            size = blockEngine.blockMap.SUB_CHUNK_SIZE,
            height = blockEngine.blockMap.SUB_CHUNK_HEIGHT
        });

        jobHandles[position] = jobs[position].Schedule(jobHandles[position]);
    }

    //-----------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------
    [BurstCompile]
    public int CalculateIndexForBlockInSubChunk(Vector3 actorPosition, Vector3 direction)
    {
        // Note: Sub chunk actual position in the world
        // Actor position to absolute values
        // Actor is always being move toward 0,0,0 however so this should never be a factor

        if (actorPosition.y > 16) actorPosition.y %= 16;

        Vector3 indexPosition; // = Vector3.one * 15;

        if (actorPosition.x > 0
            || actorPosition.x == 0
            && actorPosition.z >= 0) indexPosition.x = actorPosition.x;
        else
        {
            indexPosition.x = 15 + actorPosition.x;
        }

        if (actorPosition.z > 0
            || actorPosition.z == 0
            && actorPosition.x >= 0) indexPosition.z = actorPosition.z;
        else
        {
            indexPosition.z = 15 + actorPosition.z;
        }

        if (actorPosition.y > 0
            || actorPosition.y == 0) indexPosition.y = actorPosition.y;
        else
        {
            indexPosition.y = 15 + actorPosition.y;
        }

        indexPosition += direction;

        //Vector3 indexPosition = Vector3.one * 15;
        //indexPosition -= actorPosition;

        int index = (int)(indexPosition.z) + (int)(indexPosition.x * 16) + (int)(indexPosition.y * 256);

        Debug.Log("INDEX: " + index);

        return index;
    }

    //-----------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------
    [BurstCompile]
    public void AddBlock(Transform chunk, Vector3 subChunkPosition, int blockIndex, RawSubChunkData.BlockType type, bool hasMoreBlocksToRemove)
    {
        //                                Corners (8)   Edge (12)                                                       faces (6)
        //       3855____4095             0             if index - y * 256 = 0              "Back Left MIDDLE"          if index - y * 256 <= 15                    "Left" 
        // Y^      /     /|               15            if index - y * 256 = 15             "Front Left MIDDLE"         if index - y * 256 >= 240 && <= 255         "Right"
        //        /     / |    ^>Z        240           if index - y * 256 = 240            "Back Right MIDDLE"         if index - y * 256 % 15 has no remainder    "front"
        //   3840/_____/4080              255           if index - y * 256 = 255            "Front Right MIDDLE"        if index - y * 256 % 16 has no remainder    "back"
        //       |     |  |               3840          if <= 15                            "left BOTTOM"               if index <= 255                             "Bottom"
        //       |15   |  |255            3855          if >= 240 && <= 255                 "Right BOTTOM"              if index >= 3840                            "Top"
        //       |     | /                4080          if % no remainder by 15 && <= 255   "front BOTTOM"
        //      0|_____|/240              4095          if % no remainder by 16 && <= 255   "back BOTTOM"               if none of these are detected block placement is "Normal" (No neighbours)
        //                                              if >= 3840 && <= 3855               "left TOP"
        //      -----------------> X                    if >= 4080                          "right TOP"
        //       z >> x >> y                            if % no remainder 15 && >= 3855     "front TOP"              
        //                                              if % no remainder 16 && >= 3840     "back TOP"  

        // Corners - effect 3 neighbour sub chunks
        // Edges - effect 2 neighbour sub chunks
        // face - effect 1 neighbour sub chunks

        // Check in order Corners > Edges > faces
        // stops if find the correct block placement type
        // logic is based on order!

        // divide by 256 to get y
        int y = (int)(blockIndex / 256f);

        // make sure float division and cast to int successful!
        Debug.Log("Y position used: " + y);

        // Is corner?
        if (IsCorner(blockIndex))
        {
            Debug.Log("IS CORNER!");
            AddCornerBlock(chunk, subChunkPosition, blockIndex, RawSubChunkData.BlockType.Stone);
        }
        else
        {
            if (IsEdge(blockIndex, y))
            {
                Debug.Log("IS EDGE!");
                AddEdgeBlock(blockIndex, y);
            }
            else
            {
                if (IsFace(blockIndex, y))
                {
                    Debug.Log("IS FACE!");
                    AddFaceBlock(blockIndex, y);
                }
                else
                {
                    Debug.Log("IS NORMAL!");
                    AddBlockDefault(chunk, subChunkPosition, blockIndex, RawSubChunkData.BlockType.Stone, false);
                }
            }
        }
    }

    //-----------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------
    [BurstCompile]
    public void RemoveBlock(Transform chunk, Vector3 subChunkPosition, int blockIndex, bool hasMoreBlocksToRemove)
    {
        //                                Corners (8)   Edge (12)                                                       faces (6)
        //       3855____4095             0             if index - y * 256 = 0              "Back Left MIDDLE"          if index - y * 256 <= 15                    "Left" 
        // Y^      /     /|               15            if index - y * 256 = 15             "Front Left MIDDLE"         if index - y * 256 >= 240 && <= 255         "Right"
        //        /     / |    ^>Z        240           if index - y * 256 = 240            "Back Right MIDDLE"         if index - y * 256 % 15 has no remainder    "front"
        //   3840/_____/4080              255           if index - y * 256 = 255            "Front Right MIDDLE"        if index - y * 256 % 16 has no remainder    "back"
        //       |     |  |               3840          if <= 15                            "left BOTTOM"               if index <= 255                             "Bottom"
        //       |15   |  |255            3855          if >= 240 && <= 255                 "Right BOTTOM"              if index >= 3840                            "Top"
        //       |     | /                4080          if % no remainder by 15 && <= 255   "front BOTTOM"
        //      0|_____|/240              4095          if % no remainder by 16 && <= 255   "back BOTTOM"               if none of these are detected block placement is "Normal" (No neighbours)
        //                                              if >= 3840 && <= 3855               "left TOP"
        //      -----------------> X                    if >= 4080                          "right TOP"
        //       z >> x >> y                            if % no remainder 15 && >= 3855     "front TOP"              
        //                                              if % no remainder 16 && >= 3840     "back TOP"  

        // Corners - effect 3 neighbour sub chunks
        // Edges - effect 2 neighbour sub chunks
        // face - effect 1 neighbour sub chunks

        // Check in order Corners > Edges > faces
        // stops if find the correct block placement type
        // logic is based on order!

        // divide by 256 to get y
        int y = (int)(blockIndex / 256f);

        // make sure float division and cast to int successful!
        Debug.Log("Y position used: " + y);

        // Is corner?
        if (IsCorner(blockIndex))
        {
            Debug.Log("IS CORNER!");
            RemoveCornerBlock(blockIndex);
        }
        else
        {
            if (IsEdge(blockIndex, y))
            {
                Debug.Log("IS EDGE!");
                RemoveEdgeBlock(blockIndex, y);
            }
            else
            {
                if (IsFace(blockIndex, y))
                {
                    Debug.Log("IS FACE!");
                    RemoveFaceBlock(blockIndex, y);
                }
                else
                {
                    Debug.Log("IS NORMAL!");
                    RemoveBlockDefault(chunk, subChunkPosition, blockIndex, false);
                }
            }
        }
    }

    //-----------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------
    [BurstCompile]
    public bool IsCorner(int blockIndex)
    {
        if (blockIndex == 0
            || blockIndex == 15
            || blockIndex == 240
            || blockIndex == 255
            || blockIndex == 3840
            || blockIndex == 3855
            || blockIndex == 4080
            || blockIndex == 4095)
        {
            return true;
        }
        else return false;
    }

    //-----------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------
    [BurstCompile]
    public void AddCornerBlock(Transform chunk, Vector3 subChunkPosition, int blockIndex, RawSubChunkData.BlockType type)
    {
        switch (blockIndex)
        {
            case 0:

                AddBlockBottomBackLeftCorner(chunk, subChunkPosition, blockIndex, type);
                break;

            case 15:

                AddBlockBottomFrontLeftCorner(chunk, subChunkPosition, blockIndex, type);
                break;

            case 240:

                AddBlockBottomBackRightCorner(chunk, subChunkPosition, blockIndex, type);
                break;

            case 255:
                break;

            case 3840:
                break;

            case 3855:
                break;

            case 4080:
                break;

            case 4095:
                break;
        }
    }

    //-----------------------------------------------------------------------------
    // Bottom back left corner
    //-----------------------------------------------------------------------------
    [BurstCompile]
    public void AddBlockBottomBackLeftCorner(Transform chunk, Vector3 subChunkPosition, int blockIndex, RawSubChunkData.BlockType type)
    {
        List<SubChunkData> modifiedSubChunkData = new List<SubChunkData>();
        SubChunkData original = blockEngine.blockMap.subChunkData[subChunkPosition];

        //-----------------------------------------------------------------------------
        // Handle Front face
        //-----------------------------------------------------------------------------
        if (original.types[blockIndex + 1] > 0)
        {
            // Block exists infront of the block we are adding to
            // remove face
            original.faces[(blockIndex + 1) * 6 + (int)RawSubChunkData.FaceTypes.Back] = false;
        }
        else
        {
            // add face
            original.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Front] = true;
        }

        //-----------------------------------------------------------------------------
        // Handle Right face
        //-----------------------------------------------------------------------------
        if (original.types[blockIndex + 16] > 0)
        {
            // Block exists infront of the block we are adding to
            // remove face
            original.faces[(blockIndex + 16) * 6 + (int)RawSubChunkData.FaceTypes.Left] = false;
        }
        else
        {
            // add face
            original.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Right] = true;
        }

        //-----------------------------------------------------------------------------
        // Handle Top face
        //-----------------------------------------------------------------------------
        // if block exists above
        if (original.types[blockIndex + 256] > 0)
        {
            // Block exists infront of the block we are adding to
            // remove face
            original.faces[(blockIndex + 256) * 6 + (int)RawSubChunkData.FaceTypes.Bottom] = false;
        }
        else // block does not exist above
        {
            // add face
            original.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Top] = true;
        }

        //-----------------------------------------------------------------------------

        // need -z sub chunk and block index
        // need -x sub chunk and block index
        // need -y sub chunk and block index

        //-----------------------------------------------------------------------------
        // Handle Back Face
        // Z - need -z sub chunk and -z block index
        //-----------------------------------------------------------------------------
        Vector3 zSubChunkPosition = subChunkPosition + (Vector3.back * 16);
        int zBlockIndex = 15;
        SubChunkData subChunkDataZ;

        // Check sub chunk data exists for position
        if (blockEngine.blockMap.subChunkData.ContainsKey(zSubChunkPosition))
        {
            subChunkDataZ = blockEngine.blockMap.subChunkData[zSubChunkPosition];

            // if block exists - remove front face - z sub chunk - z block index
            if (subChunkDataZ.types[zBlockIndex] > 0)
            {
                // Remove Front face - z sub chunk - z block index
                subChunkDataZ.faces[zBlockIndex * 6 + (int)RawSubChunkData.FaceTypes.Front] = false;

                // Add this sub chunk to the list of sub chunks that need to have the mesh updated

                modifiedSubChunkData.Add(subChunkDataZ);
                blockEngine.blockMap.subChunkData[zSubChunkPosition] = subChunkDataZ;
            }
            else  // if there is not a block there do nothing to the z sub chunk
            {
                // instead add the back face to the original sub chunk
                // Add back face - sub chunk - block index
                original.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Back] = true;
            }
        }
        else
        {
            Debug.Log("Sub Chunk data does not exist at position. " + zSubChunkPosition.ToString());
            return;
        }

        //-----------------------------------------------------------------------------
        // Handle Left face
        // X - need -x sub chunk and -x block index
        //-----------------------------------------------------------------------------
        Vector3 xSubChunkPosition = subChunkPosition + (Vector3.left * 16);
        int xBlockIndex = 240;
        SubChunkData subChunkDataX;

        // Check sub chunk data exists for position
        if (blockEngine.blockMap.subChunkData.ContainsKey(xSubChunkPosition))
        {
            subChunkDataX = blockEngine.blockMap.subChunkData[xSubChunkPosition];

            if (subChunkDataX.types[xBlockIndex] > 0)
            {
                subChunkDataX.faces[xBlockIndex * 6 + (int)RawSubChunkData.FaceTypes.Right] = false;

                modifiedSubChunkData.Add(subChunkDataX);
                blockEngine.blockMap.subChunkData[xSubChunkPosition] = subChunkDataX;
            }
            else
            {
                original.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Left] = true;
            }
        }
        else
        {
            Debug.Log("Sub Chunk data does not exist at position. " + xSubChunkPosition.ToString());
            return;
        }

        Vector3 ySubChunkPosition = subChunkPosition + (Vector3.down * 16);
        int yBlockIndex = 3840;
        SubChunkData subChunkDataY;

        // is this the lowest sub chunk possible? (y = 0)
        if (subChunkPosition.y == 0)
        {
            // cant check neighbour. default to block does not exist
            original.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Bottom] = true;
        }
        else
        {
            //-----------------------------------------------------------------------------
            // Handle Bottom face
            // need -y sub chunk and -y block index
            // Check sub chunk data exists for position
            //-----------------------------------------------------------------------------
            if (blockEngine.blockMap.subChunkData.ContainsKey(ySubChunkPosition))
            {
                subChunkDataY = blockEngine.blockMap.subChunkData[ySubChunkPosition];

                if (subChunkDataY.types[yBlockIndex] > 0)
                {
                    subChunkDataY.faces[yBlockIndex * 6 + (int)RawSubChunkData.FaceTypes.Top] = false;

                    modifiedSubChunkData.Add(subChunkDataY);
                    blockEngine.blockMap.subChunkData[ySubChunkPosition] = subChunkDataY;
                }
                else
                {
                    original.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Bottom] = true;
                }
            }
            else
            {
                Debug.Log("Sub Chunk data does not exist at position. " + ySubChunkPosition.ToString());
                return;
            }
        }

        modifiedSubChunkData.Add(original);
        blockEngine.blockMap.subChunkData[subChunkPosition] = original;
    */
        /*for (int i = 0; i < modifiedSubChunkData.Count; i++)
        {
            if (subChunksActiveChunkName.ContainsKey(modifiedSubChunkData[i]) == false)
            {
                subChunksActiveChunkName.Add(subChunkPosition, chunk.name);
            }

            // Add these to list to be updated after all sub chunk data face maps have been modified.
            // For system to add/remove more blocks at once without yet getting the triangles for the sub chunk.

            if (subChunksThatNeedsNewMeshDataGenerated.ContainsKey(subChunkPosition) == false)
            {
                Debug.Log("Successfully modified sub chunk data");
                subChunksThatNeedsNewMeshDataGenerated.Add(subChunkPosition, selectedSubChunkData);
            }
            else
            {
                Debug.Log("Successfully modified sub chunk data - sub chunk data already in queue to be updated");
            }
        }*/
    /*}

    //-----------------------------------------------------------------------------
    // Bottom front left corner
    //-----------------------------------------------------------------------------
    [BurstCompile]
    public void AddBlockBottomFrontLeftCorner(Transform chunk, Vector3 subChunkPosition, int blockIndex, RawSubChunkData.BlockType type)
    {
        List<SubChunkData> modifiedSubChunkData = new List<SubChunkData>();
        SubChunkData original = blockEngine.blockMap.subChunkData[subChunkPosition];

        //-----------------------------------------------------------------------------
        // Handle Back face
        //-----------------------------------------------------------------------------
        if (original.types[blockIndex - 1] > 0)
        {
            // Block exists infront of the block we are adding to
            // remove face
            original.faces[(blockIndex - 1) * 6 + (int)RawSubChunkData.FaceTypes.Front] = false;
        }
        else
        {
            // add face
            original.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Back] = true;
        }

        //-----------------------------------------------------------------------------
        // Handle Right face
        //-----------------------------------------------------------------------------
        if (original.types[blockIndex + 16] > 0)
        {
            // Block exists infront of the block we are adding to
            // remove face
            original.faces[(blockIndex + 16) * 6 + (int)RawSubChunkData.FaceTypes.Left] = false;
        }
        else
        {
            // add face
            original.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Right] = true;
        }

        //-----------------------------------------------------------------------------
        // Handle Top face
        //-----------------------------------------------------------------------------
        // if block exists above
        if (original.types[blockIndex + 256] > 0)
        {
            // Block exists infront of the block we are adding to
            // remove face
            original.faces[(blockIndex + 256) * 6 + (int)RawSubChunkData.FaceTypes.Bottom] = false;
        }
        else // block does not exist above
        {
            // add face
            original.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Top] = true;
        }

        //-----------------------------------------------------------------------------

        // need +z sub chunk and block index
        // need -x sub chunk and block index
        // need -y sub chunk and block index

        //-----------------------------------------------------------------------------
        // Handle Front face
        // Z - need +z sub chunk and +z block index
        //-----------------------------------------------------------------------------
        Vector3 zSubChunkPosition = subChunkPosition + (Vector3.forward * 16);
        int zBlockIndex = 0;
        SubChunkData subChunkDataZ;

        // Check sub chunk data exists for position
        if (blockEngine.blockMap.subChunkData.ContainsKey(zSubChunkPosition))
        {
            subChunkDataZ = blockEngine.blockMap.subChunkData[zSubChunkPosition];

            // if block exists - remove front face - z sub chunk - z block index
            if (subChunkDataZ.types[zBlockIndex] > 0)
            {
                // Remove Front face - z sub chunk - z block index
                subChunkDataZ.faces[zBlockIndex * 6 + (int)RawSubChunkData.FaceTypes.Back] = false;

                // Add this sub chunk to the list of sub chunks that need to have the mesh updated

                modifiedSubChunkData.Add(subChunkDataZ);
                blockEngine.blockMap.subChunkData[zSubChunkPosition] = subChunkDataZ;
            }
            else  // if there is not a block there do nothing to the z sub chunk
            {
                // instead add the back face to the original sub chunk
                // Add back face - sub chunk - block index
                original.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Front] = true;
            }
        }
        else
        {
            Debug.Log("Sub Chunk data does not exist at position. " + zSubChunkPosition.ToString());
            return;
        }

        //-----------------------------------------------------------------------------
        // Handle Left face
        // X - need -x sub chunk and -x block index
        //-----------------------------------------------------------------------------
        Vector3 xSubChunkPosition = subChunkPosition + (Vector3.left * 16);
        int xBlockIndex = 255;
        SubChunkData subChunkDataX;

        // Check sub chunk data exists for position
        if (blockEngine.blockMap.subChunkData.ContainsKey(xSubChunkPosition))
        {
            subChunkDataX = blockEngine.blockMap.subChunkData[xSubChunkPosition];

            if (subChunkDataX.types[xBlockIndex] > 0)
            {
                subChunkDataX.faces[xBlockIndex * 6 + (int)RawSubChunkData.FaceTypes.Right] = false;

                modifiedSubChunkData.Add(subChunkDataX);
                blockEngine.blockMap.subChunkData[xSubChunkPosition] = subChunkDataX;
            }
            else
            {
                original.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Left] = true;
            }
        }
        else
        {
            Debug.Log("Sub Chunk data does not exist at position. " + xSubChunkPosition.ToString());
            return;
        }

        //-----------------------------------------------------------------------------
        // Handle Bottom face
        // Y - need -y sub chunk and -y block index
        //-----------------------------------------------------------------------------
        Vector3 ySubChunkPosition = subChunkPosition + (Vector3.down * 16);
        int yBlockIndex = 3855;
        SubChunkData subChunkDataY;

        // is this the lowest sub chunk possible? (y = 0)
        if (subChunkPosition.y == 0)
        {
            // cant check neighbour. default to block does not exist
            original.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Bottom] = true;
        }
        else
        {
            // Check sub chunk data exists for position
            if (blockEngine.blockMap.subChunkData.ContainsKey(ySubChunkPosition))
            {
                subChunkDataY = blockEngine.blockMap.subChunkData[ySubChunkPosition];

                if (subChunkDataY.types[yBlockIndex] > 0)
                {
                    subChunkDataY.faces[yBlockIndex * 6 + (int)RawSubChunkData.FaceTypes.Top] = false;

                    modifiedSubChunkData.Add(subChunkDataY);
                    blockEngine.blockMap.subChunkData[ySubChunkPosition] = subChunkDataY;
                }
                else
                {
                    original.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Bottom] = true;
                }
            }
            else
            {
                Debug.Log("Sub Chunk data does not exist at position. " + ySubChunkPosition.ToString());
                return;
            }
        }

        modifiedSubChunkData.Add(original);
        blockEngine.blockMap.subChunkData[subChunkPosition] = original;
    }

    //-----------------------------------------------------------------------------
    // Bottom back right corner
    //-----------------------------------------------------------------------------
    [BurstCompile]
    public void AddBlockBottomBackRightCorner(Transform chunk, Vector3 subChunkPosition, int blockIndex, RawSubChunkData.BlockType type)
    {
        List<SubChunkData> modifiedSubChunkData = new List<SubChunkData>();
        SubChunkData original = blockEngine.blockMap.subChunkData[subChunkPosition];

        //-----------------------------------------------------------------------------
        // Front face
        //-----------------------------------------------------------------------------
        if (original.types[blockIndex + 1] > 0)
        {
            // Block exists infront of the block we are adding to
            // remove face
            original.faces[(blockIndex + 1) * 6 + (int)RawSubChunkData.FaceTypes.Back] = false;
        }
        else
        {
            // add face
            original.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Front] = true;
        }

        //-----------------------------------------------------------------------------
        // Left face
        //-----------------------------------------------------------------------------
        if (original.types[blockIndex - 16] > 0)
        {
            // Block exists infront of the block we are adding to
            // remove face
            original.faces[(blockIndex - 16) * 6 + (int)RawSubChunkData.FaceTypes.Right] = false;
        }
        else
        {
            // add face
            original.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Left] = true;
        }

        //-----------------------------------------------------------------------------
        // Top face
        //-----------------------------------------------------------------------------
        // if block exists above
        if (original.types[blockIndex + 256] > 0)
        {
            // Block exists infront of the block we are adding to
            // remove face
            original.faces[(blockIndex + 256) * 6 + (int)RawSubChunkData.FaceTypes.Bottom] = false;
        }
        else // block does not exist above
        {
            // add face
            original.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Top] = true;
        }

        // need -z sub chunk and block index
        // need +x sub chunk and block index
        // need -y sub chunk and block index

        //-----------------------------------------------------------------------------
        // Handle Back face
        // Z - need -z sub chunk and -z block index
        //-----------------------------------------------------------------------------
        Vector3 zSubChunkPosition = subChunkPosition + (Vector3.back * 16);
        int zBlockIndex = 255;
        SubChunkData subChunkDataZ;

        // Check sub chunk data exists for position
        if (blockEngine.blockMap.subChunkData.ContainsKey(zSubChunkPosition))
        {
            subChunkDataZ = blockEngine.blockMap.subChunkData[zSubChunkPosition];

            // if block exists - remove front face - z sub chunk - z block index
            if (subChunkDataZ.types[zBlockIndex] > 0)
            {
                // Remove Front face - z sub chunk - z block index
                subChunkDataZ.faces[zBlockIndex * 6 + (int)RawSubChunkData.FaceTypes.Front] = false;

                // Add this sub chunk to the list of sub chunks that need to have the mesh updated

                modifiedSubChunkData.Add(subChunkDataZ);
                blockEngine.blockMap.subChunkData[zSubChunkPosition] = subChunkDataZ;
            }
            else  // if there is not a block there do nothing to the z sub chunk
            {
                // instead add the back face to the original sub chunk
                // Add back face - sub chunk - block index
                original.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Back] = true;
            }
        }
        else
        {
            Debug.Log("Sub Chunk data does not exist at position. " + zSubChunkPosition.ToString());
            return;
        }

        //-----------------------------------------------------------------------------
        // Handle Right face
        // X - need +x sub chunk and +x block index
        //-----------------------------------------------------------------------------
        Vector3 xSubChunkPosition = subChunkPosition + (Vector3.right * 16);
        int xBlockIndex = 0;
        SubChunkData subChunkDataX;

        // Check sub chunk data exists for position
        if (blockEngine.blockMap.subChunkData.ContainsKey(xSubChunkPosition))
        {
            subChunkDataX = blockEngine.blockMap.subChunkData[xSubChunkPosition];

            if (subChunkDataX.types[xBlockIndex] > 0)
            {
                subChunkDataX.faces[xBlockIndex * 6 + (int)RawSubChunkData.FaceTypes.Left] = false;

                modifiedSubChunkData.Add(subChunkDataX);
                blockEngine.blockMap.subChunkData[xSubChunkPosition] = subChunkDataX;
            }
            else
            {
                original.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Right] = true;
            }
        }
        else
        {
            Debug.Log("Sub Chunk data does not exist at position. " + xSubChunkPosition.ToString());
            return;
        }

        Vector3 ySubChunkPosition = subChunkPosition + (Vector3.down * 16);
        int yBlockIndex = 4080;
        SubChunkData subChunkDataY;

        // is this the lowest sub chunk possible? (y = 0)
        if (subChunkPosition.y == 0)
        {
            // cant check neighbour. default to block does not exist
            original.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Bottom] = true;
        }
        else
        {
            //-----------------------------------------------------------------------------
            // Handle Bottom face
            // need -y sub chunk and -y block index
            // Check sub chunk data exists for position
            //-----------------------------------------------------------------------------
            if (blockEngine.blockMap.subChunkData.ContainsKey(ySubChunkPosition))
            {
                subChunkDataY = blockEngine.blockMap.subChunkData[ySubChunkPosition];

                if (subChunkDataY.types[yBlockIndex] > 0)
                {
                    subChunkDataY.faces[yBlockIndex * 6 + (int)RawSubChunkData.FaceTypes.Top] = false;

                    modifiedSubChunkData.Add(subChunkDataY);
                    blockEngine.blockMap.subChunkData[ySubChunkPosition] = subChunkDataY;
                }
                else
                {
                    original.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Bottom] = true;
                }
            }
            else
            {
                Debug.Log("Sub Chunk data does not exist at position. " + ySubChunkPosition.ToString());
                return;
            }
        }

        modifiedSubChunkData.Add(original);
        blockEngine.blockMap.subChunkData[subChunkPosition] = original;
    }

    //-----------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------
    [BurstCompile]
    public void RemoveCornerBlock(int blockIndex)
    {
        switch (blockIndex)
        {
            case 0:
                break;

            case 15:
                break;

            case 240:
                break;

            case 255:
                break;

            case 3840:
                break;

            case 3855:
                break;

            case 4080:
                break;

            case 4095:
                break;
        }
    }

    //-----------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------
    [BurstCompile]
    public bool IsEdge(int blockIndex, int y)
    {
        if (blockIndex - y * 256 == 0
            || blockIndex - y * 256 == 15
            || blockIndex - y * 256 == 240
            || blockIndex - y * 256 == 255
            || blockIndex <= 15
            || (blockIndex >= 240 && blockIndex <= 255)
            || (blockIndex % 15 == 0 && blockIndex <= 255)
            || (blockIndex % 16 == 0 && blockIndex <= 255)
            || (blockIndex >= 3840 && blockIndex <= 3855)
            || blockIndex >= 4080
            || (blockIndex % 15 == 0 && blockIndex >= 3855)
            || (blockIndex % 16 == 0 && blockIndex >= 3840))
        {
            return true;
        }
        else return false;
    }

    //-----------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------
    [BurstCompile]
    public void AddEdgeBlock(int blockIndex, int y)
    {
        if (blockIndex - y * 256 == 0)
        {

        }
        else 
        {
            if (blockIndex - y * 256 == 15)
            {

            }
            else
            {
                if (blockIndex - y * 256 == 240)
                {

                }
                else 
                {
                    if (blockIndex - y * 256 == 255)
                    {

                    }
                    else
                    {
                        if (blockIndex <= 15)
                        {

                        }
                        else
                        {
                            if (blockIndex >= 240 && blockIndex <= 255)
                            {

                            }
                            else
                            {
                                if (blockIndex % 15 == 0 && blockIndex <= 255)
                                {

                                }
                                else
                                {
                                    if (blockIndex % 16 == 0 && blockIndex <= 255)
                                    {

                                    }
                                    else
                                    {
                                        if (blockIndex >= 3840 && blockIndex <= 3855)
                                        {

                                        }
                                        else
                                        {
                                            if (blockIndex >= 4080)
                                            {

                                            }
                                            else
                                            {
                                                if (blockIndex % 15 == 0 && blockIndex >= 3855)
                                                {

                                                }
                                                else
                                                {
                                                    if (blockIndex % 16 == 0 && blockIndex >= 3840)
                                                    {

                                                    }
                                                    else
                                                    {
                                                        Debug.Log("Tried to create edge block but missed correct check - unreachable code reached");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    //-----------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------
    [BurstCompile]
    public void RemoveEdgeBlock(int blockIndex, int y)
    {
        if (blockIndex - y * 256 == 0)
        {

        }
        else
        {
            if (blockIndex - y * 256 == 15)
            {

            }
            else
            {
                if (blockIndex - y * 256 == 240)
                {

                }
                else
                {
                    if (blockIndex - y * 256 == 255)
                    {

                    }
                    else
                    {
                        if (blockIndex <= 15)
                        {

                        }
                        else
                        {
                            if (blockIndex >= 240 && blockIndex <= 255)
                            {

                            }
                            else
                            {
                                if (blockIndex % 15 == 0 && blockIndex <= 255)
                                {

                                }
                                else
                                {
                                    if (blockIndex % 16 == 0 && blockIndex <= 255)
                                    {

                                    }
                                    else
                                    {
                                        if (blockIndex >= 3840 && blockIndex <= 3855)
                                        {

                                        }
                                        else
                                        {
                                            if (blockIndex >= 4080)
                                            {

                                            }
                                            else
                                            {
                                                if (blockIndex % 15 == 0 && blockIndex >= 3855)
                                                {

                                                }
                                                else
                                                {
                                                    if (blockIndex % 16 == 0 && blockIndex >= 3840)
                                                    {

                                                    }
                                                    else
                                                    {
                                                        Debug.Log("Tried to create edge block but missed correct check - unreachable code reached");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    //-----------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------
    [BurstCompile]
    public bool IsFace(int blockIndex, int y)
    {
        if (blockIndex - y * 256 <= 15
            || (blockIndex - y * 256 >= 240 && blockIndex <= 255)
            || (blockIndex - y * 256) % 15 == 0
            || (blockIndex - y * 256) % 16 == 0
            || blockIndex <= 255
            || blockIndex >= 3840)
        {
            return true;
        }
        else return false;
    }

    //-----------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------
    [BurstCompile]
    public void AddFaceBlock(int blockIndex, int y)
    {
        if (blockIndex - y * 256 <= 15)
        {

        }
        else
        {
            if (blockIndex - y * 256 >= 240 && blockIndex <= 255)
            {

            }
            else
            {
                if ((blockIndex - y * 256) % 15 == 0)
                {

                }
                else
                {
                    if ((blockIndex - y * 256) % 16 == 0)
                    {

                    }
                    else
                    {
                        if (blockIndex <= 255)
                        {

                        }
                        else
                        {
                            if (blockIndex >= 3840)
                            {

                            }
                            else
                            {
                                Debug.Log("Tried to create face block but missed correct check - unreachable code reached");
                            }
                        }
                    }
                }
            }
        }
    }

    //-----------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------
    [BurstCompile]
    public void RemoveFaceBlock(int blockIndex, int y)
    {
        if (blockIndex - y * 256 <= 15)
        {

        }
        else
        {
            if (blockIndex - y * 256 >= 240 && blockIndex <= 255)
            {

            }
            else
            {
                if ((blockIndex - y * 256) % 15 == 0)
                {

                }
                else
                {
                    if ((blockIndex - y * 256) % 16 == 0)
                    {

                    }
                    else
                    {
                        if (blockIndex <= 255)
                        {

                        }
                        else
                        {
                            if (blockIndex >= 3840)
                            {

                            }
                            else
                            {
                                Debug.Log("Tried to create face block but missed correct check - unreachable code reached");
                            }
                        }
                    }
                }
            }
        }
    }

    //-----------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------
    [BurstCompile]
    public void AddBlockDefault(Transform chunk, Vector3 subChunkPosition, int blockIndex, RawSubChunkData.BlockType type, bool hasMoreBlocksToRemove)
    {
        // Vertices and uvs are always the same size.
        // only triangles need to change.
        // Get sub chunk triangles

        SubChunkData selectedSubChunkData;

        if (blockEngine.blockMap.subChunkData.ContainsKey(subChunkPosition))
        {
            selectedSubChunkData = blockEngine.blockMap.subChunkData[subChunkPosition];
        }
        else
        {
            Debug.Log("Sub Chunk data does not exist at position");
            return;
        }

        // Add/place the block in the types
        // 0 - empty
        // 1+ solid

        selectedSubChunkData.types[blockIndex] = (int)type;

        //-----------------------------------------------------------------------------
        // Front face
        //-----------------------------------------------------------------------------
        if (selectedSubChunkData.types[blockIndex + 1] > 0)
        {
            // Block exists infront of the block we are adding to
            // remove face
            selectedSubChunkData.faces[(blockIndex + 1) * 6 + (int)RawSubChunkData.FaceTypes.Back] = false;
        }
        else
        {
            // add face
            selectedSubChunkData.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Front] = true;
        }

        //-----------------------------------------------------------------------------
        // Back face
        //-----------------------------------------------------------------------------
        if (selectedSubChunkData.types[blockIndex - 1] > 0)
        {
            // Block exists infront of the block we are adding to
            // remove face
            selectedSubChunkData.faces[(blockIndex - 1) * 6 + (int)RawSubChunkData.FaceTypes.Front] = false;
        }
        else
        {
            // add face
            selectedSubChunkData.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Back] = true;
        }

        //-----------------------------------------------------------------------------
        // Right face
        //-----------------------------------------------------------------------------
        if (selectedSubChunkData.types[blockIndex + 16] > 0)
        {
            // Block exists infront of the block we are adding to
            // remove face
            selectedSubChunkData.faces[(blockIndex + 16) * 6 + (int)RawSubChunkData.FaceTypes.Left] = false;
        }
        else
        {
            // add face
            selectedSubChunkData.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Right] = true;
        }

        //-----------------------------------------------------------------------------
        // Left face
        //-----------------------------------------------------------------------------
        if (selectedSubChunkData.types[blockIndex - 16] > 0)
        {
            // Block exists infront of the block we are adding to
            // remove face
            selectedSubChunkData.faces[(blockIndex - 16) * 6 + (int)RawSubChunkData.FaceTypes.Right] = false;
        }
        else
        {
            // add face
            selectedSubChunkData.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Left] = true;
        }

        //-----------------------------------------------------------------------------
        // Top face
        //-----------------------------------------------------------------------------
        // if block exists above
        if (selectedSubChunkData.types[blockIndex + 256] > 0)
        {
            // Block exists infront of the block we are adding to
            // remove face
            selectedSubChunkData.faces[(blockIndex + 256) * 6 + (int)RawSubChunkData.FaceTypes.Bottom] = false;
        }
        else // block does not exist above
        {
            // add face
            selectedSubChunkData.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Top] = true;
        }


        //-----------------------------------------------------------------------------
        // Bottom face
        //-----------------------------------------------------------------------------
        if (selectedSubChunkData.types[blockIndex - 256] > 0)
        {
            // Block exists infront of the block we are adding to
            // remove face
            selectedSubChunkData.faces[(blockIndex - 256) * 6 + (int)RawSubChunkData.FaceTypes.Top] = false;
        }
        else
        {
            // add face
            selectedSubChunkData.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Bottom] = true;
        }

        // BLOCK IS NOW ADDED WITH SURROUNDING BLOCK FACES MODIFIED ON THE SELECTED SUB CHUNK DATA.

        // REASSIGN IT TO DICTIONARY TO BE SURE ITS SAVED.
        // MORE BLOCKS MAYBE NEED TO BE ADDED SO DO NOT UPDATE CHUNK YET
        // ADD TO LIST OF CHUNKS THAT NEED UPDATED.

        // Add chunk name to track - need to update the correct gameobject (incase treadmill goes off edge changing chunk)

        blockEngine.blockMap.subChunkData[subChunkPosition] = selectedSubChunkData;

        if (subChunksActiveChunkName.ContainsKey(subChunkPosition) == false)
        {
            subChunksActiveChunkName.Add(subChunkPosition, chunk.name);
        }

        // Check if there are more changes to be done to the sub chunk data before generating new triangles for it.

        if (hasMoreBlocksToRemove == false)
        {
            GenerateTrianglesUsingFaceMap(subChunkPosition, selectedSubChunkData.faces);
        }
        else
        {
            // Add these to list to be updated after all sub chunk data face maps have been modified.
            // For system to add/remove more blocks at once without yet getting the triangles for the sub chunk.

            if (subChunksThatNeedsNewMeshDataGenerated.ContainsKey(subChunkPosition) == false)
            {
                Debug.Log("Successfully modified sub chunk data");
                subChunksThatNeedsNewMeshDataGenerated.Add(subChunkPosition, selectedSubChunkData);
            }
            else
            {
                Debug.Log("Successfully modified sub chunk data - sub chunk data already in queue to be updated");
            }
        }
    }

    //-----------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------
    [BurstCompile]
    public void RemoveBlockDefault(Transform chunk, Vector3 subChunkPosition, int blockIndex, bool hasMoreBlocksToRemove)
    {
        // Vertices and uvs are always the same size.
        // only triangles need to change.
        // Get sub chunk triangles

        SubChunkData selectedSubChunkData;

        if (blockEngine.blockMap.subChunkData.ContainsKey(subChunkPosition))
        {
            selectedSubChunkData = blockEngine.blockMap.subChunkData[subChunkPosition];
        }
        else
        {
            Debug.Log("Sub Chunk data does not exist at position");
            return;
        }

        // Add/place the block in the types
        // 0 - empty
        // 1+ solid

        selectedSubChunkData.types[blockIndex] = (int)RawSubChunkData.BlockType.Air;

        //-----------------------------------------------------------------------------
        // Front face
        //-----------------------------------------------------------------------------
        if (selectedSubChunkData.types[blockIndex + 1] != 0)
        {
            // Block exists infront of the block we are adding to
            // add face
            selectedSubChunkData.faces[(blockIndex + 1) * 6 + (int)RawSubChunkData.FaceTypes.Back] = true;
        }
        // remove face
        selectedSubChunkData.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Front] = false;

        //-----------------------------------------------------------------------------
        // Back face
        //-----------------------------------------------------------------------------
        if (selectedSubChunkData.types[blockIndex - 1] != 0)
        {
            // Block exists infront of the block we are adding to
            // add face
            selectedSubChunkData.faces[(blockIndex - 1) * 6 + (int)RawSubChunkData.FaceTypes.Front] = true;
        }
        // remove face
        selectedSubChunkData.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Back] = false;

        //-----------------------------------------------------------------------------
        // Right face
        //-----------------------------------------------------------------------------
        if (selectedSubChunkData.types[blockIndex + 16] != 0)
        {
            // Block exists infront of the block we are adding to
            // add face
            selectedSubChunkData.faces[(blockIndex + 16) * 6 + (int)RawSubChunkData.FaceTypes.Left] = true;
        }
        // remove face
        selectedSubChunkData.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Right] = false;

        //-----------------------------------------------------------------------------
        // Left face
        //-----------------------------------------------------------------------------
        if (selectedSubChunkData.types[blockIndex - 16] != 0)
        {
            // Block exists infront of the block we are adding to
            // add face
            selectedSubChunkData.faces[(blockIndex - 16) * 6 + (int)RawSubChunkData.FaceTypes.Right] = true;
        }
        // remove face
        selectedSubChunkData.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Left] = false;

        //-----------------------------------------------------------------------------
        // Top face
        //-----------------------------------------------------------------------------
        if (selectedSubChunkData.types[blockIndex + 256] != 0)
        {
            // Block exists infront of the block we are adding to
            // add face
            selectedSubChunkData.faces[(blockIndex + 256) * 6 + (int)RawSubChunkData.FaceTypes.Bottom] = true;
        }
        // remove face
        selectedSubChunkData.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Top] = false;

        //-----------------------------------------------------------------------------
        // Bottom face
        //-----------------------------------------------------------------------------
        if (selectedSubChunkData.types[blockIndex - 256] != 0)
        {
            // Block exists infront of the block we are adding to
            // add face
            selectedSubChunkData.faces[(blockIndex - 256) * 6 + (int)RawSubChunkData.FaceTypes.Top] = true;
        }
        // remove face
        selectedSubChunkData.faces[blockIndex * 6 + (int)RawSubChunkData.FaceTypes.Bottom] = false;

        // BLOCK IS NOW ADDED WITH SURROUNDING BLOCK FACES MODIFIED ON THE SELECTED SUB CHUNK DATA.

        // REASSIGN IT TO DICTIONARY TO BE SURE ITS SAVED.
        // MORE BLOCKS MAYBE NEED TO BE ADDED SO DO NOT UPDATE CHUNK YET
        // ADD TO LIST OF CHUNKS THAT NEED UPDATED.

        // Add chunk name to track - need to update the correct gameobject (incase treadmill goes off edge changing chunk)

        blockEngine.blockMap.subChunkData[subChunkPosition] = selectedSubChunkData;

        if (subChunksActiveChunkName.ContainsKey(subChunkPosition) == false)
        {
            subChunksActiveChunkName.Add(subChunkPosition, chunk.name);
        }

        // Check if there are more changes to be done to the sub chunk data before generating new triangles for it.

        if (hasMoreBlocksToRemove == false)
        {
            GenerateTrianglesUsingFaceMap(subChunkPosition, selectedSubChunkData.faces);
        }
        else
        {
            // Add these to list to be updated after all sub chunk data face maps have been modified.
            // For system to add/remove more blocks at once without yet getting the triangles for the sub chunk.

            if (subChunksThatNeedsNewMeshDataGenerated.ContainsKey(subChunkPosition) == false)
            {
                Debug.Log("Successfully modified sub chunk data");
                subChunksThatNeedsNewMeshDataGenerated.Add(subChunkPosition, selectedSubChunkData);
            }
            else
            {
                Debug.Log("Successfully modified sub chunk data - sub chunk data already in queue to be updated");
            }
        }
    }

    //-----------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------
    [BurstCompile]
    public void NeedsSetMesh(string name, Vector3 position)
    {        
        Transform subChunk = blockEngine.blockPool.subChunksPool[name][((int)position.y) / blockEngine.blockMap.SUB_CHUNK_HEIGHT];

        // Get the mesh component of the "Sub Chunk"

        Mesh mesh = subChunk.GetComponent<MeshFilter>().mesh;

        mesh.vertices = blockEngine.rawSubChunkData.vertices;
        mesh.triangles = blockEngine.blockMap.subChunkData[position].triangles;
        mesh.uv = blockEngine.rawSubChunkData.uvs;

        mesh.Optimize();
        //mesh.RecalculateNormals();

        subChunk.GetComponent<MeshCollider>().sharedMesh = mesh;
    }*/
}
