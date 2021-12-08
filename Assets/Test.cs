using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField]
    Text text;
    void Start()
    {
        NFCDevice.TagReceiveEventHandlerEvent += NFCDevice_TagReceiveEventHandlerEvent;
    }

    private void NFCDevice_TagReceiveEventHandlerEvent(string code)
    {
        text.text = code;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
