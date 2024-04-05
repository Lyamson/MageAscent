using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerManager : NetworkBehaviour
{
    public int intValue = 0;
    public NetworkVariable<int> netIntValue = new(0);
    public override void OnNetworkSpawn()
    {
        MyDebug.Log(this, "ServerManager.OnNetworkSpawn Callback");
    }
}
