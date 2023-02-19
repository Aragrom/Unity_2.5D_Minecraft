using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class Orbit : MonoBehaviour
{
    public float rotationSpeed = 1.0f;

    // Update is called once per frame
    [BurstCompile]
    void Update()
    {
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
    }
}
