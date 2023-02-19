using System.Collections;
using System.Collections.Generic;
//using System.Drawing;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

/*
internal class Map
{
    private readonly int[] data;
    private readonly int height;
    private readonly int width;

    public Map(int height, int width)
    {
        this.height = height;
        this.width = width;

        data = new int[height * width];
    }

    public int Height
    {
        get { return height; }
    }

    public int Width
    {
        get { return width; }
    }

    public int this[int x, int y]
    {
        get { return data[x + y * width]; }
        set { data[x + y * width] = value; }
    }
}
*/

[BurstCompile]
public class LightMap : MonoBehaviour
{
    public BlockEngine blockEngine = null;

    // key - sub chunk position, (width * width * height)
    public Dictionary<Vector2, int[]> subMaps = new Dictionary<Vector2, int[]>();
    public Dictionary<Vector2, List<Vector3>> subMapLightPositions = new Dictionary<Vector2, List<Vector3>>();

    //===============================
    // Debugging


    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        blockEngine = null;
    }

    [BurstCompile]
    public void CleanUp()
    {
        subMaps.Clear();
    }

    [BurstCompile]
    public void OnApplicationQuit()
    {

    }

    [BurstCompile]
    private void Awake()
    {
        blockEngine = GameObject.Find("Block Engine").GetComponent<BlockEngine>();

        //FloodFill(map, new Point(123, 123), fromValue: 0, toValue: 1);
    }

    // Start is called before the first frame update
    [BurstCompile]
    void Start()
    {

    }

    // Update is called once per frame
    [BurstCompile]
    void Update()
    {

    }

    [BurstCompile]
    public void LateUpdate()
    {

    }

    // Should generate uv2 to be used with the mesh.
    // the uv2 should hold uv coordinates for the "light level" or "brightness"
    // to be combined with the original texture in the shader.
    [BurstCompile]
    struct FloodFillLightMapJob : IJob
    {
        // Input
        // list of light positions (torches)
        // Height Map
        // Shape Map
        // Worm Map?
        // if this light map is not the most top the "Above light Map"

        int width;
        int height;

        int blockHeight;

        // Output
        // Light map

        [BurstCompile]
        public void Execute()
        {
            // "Breadth first"
            // Work from TOP to BOTTOM
            // Y > X > Z

            //var open = new Queue<Vector2>();

            for (int y = 0; y < height; y++)
            {
                // set blockHeight

                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < width; z++)
                    {
                        // Check block height at height map position. If above height map do nothing.

                        // below height map 



                        // Check shape map position
                        // shape map is solid - reached the bottom - maximum sunlight for column

                        // shape map is empty - sunlight continues.
                    }
                }
            }
        }
    }

    [BurstCompile]
    void FloodFillLightMap()
    {
        // input


    }


    // !!!! = COULD THE UVS BE JUST A BRIGHTNESS VALUE INSTEAD OF MAPPING UV TO MESH???

    // COULD CALCULATE ONCE ALL THE INDEXES THAT MUST BE VISITED. AS AN "Index Map" Which could then be used in a loop to make sure all elements are all reached? But 

    /*
    private static void FloodFill(Map map, Point start, int fromValue, int toValue)
    {
        if (map[start.X, start.Y] == fromValue == false)
        {
            return;
        }

        map[start.X, start.Y] = toValue;

        var open = new Queue<Point>();
        open.Enqueue(start);

        while (open.Count > 0)
        {
            var current = open.Dequeue();
            var x = current.X;
            var y = current.Y;

            if (x > 0)
            {
                if (map[x - 1, y] == fromValue)
                {
                    map[x - 1, y] = toValue;
                    open.Enqueue(new Point(x - 1, y));
                }
            }
            if (x < map.Width - 1)
            {
                if (map[x + 1, y] == fromValue)
                {
                    map[x + 1, y] = toValue;
                    open.Enqueue(new Point(x + 1, y));
                }
            }
            if (y > 0)
            {
                if (map[x, y - 1] == fromValue)
                {
                    map[x, y - 1] = toValue;
                    open.Enqueue(new Point(x, y - 1));
                }
            }
            if (y < map.Height - 1)
            {
                if (map[x, y + 1] == fromValue)
                {
                    map[x, y + 1] = toValue;
                    open.Enqueue(new Point(x, y + 1));
                }
            }
        }
    */
}
