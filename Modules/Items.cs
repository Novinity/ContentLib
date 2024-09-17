using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ContentLib.Modules;

public class Items {
    public static List<ArtifactItem> registeredArtifacts = new List<ArtifactItem>();

    public static void RegisterItemInDatabase(Item item) {
        Plugin.Logger.LogInfo($"[ContentLib] Adding item with ID {item.id} to ItemDatabase");
        ItemDatabase._instance.Objects.AddItem(item);
        foreach (Item i in ItemDatabase._instance.Objects)
            Plugin.Logger.LogInfo(i.id);
    }

    public static void RegisterArtifact(Item item) {
        var artifact = registeredArtifacts.FirstOrDefault(x => x.item == item);
        if (artifact != null) {
            Plugin.Logger.LogWarning($"[ContentLib] Attempted to register artifact that is already registered! - Item Name: {item.name}");
            return;
        }
        Plugin.Logger.LogInfo($"[ContentLib] Registering artifact with ID {item.id}");

        ArtifactItem artifactItem = new ArtifactItem(item);

        var callingAssembly = Assembly.GetCallingAssembly();
        var modDLL = callingAssembly.GetName().Name;
        artifactItem.modName = modDLL;

        registeredArtifacts.Add(artifactItem);
    }

    public class ArtifactItem {
        public Item item;

        public string modName = "Unknown";

        public ArtifactItem(Item item) {
            this.item = item;
        }
    }
}
