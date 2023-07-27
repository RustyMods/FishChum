using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace FishingBait;

public class SpawnAreaPatch
{
    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    public static class SpawnAreaPatcher
    {
        public static void Postfix(ZNetScene __instance)
        {
            if (!__instance) return;
            AddSpawnAreaSpawnData(__instance, "SerpentChumSpawner", "Serpent", 1, 1, 2, 10);
        }
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
        var gameObjectSpawnAreaScript = scene.GetPrefab(spawnerAreaPrefabName).GetComponent<SpawnArea>();
        if (!gameObjectSpawnAreaScript) return;
        var gameObject = scene.GetPrefab(gameObjectName);
        if (!gameObject) return;
        
        var gameObjectData = new SpawnArea.SpawnData
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
    
    [HarmonyPatch(typeof(SpawnArea), nameof(SpawnArea.UpdateSpawn))]
    static class SpawnAreaUpdateSpawnPatch
    {
        static bool Prefix(SpawnArea __instance)
        {
            if (!__instance) return false;
            var objName = __instance.name;
            if (isSerpentChumSpawner(objName))
            {
                switch (Heightmap.FindBiome(__instance.transform.position))
                {
                    case Heightmap.Biome.Ocean:
                        return true;
                    default:
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "Serpent Chum doesn't work in shallow waters");
                        return false;
                }
            }
            return true;
        }
        private static bool isSerpentChumSpawner(string gameObjectName)
        {
            if (gameObjectName == "SerpentChumSpawner(Clone)") return true;
            return false;
        }
    }
}