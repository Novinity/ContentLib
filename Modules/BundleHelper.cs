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
    public static AssetBundle LoadAssetBundle(string filename, Assembly assembly) {
        Debug.Log($"Loading Asset Bundle {filename}");
        // Get the calling mod's DLL location
        string sAssemblyLocation = Path.GetDirectoryName(assembly.Location);
        // Load the bundle using the DLL location and passed file name
        AssetBundle loadedBundle = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, filename));
        // If the bundle doesn't exist
        if (loadedBundle == null) {
            Debug.Log($"Failed to load asset bundle {filename} [{sAssemblyLocation}]");
            return null;
        }
        return loadedBundle;
    }

    public static AssetBundle LoadEmbeddedAssetBundle(string resourceName, Assembly assembly) {
        Debug.Log($"Loading Embedded Bundle {resourceName}");
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            return AssetBundle.LoadFromStream(stream);
        }
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