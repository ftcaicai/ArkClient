using UnityEngine;
using System.Collections;

public class UIMessageBase : MonoBehaviour
{
    public virtual void ProcessUIMessage(UIMessageObject message)
    {
        Debug.Log("Process = " + message.messageType.ToString());
    }
}
