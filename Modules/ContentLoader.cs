﻿using DefaultNamespace;
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
    // private static void AddItems() {
    //     foreach (Items.CustomItem item in Items.registeredItems) { 
    //         Items.RegisterItemInDatabase(item.item);
    //     }
    // }

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
    // [HarmonyPatch(typeof(ShopHandler))]
    // [HarmonyPatch(nameof(ShopHandler.InitShopHandler))]
    // [HarmonyPrefix]
    // private static bool Postfix_ShopHandler_InitShop(ShopHandler __instance) {
    //     if (!Plugin.Instance.shopInitialized) {
    //         AddItems();
    //         Plugin.Instance.shopInitialized = true;
    //     }
    //     return true;
    // }
}