using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngine.InputSystem;

[BurstCompile]
public class PlayerInput : MonoBehaviour
{
    public PlayerEngine playerEngine = null;

    public Vector2 move = Vector2.zero;
    public bool jump = false;

    public bool rotateLeft = false;
    public bool rotateRight = false;

    public bool run = false;

    public bool sliding = false;

    public bool zoom = false;

    public bool dig = false;

    public bool menu = false;
    public bool boost = false;

    /*public Vector2 look = Vector2.zero;
    
    public bool boosting = false;
    public bool rotateLeft = false;
    public bool rotatateRight = false;*/

    PlayerControls controls;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        playerEngine = null;
        controls.Gameplay.Disable();
        controls = null;
    }

    // auto play to the right
    /*[BurstCompile]
    public void Update()
    {
        // force to auto play
        jump = true;
        move = Vector2.right;
        run = true;
    }*/

    [BurstCompile]
    public void Awake()
    {
        playerEngine = GetComponent<PlayerEngine>();

        controls = new PlayerControls();

        // Could be changed to call the functions for events

        controls.Gameplay.Jump.performed += context => jump = true;
        controls.Gameplay.Jump.canceled += context => jump = false;

        controls.Gameplay.Boost.performed += context => boost = true;
        controls.Gameplay.Boost.canceled += context => boost = false;

        controls.Gameplay.Zoom.performed += context => zoom = true;

        controls.Gameplay.Run.performed += context => run = true;
        controls.Gameplay.Run.canceled += context => run = false;

        controls.Gameplay.RotateLeft.performed += context => rotateLeft = true;
        controls.Gameplay.RotateRight.performed += context => rotateRight = true;

        controls.Gameplay.Move.performed += context => move = context.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += context => move = Vector2.zero;

        controls.Gameplay.Dig.performed += context => dig = true;
        controls.Gameplay.Dig.canceled += context => dig = false;

        controls.Gameplay.Menu.performed += context => menu = true;
        controls.Gameplay.Menu.canceled += context => menu = false;
    }

    [BurstCompile]
    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    [BurstCompile]
    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }
}
