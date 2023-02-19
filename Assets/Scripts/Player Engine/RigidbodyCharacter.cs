using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class RigidbodyCharacter : MonoBehaviour
{
    //private PlayerEngine playerEngine = null;
    public InputEngine inputEngine = null;

    public float Speed = 5f;
    public float JumpHeight = 2f;
    public float GroundDistance = 0.2f;
    public float DashDistance = 5f;
    public LayerMask Ground;

    private Rigidbody _body;
    private Vector3 _inputs = Vector3.zero;
    private bool _isGrounded = true;
    private Transform _groundChecker;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory
        //playerEngine = null;
        inputEngine = null;

        _body = null;
        _groundChecker = null;
    }

    [BurstCompile]
    void Start()
    {
        inputEngine = GameObject.Find("Input Engine").GetComponent<InputEngine>();
        _body = GetComponent<Rigidbody>();
        _groundChecker = transform.GetChild(0);
    }

    [BurstCompile]
    void Update()
    {
        _isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);


        _inputs = Vector3.zero;
        _inputs.x = inputEngine.move.x;
        //_inputs.y = playerEngine.playerInput.move.y;
        if (_inputs != Vector3.zero)
            transform.forward = _inputs;

        if (inputEngine.jump && _isGrounded)
        {
            _body.AddForce(Vector3.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }
        if (inputEngine.run)
        {
            Vector3 dashVelocity = Vector3.Scale(transform.forward, DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime), (Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime), 0));
            _body.AddForce(dashVelocity, ForceMode.VelocityChange);
        }
    }

    [BurstCompile]
    void FixedUpdate()
    {
        _body.MovePosition(_body.position + _inputs * Speed * Time.fixedDeltaTime);
    }
}