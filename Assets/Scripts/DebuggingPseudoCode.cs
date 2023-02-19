using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngine.EventSystems;
// zzzzz
// 15
// 14
// 13
// 12
// 11
// 10
// 9
// 8
// 7
// 6
// 5
// 4
// 3  (19)
// 2  (18)
// 1  (17)                                  (241)
// 0 1(16) 2 3 4 5 6 7 8 9 10 11 12 13 14 15(240) XXXXX

// Corners: (0,0) (0, 15) (15, 0) (15, 15)
// edge: 0 or 15.

// (0, 0)   =   north   (0, 1)
//          =   south   (0, 15)     SOUTH TEXTURE
//          =   east    (1, 0)
//          =   west    (15, 0)     WEST TEXTURE

// (10, 0)  =   north   (10, 1)
//          =   south   (10, 15)    SOUTH TEXTURE
//          =   east    (11, 0)
//          =   west    (9, 0)

[BurstCompile]
public struct NewName
{
    public string first;
    public string second;
}

[BurstCompile]
public struct NewAgeArray
{
    public int[] array;
}

// Class is constantly changing to test game logic easily.
[BurstCompile]
public class DebuggingPseudoCode : MonoBehaviour
{
    private Main main = null;

    public Material diceMaterial = null;

    public int index;
    public int newIndex;

    public Vector3 position;
    public int SIZE = 16;

    public int X;
    public int Z;

    public NewName a;
    public NewName b;

    public NewAgeArray newAgeArray;
    public NewAgeArray newerAgeArray;

    public bool[] faces;

    public int lightCounter = 0;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory
        main = null;
        diceMaterial = null;
        faces = null;
    }

    // Start is called before the first frame update
    [BurstCompile]
    void Start()
    {
        main = GameObject.Find("Main").GetComponent<Main>();

        //CreateSimpleCube();

        /*a = new NewName();
        a.first = "graham";
        a.second = "Your Biy";

        b = a;

        a.first = "V";

        newAgeArray = new NewAgeArray();
        newAgeArray.array = new int[] { 1, 2, 3, 4 };

        newerAgeArray = newAgeArray;

        newAgeArray.array[0] = 8;

        faces = new bool[20];*/

        //Debug.Log("RESULT: " + faces[10]);
    }

    // Update is called once per frame
    [BurstCompile]
    void Update()
    {
        /*int x = index / SIZE;
        int y = index / (SIZE * SIZE);
        int z = index % SIZE;

        if (index >= SIZE * SIZE)
        {
            x = (index - y * SIZE * SIZE) / SIZE;
            z = (index - y * SIZE * SIZE) % SIZE;
        }

        position = new Vector3(x, y, z);

        newIndex = (X * SIZE) + Z;*/
    }

    [BurstCompile]
    private void CreateSimpleCube()
    {
        // 8 Vertice cube

        /*Vector3[] vertices = {
            new Vector3 (0, 0, 0),  // 0      4______5
            new Vector3 (1, 0, 0),  // 1     /      /|
            new Vector3 (1, 1, 0),  // 2   3/_____2/ |   
            new Vector3 (0, 1, 0),  // 3    |     |  |
            new Vector3 (0, 1, 1),  // 4    |  7  |  |6
            new Vector3 (1, 1, 1),  // 5    |     | /
            new Vector3 (1, 0, 1),  // 6    |_____|/
            new Vector3 (0, 0, 1),  // 7    0     1
        };     

        // 12 Complete triangles (36 indices)

        int[] triangles = {
            0, 2, 1, //face front
	        0, 3, 2,
            2, 3, 4, //face top
	        2, 4, 5,
            1, 2, 5, //face right
	        1, 5, 6,
            0, 7, 4, //face left
	        0, 4, 3,
            5, 4, 7, //face back
	        5, 7, 6,
            0, 6, 7, //face bottom
	        0, 1, 6
        };

        Vector2[] uvs = {
            new Vector2(0, 1f),
            new Vector2(0, 1f),
            new Vector2(0, 1f),
            new Vector2(0, 1f),
            new Vector2(0, 1f),
            new Vector2(0, 1f),
            new Vector2(0, 1f),
            new Vector2(0, 1f),
        };

        //spawn object
        GameObject cube = new GameObject("8 Vertice Cube");
        */

        float size = 1f;
        Vector3[] vertices = {
            new Vector3(0, size, 0),
            new Vector3(0, 0, 0),
            new Vector3(size, size, 0),
            new Vector3(size, 0, 0),

            new Vector3(0, 0, size),
            new Vector3(size, 0, size),
            new Vector3(0, size, size),
            new Vector3(size, size, size),

            new Vector3(0, size, 0),
            new Vector3(size, size, 0),

            new Vector3(0, size, 0),
            new Vector3(0, size, size),

            new Vector3(size, size, 0),
            new Vector3(size, size, size),
        };

        int[] triangles = {
            0, 2, 1, // front
			1, 2, 3,
            4, 5, 6, // back
			5, 7, 6,
            6, 7, 8, //top
			7, 9 ,8,
            1, 3, 4, //bottom
			3, 5, 4,
            1, 11,10,// left
			1, 4, 11,
            3, 12, 5,//right
			5, 12, 13
        };

        Vector2[] uvs = {
            new Vector2(0, 0.6666f),
            new Vector2(0.25f, 0.6666f),
            new Vector2(0, 0.3333f),
            new Vector2(0.25f, 0.3333f),

            new Vector2(0.5f, 0.6666f),
            new Vector2(0.5f, 0.3333f),
            new Vector2(0.75f, 0.6666f),
            new Vector2(0.75f, 0.3333f),

            new Vector2(1, 0.6666f),
            new Vector2(1, 0.3333f),

            new Vector2(0.25f, 1),
            new Vector2(0.5f, 1),

            new Vector2(0.25f, 0),
            new Vector2(0.5f, 0),
        };

        GameObject cube = new GameObject("14 Vertice Dice");

        //Add Components
        cube.AddComponent<MeshFilter>();
        cube.AddComponent<MeshCollider>();
        cube.AddComponent<MeshRenderer>();

        Mesh mesh = cube.GetComponent<MeshFilter>().mesh;

        mesh.Clear();
        //mesh.vertices = vertices;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.Optimize();
        mesh.RecalculateNormals();

        // Doesn't need collider
        cube.GetComponent<MeshCollider>().sharedMesh = mesh;
        cube.GetComponent<MeshRenderer>().material = diceMaterial;

        //cube.transform.position = Vector3.up * 241.03f + Vector3.back * 0.05f;

        cube.layer = 8;

        cube.transform.rotation = Quaternion.Euler(0, 270, 0);

        //Instantiate(cube, Vector3.zero, Quaternion.identity);
    }

    [BurstCompile]
    public void CreateLight()
    {
        /*// Choose a random number between 0 and 65336 (256 * 256)

        int randomIndex = Random.Range(0, 65336);

        // Sample the far height map.

        float height = main.blockEngine.terrainHeightMap.subMaps[main.blockEngine.terrainHeightMap.subMapPosition][randomIndex] + 1;

        // Turn index into vector3

        int x = randomIndex / 256;
        int z = randomIndex % 256;

        Vector3 position = new Vector3(main.blockEngine.terrainHeightMap.subMapPosition.x + x * 16,
            height + 16,
            main.blockEngine.terrainHeightMap.subMapPosition.y + z * 16);

        // Spawn a point light using vector2.xz and the (height + 1) as the position.
        // Make a game object
        GameObject gameObject = new GameObject("Light " + lightCounter);
        gameObject.transform.position = position;

        // Add the light component
        Light light = gameObject.AddComponent<Light>();
        light.type = LightType.Point;

        light.color = Color.blue;

        light.range = 20.0f;

        light.intensity = 10f;

        lightCounter++;

        // RESET ui event system so button is not in focus (removes the button being highlighted.)

        EventSystem.current.SetSelectedGameObject(null);*/
    }
}
