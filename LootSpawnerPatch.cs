using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace FishingBait;

public class LootSpawnerPatch
{
    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    public static class LootSpawnerPatcher
    {
        public static void Postfix(ZNetScene __instance)
        {
            if (!__instance) return;
            AddLootSpawnerItemDrops(__instance, "FishChumMeadowsSpawner", 1, 1, 1, "Fish1", "Fish2");
            AddLootSpawnerItemDrops(__instance, "FishChumBlackforestSpawner", 1, 1, 1, "Fish1", "Fish2", "Fish5");
            AddLootSpawnerItemDrops(__instance, "FishChumSwampsSpawner", 1, 1, 1, "Fish2", "Fish6");
            AddLootSpawnerItemDrops(__instance, "FishChumOceanSpawner", 1, 1, 1, "Fish3", "Fish8", "Fish12");
            AddLootSpawnerItemDrops(__instance, "FishChumMountainsSpawner", 1, 1, 1, "Fish4_cave");
            AddLootSpawnerItemDrops(__instance, "FishChumPlainsSpawner", 1, 1, 1, "Fish7", "Fish8");
            AddLootSpawnerItemDrops(__instance, "FishChumMistlandsSpawner", 1, 1, 1, "Fish9", "Fish12");
            AddLootSpawnerItemDrops(__instance, "FishChumAshlandsSpawner", 1, 1, 1, "Fish11", "Fish12");
            AddLootSpawnerItemDrops(__instance, "FishChumDeepnorthSpawner", 1, 1, 1, "Fish10", "Fish12");
        }
    }

    private static void AddLootSpawnerItemDrops(
        ZNetScene scene,
        string spawnerPrefabName,
        int stackMinValue = 1,
        int stackMaxValue= 1,
        float weightValue = 1,
        params string[] itemPrefabNames
    )
    {
        var spawnerPrefabLootSpawnerScript = scene.GetPrefab(spawnerPrefabName).GetComponent<LootSpawner>();
        if (!spawnerPrefabLootSpawnerScript) return;
        
        foreach (var itemPrefabName in itemPrefabNames)
        {
            GameObject gameObject = scene.GetPrefab(itemPrefabName);
            if (!gameObject) return;
            
            DropTable.DropData dropData = new DropTable.DropData
            {
                m_item = gameObject,
                m_weight = weightValue,
                m_stackMax = stackMaxValue,
                m_stackMin = stackMinValue
            };
            spawnerPrefabLootSpawnerScript.m_items.m_drops.Add(dropData);
        }
        spawnerPrefabLootSpawnerScript.m_respawnTimeMinuts = 1;
    }
    
    [HarmonyPatch(typeof(LootSpawner),nameof(LootSpawner.UpdateSpawner))]
    static class LootSpawnerUpdateSpawnerPatch
    {
        static bool Prefix(LootSpawner __instance)
        {
            if (!__instance) return false;
            var objName = __instance.name;
            var parentName = getSetPrefabSharedName(objName);
            switch (Heightmap.FindBiome(__instance.transform.position))
            {
                case Heightmap.Biome.None:
                    return false;
                case Heightmap.Biome.Meadows:
                    if (objName == "FishChumMeadowsSpawner(Clone)") return true;
                    break;
                case Heightmap.Biome.Swamp:
                    if (objName == "FishChumSwampsSpawner(Clone)") return true;
                    break;
                case Heightmap.Biome.Mountain:
                    if (objName == "FishChumMountainsSpawner(Clone)") return true;
                    break;
                case Heightmap.Biome.BlackForest:
                    if (objName == "FishChumBlackforestSpawner(Clone)") return true;
                    break;
                case Heightmap.Biome.Plains:
                    if (objName == "FishChumPlainsSpawner(Clone)") return true;
                    break;
                case Heightmap.Biome.AshLands:
                    if (objName == "FishChumAshlandsSpawner(Clone)") return true;
                    break;
                case Heightmap.Biome.DeepNorth:
                    if (objName == "FishChumDeepnorthSpawner(Clone)") return true;
                    break;
                case Heightmap.Biome.Ocean:
                    if (objName == "FishChumOceanSpawner(Clone)") return true;
                    break;
                case Heightmap.Biome.Mistlands:
                    if (objName == "FishChumMistlandsSpawner(Clone)") return true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, $"{parentName} is not attracting any fish in this area");
            return false;
        }

        private static string getSetPrefabSharedName(string objName)
        {
            switch (objName)
            {
                case "FishChumMeadowsSpawner(Clone)":
                    return getItemDropSharedName(ZNetScene.instance, "FishChumMeadows");
                case "FishChumBlackforestSpawner(Clone)":
                    return getItemDropSharedName(ZNetScene.instance, "FishChumBlackforest");
                case "FishChumSwampsSpawner(Clone)":
                    return getItemDropSharedName(ZNetScene.instance, "FishChumSwamps");
                case "FishChumMountainsSpawner(Clone)":
                    return getItemDropSharedName(ZNetScene.instance, "FishChumMountains");
                case "FishChumOceanSpawner(Clone)":
                    return getItemDropSharedName(ZNetScene.instance, "FishChumOcean");
                case "FishChumPlainsSpawner(Clone)":
                    return getItemDropSharedName(ZNetScene.instance, "FishChumPlains");
                case "FishChumMistlandsSpawner(Clone)":
                    return getItemDropSharedName(ZNetScene.instance, "FishChumMistlands");
                case "FishChumAshlandsSpawner(Clone)":
                    return getItemDropSharedName(ZNetScene.instance, "FishChumAshlands");
                case "FishChumDeepnorthSpawner(Clone)":
                    return getItemDropSharedName(ZNetScene.instance, "FishChumDeepnorth");
                default:
                    return "Fish Chum";
                    ;
            }
        }

        private static string getItemDropSharedName(ZNetScene scene, string gameObjectName)
        {
            var gameObjectItemDrop = scene.GetPrefab(gameObjectName).GetComponent<ItemDrop>();
            return gameObjectItemDrop.m_itemData.m_shared.m_name;
        }
    }
}