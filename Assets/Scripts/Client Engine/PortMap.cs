using Open.Nat;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Burst;
using UnityEngine;

// class for pointer (reference)
[BurstCompile]
[System.Serializable]
public class PortMapData
{
    public List<string> openDotNatResult = new List<string>();
    public string openDotNatStatus = null;
    public bool doneNatTesting = false;
}

[BurstCompile]
public class PortMap : MonoBehaviour
{
    public PortMapData portMapData = new PortMapData();

    // Ip and port we are trying to use to open and close port map.
    public string ipAddress = "";
    public int port = -1;

    public static System.Threading.Tasks.Task GetPortMaps(PortMapData portMapData)
    {
        portMapData.openDotNatResult = new List<string>();

        var natDiscoverer = new Open.Nat.NatDiscoverer();
        var cancellationTokenSource = new CancellationTokenSource();
        //cancellationTokenSource.CancelAfter(5000);

        NatDevice natDevice = null;
        var stringBuilder = new StringBuilder();

        portMapData.openDotNatStatus = "Discovering Device...";

        return natDiscoverer.DiscoverDeviceAsync(Open.Nat.PortMapper.Upnp, cancellationTokenSource)
        .ContinueWith(task =>
        {
            natDevice = task.Result;
            portMapData.openDotNatStatus = "Getting all port maps...";
            return natDevice.GetAllMappingsAsync();
        })
        .Unwrap()
        .ContinueWith(task =>
        {
            portMapData.openDotNatStatus = "Looping all maps...";

            foreach (var mapping in task.Result)
            {
                stringBuilder.AppendFormat("{0} {1} {2} {3} {4} {5}",
                    mapping.PublicPort,
                    mapping.PrivateIP,
                    mapping.PrivatePort,
                    mapping.Description,
                    mapping.Protocol == Open.Nat.Protocol.Tcp ? "TCP" : "UDP",
                    mapping.Expiration.ToLocalTime());

                portMapData.openDotNatResult.Add(stringBuilder.ToString());
                stringBuilder = new StringBuilder();
            }

            portMapData.openDotNatStatus = "Done getting maps.";

            portMapData.doneNatTesting = true;
        });
    }

    public static System.Threading.Tasks.Task CreatePortMap(int port, string internalIpAddress, PortMapData portMapData)
    {
        portMapData.openDotNatResult = new List<string>();

        portMapData.openDotNatStatus = "Initializing...";

        IPAddress ip;

        IPAddress.TryParse(internalIpAddress, out ip);

        var natDiscoverer = new Open.Nat.NatDiscoverer();
        var cancellationTokenSource = new CancellationTokenSource();
        //cancellationTokenSource.CancelAfter(5000);

        NatDevice natDevice = null;
        var stringBuilder = new StringBuilder();

        bool tcpOpened = false;
        bool udpOpened = false;

        portMapData.openDotNatStatus = "Discovering Device...";

        return natDiscoverer.DiscoverDeviceAsync(Open.Nat.PortMapper.Upnp, cancellationTokenSource)
        .ContinueWith(task =>
        {
            natDevice = task.Result;
            portMapData.openDotNatStatus = "Creating new port map - TCP...";
            return natDevice.CreatePortMapAsync(new Open.Nat.Mapping(Open.Nat.Protocol.Tcp, ip, port, port, 0, "Chess - TCP"));
        })
        .Unwrap()
        .ContinueWith(task =>
        {
            portMapData.openDotNatStatus = "Creating new port map - UDP...";
            return natDevice.CreatePortMapAsync(new Open.Nat.Mapping(Open.Nat.Protocol.Udp, ip, port, port, 0, "Chess - UDP"));
        })
        .Unwrap()
        .ContinueWith(task =>
        {
            portMapData.openDotNatStatus = "Getting all port maps...";
            return natDevice.GetAllMappingsAsync();
        })
        .Unwrap()
        .ContinueWith(task =>
        {
            // NAT devices' mapping table is a scarce resource and that
            // means we cannot have an infinite number of mappings.
            // These tables have a limit, some have 10, 16, 32 and even
            // more entries and then, the question is: how should the 
            // NAT response once the its limit is reached? Well, that's
            // the problem, the specs do not say anything about this so,
            // some NATs just accept the addNewPortMapping request and
            // response with status 200 OK and for that reason the clients,
            // like Open.NAT, have no cheap way to know about that fact.

            // Workaround:

            // Clients can verify that the new requested port mappings
            // have been append to the mapping table by requesting the full
            // list of port mappings and check those ports are listed.

            portMapData.openDotNatStatus = "Looping all maps...";

            foreach (var mapping in task.Result)
            {
                stringBuilder.AppendFormat("{0} {1} {2} {3} {4} {5}",
                    mapping.PublicPort,
                    mapping.PrivateIP,
                    mapping.PrivatePort,
                    mapping.Description,
                    mapping.Protocol == Open.Nat.Protocol.Tcp ? "TCP" : "UDP",
                    mapping.Expiration.ToLocalTime());

                portMapData.openDotNatResult.Add(stringBuilder.ToString());
                stringBuilder = new StringBuilder();

                if (mapping.PublicPort == port)
                {
                    if (mapping.Protocol == Open.Nat.Protocol.Tcp)
                    {
                        tcpOpened = true;
                    }
                    else
                    {
                        udpOpened = true;
                    }
                }
            }

            if (tcpOpened && udpOpened)
            {
                portMapData.openDotNatStatus = "Success Opening Port.";
            }
            else
            {
                portMapData.openDotNatStatus = "Failed Opening Port.";
            }

            portMapData.doneNatTesting = true;
        });
    }

    public static System.Threading.Tasks.Task DeletePortMap(int port, PortMapData portMapData)
    {
        portMapData.openDotNatResult = new List<string>();

        portMapData.openDotNatStatus = "Initializing...";

        var natDiscoverer = new Open.Nat.NatDiscoverer();
        var cancellationTokenSource = new CancellationTokenSource();
        //cancellationTokenSource.CancelAfter(5000);

        NatDevice natDevice = null;
        var stringBuilder = new StringBuilder();

        bool tcpOpened = false;
        bool udpOpened = false;

        portMapData.openDotNatStatus = "Discovering Device...";

        return natDiscoverer.DiscoverDeviceAsync(Open.Nat.PortMapper.Upnp, cancellationTokenSource)
        .ContinueWith(task =>
        {
            natDevice = task.Result;
            portMapData.openDotNatStatus = "Deleting port map - TCP...";
            return natDevice.DeletePortMapAsync(new Open.Nat.Mapping(Open.Nat.Protocol.Tcp, port, port));
        })
        .Unwrap()
        .ContinueWith(task =>
        {
            portMapData.openDotNatStatus = "Deleting port map - UDP...";
            return natDevice.DeletePortMapAsync(new Open.Nat.Mapping(Open.Nat.Protocol.Udp, port, port));
        })
        .Unwrap()
        .ContinueWith(task =>
        {
            portMapData.openDotNatStatus = "Getting all port maps...";
            return natDevice.GetAllMappingsAsync();
        })
        .Unwrap()
        .ContinueWith(task =>
        {
            portMapData.openDotNatStatus = "Looping all maps...";

            foreach (var mapping in task.Result)
            {
                stringBuilder.AppendFormat("{0} {1} {2} {3} {4} {5}",
                    mapping.PublicPort,
                    mapping.PrivateIP,
                    mapping.PrivatePort,
                    mapping.Description,
                    mapping.Protocol == Open.Nat.Protocol.Tcp ? "TCP" : "UDP",
                    mapping.Expiration.ToLocalTime());

                portMapData.openDotNatResult.Add(stringBuilder.ToString());
                stringBuilder = new StringBuilder();

                if (mapping.PublicPort == port)
                {
                    if (mapping.Protocol == Open.Nat.Protocol.Tcp)
                    {
                        tcpOpened = true;
                    }
                    else
                    {
                        udpOpened = true;
                    }
                }
            }

            if (!tcpOpened && !udpOpened)
            {
                portMapData.openDotNatStatus = "Success Closing Port.";
            }
            else
            {
                portMapData.openDotNatStatus = "Failed Closing Port.";
            }

            portMapData.doneNatTesting = true;
        });
    }
}
