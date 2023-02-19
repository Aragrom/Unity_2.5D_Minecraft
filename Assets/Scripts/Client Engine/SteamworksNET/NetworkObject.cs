using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

// An Object Identifier is needed so we know what kind of object it is.
// A Local Identifier tells us whether or not we own the object
// A Network ID is required for each client reference the correct object.
// An ID counter tells the host the next number to assign as an Id
// Serialize / Read Functions Allow us to work with the object's data we need to send 
public abstract class NetworkObject : MonoBehaviour
{
    //public SpawnableObjects mObjectType = SpawnableObject.kUnassigned;
    public bool mIsLocal = false;
    public int mNetworkID = -1;

    public static int mIDCount = 1;

    public abstract void Serialize(ref System.IO.BinaryWriter memoryWriter);
    public abstract void Read(ref System.IO.BinaryWriter memoryReader);
}
