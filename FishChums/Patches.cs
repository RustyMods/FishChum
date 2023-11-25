using System;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace FishChums;

public static class Patches
{
    [HarmonyPatch(typeof(LootSpawner), nameof(LootSpawner.UpdateSpawner))]
    static class LootSpawnerUpdateSpawnerPatch
    {
        static bool Prefix(LootSpawner __instance)
        {
            if (!__instance) return false;
            string objName = __instance.name;
            string parentName = getSetPrefabSharedName(objName);
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

            MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft,
                $"{parentName} is not attracting any fish in this area");
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
            }
        }
        
        private static string getItemDropSharedName(ZNetScene scene, string gameObjectName)
        {
            ItemDrop? gameObjectItemDrop = scene.GetPrefab(gameObjectName).GetComponent<ItemDrop>();
            return gameObjectItemDrop.m_itemData.m_shared.m_name;
        }
    }

    [HarmonyPatch(typeof(SpawnArea), nameof(SpawnArea.UpdateSpawn))]
    static class SpawnAreaUpdateSpawnPatch
    {
        static bool Prefix(SpawnArea __instance)
        {
            if (!__instance) return false;
            string objName = __instance.name;
            if (isSerpentChumSpawner(objName))
            {
                switch (Heightmap.FindBiome(__instance.transform.position))
                {
                    case Heightmap.Biome.Ocean:
                        return true;
                    default:
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft,
                            "Serpent Chum doesn't work in shallow waters");
                        return false;
                }
            }

            if (objName == "leviathanChumSpawner(Clone)")
            {
                switch (Heightmap.FindBiome(__instance.transform.position))
                {
                    case Heightmap.Biome.Ocean:
                        return true;
                    default:
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft,
                            "Leviathan Chum doesn't work in shallow waters");
                        return false;
                }
            }

            return true;
        }
        
        private static bool isSerpentChumSpawner(string gameObjectName)
        {
            return gameObjectName == "SerpentChumSpawner(Clone)";
        }
    }

    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    static class OvenPatch
    {
        public static void Postfix(ZNetScene __instance)
        {
            if (!__instance) return;
            AddCookingStationConversion(__instance, "TrollfishMeat", "TrollfishMeatCooked", 25f,
                "piece_cookingstation", "piece_cookingstation_iron");
            AddCookingStationConversion(__instance, "FishBallUncooked", "FishBallCooked", 10f, "piece_oven");
            AddCookingStationConversion(__instance, "CoralDelightUncooked", "CoralDelightCooked", 10f,
                "piece_oven");
            AddCookingStationConversion(__instance, "BakedSalmonUncooked", "BakedSalmonCooked", 10f, "piece_oven");
            AddCookingStationConversion(__instance, "BakedMagmaFishUncooked", "BakedMagmaFishCooked", 10f,
                "piece_oven");
        }
        
        private static void AddCookingStationConversion(ZNetScene scene, string fromItemName, string toItemName,
            float cookTime, params string[] stationNames)
        {
            GameObject fromItem = scene.GetPrefab(fromItemName);
            if (!fromItem) return;

            GameObject toItem = scene.GetPrefab(toItemName);
            if (!toItem) return;

            foreach (var stationName in stationNames)
            {
                GameObject station = scene.GetPrefab(stationName);
                if (!station) continue;

                var stationScript = station.GetComponent<CookingStation>();
                var itemData = new CookingStation.ItemConversion
                {
                    m_from = fromItem.GetComponent<ItemDrop>(),
                    m_to = toItem.GetComponent<ItemDrop>(),
                    m_cookTime = cookTime
                };
                stationScript.m_conversion.Add(itemData);
            }
        }
    }
    
    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    public static class LootSpawnerPatch
    {
        public static void Postfix(ZNetScene __instance)
        {
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
        
        private static void AddLootSpawnerItemDrops(
            ZNetScene scene,
            string spawnerPrefabName,
            int stackMinValue = 1,
            int stackMaxValue = 1,
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
    }
    
    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    public static class SpawnAreaPatch
    {
        public static void Postfix(ZNetScene __instance)
        {
            if (!__instance) return;
            AddSpawnAreaSpawnData(__instance, "SerpentChumSpawner", "Serpent", 1, 1, 2, 10);
            AddSpawnAreaSpawnData(__instance, "leviathanChumSpawner", "Leviathan", 1, 1, 1, 40);
        }
        
        private static void AddSpawnAreaSpawnData(
            ZNetScene scene,
            string spawnerAreaPrefabName,
            string gameObjectName,
            float weight = 1f,
            int minLevel = 1,
            int maxLevel = 1,
            int spawnIntervalSec = 30,
            int maxNear = 1,
            int maxTotal = 1
        )
        {
            if (!scene) return;
            SpawnArea gameObjectSpawnAreaScript = scene.GetPrefab(spawnerAreaPrefabName).GetComponent<SpawnArea>();
            if (!gameObjectSpawnAreaScript) return;
            GameObject? gameObject = scene.GetPrefab(gameObjectName);
            if (!gameObject) return;

            SpawnArea.SpawnData gameObjectData = new SpawnArea.SpawnData
            {
                m_prefab = gameObject,
                m_weight = weight,
                m_minLevel = minLevel,
                m_maxLevel = maxLevel
            };
            gameObjectSpawnAreaScript.m_prefabs.Add(gameObjectData);
            // SPAWN AREA SCRIPT SETTINGS
            gameObjectSpawnAreaScript.m_levelupChance = 0;
            gameObjectSpawnAreaScript.m_spawnIntervalSec = spawnIntervalSec;
            gameObjectSpawnAreaScript.m_triggerDistance = 50;
            gameObjectSpawnAreaScript.m_setPatrolSpawnPoint = true;
            gameObjectSpawnAreaScript.m_spawnRadius = 25;
            gameObjectSpawnAreaScript.m_nearRadius = 20;
            gameObjectSpawnAreaScript.m_farRadius = 1000;
            gameObjectSpawnAreaScript.m_maxNear = maxNear;
            gameObjectSpawnAreaScript.m_maxTotal = maxTotal;
            gameObjectSpawnAreaScript.m_onGroundOnly = false;
        }
    }

    [HarmonyPatch(typeof(SpawnArea), nameof(SpawnArea.SpawnOne))]
    static class SpawnAreaSpawnOnePatch
    {
        private static bool Prefix(SpawnArea __instance)
        {
            if (!__instance) return false;

            if (__instance.m_prefabs.Find(x => x.m_prefab.name == "Leviathan") == null) return true;
            
            SpawnArea.SpawnData spawnData = __instance.SelectWeightedPrefab();
            if (spawnData == null || !__instance.FindSpawnPoint(spawnData.m_prefab, out Vector3 point)) return false;

            UnityEngine.Object.Instantiate(spawnData.m_prefab, point,
                Quaternion.Euler(0.0f, (float)UnityEngine.Random.Range(0, 360), 0.0f));
                
            return false;

        }
    }
}