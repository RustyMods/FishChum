using System.Configuration;
using BepInEx.Configuration;
using HarmonyLib;
using ItemManager;
using UnityEngine;

namespace FishChum;

public class ItemRecipePatch
{
    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    public static class ItemRecipePatcher
    {
        public static void Postfix(ZNetScene __instance)
        {
            if (!__instance) return;
            GameObject FishingRod = __instance.GetPrefab("FishingRod");
            if (!FishingRod) return;
            GameObject FishingBait = __instance.GetPrefab("FishingBait");
            if (!FishingBait) return;
            
            Item FishingRodData = new Item(FishingRod, false);
            FishingRodData.Crafting.Add(CraftingTable.Forge, 3);
            FishingRodData.RequiredItems.Add("FineWood", 10);
            FishingRodData.RequiredItems.Add("Copper", 5);
            FishingRodData.RequiredItems.Add("LeatherScraps", 10);
            FishingRodData.CraftAmount = 1;
            FishingRodData.Configurable = Configurability.Full;

            Item FishingBaitData = new Item(FishingBait, false);
            FishingBaitData.Crafting.Add(CraftingTable.Cauldron, 1);
            FishingBaitData.RequiredItems.Add("NeckTail", 5);
            FishingBaitData.CraftAmount = 10;
            FishingBaitData.Configurable = Configurability.Recipe;
        }
    }
    
}