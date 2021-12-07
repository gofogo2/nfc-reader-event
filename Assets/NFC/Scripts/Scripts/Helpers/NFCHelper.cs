using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NFCHelper : MonoBehaviour
{
    private static NFCHelper instance = null;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static NFCHelper Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    public bool tagFound = false;
    private AndroidJavaObject mActivity;
    private AndroidJavaObject mIntent;
    private string sAction;

    public delegate void TagReceiveEventHandler(string code);
    public event TagReceiveEventHandler TagReceiveEventHandlerEvent;

    public void Run()
    {
        if (IsInvoking("NfcRun"))
        {
            CancelInvoke("NfcRun");
        }
        InvokeRepeating("NfcRun", 0.0f, 0.01f);
    }

    void NfcRun()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            FindNfcTag();
        }
    }

    void FindNfcTag()
    {
        if (!tagFound)
        {
            // Create new NFC Android object
            mActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            mIntent = mActivity.Call<AndroidJavaObject>("getIntent");
            sAction = mIntent.Call<String>("getAction");
            if (sAction == "android.nfc.action.NDEF_DISCOVERED")
            {
                tagFound = true;
                string text = GetPayload();
                TagReceiveEventHandlerEvent(text);
                Invoke("DeselectNFC", 0.5f);
            }
            else if (sAction == "android.nfc.action.TECH_DISCOVERED")
            {
                tagFound = true;
                string text = GetPayload();
                TagReceiveEventHandlerEvent(text);
                Invoke("DeselectNFC", 0.5f);
            }
            else if (sAction == "android.nfc.action.TAG_DISCOVERED")
            {
                Debug.Log("This type of tag is not supported !");
            }
        }
    }

    void DeselectNFC()
    {
        mIntent.Call("removeExtra", "android.nfc.extra.TAG");
        mIntent.Call("removeExtra", "android.nfc.extra.NDEF_MESSAGES");
        tagFound = false;
    }

    string GetPayload()
    {
        AndroidJavaObject[] mNdefMessage = mIntent.Call<AndroidJavaObject[]>("getParcelableArrayExtra", "android.nfc.extra.NDEF_MESSAGES");
        AndroidJavaObject[] mNdefRecord = mNdefMessage[0].Call<AndroidJavaObject[]>("getRecords");
        byte[] payLoad = mNdefRecord[0].Call<byte[]>("getPayload");
        string text = System.Text.Encoding.UTF8.GetString(payLoad);
        return text;
    }
}
