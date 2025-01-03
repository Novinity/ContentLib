using UnityEngine;

namespace ContentLib;

public class Logger
{
    static readonly string PREFIX_INFO = "[ContentLib]";
    static readonly string PREFIX_DEBUG = "[ContentLib_DEBUG]";
    
    public static void LogInfo(string message) {
        Debug.Log(PREFIX_INFO + ' ' + message);
    }
    public static void LogDebug(string message) {
        Debug.Log(PREFIX_DEBUG + ' ' + message);
    }
    public static void LogWarning(string message) {
        Debug.Log(PREFIX_INFO + "WARNING: " + message);
    }
    public static void LogError(string message) {
        Debug.Log(PREFIX_INFO + "ERROR: " + message);
    }
}