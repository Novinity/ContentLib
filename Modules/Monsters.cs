using DefaultNamespace;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.UI;

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
            Plugin.Logger.LogWarning($"[ContentLib] Attempted to register monster that is already registered! - Monster Name: {existing.name}");
            return null;
        }
        Plugin.Logger.LogInfo($"[ContentLib] Registering monster with name {monster.name}");

        // If the user hasn't passed the modName, add it ourself.
        if (monster.modName == "Unknown") {
            var callingAssembly = Assembly.GetCallingAssembly();
            var modDLL = callingAssembly.GetName().Name;
            monster.modName = modDLL;
        }

        // Run material fix up to allow monster to be rendered
        FixMaterials(monster.objectPrefab);

        // Add the monster object to the allowed network prefabs and add it to the registeredMonsters list
        ContentLoader.AddObjectToPool(monster.objectPrefab);
        registeredMonsters.Add(monster);

        // Pass back the monster
        return monster;
    }

    /// <summary>
    /// Register a single monster using it's prefab and weight
    /// </summary>
    /// <param name="objectPrefab"></param>
    /// <param name="weight"></param>
    /// <returns></returns>
    public static CustomMonster RegisterMonster(GameObject objectPrefab, float weight) {
        // If the monster has already been registered, don't continue.
        var existing = registeredMonsters.FirstOrDefault(x => x.objectPrefab == objectPrefab);
        if (existing != null) {
            Plugin.Logger.LogWarning($"[ContentLib] Attempted to register monster that is already registered! - Monster Name: {existing.name}");
            return null;
        }

        // Create a new CustomMonster object using the passed parameters
        CustomMonster monster = new CustomMonster(objectPrefab, weight);

        Plugin.Logger.LogInfo($"[ContentLib] Registering monster with name {objectPrefab.name}");

        // Set the modName of the custom monster
        var callingAssembly = Assembly.GetCallingAssembly();
        var modDLL = callingAssembly.GetName().Name;
        monster.modName = modDLL;

        // Run material fix up to allow monster to be rendered
        FixMaterials(monster.objectPrefab);

        // Add the monster object to the allowed network prefabs and add it to the registeredMonsters list
        ContentLoader.AddObjectToPool(objectPrefab);
        registeredMonsters.Add(monster);

        // Pass back the monster
        return monster;
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
        Material targetMaterial = new Material(Resources.Load<GameObject>("Zombe").GetComponentInChildren<SkinnedMeshRenderer>().material);
        // Go through all the renderers in the monster and set it's material to the M_Monster material
        foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
            renderer.material = targetMaterial;
    }

    /// <summary>
    /// Method to fix up all materials in a object
    /// </summary>
    /// <param name="gameObject"></param>
    private static void FixMaterials(GameObject gameObject) {
        foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>()) {
            // Loop through all materials in the current renderer
            for (int i = 0; i < renderer.materials.Length; i++) {
                // Create a new material using the shader name from the original, but not the actual shader it uses
                Material targetMaterial = new Material(Shader.Find(renderer.materials[i].shader.name));
                // Copy all properties from the original material to the new material
                targetMaterial.CopyMatchingPropertiesFromMaterial(renderer.materials[i]);
                // Set the renderer material at the current index to the newly created one
                renderer.materials[i] = targetMaterial;
            }
        }
    }

    // Methods to add custom materials to selected renderer(s) or gameobjects.

    /// <summary>
    /// Default method to just set the material of a single renderer
    /// </summary>
    /// <param name="renderer"></param>
    /// <param name="material"></param>
    public static void SetCustomMaterial(Renderer renderer, Material material) {
        // Create a new material using the shader name from the original, but not the actual shader it uses
        // This is done this way because for whatever reason shaders don't load from asset bundles properly
        // So we make an attempt to just use the same shader it uses
        Material targetMaterial = new Material(Shader.Find(material.shader.name));
        // Copy all properties from the passed material to the new material
        targetMaterial.CopyMatchingPropertiesFromMaterial(material);
        // Set the renderer's material to the new material
        renderer.material = targetMaterial;
    }

    /// <summary>
    /// Overload to set certain index of renderer materials
    /// </summary>
    /// <param name="renderer"></param>
    /// <param name="material"></param>
    /// <param name="index"></param>
    public static void SetCustomMaterial(Renderer renderer, Material material, int index) {
        // Create a new material using the shader name from the original, but not the actual shader it uses
        Material targetMaterial = new Material(Shader.Find(material.shader.name));
        // Copy all properties from the passed material to the new material
        targetMaterial.CopyMatchingPropertiesFromMaterial(material);
        // Set the renderer's material to the new material
        renderer.materials[index] = targetMaterial;
    }

    /// <summary>
    /// Overload to set the material of an array of renderers
    /// </summary>
    /// <param name="renderers"></param>
    /// <param name="material"></param>
    public static void SetCustomMaterial(Renderer[] renderers, Material material) {
        // Create a new material using the shader name from the original, but not the actual shader it uses
        Material targetMaterial = new Material(Shader.Find(material.shader.name));
        // Copy all properties from the passed material to the new material
        targetMaterial.CopyMatchingPropertiesFromMaterial(material);
        foreach (Renderer r in renderers)
            // Set the renderer's material to the new material
            r.material = targetMaterial;
    }

    /// <summary>
    /// Overload to set all renderers' materials under a gameobject
    /// </summary>
    /// <param name="monsterPrefab"></param>
    /// <param name="material"></param>
    public static void SetCustomMaterial(GameObject monsterPrefab, Material material) {
        // Create a new material using the shader name from the original, but not the actual shader it uses
        Material targetMaterial = new Material(Shader.Find(material.shader.name));
        // Copy all properties from the passed material to the new material
        targetMaterial.CopyMatchingPropertiesFromMaterial(material);
        foreach (Renderer renderer in monsterPrefab.GetComponentsInChildren<Renderer>())
            // Set the renderer's material to the new material
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
