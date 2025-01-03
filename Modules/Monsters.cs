using DefaultNamespace;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ContentLib.Modules;

/// <summary>
/// Class that handles the registration and loading of custom monsters
/// </summary>
public class Monsters {
    // The list of registered monsters from every mod
    public static List<CustomMonster> registeredMonsters = new List<CustomMonster>();

    /// <summary>
    /// Register a single monster using a instance of the CustomMonster
    /// class created by the user
    /// </summary>
    /// <param name="monster"></param>
    /// <returns></returns>
    public static CustomMonster RegisterMonster(CustomMonster monster) {
        // If the monster has already been registered, don't continue.
        var existing = registeredMonsters.FirstOrDefault(x => x == monster);
        if (existing != null) {
            Debug.Log($"[ContentLib] Attempted to register monster that is already registered! - Monster Name: {existing.name}");
            return null;
        }
        Debug.Log($"[ContentLib] Registering monster with name {monster.name}");

        // If the user hasn't passed the modName, add it ourself.
        if (monster.modName == "Unknown") {
            var callingAssembly = Assembly.GetCallingAssembly();
            var modDLL = callingAssembly.GetName().Name;
            monster.modName = modDLL;
        }

        // Run material fix up to allow monster to be rendered
        ContentLoader.FixMaterials(monster.objectPrefab);

        // Add the monster object to the allowed network prefabs and add it to the registeredMonsters list
        ContentLoader.AddObjectToPool(monster.objectPrefab);
        registeredMonsters.Add(monster);

        // Pass back the monster
        return monster;
    }

    /// <summary>
    /// Register a single monster using its prefab and weight
    /// </summary>
    /// <param name="objectPrefab"></param>
    /// <param name="weight"></param>
    /// <returns></returns>
    public static CustomMonster RegisterMonster(GameObject objectPrefab, float weight) {
        // If this monster object has already been registered, don't continue.
        var existing = registeredMonsters.FirstOrDefault(x => x.objectPrefab == objectPrefab);
        if (existing != null) {
            Debug.Log($"[ContentLib] Attempted to register monster prefab that is already registered! - Monster Name: {existing.name}, Prefab Name: {objectPrefab.name}");
            return null;
        }

        // Create a new CustomMonster object using the passed parameters
        CustomMonster monster = new CustomMonster(objectPrefab, weight);

        // Return the result of RegisterMonster using the CustomMonster object
        return RegisterMonster(monster);
    }

    /// <summary>
    /// Registers all found monsters in a given asset bundle using the passed weight
    /// </summary>
    /// <param name="assetBundle"></param>
    /// <param name="weight"></param>
    /// <returns></returns>
    public static CustomMonster[] RegisterAllInBundle(AssetBundle assetBundle, int weight = 1) {
        var assets = assetBundle.LoadAllAssets<GameObject>().Where(v => v.GetComponent<IBudgetCost>() != null);
        List<CustomMonster> tempRegistered = new List<CustomMonster>();

        foreach (var asset in assets) tempRegistered.Add(RegisterMonster(asset, weight));

        return tempRegistered.ToArray();
    }

    /// <summary>
    /// Method that can be called to set all renderer materials to the default monster material
    /// </summary>
    /// <param name="gameObject"></param>
    public static void UseMonsterMaterial(GameObject gameObject) {
        // Get the M_Monster material from one of the vanilla monsters
        Material targetMaterial = new Material(Resources.Load<GameObject>("Zombe")
            .GetComponentInChildren<SkinnedMeshRenderer>().material);
        // Go through all the renderers in the monster and set it's material to the M_Monster material
        foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
            renderer.material = targetMaterial;
    }

    /// <summary>
    /// A custom class for handling all necessary info for registering monsters
    /// </summary>
    public class CustomMonster {
        public GameObject objectPrefab;
        public float weight = 1;

        public string name = "";
        public string modName = "Unknown";

        public CustomMonster(GameObject objectPrefab, float weight) {
            this.weight = weight;
            this.objectPrefab = objectPrefab;
            name = objectPrefab.name;
        }
    }
}
