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

        private static void AddMonstersToRoundSpawner(RoundSpawner spawner) {
            if (!PhotonNetwork.IsMasterClient) return;
            Plugin.Logger.LogInfo("[ContentLib] Adding monsters to round spawner");
            List<IBudgetCost> list = new List<IBudgetCost>();
            int num = 0;
            foreach (Monsters.CustomMonster monster in Monsters.registeredMonsters) {
                float spawnWeight = monster.weight;
                if (UnityEngine.Random.value <= spawnWeight) {
                    IBudgetCost val = LoadMonster(monster);
                    list.Add(val);
                    num += val.Cost;
                }
            }
            spawner.testBudget += num;
            spawner.possibleSpawns = spawner.possibleSpawns.Concat(list.Select((IBudgetCost m) => m.gameObject)).ToArray();
        }

        private static IBudgetCost LoadMonster(Monsters.CustomMonster customMonster) {
            return customMonster.objectPrefab.GetComponent<IBudgetCost>();
        }

        private void AddShopItems() {

        }

        [HarmonyPatch(typeof(RoundSpawner))]
        [HarmonyPatch(nameof(RoundSpawner.Start))]
        [HarmonyPostfix]
        private static void Postfix_RoundSpawner_Start(RoundSpawner __instance) {
            Plugin.Logger.LogInfo("[ContentLib] RoundSpawner postfix called");
            AddMonstersToRoundSpawner(__instance);
        }

        /*[HarmonyPatch(nameof(ShopHandler.InitShop))]
        private static void Postfix_ShopHandler_InitShop(ShopHandler __instance) {
            //shopInitialized = true;
        }*/
    }
}
