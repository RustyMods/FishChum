using System;
using System.Collections.Generic;
using HarmonyLib;
using ItemManager;
using UnityEngine;

namespace FishingBait;

public class CookingStationPatch
{
    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    public static class OvenPatch
    {
        public static void Postfix(ZNetScene __instance)
        {
            AddCookingStationConversion(__instance, "TrollfishMeat", "TrollfishMeatCooked", 25f, "piece_cookingstation", "piece_cookingstation_iron");
            AddCookingStationConversion(__instance, "FishBallUncooked", "FishBallCooked", 10f, "piece_oven");
            AddCookingStationConversion(__instance, "CoralDelightUncooked", "CoralDelightCooked", 10f, "piece_oven");
            AddCookingStationConversion(__instance, "BakedSalmonUncooked", "BakedSalmonCooked", 10f, "piece_oven");
            AddCookingStationConversion(__instance, "BakedMagmaFishUncooked", "BakedMagmaFishCooked", 10f, "piece_oven");
        }

        private static void AddCookingStationConversion(ZNetScene scene, string fromItemName, string toItemName, float cookTime, params string[] stationNames)
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
}
