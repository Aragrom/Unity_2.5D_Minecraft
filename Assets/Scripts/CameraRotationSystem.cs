using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class CameraRotationSystem : MonoBehaviour
{
    public Vector3 target = new Vector3(256, 50, 256);
    public float speed = 20.0f;
    public Vector3 step = Vector3.right;

    // Start is called before the first frame update
    [BurstCompile]
    void Start()
    {
        step *= speed;
    }

    // Update is called once per frame
    [BurstCompile]
    void Update()
    {
        transform.LookAt(target);
        transform.Translate(step * Time.deltaTime);
    }
}
