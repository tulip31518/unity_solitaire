using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationPauseWatcher : MonoBehaviour {

    public static Action OnAppPauseCB;

    public static void RegisterOnAppPauseCB(Action cb)
    {
        OnAppPauseCB += cb;
    }


    public static void UnregisterOnAppPauseCB(Action cb)
    {
        OnAppPauseCB -= cb;
    }

    private void OnApplicationPause(bool pause)
    {
       if(pause)
        {
                OnAppPauseCB.RunAction();
       }
    }

}
