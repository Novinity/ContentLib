using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Zorro.Core;

namespace ContentLib.Modules;

// TODO
public class Items {
    public static List<CustomItem> registeredItems = new List<CustomItem>();

    /// <summary>
    /// Registers the given item in the game's item database
    /// </summary>
    /// <param name="item"></param>
    public static void RegisterItemInDatabase(Item item) {
        ContentLib.Logger.LogDebug($"Adding item with ID {item.id} to ItemDatabase");
        // Get the current instance of the ItemDatabase
        ItemDatabase db = SingletonAsset<ItemDatabase>.Instance;
        // Create a copy of the registered item array as a list
        List<Item> items = db.Objects;
        // Add the new item to the list
        items.Add(item);
        // Give the list back to the ItemDatabase as an array
        db.Objects = items;
    }

    /// <summary>
    /// Register the given item
    /// </summary>
    /// <param name="item"></param>
    public static void RegisterItem(Item item) {
        // Check if the item is already registered
        var artifact = registeredItems.FirstOrDefault(x => x.item == item);
        if (artifact != null) {
           ContentLib.Logger.LogWarning($"Attempted to register item that is already registered! - Item Name: {item.name}");
            return;
        }
        ContentLib.Logger.LogDebug($"Registering item with ID {item.id}");

        // Create a new CustomItem object using the passed item
        CustomItem customItem = new CustomItem(item);

        // Set the modName of the custom item
        var callingAssembly = Assembly.GetCallingAssembly();
        var modDLL = callingAssembly.GetName().Name;
        customItem.modName = modDLL;

        ContentLoader.FixMaterials(item.itemObject);
        ContentLoader.AddObjectToPool(item.itemObject);

        // Add the item to the registeredItems list
        registeredItems.Add(customItem);
    }

    /// <summary>
    /// Custom class to identify registered items
    /// </summary>
    public class CustomItem {
        public Item item;

        public string modName = "Unknown";

        public CustomItem(Item item) {
            this.item = item;
        }
    }
}
