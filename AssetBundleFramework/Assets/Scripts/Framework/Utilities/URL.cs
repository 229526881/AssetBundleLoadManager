using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class URL
{
    public static string GetPlatformStr()
    {
        string platformStr = "Android";
#if UNITY_ANDROID
        platformStr="Android";
#elif UNITY_IOS
        platformStr="IOS";
#endif
        return platformStr;
    }
}
