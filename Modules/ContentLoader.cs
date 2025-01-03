using DefaultNamespace;
using HarmonyLib;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ContentLib.Modules;

/// <summary>
/// Class that handles loading all custom monsters, items, and maps
/// </summary>
[HarmonyPatch]
public class ContentLoader {
    /// <summary>
    /// Method to add a given object to the Photon spawnable prefab pool
    /// This allows custom objects to be spawned on the network
    /// </summary>
    /// <param name="go"></param>
    public static void AddObjectToPool(GameObject go) {
        DefaultPool? pool = PhotonNetwork.PrefabPool as DefaultPool;
        if (pool != null) {
            pool.ResourceCache.Add(go.name, go);
        }
    }

    /// <summary>
    /// Method to add all registered monsters to the passed RoundSpawner, allowing them to spawn naturally
    /// </summary>
    /// <param name="spawner"></param>
    private static void AddMonstersToRoundSpawner(RoundSpawner spawner) {
        // If we aren't the master client (host), stop
        if (!PhotonNetwork.IsMasterClient) return;
        Debug.Log("[ContentLib] Adding monsters to round spawner");

        List<IBudgetCost> list = new List<IBudgetCost>();
        int num = 0;
        // Go through all the registered monsters
        foreach (Monsters.CustomMonster monster in Monsters.registeredMonsters) {
            float spawnWeight = monster.weight;
            // If the value is less than spawnWeight, add the monster to the list of spawns
            if (UnityEngine.Random.value <= spawnWeight) {
                IBudgetCost val = LoadMonster(monster);
                list.Add(val);
                num += val.Cost;
            }
        }
        // Add our final value to the budget and include the list of custom monsters in the possibleSpawns
        spawner.testBudget += num;
        spawner.possibleSpawns = spawner.possibleSpawns.Concat(list.Select((IBudgetCost m) => m.gameObject)).ToArray();
    }

    /// <summary>
    /// Method to return the passed monster's IBudgetCost component
    /// </summary>
    /// <param name="customMonster"></param>
    /// <returns></returns>
    private static IBudgetCost LoadMonster(Monsters.CustomMonster customMonster) {
        return customMonster.objectPrefab.GetComponent<IBudgetCost>();
    }
    
    /// <summary>
    /// Adds all registered items to the Item Database
    /// </summary>
    private static void AddItems() {
        foreach (Items.CustomItem item in Items.registeredItems) { 
            Items.RegisterItemInDatabase(item.item);
        }
    }

    /// <summary>
    /// Patch to call the AddMonstersToRoundSpawner method once it starts
    /// </summary>
    [HarmonyPatch(typeof(RoundSpawner))]
    [HarmonyPatch(nameof(RoundSpawner.Start))]
    [HarmonyPostfix]
    private static void Postfix_RoundSpawner_Start(RoundSpawner __instance) {
        Debug.Log("[ContentLib] RoundSpawner postfix called");
        AddMonstersToRoundSpawner(__instance);
    }
    
    /// <summary>
    /// Patch to add items to the shop
    /// </summary>
    [HarmonyPatch(typeof(ShopHandler))]
    [HarmonyPatch(nameof(ShopHandler.InitShopHandler))]
    [HarmonyPrefix]
    private static bool Postfix_ShopHandler_InitShop(ShopHandler __instance) {
        if (!Plugin.shopInitialized) {
            AddItems();
            Plugin.shopInitialized = true;
        }
        return true;
    }
    
    /// <summary>
    /// Method to fix up all materials in a object
    /// </summary>
    /// <param name="gameObject"></param>
    public static void FixMaterials(GameObject gameObject) {
        Logger.LogDebug("Fixing materials for " + gameObject.name);
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
}