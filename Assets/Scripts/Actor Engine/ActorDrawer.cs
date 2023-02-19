using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class ActorDrawer : MonoBehaviour
{
    public HeightMap heightMap = null;

    const float BATCH_MAX_FLOAT = 1023f;
    const int BATCH_MAX = 1023;

    public GameObject prefab;
    public Material meshMaterial;
    public int width;
    public int depth;
    public float spacing;

    public bool animate = true;
    public int spriteSheetWidth = 9;              // sprites per row      (based on active base map - sprite sheet for material)
    public int spriteSheetHeight = 3;             // sprites per column   (based on active base map - sprite sheet for material)
    public int currentRow = 2;
    public int currentColumn = 0;
    public int currentSprite = 0;
    public int maxSprites = 24;
    public int frameCount = 0;
    public int numberOfFrames = 8;
    public int startingColumn = 0;
    public Vector2 materialOffset = Vector2.zero;
    public float animationSpeed = 1/8;
    public float animationTimer = 1/8;

    private MeshFilter mMeshFilter;

    private Matrix4x4[] matrices;

    public Vector3 startingPosition = new Vector3(0, 260, 10);
    public Vector3 fixedTranslation = new Vector3(0, 0, -0.1f);
    public Vector3 translation = new Vector3(0, 0, -0.1f);
    public float heightAdjustRate = 1;
    public float heightTimer = 1;

    [BurstCompile]
    void Start()
    {
        heightMap = GameObject.Find("Block Engine").GetComponent<HeightMap>();

        startingPosition.x = -(width / 2) + 0.5f;
        startingPosition.y = startingPosition.y + 0.5f;
        startingPosition.z = startingPosition.z + 0.5f;

        mMeshFilter = prefab.GetComponent<MeshFilter>();

        InitData();
    }

    [BurstCompile]
    private void InitData()
    {
        int count = width * depth;

        matrices = new Matrix4x4[count];
        Vector3 pos = new Vector3();
        Vector3 scale = new Vector3(4, 4, 4);

        int idx = 0;

        for (int i = (int)startingPosition.x; i < (int)startingPosition.x + width; ++i)
        {
            for (int j = (int)startingPosition.z; j < (int)startingPosition.z + depth; ++j)
            {
                idx = (i - (int)startingPosition.x) * depth + (j - (int)startingPosition.z);

                matrices[idx] = Matrix4x4.identity;

                pos.x = i * spacing;
                pos.y = 260;
                pos.z = j * spacing;

                matrices[idx].SetTRS(pos, Quaternion.identity, scale);
            }
        }
    }

    [BurstCompile]
    void Update()
    {
        animationTimer -= Time.deltaTime;
        heightTimer -= Time.deltaTime;

        if (animationTimer < 0)
        {
            animationTimer = animationSpeed;
            UpdateMaterialOffset();
        }

        // calculate timed step (Just once - optimized)

        translation = fixedTranslation * Time.deltaTime;

        // transform each position
        // TO DO: ADD TREADMILL TRUE POSITION HERE AS IT CHANGES.

        // TO DO: QUERY HEIGHT MAP FOR Y POSITION at (X, Z)

        /*if (heightTimer < 0)
        {
            heightTimer = heightAdjustRate;
            float height = 0;
            Vector2 chunkPosition = Vector2.zero;
            int index = 0;

            for (int i = 0; i < matrices.Length; i++)
            {
                chunkPosition = new Vector2(((int)(matrices[i][0, 3] / 16f) * 16f), ((int)(matrices[i][2, 3] / 16f) * 16f));
                index = (int)Mathf.Floor((matrices[i][0, 3] % 16f) * 16) + (int)Mathf.Floor(((matrices[i][2, 3] % 16f)));

                //if (index > 255) index = 255;

                if (index < 0)
                {
                    index = 255 + index;
                }

                Debug.Log("Position: " + matrices[i][0, 3] + " , " + matrices[i][1, 3] + " , " + matrices[i][2, 3] + "  Chunk: " + chunkPosition.ToString() + "   Index: " + index.ToString());

                height = heightMap.subMaps[chunkPosition][index]; // /16 * 16 to average out the difference (23, 0 , 0) > (16, 0, 0)
                                                                  // %16 * 16 to find the position inside the chunk

                matrices[i][0, 3] += translation.x;
                matrices[i][1, 3] += translation.y;
                matrices[i][2, 3] += translation.z;

                // set height
                matrices[i][1, 3] = height - 3;
            }
        }
        else
        {*/
            for (int i = 0; i < matrices.Length; i++)
            {
                matrices[i][0, 3] += translation.x;
                matrices[i][1, 3] += translation.y;
                matrices[i][2, 3] += translation.z;
            }
        //}

        int total = width * depth;
        int batches = Mathf.CeilToInt(total / BATCH_MAX_FLOAT);

        for (int i = 0; i < batches; ++i)
        {
            int batchCount = Mathf.Min(BATCH_MAX, total - (BATCH_MAX * i));
            int start = Mathf.Max(0, (i - 1) * BATCH_MAX);

            Matrix4x4[] batchedMatrices = GetBatchedMatrices(start, batchCount);
            Graphics.DrawMeshInstanced(mMeshFilter.sharedMesh, 0, meshMaterial, batchedMatrices, batchCount);
        }
    }

    [BurstCompile]
    private Matrix4x4[] GetBatchedMatrices(int offset, int batchCount)
    {
        Matrix4x4[] batchedMatrices = new Matrix4x4[batchCount];

        for (int i = 0; i < batchCount; ++i)
        {
            batchedMatrices[i] = matrices[i + offset];
        }

        return batchedMatrices;
    }

    [BurstCompile]
    void UpdateMaterialOffset()
    {
        currentColumn += 3;
        currentSprite += 3;

        // Check first if we have reached the last sprite in the animation

        if (currentSprite >= maxSprites)
        {
            currentColumn = startingColumn;
            currentRow = 2;

            currentSprite = startingColumn;
        }

        // increment column and row to adjust offset

        if (currentColumn >= spriteSheetWidth)
        {
            currentColumn = startingColumn;
            currentRow--;

            if (currentRow < 0)
            {
                currentRow = 2;
            }
        }

        materialOffset.x = (float)currentColumn * (1 / (float)spriteSheetWidth);
        materialOffset.y = (float)currentRow * (1 / (float)spriteSheetHeight);
        meshMaterial.SetTextureOffset("_BaseMap", materialOffset);
    }
}
