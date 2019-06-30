using Fungus;
using UnityEngine;

/// <summary>
/// アセット「Fungus」用。OnTriggerEnter でイベントを開始させる例
/// </summary>
public class Sendmessage : MonoBehaviour
{
    private void OnTriggerEnter ( Collider other )
    {
        if ( !other.gameObject.CompareTag ( "Robot" ) ) return;

        var messageReceived = GameObject.FindObjectOfType<MessageReceived>();
        messageReceived?.OnSendFungusMessage ("itai" );
    }
}