using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ContentLib.Modules;

public class BundleHelper {
    public static AssetBundle LoadAssetBundle(string filename) {
        Plugin.Logger.LogDebug($"Loading Asset Bundle {filename}");
        string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        AssetBundle loadedBundle = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, filename));
        if (loadedBundle == null) {
            Plugin.Logger.LogError($"Failed to load asset bundle {filename} [{sAssemblyLocation}]");
            return null;
        }
        return loadedBundle;
    }

    public static AssetBundle LoadAssetBundleFromDirectPath(string path) {
        Plugin.Logger.LogDebug($"Loading Asset Bundle from path {path}");
        AssetBundle loadedBundle = AssetBundle.LoadFromFile(path);
        if (loadedBundle == null) {
            Plugin.Logger.LogError($"Failed to load asset bundle {path}");
            return null;
        }
        return loadedBundle;
    }
}