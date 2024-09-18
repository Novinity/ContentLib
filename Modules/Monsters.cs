using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ContentLib.Modules;

public class Monsters {
    public static List<CustomMonster> registeredMonsters = new List<CustomMonster>();

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

    private static void FixMaterials(GameObject gameObject) {
        // Get the M_Monster material from one of the vanilla monsters
        Material targetMaterial = Resources.Load<GameObject>("Zombe").GetComponentInChildren<SkinnedMeshRenderer>().material;
        // Go through all the renderers in the monster and set it's material to the M_Monster material
        foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
            renderer.material = targetMaterial;
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            skinnedMeshRenderer.material = targetMaterial;
    }

    // TODO
    public static void SetCustomMaterial(Renderer renderer, Material material) {
        renderer.material = material;
    }

    public static void SetCustomMaterial(Renderer[] renderers, Material material) {
        foreach (Renderer r in renderers)
            r.material = material;
    }

    public static void SetCustomMaterial(GameObject monsterPrefab, Material material) {
        GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Plane);
        primitive.active = false;
        Material diffuse = primitive.GetComponent<MeshRenderer>().sharedMaterial;
        GameObject.DestroyImmediate(primitive);
        foreach (Renderer renderer in monsterPrefab.GetComponentsInChildren<Renderer>())
            renderer.material = diffuse;
    }

    // A custom class for handling all necessary info for registering monsters
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
