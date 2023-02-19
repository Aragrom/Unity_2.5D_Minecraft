using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class InternetAccess : MonoBehaviour
{
    public NetworkReachability networkReachability;
    public bool hasAccess = false;

    // Update is called once per frame
    [BurstCompile]
    public void CheckAccess()
    {
        // Get internet access state enum - NetworkReachability.

        if ((networkReachability = Application.internetReachability) != NetworkReachability.NotReachable) hasAccess = true;
        else hasAccess = false;
    }
}
