using DefaultNamespace;
using HarmonyLib;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ContentLib.Modules {
    [HarmonyPatch]
    public class ContentLoader {
        public static void AddObjectToPool(GameObject go) {
            DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
            if (pool != null) {
                pool.ResourceCache.Add(go.name, go);
            }
        }

        // Method to add all registered monsters to the passed RoundSpawner, allowing them to spawn naturally
        private static void AddMonstersToRoundSpawner(RoundSpawner spawner) {
            // If we aren't the master client (host), stop
            if (!PhotonNetwork.IsMasterClient) return;
            Plugin.Logger.LogInfo("[ContentLib] Adding monsters to round spawner");

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
            // Add our final value to the budges and include the list of custom monsters in the possibleSpawns
            spawner.testBudget += num;
            spawner.possibleSpawns = spawner.possibleSpawns.Concat(list.Select((IBudgetCost m) => m.gameObject)).ToArray();
        }

        // Method to return the passed monster's IBudgetCost component
        private static IBudgetCost LoadMonster(Monsters.CustomMonster customMonster) {
            return customMonster.objectPrefab.GetComponent<IBudgetCost>();
        }

        // TODO
        private void AddShopItems() {

        }

        // Patch to call the AddMonstersToRoundSpawner method once it starts
        [HarmonyPatch(typeof(RoundSpawner))]
        [HarmonyPatch(nameof(RoundSpawner.Start))]
        [HarmonyPostfix]
        private static void Postfix_RoundSpawner_Start(RoundSpawner __instance) {
            Plugin.Logger.LogInfo("[ContentLib] RoundSpawner postfix called");
            AddMonstersToRoundSpawner(__instance);
        }

        // TODO
        /*[HarmonyPatch(nameof(ShopHandler.InitShop))]
        private static void Postfix_ShopHandler_InitShop(ShopHandler __instance) {
            //shopInitialized = true;
        }*/
    }
}
