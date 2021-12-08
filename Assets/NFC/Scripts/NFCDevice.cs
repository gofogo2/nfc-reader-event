using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class NFCDevice : MonoBehaviour
{
    public string tagID;
    public bool tagFound = false;

    private AndroidJavaObject mActivity;
    private AndroidJavaObject mIntent;
    private string sAction;

    public delegate void TagReceiveEventHandler(string code);
    public static event TagReceiveEventHandler TagReceiveEventHandlerEvent;

    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            FindNfcTag();
        }
    }

    void FindNfcTag()
    {
        try
        {
            if (!tagFound)
            {
                // Create new NFC Android object
                mActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                mIntent = mActivity.Call<AndroidJavaObject>("getIntent");
                sAction = mIntent.Call<String>("getAction");
                if (sAction == "android.nfc.action.NDEF_DISCOVERED")
                {
                    Debug.Log("Tag of type NDEF");

                    var text = Test();
                    Debug.Log("TAG TEXT : " + text);

                    tagFound = true;
                    TagReceiveEventHandlerEvent(text);
                    Invoke("DeselectNFC", 2);

                }
                else if (sAction == "android.nfc.action.TAG_DISCOVERED")
                {
                    Debug.Log("This type of tag is not supported !");
                }
                else
                {
                    return;
                }
            }
        }
        catch (Exception e)
        {
            return;
        }
    }

    string Test()
    {
        AndroidJavaObject[] mNdefMessage = mIntent.Call<AndroidJavaObject[]>("getParcelableArrayExtra", "android.nfc.extra.NDEF_MESSAGES");
        AndroidJavaObject[] mNdefRecord = mNdefMessage[0].Call<AndroidJavaObject[]>("getRecords");
        byte[] payLoad = mNdefRecord[0].Call<byte[]>("getPayload");
        string text = System.Text.Encoding.UTF8.GetString(payLoad);
        return text;
    }

    void DeselectNFC()
    {
        mIntent.Call("removeExtra", "android.nfc.extra.TAG");
        mIntent.Call("removeExtra", "android.nfc.extra.NDEF_MESSAGES");
        tagFound = false;
    }
}