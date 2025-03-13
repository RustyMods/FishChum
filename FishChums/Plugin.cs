using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using ItemManager;
using JetBrains.Annotations;
using ServerSync;
using UnityEngine;

namespace FishChums;

[BepInPlugin(ModGUID, ModName, ModVersion)]
public class FishChumsPlugin : BaseUnityPlugin
{
    internal const string ModName = "FishChum";
    internal const string ModVersion = "1.0.4";
    internal const string Author = "RustyMods";
    private const string ModGUID = Author + "." + ModName;
    private static readonly string ConfigFileName = ModGUID + ".cfg";
    private static readonly string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
    internal static string ConnectionError = "";
    private readonly Harmony _harmony = new(ModGUID);
    public static readonly ManualLogSource FishChumsLogger = BepInEx.Logging.Logger.CreateLogSource(ModName);

    private static readonly ConfigSync ConfigSync = new(ModGUID)
        { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };

    public enum Toggle
    {
        On = 1,
        Off = 0
    }

    public static readonly AssetBundle _AssetBundle = GetAssetBundle("fishingbaitbundle");
    private static ConfigEntry<Toggle> _serverConfigLocked = null!;

    private void InitConfigs()
    {
        _serverConfigLocked = config("1 - General", "Lock Configuration", Toggle.On,
            "If on, the configuration is locked and can be changed by server admins only.");
        _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);
    }

    public void Awake()
    {
        InitConfigs();

        Item LeviathanChum = new("fishingbaitbundle", "LeviathanChum");
        LeviathanChum.Name.English("Leviathan Chum");
        LeviathanChum.Description.English("Perfect for attracting leviathans");
        LeviathanChum.Crafting.Add(CraftingTable.Forge, 4);
        LeviathanChum.RequiredItems.Add("FishingBait", 100);
        LeviathanChum.RequiredItems.Add("SerpentScale", 5);
        LeviathanChum.RequiredItems.Add("HardAntler", 10);
        LeviathanChum.RequiredItems.Add("Copper", 5);
        LeviathanChum.CraftAmount = 1;
        LeviathanChum.Configurable = Configurability.Recipe;
        PrefabManager.RegisterPrefab(PrefabManager.RegisterAssetBundle("fishingbaitbundle"), "leviathanChumSpawner");

        Item FishChumMeadows = new("fishingbaitbundle", "FishChumMeadows");
        FishChumMeadows.Name.English("Common Chum");
        FishChumMeadows.Description.English("Perfect for attracting perch and pike.");
        FishChumMeadows.Crafting.Add(CraftingTable.Cauldron, 0);
        FishChumMeadows.RequiredItems.Add("FishingBait", 5);
        FishChumMeadows.RequiredItems.Add("Dandelion", 10);
        FishChumMeadows.RequiredItems.Add("LeatherScraps", 2);
        FishChumMeadows.CraftAmount = 10;
        FishChumMeadows.Configurable = Configurability.Recipe;
        PrefabManager.RegisterPrefab(PrefabManager.RegisterAssetBundle("fishingbaitbundle"), "FishChumMeadowsSpawner");

        Item FishChumBlackforest = new("fishingbaitbundle", "FishChumBlackforest");
        FishChumBlackforest.Name.English("Curious Chum");
        FishChumBlackforest.Description.English("Perfect for attracting trollfish.");
        FishChumBlackforest.Crafting.Add(CraftingTable.Cauldron, 0);
        FishChumBlackforest.RequiredItems.Add("FishingBait", 5);
        FishChumBlackforest.RequiredItems.Add("Thistle", 3);
        FishChumBlackforest.RequiredItems.Add("TrollHide", 1);
        FishChumBlackforest.CraftAmount = 10;
        FishChumBlackforest.Configurable = Configurability.Recipe;
        PrefabManager.RegisterPrefab(PrefabManager.RegisterAssetBundle("fishingbaitbundle"),
            "FishChumBlackforestSpawner");

        Item FishChumSwamps = new("fishingbaitbundle", "FishChumSwamps");
        FishChumSwamps.Name.English("Bloody Chum");
        FishChumSwamps.Description.English("perfect for attracting giant herrings.");
        FishChumSwamps.Crafting.Add(CraftingTable.Cauldron, 1);
        FishChumSwamps.RequiredItems.Add("FishingBait", 5);
        FishChumSwamps.RequiredItems.Add("Bloodbag", 3);
        FishChumSwamps.RequiredItems.Add("Guck", 1);
        FishChumSwamps.CraftAmount = 10;
        FishChumSwamps.Configurable = Configurability.Recipe;
        PrefabManager.RegisterPrefab(PrefabManager.RegisterAssetBundle("fishingbaitbundle"), "FishChumSwampsSpawner");

        Item FishChumMountains = new("fishingbaitbundle", "FishChumMountains");
        FishChumMountains.Name.English("Frozen Chum");
        FishChumMountains.Description.English("perfect for attracting tetra fish.");
        FishChumMountains.Crafting.Add(CraftingTable.Cauldron, 2);
        FishChumMountains.RequiredItems.Add("FishingBait", 5);
        FishChumMountains.RequiredItems.Add("FreezeGland", 3);
        FishChumMountains.RequiredItems.Add("WolfPelt", 2);
        FishChumMountains.CraftAmount = 10;
        FishChumMountains.Configurable = Configurability.Recipe;
        PrefabManager.RegisterPrefab(PrefabManager.RegisterAssetBundle("fishingbaitbundle"),
            "FishChumMountainsSpawner");

        Item FishChumPlains = new("fishingbaitbundle", "FishChumPlains");
        FishChumPlains.Name.English("Dry Chum");
        FishChumPlains.Description.English("perfect for attracting groupers.");
        FishChumPlains.Crafting.Add(CraftingTable.Cauldron, 2);
        FishChumPlains.RequiredItems.Add("FishingBait", 5);
        FishChumPlains.RequiredItems.Add("Barley", 5);
        FishChumPlains.RequiredItems.Add("Flax", 5);
        FishChumPlains.CraftAmount = 10;
        FishChumPlains.Configurable = Configurability.Recipe;
        PrefabManager.RegisterPrefab(PrefabManager.RegisterAssetBundle("fishingbaitbundle"), "FishChumPlainsSpawner");

        Item FishChumMistlands = new("fishingbaitbundle", "FishChumMistlands");
        FishChumMistlands.Name.English("Mysterious Chum");
        FishChumMistlands.Description.English("perfect for attracting anglerfish.");
        FishChumMistlands.Crafting.Add(CraftingTable.Cauldron, 3);
        FishChumMistlands.RequiredItems.Add("FishingBait", 5);
        FishChumMistlands.RequiredItems.Add("ChickenEgg", 1);
        FishChumMistlands.RequiredItems.Add("HareMeat", 1);
        FishChumMistlands.RequiredItems.Add("LinenThread", 2);
        FishChumMistlands.CraftAmount = 10;
        FishChumMistlands.Configurable = Configurability.Recipe;
        PrefabManager.RegisterPrefab(PrefabManager.RegisterAssetBundle("fishingbaitbundle"),
            "FishChumMistlandsSpawner");

        Item FishChumAshlands = new("fishingbaitbundle", "FishChumAshlands");
        FishChumAshlands.Name.English("Burnt Chum");
        FishChumAshlands.Description.English("perfect for attracting magmafish.");
        FishChumAshlands.Crafting.Add(CraftingTable.Cauldron, 3);
        FishChumAshlands.RequiredItems.Add("FishingBait", 5);
        FishChumAshlands.RequiredItems.Add("SurtlingCore", 5);
        FishChumAshlands.RequiredItems.Add("Coal", 5);
        FishChumAshlands.RequiredItems.Add("LinenThread", 2);
        FishChumAshlands.CraftAmount = 10;
        FishChumAshlands.Configurable = Configurability.Recipe;
        PrefabManager.RegisterPrefab(PrefabManager.RegisterAssetBundle("fishingbaitbundle"), "FishChumAshlandsSpawner");

        Item FishChumDeepnorth = new("fishingbaitbundle", "FishChumDeepnorth");
        FishChumDeepnorth.Name.English("Frosted Chum");
        FishChumDeepnorth.Description.English("perfect for attracting northern salmon.");
        FishChumDeepnorth.Crafting.Add(CraftingTable.Cauldron, 3);
        FishChumDeepnorth.RequiredItems.Add("FishingBait", 5);
        FishChumDeepnorth.RequiredItems.Add("BugMeat", 2);
        FishChumDeepnorth.RequiredItems.Add("MushroomJotunPuffs", 1);
        FishChumDeepnorth.RequiredItems.Add("LinenThread", 2);
        FishChumDeepnorth.CraftAmount = 10;
        FishChumDeepnorth.Configurable = Configurability.Recipe;
        PrefabManager.RegisterPrefab(PrefabManager.RegisterAssetBundle("fishingbaitbundle"),
            "FishChumDeepnorthSpawner");

        Item FishChumOcean = new("fishingbaitbundle", "FishChumOcean");
        FishChumOcean.Name.English("Deepsea Chum");
        FishChumOcean.Description.English("perfect for attracting tuna and coral cod.");
        FishChumOcean.Crafting.Add(CraftingTable.Cauldron, 1);
        FishChumOcean.RequiredItems.Add("FishingBait", 5);
        FishChumOcean.RequiredItems.Add("Ooze", 3);
        FishChumOcean.RequiredItems.Add("LeatherScraps", 2);
        FishChumOcean.RequiredItems.Add("Dandelion", 5);
        FishChumOcean.CraftAmount = 10;
        FishChumOcean.Configurable = Configurability.Recipe;
        PrefabManager.RegisterPrefab(PrefabManager.RegisterAssetBundle("fishingbaitbundle"), "FishChumOceanSpawner");

        Item SerpentChum = new("fishingbaitbundle", "SerpentChum");
        SerpentChum.Name.English("Serpent chum");
        SerpentChum.Description.English("Perfect for attracting serpents");
        SerpentChum.Crafting.Add(CraftingTable.Cauldron, 3);
        SerpentChum.RequiredItems.Add("WolfMeat", 1);
        SerpentChum.RequiredItems.Add("Bloodbag", 2);
        SerpentChum.RequiredItems.Add("LeatherScraps", 3);
        SerpentChum.RequiredItems.Add("Thistle", 2);
        SerpentChum.CraftAmount = 1;
        SerpentChum.Configurable = Configurability.Recipe;
        PrefabManager.RegisterPrefab(PrefabManager.RegisterAssetBundle("fishingbaitbundle"), "SerpentChumSpawner");

        Item PerchStew = new("fishingbaitbundle", "PerchStew");
        PerchStew.Name.English("Perch Stew");
        PerchStew.Description.English("Deliciously balanced");
        PerchStew.Crafting.Add(CraftingTable.Cauldron, 1);
        PerchStew.RequiredItems.Add("Fish1", 1);
        PerchStew.RequiredItems.Add("Carrot", 1);
        PerchStew.RequiredItems.Add("Turnip", 1);
        PerchStew.QualityResultAmountMultiplier = 2f;
        PerchStew.CraftAmount = 1;

        Item TrollfishMeat = new("fishingbaitbundle", "TrollfishMeat");
        TrollfishMeat.Name.English("Trollfish Meat");
        TrollfishMeat.Description.English("Juicy, slimy troll fish meat");
        TrollfishMeat.Crafting.Add(CraftingTable.Cauldron, 0);
        TrollfishMeat.RequiredItems.Add("Fish5", 1);
        TrollfishMeat.CraftAmount = 1;

        Item TrollfishMeatCooked = new("fishingbaitbundle", "TrollfishMeatCooked");
        TrollfishMeatCooked.Name.English("Cooked Trollfish");
        TrollfishMeatCooked.Description.English("Still unusually slimy");
        TrollfishMeatCooked.Crafting.Add(CraftingTable.Cauldron, 0);
        TrollfishMeatCooked.RequiredItems.Add("SwordCheat", 1);
        TrollfishMeatCooked.CraftAmount = 1;
        TrollfishMeatCooked.Configurable = Configurability.Stats;

        Item PikeSoup = new("fishingbaitbundle", "PikeSoup");
        PikeSoup.Name.English("Pike Soup");
        PikeSoup.Description.English("Smells of the sea");
        PikeSoup.Crafting.Add(CraftingTable.Cauldron, 0);
        PikeSoup.RequiredItems.Add("Fish2", 1);
        PikeSoup.RequiredItems.Add("Thistle", 2);
        PikeSoup.RequiredItems.Add("Turnip", 3);
        PikeSoup.CraftAmount = 1;

        Item FishBallUncooked = new("fishingbaitbundle", "FishBallUncooked");
        FishBallUncooked.Name.English("Uncooked fishballs");
        FishBallUncooked.Description.English("Delicious breaded fishballs ready for the oven");
        FishBallUncooked.Crafting.Add(CraftingTable.Cauldron, 2);
        FishBallUncooked.RequiredItems.Add("Fish7", 1);
        FishBallUncooked.RequiredItems.Add("BarleyFlour", 5);
        FishBallUncooked.RequiredItems.Add("Dandelion", 3);
        FishBallUncooked.CraftAmount = 1;

        Item FishBallCooked = new("fishingbaitbundle", "FishBallCooked");
        FishBallCooked.Name.English("Fishballs");
        FishBallCooked.Description.English("Delicious breaded fishballs");
        FishBallCooked.Crafting.Add(CraftingTable.Cauldron, 4);
        FishBallCooked.RequiredItems.Add("SwordCheat", 1);
        FishBallCooked.CraftAmount = 1;
        FishBallCooked.Configurable = Configurability.Stats;

        Item TunaSalad = new("fishingbaitbundle", "TunaSalad");
        TunaSalad.Name.English("Tuna salad");
        TunaSalad.Description.English("Surprisingly healthy");
        TunaSalad.Crafting.Add(CraftingTable.Cauldron, 2);
        TunaSalad.RequiredItems.Add("Fish3", 1);
        TunaSalad.RequiredItems.Add("Thistle", 3);
        TunaSalad.RequiredItems.Add("Onion", 1);
        TunaSalad.RequiredItems.Add("Dandelion", 3);
        TunaSalad.CraftAmount = 1;

        Item TetraStew = new("fishingbaitbundle", "TetraStew");
        TetraStew.Name.English("Tetra stew");
        TetraStew.Description.English("Stewed to chilled perfection");
        TetraStew.Crafting.Add(CraftingTable.Cauldron, 2);
        TetraStew.RequiredItems.Add("Fish4_cave", 1);
        TetraStew.RequiredItems.Add("FreezeGland", 1);
        TetraStew.RequiredItems.Add("Onion", 1);
        TetraStew.RequiredItems.Add("Turnip", 3);
        TetraStew.CraftAmount = 1;

        Item SteamedHerring = new("fishingbaitbundle", "SteamedHerring");
        SteamedHerring.Name.English("Steamed herring");
        SteamedHerring.Description.English("Steamed and ready to eat");
        SteamedHerring.Crafting.Add(CraftingTable.Cauldron, 2);
        SteamedHerring.RequiredItems.Add("Fish6", 1);
        SteamedHerring.RequiredItems.Add("Root", 1);
        SteamedHerring.CraftAmount = 1;

        Item CoralDelightUncooked = new("fishingbaitbundle", "CoralDelightUncooked");
        CoralDelightUncooked.Name.English("Uncooked coral delight");
        CoralDelightUncooked.Description.English("Dredged in egg yolk and barley flour");
        CoralDelightUncooked.Crafting.Add(CraftingTable.Cauldron, 2);
        CoralDelightUncooked.RequiredItems.Add("Fish8", 1);
        CoralDelightUncooked.RequiredItems.Add("ChickenEgg", 1);
        CoralDelightUncooked.RequiredItems.Add("BarleyFlour", 10);
        CoralDelightUncooked.CraftAmount = 1;

        Item CoralDelightCooked = new("fishingbaitbundle", "CoralDelightCooked");
        CoralDelightCooked.Name.English("Coral Delight");
        CoralDelightCooked.Description.English("Baked to perfection");
        CoralDelightCooked.Crafting.Add(CraftingTable.Cauldron, 4);
        CoralDelightCooked.RequiredItems.Add("SwordCheat", 1);
        CoralDelightCooked.CraftAmount = 1;
        CoralDelightCooked.Configurable = Configurability.Stats;

        Item BakedSalmonUncooked = new("fishingbaitbundle", "BakedSalmonUncooked");
        BakedSalmonUncooked.Name.English("Uncooked salmon");
        BakedSalmonUncooked.Description.English("Sliced and plated, ready for the oven");
        BakedSalmonUncooked.Crafting.Add(CraftingTable.Cauldron, 2);
        BakedSalmonUncooked.RequiredItems.Add("Fish10", 1);
        BakedSalmonUncooked.RequiredItems.Add("MushroomMagecap", 5);
        BakedSalmonUncooked.RequiredItems.Add("Thistle", 10);
        BakedSalmonUncooked.CraftAmount = 1;

        Item BakedSalmonCooked = new("fishingbaitbundle", "BakedSalmonCooked");
        BakedSalmonCooked.Name.English("Baked Salmon");
        BakedSalmonCooked.Description.English("Smooth balanced flavors");
        BakedSalmonCooked.Crafting.Add(CraftingTable.Cauldron, 4);
        BakedSalmonCooked.RequiredItems.Add("SwordCheat", 1);
        BakedSalmonCooked.CraftAmount = 1;
        BakedSalmonCooked.Configurable = Configurability.Stats;

        Item BakedMagmafishUncooked = new("fishingbaitbundle", "BakedMagmafishUncooked");
        BakedMagmafishUncooked.Name.English("Uncooked Magmafish");
        BakedMagmafishUncooked.Description.English("Just plate it and toss it in the oven");
        BakedMagmafishUncooked.Crafting.Add(CraftingTable.Cauldron, 2);
        BakedMagmafishUncooked.RequiredItems.Add("Fish11", 1);
        BakedMagmafishUncooked.RequiredItems.Add("Dandelion", 5);
        BakedMagmafishUncooked.RequiredItems.Add("Thistle", 5);
        BakedMagmafishUncooked.RequiredItems.Add("RoyalJelly", 10);
        BakedMagmafishUncooked.CraftAmount = 1;

        Item BakedMagmafishCooked = new("fishingbaitbundle", "BakedMagmafishCooked");
        BakedMagmafishCooked.Name.English("Baked Magmafish");
        BakedMagmafishCooked.Description.English("Spicy hot baked fish");
        BakedMagmafishCooked.Crafting.Add(CraftingTable.Cauldron, 4);
        BakedMagmafishCooked.RequiredItems.Add("SwordCheat", 1);
        BakedMagmafishCooked.CraftAmount = 1;
        BakedMagmafishCooked.Configurable = Configurability.Stats;

        var assembly = Assembly.GetExecutingAssembly();
        _harmony.PatchAll(assembly);
        SetupWatcher();
    }

    private void OnDestroy()
    {
        Config.Save();
    }

    private static AssetBundle GetAssetBundle(string fileName)
    {
        var execAssembly = Assembly.GetExecutingAssembly();
        var resourceName = execAssembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
        using var stream = execAssembly.GetManifestResourceStream(resourceName);
        return AssetBundle.LoadFromStream(stream);
    }

    private void SetupWatcher()
    {
        FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
        watcher.Changed += ReadConfigValues;
        watcher.Created += ReadConfigValues;
        watcher.Renamed += ReadConfigValues;
        watcher.IncludeSubdirectories = true;
        watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
        watcher.EnableRaisingEvents = true;
    }

    private void ReadConfigValues(object sender, FileSystemEventArgs e)
    {
        if (!File.Exists(ConfigFileFullPath)) return;
        try
        {
            FishChumsLogger.LogDebug("ReadConfigValues called");
            Config.Reload();
        }
        catch
        {
            FishChumsLogger.LogError($"There was an issue loading your {ConfigFileName}");
            FishChumsLogger.LogError("Please check your config entries for spelling and format!");
        }
    }

    private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
        bool synchronizedSetting = true)
    {
        ConfigDescription extendedDescription =
            new(
                description.Description +
                (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                description.AcceptableValues, description.Tags);
        var configEntry = Config.Bind(group, name, value, extendedDescription);
        //var configEntry = Config.Bind(group, name, value, description);

        var syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
        syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

        return configEntry;
    }

    private ConfigEntry<T> config<T>(string group, string name, T value, string description,
        bool synchronizedSetting = true)
    {
        return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
    }

    private class ConfigurationManagerAttributes
    {
        [UsedImplicitly] public int? Order;
        [UsedImplicitly] public bool? Browsable;
        [UsedImplicitly] public string? Category;
        [UsedImplicitly] public Action<ConfigEntryBase>? CustomDrawer;
    }
}