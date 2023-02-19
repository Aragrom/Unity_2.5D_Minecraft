using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class PlayerAutoPlay : MonoBehaviour
{
    public InputEngine inputEngine = null;
    public PlayerRotation playerRotation = null;

    public float AUTO_RESET_TIMER = 30.0f;
    public float autoRotateTimer = 30.0f;

    // Update is called once per frame
    [BurstCompile]
    void Update()
    {
        // force to auto play
        inputEngine.jump = true;
        inputEngine.move = Vector2.right;
        inputEngine.run = true;

        autoRotateTimer -= Time.deltaTime;

        if (autoRotateTimer < 0)
        {
            autoRotateTimer = AUTO_RESET_TIMER;
            AutoRotate();
        }
    }

    [BurstCompile]
    public void AutoRotate()
    {
        float randomNumber = Random.Range(-10, 10);

        if (randomNumber > 0)
        {
            inputEngine.rotateLeft = true;
        }
        else
        {
            inputEngine.rotateRight = true;
        }
    }
}
