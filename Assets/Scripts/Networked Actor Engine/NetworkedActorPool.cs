using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class NetworkedActorPool : MonoBehaviour
{
    public GameObject prefab;

    public Transform[] transforms = new Transform[100]; // max number of players on the server

    public int active = 0; // each element refers to an index in transform that is active

    // Start is called before the first frame update
    [BurstCompile]
    void Awake()
    {
        // Removed as its not currently being used
        //SetUp();
    }

    [BurstCompile]
    void SetUp()
    {
        Vector3 startingPosition = new Vector3(0f, 260f, 3.5f);

        for (int i = 0; i < transforms.Length; i++)
        {
            transforms[i] = Instantiate(prefab, startingPosition, Quaternion.identity).transform;
        }
    }

    [BurstCompile]
    public void AdjustActiveGameObjects(int count)
    {
        int difference = active - count;

        for (int i = 0; i < difference; i++)
        {
            transforms[(active - 1) + i].gameObject.SetActive(true);
        }

        /*
         if (active == count) return; // same size

            if (active > count)
            {
                // Removing active

                int difference = active - count;

                for (int i = 0; i < difference; i++)
                {
                    transforms[(active - 1) - i].gameObject.SetActive(false);
                }
            }
            else
            {
                // Adding active

                int difference = count - active;

                for (int i = 0; i < difference; i++)
                {
                    transforms[(active - 1) + i].gameObject.SetActive(true);
                }
            }
         */
    }
}
