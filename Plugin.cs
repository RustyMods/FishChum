using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CreatureManager;
using HarmonyLib;
using ItemManager;
using JetBrains.Annotations;
using LocalizationManager;
using LocationManager;
using PieceManager;
using ServerSync;
using SkillManager;
using StatusEffectManager;
using UnityEngine;
using CraftingTable = ItemManager.CraftingTable;
using PrefabManager = ItemManager.PrefabManager;
using Range = LocationManager.Range;

namespace FishChum
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class FishChumPlugin : BaseUnityPlugin
    {
        internal const string ModName = "FishChum";
        internal const string ModVersion = "0.0.1";
        internal const string Author = "RustyMods";
        private const string ModGUID = Author + "." + ModName;
        private static string ConfigFileName = ModGUID + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        internal static string ConnectionError = "";
        private readonly Harmony _harmony = new(ModGUID);

        public static readonly ManualLogSource FishingBaitLogger =
            BepInEx.Logging.Logger.CreateLogSource(ModName);

        private static readonly ConfigSync ConfigSync = new(ModGUID)
            { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };
        
        // Location Manager variables
        // public Texture2D tex;
        // private Sprite mySprite;
        // private SpriteRenderer sr;

        public enum Toggle
        {
            On = 1,
            Off = 0
        }

        public void Awake()
        {
            // Uncomment the line below to use the LocalizationManager for localizing your mod.
            //Localizer.Load(); // Use this to initialize the LocalizationManager (for more information on LocalizationManager, see the LocalizationManager documentation https://github.com/blaxxun-boop/LocalizationManager#example-project).

            _serverConfigLocked = config("1 - General", "Lock Configuration", Toggle.On,
                "If on, the configuration is locked and can be changed by server admins only.");
            _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);

            /*#region PieceManager Example Code

            // Globally turn off configuration options for your pieces, omit if you don't want to do this.
            BuildPiece.ConfigurationEnabled = false;
            
            // Format: new("AssetBundleName", "PrefabName", "FolderName");
            BuildPiece examplePiece1 = new("funward", "funward", "FunWard");

            examplePiece1.Name.English("Fun Ward"); // Localize the name and description for the building piece for a language.
            examplePiece1.Description.English("Ward For testing the Piece Manager");
            examplePiece1.RequiredItems.Add("FineWood", 20, false); // Set the required items to build. Format: ("PrefabName", Amount, Recoverable)
            examplePiece1.RequiredItems.Add("SurtlingCore", 20, false);
            examplePiece1.Category.Add(PieceManager.BuildPieceCategory.Misc);
            examplePiece1.Crafting.Set(PieceManager.CraftingTable.ArtisanTable); // Set a crafting station requirement for the piece.
            examplePiece1.Extension.Set(PieceManager.CraftingTable.Forge, 2); // Makes this piece a station extension, can change the max station distance by changing the second value. Use strings for custom tables.
            //examplePiece1.Crafting.Set("CUSTOMTABLE"); // If you have a custom table you're adding to the game. Just set it like this.
            //examplePiece1.SpecialProperties.NoConfig = true;  // Do not generate a config for this piece, omit this line of code if you want to generate a config.
            examplePiece1.SpecialProperties = new SpecialProperties() { AdminOnly = true, NoConfig = true}; // You can declare multiple properties in one line           


            BuildPiece examplePiece2 = new("bamboo", "Bamboo_Wall"); // Note: If you wish to use the default "assets" folder for your assets, you can omit it!
            examplePiece2.Name.English("Bamboo Wall");
            examplePiece2.Description.English("A wall made of bamboo!");
            examplePiece2.RequiredItems.Add("BambooLog", 20, false);
            examplePiece2.Category.Add(PieceManager.BuildPieceCategory.Building);
            examplePiece2.Crafting.Set("CUSTOMTABLE"); // If you have a custom table you're adding to the game. Just set it like this.
            examplePiece2.SpecialProperties.AdminOnly = true;  // You can declare these one at a time as well!.


            // If you want to add your item to the cultivator or another hammer with vanilla categories
            // Format: (AssetBundle, "PrefabName", addToCustom, "Item that has a piecetable")
            BuildPiece examplePiece3 = new(PiecePrefabManager.RegisterAssetBundle("bamboo"), "Bamboo_Sapling", true, "Cultivator");
            examplePiece3.Name.English("Bamboo Sapling");
            examplePiece3.Description.English("A young bamboo tree, called a sapling");
            examplePiece3.RequiredItems.Add("BambooSeed", 20, false);
            examplePiece3.SpecialProperties.NoConfig = true;
            
            // If you don't want to make an icon inside unity, but want the PieceManager to snag one for you, simply add .Snapshot() to your piece.
            examplePiece3.Snapshot(); // Optionally, you can use the lightIntensity parameter to set the light intensity of the snapshot. Default is 1.3 or the cameraRotation parameter to set the rotation of the camera. Default is null.


            // Need to add something to ZNetScene but not the hammer, cultivator or other? 
            PiecePrefabManager.RegisterPrefab("bamboo", "Bamboo_Beam_Light");
            
            // Does your model need to swap materials with a vanilla material? Format: (GameObject, isJotunnMock)
            MaterialReplacer.RegisterGameObjectForMatSwap(examplePiece3.Prefab, false);
            
            
            // What if you want to use a custom shader from the game (like Custom/Piece that allows snow!!!) but your unity shader isn't set to Custom/Piece? Format: (GameObject, MaterialReplacer.ShaderType.)
            //MaterialReplacer.RegisterGameObjectForShaderSwap(examplePiece3.Prefab, MaterialReplacer.ShaderType.PieceShader);

            // Detailed instructions on how to use the MaterialReplacer can be found on the current PieceManager Wiki. https://github.com/AzumattDev/PieceManager/wiki
            #endregion*/

            /*#region SkillManager Example Code

            Skill
                tenacity = new("Tenacity",
                    "tenacity-icon.png"); // Skill name along with the skill icon. By default the icon is found in the icons folder. Put it there if you wish to load one.

            tenacity.Description.English("Reduces damage taken by 0.2% per level.");
            tenacity.Name.German("Hartnäckigkeit"); // Use this to localize values for the name
            tenacity.Description.German(
                "Reduziert erlittenen Schaden um 0,2% pro Stufe."); // You can do the same for the description
            tenacity.Configurable = true;

            #endregion*/

            /*#region LocationManager Example Code

            _ = new LocationManager.Location("guildfabs", "GuildAltarSceneFab")
            {
                MapIcon = "portalicon.png",
                ShowMapIcon = ShowIcon.Explored,
                MapIconSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f),
                    100.0f),
                CanSpawn = true,
                SpawnArea = Heightmap.BiomeArea.Everything,
                Prioritize = true,
                PreferCenter = true,
                Rotation = Rotation.Slope,
                HeightDelta = new Range(0, 2),
                SnapToWater = false,
                ForestThreshold = new Range(0, 2.19f),
                Biome = Heightmap.Biome.Meadows,
                SpawnDistance = new Range(500, 1500),
                SpawnAltitude = new Range(10, 100),
                MinimumDistanceFromGroup = 100,
                GroupName = "groupName",
                Count = 15,
                Unique = true
            };
            
            LocationManager.Location location = new("krumpaclocations", "WaterPit1")
            {
                MapIcon = "K_Church_Ruin01.png",
                ShowMapIcon = ShowIcon.Always,
                Biome = Heightmap.Biome.Meadows,
                SpawnDistance = new Range(100, 1500),
                SpawnAltitude = new Range(5, 150),
                MinimumDistanceFromGroup = 100,
                Count = 15
            };
		
            // If your location has creature spawners, you can configure the creature they spawn like this.
            location.CreatureSpawner.Add("Spawner_1", "Neck");
            location.CreatureSpawner.Add("Spawner_2", "Troll");
            location.CreatureSpawner.Add("Spawner_3", "Greydwarf");
            location.CreatureSpawner.Add("Spawner_4", "Neck");
            location.CreatureSpawner.Add("Spawner_5", "Troll");
            location.CreatureSpawner.Add("Spawner_6", "Greydwarf");
            
            #region Location Notes

            // MapIcon                      Sets the map icon for the location.
            // ShowMapIcon                  When to show the map icon of the location. Requires an icon to be set. Use "Never" to not show a map icon for the location. Use "Always" to always show a map icon for the location. Use "Explored" to start showing a map icon for the location as soon as a player has explored the area.
            // MapIconSprite                Sets the map icon for the location.
            // CanSpawn                     Can the location spawn at all.
            // SpawnArea                    If the location should spawn more towards the edge of the biome or towards the center. Use "Edge" to make it spawn towards the edge. Use "Median" to make it spawn towards the center. Use "Everything" if it doesn't matter.</para>
            // Prioritize                   If set to true, this location will be prioritized over other locations, if they would spawn in the same area.
            // PreferCenter                 If set to true, Valheim will try to spawn your location as close to the center of the map as possible.
            // Rotation                     How to rotate the location. Use "Fixed" to use the rotation of the prefab. Use "Random" to randomize the rotation. Use "Slope" to rotate the location along a possible slope.
            // HeightDelta                  The minimum and maximum height difference of the terrain below the location.
            // SnapToWater                  If the location should spawn near water.
            // ForestThreshold              If the location should spawn in a forest. Everything above 1.15 is considered a forest by Valheim. 2.19 is considered a thick forest by Valheim.
            // Biome
            // SpawnDistance                Minimum and maximum range from the center of the map for the location.
            // SpawnAltitude                Minimum and maximum altitude for the location.
            // MinimumDistanceFromGroup     Locations in the same group will keep at least this much distance between each other.
            // GroupName                    The name of the group of the location, used by the minimum distance from group setting.
            // Count                        Maximum number of locations to spawn in. Does not mean that this many locations will spawn. But Valheim will try its best to spawn this many, if there is space.
            // Unique                       If set to true, all other locations will be deleted, once the first one has been discovered by a player.

            #endregion

            #endregion*/

            /*#region StatusEffectManager Example Code

             CustomSE mycooleffect = new("Toxicity");
            mycooleffect.Name.English("Toxicity");
            mycooleffect.Type = EffectType.Consume;
            mycooleffect.IconSprite = null;
            mycooleffect.Name.German("Toxizität"); 
            mycooleffect.Effect.m_startMessageType = MessageHud.MessageType.TopLeft;
            mycooleffect.Effect.m_startMessage = "My Cool Status Effect Started"; 
            mycooleffect.Effect.m_stopMessageType = MessageHud.MessageType.TopLeft;
            mycooleffect.Effect.m_stopMessage = "Not cool anymore, ending effect."; 
            mycooleffect.Effect.m_tooltip = "<color=orange>Toxic damage over time</color>"; 
            mycooleffect.AddSEToPrefab(mycooleffect, "SwordIron");
            
            CustomSE drunkeffect = new("se_drunk", "se_drunk_effect");
			drunkeffect.Name.English("Drunk"); // You can use this to fix the display name in code
			drunkeffect.Icon = "DrunkIcon.png"; // Use this to add an icon (64x64) for the status effect. Put your icon in an "icons" folder
			drunkeffect.Name.German("Betrunken"); // Or add translations for other languages
			drunkeffect.Effect.m_startMessageType = MessageHud.MessageType.Center; // Specify where the start effect message shows
			drunkeffect.Effect.m_startMessage = "I'm drunk!"; // What the start message says
			drunkeffect.Effect.m_stopMessageType = MessageHud.MessageType.Center; // Specify where the stop effect message shows
			drunkeffect.Effect.m_stopMessage = "Sober...again."; // What the stop message says
			drunkeffect.Effect.m_tooltip = "<color=red>Your vision is blurry</color>"; // Tooltip that will describe the effect applied to the player
			drunkeffect.AddSEToPrefab(drunkeffect, "TankardAnniversary"); // Adds the status effect to the Anniversary Tankard. Applies when equipped.
			
			// Create a new status effect in code and apply it to a prefab.
			CustomSE codeSE = new("CodeStatusEffect");
			codeSE.Name.English("New Effect");
			codeSE.Type = EffectType.Consume; // Set the type of status effect this should be.
			codeSE.Icon = "ModDevPower.png";
			codeSE.Name.German("Betrunken"); // Or add translations for other languages
			codeSE.Effect.m_startMessageType = MessageHud.MessageType.Center; // Specify where the start effect message shows
			codeSE.Effect.m_startMessage = "Mod Dev power, granted."; // What the start message says
			codeSE.Effect.m_stopMessageType = MessageHud.MessageType.Center; // Specify where the stop effect message shows
			codeSE.Effect.m_stopMessage = "Mod Dev power, removed."; // What the stop message says
			codeSE.Effect.m_tooltip = "<color=green>You now have Mod Dev POWER!</color>"; // Tooltip that will describe the effect applied to the player
			codeSE.AddSEToPrefab(codeSE, "SwordCheat"); // Adds the status effect to the Cheat Sword. Applies when equipped.
		


            #endregion*/
            
            # region fishChums
            
            Item FishChumMeadows = new("fishingbaitbundle", "FishChumMeadows", "assets");
            FishChumMeadows.Name.English("Common Chum");
            FishChumMeadows.Description.English("Perfect for attracting perch and pike.");
            FishChumMeadows.Crafting.Add(CraftingTable.Cauldron, 0);
            FishChumMeadows.RequiredItems.Add("FishingBait", 5);
            FishChumMeadows.RequiredItems.Add("Dandelion", 10);
            FishChumMeadows.RequiredItems.Add("LeatherScraps", 2);
            FishChumMeadows.CraftAmount = 10;
            FishChumMeadows.Configurable = Configurability.Recipe;
            ItemManager.PrefabManager.RegisterPrefab(PrefabManager.RegisterAssetBundle("fishingbaitbundle"), "FishChumMeadowsSpawner", false); 
            
            Item FishChumBlackforest = new("fishingbaitbundle", "FishChumBlackforest", "assets");
            FishChumBlackforest.Name.English("Curious Chum");
            FishChumBlackforest.Description.English("perfect for attracting trollfish.");
            FishChumBlackforest.Crafting.Add(CraftingTable.Cauldron, 0);
            FishChumBlackforest.RequiredItems.Add("FishingBait", 5);
            FishChumBlackforest.RequiredItems.Add("Thistle", 3);
            FishChumBlackforest.RequiredItems.Add("TrollHide", 1);
            FishChumBlackforest.CraftAmount = 10;
            FishChumBlackforest.Configurable = Configurability.Recipe;
            ItemManager.PrefabManager.RegisterPrefab(PrefabManager.RegisterAssetBundle("fishingbaitbundle"), "FishChumBlackforestSpawner", false); 
            
            Item FishChumSwamps = new("fishingbaitbundle", "FishChumSwamps", "assets");
            FishChumSwamps.Name.English("Bloody Chum");
            FishChumSwamps.Description.English("perfect for attracting giant herrings.");
            FishChumSwamps.Crafting.Add(CraftingTable.Cauldron, 1);
            FishChumSwamps.RequiredItems.Add("FishingBait", 5);
            FishChumSwamps.RequiredItems.Add("Bloodbag", 3);
            FishChumSwamps.RequiredItems.Add("Guck", 1);
            FishChumSwamps.CraftAmount = 10;
            FishChumSwamps.Configurable = Configurability.Recipe;
            ItemManager.PrefabManager.RegisterPrefab(PrefabManager.RegisterAssetBundle("fishingbaitbundle"), "FishChumSwampsSpawner", false); 
            
            Item FishChumMountains = new("fishingbaitbundle", "FishChumMountains", "assets");
            FishChumMountains.Name.English("Frozen Chum");
            FishChumMountains.Description.English("perfect for attracting tetra fish.");
            FishChumMountains.Crafting.Add(CraftingTable.Cauldron, 2);
            FishChumMountains.RequiredItems.Add("FishingBait", 5);
            FishChumMountains.RequiredItems.Add("FreezeGland", 3);
            FishChumMountains.RequiredItems.Add("WolfPelt", 2);
            FishChumMountains.CraftAmount = 10;
            FishChumMountains.Configurable = Configurability.Recipe;
            ItemManager.PrefabManager.RegisterPrefab(PrefabManager.RegisterAssetBundle("fishingbaitbundle"), "FishChumMountainsSpawner", false); 
            
            Item FishChumPlains = new("fishingbaitbundle", "FishChumPlains", "assets");
            FishChumPlains.Name.English("Dry Chum");
            FishChumPlains.Description.English("perfect for attracting groupers.");
            FishChumPlains.Crafting.Add(CraftingTable.Cauldron, 2);
            FishChumPlains.RequiredItems.Add("FishingBait", 5);
            FishChumPlains.RequiredItems.Add("Barley", 5);
            FishChumPlains.RequiredItems.Add("Flax", 5);
            FishChumPlains.CraftAmount = 10;
            FishChumPlains.Configurable = Configurability.Recipe;
            ItemManager.PrefabManager.RegisterPrefab(PrefabManager.RegisterAssetBundle("fishingbaitbundle"), "FishChumPlainsSpawner", false); 

            Item FishChumMistlands = new("fishingbaitbundle", "FishChumMistlands", "assets");
            FishChumMistlands.Name.English("Mysterious Chum");
            FishChumMistlands.Description.English("perfect for attracting anglerfish.");
            FishChumMistlands.Crafting.Add(CraftingTable.Cauldron, 3);
            FishChumMistlands.RequiredItems.Add("FishingBait", 5);
            FishChumMistlands.RequiredItems.Add("ChickenEgg", 1);
            FishChumMistlands.RequiredItems.Add("HareMeat", 1);
            FishChumMistlands.RequiredItems.Add("LinenThread", 2);
            FishChumMistlands.CraftAmount = 10;
            FishChumMistlands.Configurable = Configurability.Recipe;
            ItemManager.PrefabManager.RegisterPrefab(PrefabManager.RegisterAssetBundle("fishingbaitbundle"), "FishChumMistlandsSpawner", false); 

            Item FishChumAshlands = new("fishingbaitbundle", "FishChumAshlands", "assets");
            FishChumAshlands.Name.English("Burnt Chum");
            FishChumAshlands.Description.English("perfect for attracting magmafish.");
            FishChumAshlands.Crafting.Add(CraftingTable.Cauldron, 3);
            FishChumAshlands.RequiredItems.Add("FishingBait", 5);
            FishChumAshlands.RequiredItems.Add("SurtlingCore", 5);
            FishChumAshlands.RequiredItems.Add("Coal", 5);
            FishChumAshlands.RequiredItems.Add("LinenThread", 2);
            FishChumAshlands.CraftAmount = 10;
            FishChumAshlands.Configurable = Configurability.Recipe;
            ItemManager.PrefabManager.RegisterPrefab(PrefabManager.RegisterAssetBundle("fishingbaitbundle"), "FishChumAshlandsSpawner", false); 

            Item FishChumDeepnorth = new("fishingbaitbundle", "FishChumDeepnorth", "assets");
            FishChumDeepnorth.Name.English("Frosted Chum");
            FishChumDeepnorth.Description.English("perfect for attracting northern salmon.");
            FishChumDeepnorth.Crafting.Add(CraftingTable.Cauldron, 3);
            FishChumDeepnorth.RequiredItems.Add("FishingBait", 5);
            FishChumDeepnorth.RequiredItems.Add("BugMeat", 2);
            FishChumDeepnorth.RequiredItems.Add("MushroomJotunPuffs", 1);
            FishChumDeepnorth.RequiredItems.Add("LinenThread", 2);
            FishChumDeepnorth.CraftAmount = 10;
            FishChumDeepnorth.Configurable = Configurability.Recipe;
            ItemManager.PrefabManager.RegisterPrefab(PrefabManager.RegisterAssetBundle("fishingbaitbundle"), "FishChumDeepnorthSpawner", false); 
            
            Item FishChumOcean = new("fishingbaitbundle", "FishChumOcean", "assets");
            FishChumOcean.Name.English("Deepsea Chum");
            FishChumOcean.Description.English("perfect for attracting tuna and coral cod.");
            FishChumOcean.Crafting.Add(CraftingTable.Cauldron, 1);
            FishChumOcean.RequiredItems.Add("FishingBait", 5);
            FishChumOcean.RequiredItems.Add("Ooze", 3);
            FishChumOcean.RequiredItems.Add("LeatherScraps", 2);
            FishChumOcean.RequiredItems.Add("Dandelion", 5);
            FishChumOcean.CraftAmount = 10;
            FishChumOcean.Configurable = Configurability.Recipe;
            ItemManager.PrefabManager.RegisterPrefab(PrefabManager.RegisterAssetBundle("fishingbaitbundle"), "FishChumOceanSpawner", false); 

            Item SerpentChum = new("fishingbaitbundle", "SerpentChum");
            SerpentChum.Name.English("Serpent chum");
            SerpentChum.Description.English("Perfect for attracting serpents");
            SerpentChum.Crafting.Add(CraftingTable.Cauldron, 1);
            SerpentChum.RequiredItems.Add("WolfMeat", 1);
            SerpentChum.RequiredItems.Add("Bloodbag", 2);
            SerpentChum.RequiredItems.Add("LeatherScraps", 3);
            SerpentChum.RequiredItems.Add("Thistle", 2);
            SerpentChum.CraftAmount = 5;
            SerpentChum.Configurable = Configurability.Recipe;
            ItemManager.PrefabManager.RegisterPrefab(PrefabManager.RegisterAssetBundle("fishingbaitbundle"), "SerpentChumSpawner",
                false); 
            
            # endregion
            
            # region fishFood
            
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

            #endregion


            /*#region CreatureManager Example Code

            Creature wereBearBlack = new("werebear", "WereBearBlack")
            {
                Biome = Heightmap.Biome.Meadows,
                GroupSize = new CreatureManager.Range(1, 2),
                CheckSpawnInterval = 600,
                RequiredWeather = Weather.Rain | Weather.Fog,
                Maximum = 2
            };
            wereBearBlack.Localize()
                .English("Black Werebear")
                .German("Schwarzer Werbär")
                .French("Ours-Garou Noir");
            wereBearBlack.Drops["Wood"].Amount = new CreatureManager.Range(1, 2);
            wereBearBlack.Drops["Wood"].DropChance = 100f;

            Creature wereBearRed = new("werebear", "WereBearRed")
            {
                Biome = Heightmap.Biome.AshLands,
                GroupSize = new CreatureManager.Range(1, 1),
                CheckSpawnInterval = 900,
                AttackImmediately = true,
                RequiredGlobalKey = GlobalKey.KilledYagluth,
            };
            wereBearRed.Localize()
                .English("Red Werebear")
                .German("Roter Werbär")
                .French("Ours-Garou Rouge");
            wereBearRed.Drops["Coal"].Amount = new CreatureManager.Range(1, 2);
            wereBearRed.Drops["Coal"].DropChance = 100f;
            wereBearRed.Drops["Flametal"].Amount = new CreatureManager.Range(1, 1);
            wereBearRed.Drops["Flametal"].DropChance = 10f;

            #endregion*/


            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmony.PatchAll(assembly);
            SetupWatcher();
        }

        private void OnDestroy()
        {
            Config.Save();
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
                FishingBaitLogger.LogDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                FishingBaitLogger.LogError($"There was an issue loading your {ConfigFileName}");
                FishingBaitLogger.LogError("Please check your config entries for spelling and format!");
            }
        }


        #region ConfigOptions

        private static ConfigEntry<Toggle> _serverConfigLocked = null!;

        private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription =
                new(
                    description.Description +
                    (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                    description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);
            //var configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
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
        class AcceptableShortcuts : AcceptableValueBase
        {
            public AcceptableShortcuts() : base(typeof(KeyboardShortcut))
            {
                
            }

            public override object Clamp(object value) => value;
            public override bool IsValid(object value) => true;

            public override string ToDescriptionString() =>
                "# Acceptable values: " + string.Join(", ", KeyboardShortcut.AllKeyCodes);
        }

        #endregion
    }
}