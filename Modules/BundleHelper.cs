using System.IO;
using System.Reflection;
using UnityEngine;

namespace ContentLib.Modules;

/// <summary>
/// Class for easily loading asset bundles
/// </summary>
public class BundleHelper {
    /// <summary>
    /// Loads an asset bundle with the given filename relative to the calling mod's dll path
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static AssetBundle LoadAssetBundle(string filename) {
        Debug.Log($"Loading Asset Bundle {filename}");
        // Get the calling mod's DLL location
        string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        // Load the bundle using the DLL location and passed file name
        AssetBundle loadedBundle = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, filename));
        // If the bundle doesn't exist
        if (loadedBundle == null) {
            Debug.Log($"Failed to load asset bundle {filename} [{sAssemblyLocation}]");
            return null;
        }
        return loadedBundle;
    }

    /// <summary>
    /// Loads an asset bundle from the given path
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static AssetBundle LoadAssetBundleFromDirectPath(string path) {
        Debug.Log($"Loading Asset Bundle from path {path}");
        // Load the bundle using just the passed path
        AssetBundle loadedBundle = AssetBundle.LoadFromFile(path);
        // If the bundle doesn't exist
        if (loadedBundle == null) {
            Debug.Log($"Failed to load asset bundle {path}");
            return null;
        }
        return loadedBundle;
    }
}