using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public struct MovementConfiguration
{
    public int frames;
    public float time;
    public int heightOfMario;
    public int distanceOfFallPixels;
    public float distanceOfFall;
    public float acceleration;

    public float gravity;

    public MovementConfiguration(int frames,
        float time,
        int heightOfMario,
        int distanceOfFallPixels,
        float distanceOfFall,
        float acceleration,
        float gravity)
    {
        this.frames = frames;
        this.time = time;
        this.heightOfMario = heightOfMario;
        this.distanceOfFallPixels = distanceOfFallPixels;
        this.distanceOfFall = distanceOfFall;
        this.acceleration = acceleration;
        this.gravity = gravity;
    }
}

[BurstCompile]
public class PlayerMovementConfigurations : MonoBehaviour
{
    // Used to replicate other game movements. Super Mario World/1/2/3, Sonic, Smash Brother Melee, etc.
    private List<MovementConfiguration> movementConfigurations = new List<MovementConfiguration>();

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        movementConfigurations.Clear();
        movementConfigurations = null;
    }

    [BurstCompile]
    public void Start()
    {
        DefineMovementConfigurations();
    }

    [BurstCompile]
    public void DefineMovementConfigurations()
    {
        /*                                                (Pixels)      (m)
                                              Height      Distance    Distance     (m/s)^2      (g)
                              Frames   Time   of Mario    of Fall     of fall   Acceleration  Acceleration
         Super Mario Bros.	    15  	0.5	    39	        292	        11.4	    91.28	    9.31
        Super Mario Bros. 2	    12	    0.4	    45	        255	        8.6	        107.95	    11
        Super Mario Bros. 3	    15	    0.5	    35	        265	        11.5	    92.31	    9.42
        Super Mario World	    15	    0.5	    38	        193	        7.7	        61.92	    6.32
            Super Mario 64	    10	    0.33    86	        217	        3.8	        69.22	    7.06
        Super Mario Sunshine	23	    0.77    119	        988	        12.7	    43.05	    4.4
        Super Paper Mario	    12	    0.4	    288	        748	        4	        49.47	    5.05
         */

        movementConfigurations.Add(new MovementConfiguration(15, 0.5f, 35, 265, 11.5f, 92.31f, 9.42f));  // Super mario bros 3        
    }
}
