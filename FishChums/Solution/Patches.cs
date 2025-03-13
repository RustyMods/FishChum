using HarmonyLib;
using UnityEngine;

namespace FishChums.Solution;

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
                    break;
            }

            Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft,
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
                        Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Serpent Chum doesn't work in the shallow waters");
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
                        Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Serpent Chum doesn't work in the shallow waters");
                        return false;
                }
            }

            return true;
        }
        
        private static bool isSerpentChumSpawner(string gameObjectName)
        {
            return gameObjectName.Replace("(Clone)",string.Empty) == "SerpentChumSpawner";
        }
    }

    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    static class OvenPatch
    {
        public static void Postfix(ZNetScene __instance)
        {
            if (!__instance) return;
            AddCookingStationConversion("TrollfishMeat", "TrollfishMeatCooked", 25f,
                "piece_cookingstation", "piece_cookingstation_iron");
            AddCookingStationConversion("FishBallUncooked", "FishBallCooked", 10f, "piece_oven");
            AddCookingStationConversion("CoralDelightUncooked", "CoralDelightCooked", 10f,
                "piece_oven");
            AddCookingStationConversion("BakedSalmonUncooked", "BakedSalmonCooked", 10f, "piece_oven");
            AddCookingStationConversion("BakedMagmaFishUncooked", "BakedMagmaFishCooked", 10f,
                "piece_oven");
        }
        
        private static void AddCookingStationConversion(string fromItemName, string toItemName,
            float cookTime, params string[] stationNames)
        {
            if (ZNetScene.instance.GetPrefab(fromItemName) is not { } fromItem ||
                ZNetScene.instance.GetPrefab(toItemName) is not { } toItem) return;
            
            foreach (var stationName in stationNames)
            {
                if (ZNetScene.instance.GetPrefab(stationName) is not { } station || !station.TryGetComponent(out CookingStation stationScript)) continue;
                stationScript.m_conversion.Add(new CookingStation.ItemConversion
                {
                    m_from = fromItem.GetComponent<ItemDrop>(),
                    m_to = toItem.GetComponent<ItemDrop>(),
                    m_cookTime = cookTime
                });
            }
        }
    }
    
    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    public static class LootSpawnerPatch
    {
        public static void Postfix(ZNetScene __instance)
        {
            AddLootSpawnerItemDrops("FishChumMeadowsSpawner", 1, 1, 1, "Fish1", "Fish2");
            AddLootSpawnerItemDrops("FishChumBlackforestSpawner", 1, 1, 1, "Fish1", "Fish2", "Fish5");
            AddLootSpawnerItemDrops("FishChumSwampsSpawner", 1, 1, 1, "Fish2", "Fish6");
            AddLootSpawnerItemDrops("FishChumOceanSpawner", 1, 1, 1, "Fish3", "Fish8", "Fish12");
            AddLootSpawnerItemDrops("FishChumMountainsSpawner", 1, 1, 1, "Fish4_cave");
            AddLootSpawnerItemDrops("FishChumPlainsSpawner", 1, 1, 1, "Fish7", "Fish8");
            AddLootSpawnerItemDrops("FishChumMistlandsSpawner", 1, 1, 1, "Fish9", "Fish12");
            AddLootSpawnerItemDrops("FishChumAshlandsSpawner", 1, 1, 1, "Fish11", "Fish12");
            AddLootSpawnerItemDrops("FishChumDeepnorthSpawner", 1, 1, 1, "Fish10", "Fish12");
        }
        
        private static void AddLootSpawnerItemDrops(
            string spawnerPrefabName,
            int stackMinValue = 1,
            int stackMaxValue = 1,
            float weightValue = 1,
            params string[] itemPrefabNames
        )
        {
            if (ZNetScene.instance.GetPrefab(spawnerPrefabName) is not { } spawnerPrefab ||
                !spawnerPrefab.TryGetComponent(out LootSpawner component)) return;

            foreach (var itemPrefabName in itemPrefabNames)
            {
                if (ZNetScene.instance.GetPrefab(itemPrefabName) is not { } prefab) continue;
                component.m_items.m_drops.Add(new DropTable.DropData
                {
                    m_item = prefab,
                    m_weight = weightValue,
                    m_stackMax = stackMaxValue,
                    m_stackMin = stackMinValue
                });
            }
            component.m_respawnTimeMinuts = 1;
        }
    }

    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    private static class RegisterPrefabs
    {
        private static void Postfix(ZNetScene __instance)
        {
            if (!__instance) return;
            var sfx_fishchum_throw = FishChumsPlugin._AssetBundle.LoadAsset<GameObject>("sfx_fishchum_throw");
            if (sfx_fishchum_throw)
            {
                sfx_fishchum_throw.AddComponent<ZNetView>();
                if (!__instance.m_prefabs.Contains(sfx_fishchum_throw))
                {
                    __instance.m_prefabs.Add(sfx_fishchum_throw);
                }
                __instance.m_namedPrefabs[sfx_fishchum_throw.name.GetStableHashCode()] = sfx_fishchum_throw;
            }
            var sfx_fishchum_explode = FishChumsPlugin._AssetBundle.LoadAsset<GameObject>("sfx_fishchum_explode");
            if (sfx_fishchum_explode)
            {
                sfx_fishchum_explode.AddComponent<ZNetView>();
                if (!__instance.m_prefabs.Contains(sfx_fishchum_explode))
                {
                    __instance.m_prefabs.Add(sfx_fishchum_explode);
                }

                __instance.m_namedPrefabs[sfx_fishchum_explode.name.GetStableHashCode()] = sfx_fishchum_explode;
            }
        }
    }

    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    public static class SpawnAreaPatch
    {
        public static void Postfix(ZNetScene __instance)
        {
            if (!__instance) return;
            AddSpawnAreaSpawnData("SerpentChumSpawner", "Serpent", 1, 1, 2, 10);
            AddSpawnAreaSpawnData("leviathanChumSpawner", "Leviathan", 1, 1, 1, 40);
        }
        
        private static void AddSpawnAreaSpawnData(
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
            if (ZNetScene.instance.GetPrefab(spawnerAreaPrefabName) is not { } spawnerAreaPrefab ||
                !spawnerAreaPrefab.TryGetComponent(out SpawnArea component)) return;
            if (ZNetScene.instance.GetPrefab(gameObjectName) is not { } prefab) return;
            component.m_prefabs.Add(new SpawnArea.SpawnData
            {
                m_prefab = prefab,
                m_weight = weight,
                m_minLevel = minLevel,
                m_maxLevel = maxLevel
            });
            // SPAWN AREA SCRIPT SETTINGS
            component.m_levelupChance = 0;
            component.m_spawnIntervalSec = spawnIntervalSec;
            component.m_triggerDistance = 50;
            component.m_setPatrolSpawnPoint = true;
            component.m_spawnRadius = 25;
            component.m_nearRadius = 20;
            component.m_farRadius = 1000;
            component.m_maxNear = maxNear;
            component.m_maxTotal = maxTotal;
            component.m_onGroundOnly = false;
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

    [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.DoCrafting))]
    private static class DoCraftingPatch
    {
        private static void Postfix(InventoryGui __instance, Player player)
        {
            if (!__instance || !player) return;
            if (__instance.m_craftRecipe == null) return;
            if (__instance.m_craftRecipe.m_item.m_itemData.m_shared.m_itemType is not ItemDrop.ItemData.ItemType.Consumable) return;
            foreach (Piece.Requirement? requirement in __instance.m_craftRecipe.m_resources)
            {
                if (requirement.m_resItem.m_itemData.m_shared.m_itemType is not ItemDrop.ItemData.ItemType.Fish) continue;
                ItemDrop.ItemData? inventoryItem = player.GetInventory().GetItem(requirement.m_resItem.m_itemData.m_shared.m_name);
                if (inventoryItem is not { m_quality: > 1 }) continue;
                player.GetInventory().AddItem(__instance.m_craftRecipe.m_item.gameObject.name, inventoryItem.m_quality,
                    1, -1, player.GetPlayerID(), player.GetPlayerName());
                return;
            }
        }
    }
}