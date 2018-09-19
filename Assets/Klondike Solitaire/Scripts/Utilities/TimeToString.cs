using UnityEngine;
using System.Collections;

public class TimeToString : MonoBehaviour {
    public static string GetTimeStringMSFormat(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        string niceTime = string.Format("{0:00}:{1:00}", minutes, seconds);
        return niceTime;
    }

    public static string GetLongTimeString(float time)
    {
        if (time > 86400f)
            return GetTimeStringDHFormat(time);
        return GetTimeStringHMSFormat(time);
    }

    public static string GetTimeStringHMSFormat(float time)
    {
        int hours = Mathf.FloorToInt(time / 3600F);
        int minutes = Mathf.FloorToInt((time - hours * 3600) / 60F);
        int seconds = Mathf.FloorToInt(time - hours * 3600 - minutes * 60);
        string niceTime = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        return niceTime;
    }

    public static string GetTimeStringDHFormat(float time)
    {
        int days = Mathf.FloorToInt(time / 86400F);
        int hours = Mathf.FloorToInt((time - days * 86400) / 3600F);
        string niceTime = days + "d " + hours + "h";
        return niceTime;
    }
}
