using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class Look : MonoBehaviour
{
    private InputEngine inputEngine = null;
    private PlayerRotation playerRotation = null;

    public Rect boundary = new Rect(-64, -64, 48, 48);

    public int cameraMoveSpeed = 10;

    public Transform sprite = null;
    public Transform player = null;
    public Transform cameraRig = null;

    public bool isReset = true;

    public enum Direction { North, East, South, West }
    public Direction direction = Direction.North;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory
        cameraRig = null;
    }

    [BurstCompile]
    private void Awake()
    {
        inputEngine = GameObject.Find("Input Engine").GetComponent<InputEngine>();
        cameraRig = GameObject.Find("Camera Rig").transform;
        player = cameraRig.transform.parent;
        sprite = GameObject.Find("Player Sprite").transform;

        playerRotation = GameObject.Find("Player Engine").GetComponent<PlayerRotation>();
    }

    /*// Start is called before the first frame update
    void Start()
    {
        
    }
    */


    // Update is called once per frame
    [BurstCompile]
    void Update()
    {
        if (inputEngine.look != Vector2.zero
            && playerRotation.isRotating == false ) // can not be rotating
        {
            AdjustCameraRigLocalPosition();
        }
        else
        {
            if (!isReset)
            {
                ResetCameraRigLocalPositon();
            }
        }
    }

    [BurstCompile]
    public void AdjustCameraRigLocalPosition()
    {
        if (isReset == true) 
        {
            // attach the player sprite to the player transform

            sprite.transform.parent = player;
        }

        isReset = false;

        switch (direction)
        {
            case Direction.North:
                cameraRig.localPosition += new Vector3(inputEngine.look.x * cameraMoveSpeed * Time.deltaTime, inputEngine.look.y * cameraMoveSpeed * Time.deltaTime, 0);
                return;

            case Direction.South:
                cameraRig.localPosition += new Vector3(-inputEngine.look.x * cameraMoveSpeed * Time.deltaTime, inputEngine.look.y * cameraMoveSpeed * Time.deltaTime, 0);
                return;

            case Direction.East:
                cameraRig.localPosition += new Vector3(0, inputEngine.look.y * cameraMoveSpeed * Time.deltaTime, -inputEngine.look.x * cameraMoveSpeed * Time.deltaTime);
                return;

            case Direction.West:
                cameraRig.localPosition += new Vector3(0, inputEngine.look.y * cameraMoveSpeed * Time.deltaTime, inputEngine.look.x * cameraMoveSpeed * Time.deltaTime);
                return;
        }

        // Parent the object back to the camera.
    }

    [BurstCompile]
    public void ResetCameraRigLocalPositon()
    {
        isReset = true;
        cameraRig.localPosition = Vector3.zero;

        // attach the player sprite to the camera rig

        sprite.transform.parent = cameraRig;
    }
}
