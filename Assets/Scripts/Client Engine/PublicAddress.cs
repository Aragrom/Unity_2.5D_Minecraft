using Open.Nat;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
[System.Serializable]
public class PublicAddressData 
{
    public string ipv4;
}

[BurstCompile]
public class PublicAddress : MonoBehaviour
{
    public PublicAddressData publicAddressData = new PublicAddressData();

    public static System.Threading.Tasks.Task GetExternalIp(PublicAddressData publicAddressData)
    {
        var natDiscoverer = new Open.Nat.NatDiscoverer();
        var cancellationTokenSource = new CancellationTokenSource();
        //cancellationTokenSource.CancelAfter(5000);

        NatDevice natDevice = null;

        return natDiscoverer.DiscoverDeviceAsync(Open.Nat.PortMapper.Upnp, cancellationTokenSource)
        .ContinueWith(task =>
        {
            natDevice = task.Result;
            return natDevice.GetExternalIPAsync();
        })
        .Unwrap()
        .ContinueWith(task =>
        {
            publicAddressData.ipv4 = task.Result.ToString();
        });
    }
}
