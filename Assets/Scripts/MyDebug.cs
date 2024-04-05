using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

public class MyDebug : MonoBehaviour
{
    public static void Log(NetworkBehaviour sender, string messageBefore, string messageAfter = null)
    {
        StringBuilder message = new StringBuilder();
        message.Append(messageBefore).Append($"IsLocal: {sender.IsLocalPlayer}\tIsOwner: {sender.IsOwner}\tIsServer: {sender.IsServer}\tIsHost: {sender.IsHost}");
        if (messageAfter != null)
        {
            message.Append(messageAfter);
        }
        Debug.Log(message, sender);
    }
    public static void Log(NetworkObject sender, string messageBefore, string messageAfter = null)
    {
        StringBuilder message = new StringBuilder();
        message.Append(messageBefore).Append($"IsLocal: {sender.IsLocalPlayer}\tIsOwner: {sender.IsOwner}");
        if (messageAfter != null)
        {
            message.Append(messageAfter);
        }
        Debug.Log(message, sender);
    }
}
