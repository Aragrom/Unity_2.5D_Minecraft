using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class LocalAddresses : MonoBehaviour
{
    public System.Collections.Generic.List<string> localAddresses;

    [BurstCompile]
    public void GetLocalDnsData()
    {
        // Find host by name (name of device) - 3rd element is IPv4 ip address

        System.Net.IPHostEntry ipHostEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());

        for (int index = 0; index < ipHostEntry.AddressList.Length; index++)
        {
            localAddresses.Add(ipHostEntry.AddressList[index].ToString());
        }

        // Debug Read out ==============================================

        string internetPropetiesReadOut = "";

        internetPropetiesReadOut += "Host Name - " + ipHostEntry.HostName + " / ";

        foreach (System.Net.IPAddress ipAddress in ipHostEntry.AddressList)
        {
            internetPropetiesReadOut += "IpAddress - " + ipAddress.ToString() + " / ";
        }

        foreach (string alias in ipHostEntry.Aliases)
        {
            internetPropetiesReadOut += "Alias - " + alias + " / ";
        }

        UnityEngine.Debug.Log("Internet Properties - " + internetPropetiesReadOut);

        // ============================================================
    }
}
