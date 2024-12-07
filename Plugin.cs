using HarmonyLib;
using System.Security.Permissions;
using UnityEngine;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
namespace ContentLib;

[ContentWarningPlugin("novinity.ContentLib", "1.0.1", false)]
public class Plugin {
    public static Plugin Instance { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }

    public bool shopInitialized = false;

    private void Awake() {
        Instance = this;

        Patch();

        Debug.Log($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    internal static void Patch() {
        Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

        Debug.Log("Patching...");

        Harmony.PatchAll();

        Debug.Log("Finished patching!");
    }

    internal static void Unpatch() {
        Debug.Log("Unpatching...");

        Harmony?.UnpatchSelf();

        Debug.Log("Finished unpatching!");
    }
}