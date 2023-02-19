using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

// Used for debugging! Output mesh data of gameobject to debug log.
[BurstCompile]
public class ExposePrintMeshData : MonoBehaviour
{
    public GameObject go = null;

    public bool trigger = false;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        go = null;
    }

    // Update is for DEBUGGING only. SHOULD BE REMOVED.
    [BurstCompile]
    public void Update()
    {
        if (trigger)
        {
            trigger = false;
            OutputSubChunkMeshDataToDebugLog();
        }
    }

    /// <summary>
    /// All data generated is output to multiple debug log message
    /// Very inefficent - use string builder
    /// </summary>
    [BurstCompile]
    public void OutputSubChunkMeshDataToDebugLog()
    {
        Mesh mesh = go.GetComponent<MeshFilter>().mesh;

        List<string> verticeStrings = ConvertVertices(mesh.vertices);
        List<string> triangleStrings = ConvertTriangles(mesh.triangles);
        List<string> uvStrings = ConvertUv(mesh.uv);

        // Output to debug log

        for (int i = 0; i < verticeStrings.Count; i++)
        {
            Debug.Log(i + " " + verticeStrings[i]);
        }
        for (int i = 0; i < triangleStrings.Count; i++)
        {
            Debug.Log(i + " " + triangleStrings[i]);
        }
        for (int i = 0; i < uvStrings.Count; i++)
        {
            Debug.Log(i + " " + uvStrings[i]);
        }
    }

    /// <summary>
    /// Convert mesh vertices to a string
    /// </summary>
    /// <param name="vertices"> Mesh vertices </param>
    /// <returns> Result can be to long/large for a single string </returns>
    [BurstCompile]
    List<string> ConvertVertices(Vector3[] vertices)
    {
        List<string> result = new List<string>();
        string temp = "";
        int counter = 0;

        for (int i = 0; i < vertices.Length; i++)
        {
            temp += "new Vector3(" + vertices[i].x.ToString() + "f, "
                + vertices[i].y.ToString() + "f, "
                + vertices[i].z.ToString() + "f), ";

            counter++;

            if (counter > 50)
            {
                // store string. wipe temp.

                result.Add(temp);
                temp = "";
                counter = 0;
            }
        }

        result.Add(temp);

        return result;
    }

    /// <summary>
    /// Convert mesh triangles to a string
    /// </summary>
    /// <param name="triangles"> Mesh triangles </param>
    /// <returns> Result can be to long/large for a single string </returns>
    List<string> ConvertTriangles(int[] triangles)
    {
        List<string> result = new List<string>();
        //int counter = 0;    // Reset
        string temp = "";

        for (int i = 0; i < triangles.Length; i++)
        {
            temp += triangles[i].ToString() + ", ";

            /*counter++;

            if (counter > 50)   
            {
                // store string. wipe temp.

                result.Add(temp);
                temp = "";
                counter = 0;
            }*/
        }

        result.Add(temp);

        return result;
    }

    /// <summary>
    /// Convert mesh uv to a string
    /// </summary>
    /// <param name="uv"> Mesh uvs </param>
    /// <returns> Result can be to long/large for a single string </returns>
    List<string> ConvertUv(Vector2[] uv)
    {
        List<string> result = new List<string>();
        int counter = 0;    // Reset
        string temp = "";

        for (int i = 0; i < uv.Length; i++)
        {
            temp += "new Vector2(" + uv[i].x.ToString() + "f, "
                + uv[i].y.ToString() + "f), ";

            counter++;

            if (counter > 50)
            {
                // store string. wipe temp.

                result.Add(temp);
                temp = "";
                counter = 0;
            }
        }

        result.Add(temp);

        return result;
    }
}

