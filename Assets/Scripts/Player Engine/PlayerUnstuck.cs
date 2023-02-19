using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnstuck : MonoBehaviour
{
    public float RESET_TIME = 1.0f;
    public float timer = 1.0f;

    public PlayerRotation playerRotation = null;
    public CharacterController characterController = null;
    public Transform player = null;
    public Vector3 oldPosition = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            timer = RESET_TIME;

            if (playerRotation.direction == PlayerRotation.Direction.North
                || playerRotation.direction == PlayerRotation.Direction.South)
            {
                if (oldPosition.x == player.position.x)
                {
                    // need to reset position

                    characterController.enabled = false;
                    player.position += Vector3.up * 100;
                    characterController.enabled = true;
                }
            }
            else
            {
                if (oldPosition.z == player.position.z)
                {
                    // need to reset position

                    characterController.enabled = false;
                    player.position += Vector3.up * 100;
                    characterController.enabled = true;
                }
            }

            oldPosition = player.position;
        }
    }
}
