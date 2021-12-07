using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class NFCDevice : MonoBehaviour
{
    [SerializeField]
    public Text tag_output_text;

    private void Start()
    {
        NFCHelper.Instance.TagReceiveEventHandlerEvent += Instance_TagReceiveEventHandlerEvent;
        NFCHelper.Instance.Run();
    }

    private void Instance_TagReceiveEventHandlerEvent(string code)
    {
        tag_output_text.text = code;
    }
}