using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ContentLib.Modules;

public class Monsters {
    public static List<CustomMonster> registeredMonsters = new List<CustomMonster>();

    public static CustomMonster RegisterMonster(CustomMonster monster) {
        var existing = registeredMonsters.FirstOrDefault(x => x == monster);
        if (existing != null) {
            Plugin.Logger.LogWarning($"[ContentLib] Attempted to register monster that is already registered! - Monster Name: {existing.name}");
            return null;
        }
        Plugin.Logger.LogInfo($"[ContentLib] Registering monster with name {monster.name}");

        if (monster.modName == "Unknown") {
            var callingAssembly = Assembly.GetCallingAssembly();
            var modDLL = callingAssembly.GetName().Name;
            monster.modName = modDLL;
        }

        FixMaterials(monster.objectPrefab);

        ContentLoader.AddObjectToPool(monster.objectPrefab);
        registeredMonsters.Add(monster);

        return monster;
    }

    public static CustomMonster RegisterMonster(GameObject objectPrefab, float weight) {
        var existing = registeredMonsters.FirstOrDefault(x => x.objectPrefab == objectPrefab);
        if (existing != null) {
            Plugin.Logger.LogWarning($"[ContentLib] Attempted to register monster that is already registered! - Monster Name: {existing.name}");
            return null;
        }

        CustomMonster monster = new CustomMonster(objectPrefab, weight);


        Plugin.Logger.LogInfo($"[ContentLib] Registering monster with name {objectPrefab.name}");

        var callingAssembly = Assembly.GetCallingAssembly();
        var modDLL = callingAssembly.GetName().Name;
        monster.modName = modDLL;

        FixMaterials(monster.objectPrefab);

        ContentLoader.AddObjectToPool(objectPrefab);
        registeredMonsters.Add(monster);

        return monster;
    }

    private static void FixMaterials(GameObject gameObject) {
        Material targetMaterial = Resources.Load<GameObject>("Zombe").GetComponentInChildren<SkinnedMeshRenderer>().material;
        foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
            renderer.material = targetMaterial;
    }

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
