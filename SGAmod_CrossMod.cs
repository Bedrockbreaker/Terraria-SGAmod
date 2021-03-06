//#define WebmilioCommonsPresent
#define DEBUG
#define DefineHellionUpdate
#define Dimensions


using System;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.GameContent.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria.DataStructures;
using Terraria.GameContent.UI;
using Idglibrary;
using System.IO;
using System.Diagnostics;
using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.World;
using SGAmod.NPCs;
using SGAmod.NPCs.Wraiths;
using SGAmod.NPCs.Hellion;
using SGAmod.NPCs.SpiderQueen;
using SGAmod.NPCs.Murk;
using SGAmod.NPCs.Sharkvern;
using SGAmod.NPCs.Cratrosity;
using SGAmod.HavocGear.Items;
using SGAmod.HavocGear.Items.Weapons;
using SGAmod.HavocGear.Items.Accessories;
using SGAmod.Items;
using SGAmod.Items.Weapons;
using SGAmod.Items.Armors;
using SGAmod.Items.Accessories;
using SGAmod.Items.Consumable;
using SGAmod.Items.Weapons.Caliburn;
using SGAmod.UI;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using System.Reflection;
using SGAmod.Items.Weapons.Shields;
#if Dimensions
using SGAmod.Dimensions;
#endif

//using SubworldLibrary;

namespace SGAmod
{


	public partial class SGAmod : Mod
	{



		public static void BoostModdedDamage(Player player, float damage, int crit)
		{
			if (SGAmod.Calamity)
			{
				PropertyBoostModdedDamage = new ModdedDamage(player, damage, crit);
			}
		}

		public static ModdedDamage PropertyBoostModdedDamage
		{
			set
			{

				if (SGAmod.Calamity)
				{

					CalamityPlayer calply = value.player.GetModPlayer<CalamityPlayer>();
					calply.throwingDamage += value.damage;
					calply.throwingCrit += value.crit;
				}
			}
		}

		public static void CalamityNoRevengenceNoDeathNoU()
		{
			if (SGAmod.Calamity)
			{
				bool nothing = CalamityFlipoffRevengence;
			}
		}

		public static bool CalamityFlipoffRevengence
		{
			get
			{
				Player player = Main.player[Main.myPlayer];

				if (!SGAmod.Calamity)
					return false;

				CalamityPlayer calply = player.GetModPlayer<CalamityPlayer>();
				if (CalamityMod.World.CalamityWorld.revenge)
					ModContent.GetInstance<CalamityMod.Items.DifficultyItems.Revenge>().UseItem(player);
				if (CalamityMod.World.CalamityWorld.death)
					ModContent.GetInstance<CalamityMod.Items.DifficultyItems.Death>().UseItem(player);

				return true;
			}
		}

		public override void PostSetupContent()
		{

			musicToItemReference.Clear();
			itemToMusicReference.Clear();

			FieldInfo itemToMusicField = typeof(SoundLoader).GetField("itemToMusic", BindingFlags.Static | BindingFlags.NonPublic);//Found on the TML discord, helpful, Thanks JopoJelly!
			FieldInfo musicToItemField = typeof(SoundLoader).GetField("musicToItem", BindingFlags.Static | BindingFlags.NonPublic);

			itemToMusicReference = (Dictionary<int, int>)itemToMusicField.GetValue(null);
			musicToItemReference = (Dictionary<int, int>)musicToItemField.GetValue(null);

			Calamity = ModLoader.GetMod("CalamityMod") != null;

			overpoweredMod = 0;// (ModLoader.GetMod("AFKPETS") != null ? 0.5f : 0) + (ModLoader.GetMod("AlchemistNPC") != null ? 1.5f : 0) +(ModLoader.GetMod("Luiafk") != null ? 2f : 0) +(ModLoader.GetMod("FargowiltasSouls") != null ? 2.5f : 0);
							   //Why do people still use Luiafk in legit playthroughs? I donno...

			Mod fargos = ModLoader.GetMod("Fargowiltas");
			if (fargos != null)
			{
				// AddSummon, order or value in terms of vanilla bosses, your mod internal name, summon  
				//item internal name, inline method for retrieving downed value, price to sell for in copper
				fargos.Call("AddSummon", 0.05f, "SGAmod", "WraithCoreFragment", (Func<bool>)(() => SGAWorld.downedWraiths > 0), Item.buyPrice(0, 2, 0, 0));
				fargos.Call("AddSummon", 3.5f, "SGAmod", "AcidicEgg", (Func<bool>)(() => SGAWorld.downedSpiderQueen), Item.buyPrice(0, 1, 5, 0));
				fargos.Call("AddSummon", 5.5f, "SGAmod", "RoilingSludge", (Func<bool>)(() => SGAWorld.downedMurk > 1), Item.buyPrice(0, 2, 50, 0));
				fargos.Call("AddSummon", 6.4f, "SGAmod", "WraithCoreFragment2", (Func<bool>)(() => SGAWorld.downedWraiths > 1), Item.buyPrice(0, 5, 0, 0));
				fargos.Call("AddSummon", 6.5f, "SGAmod", "Nineball", (Func<bool>)(() => SGAWorld.downedCirno), Item.buyPrice(0, 5, 0, 0));
				fargos.Call("AddSummon", 9.5f, "SGAmod", "ConchHorn", (Func<bool>)(() => SGAWorld.downedSharkvern), Item.buyPrice(0, 5, 0, 0));
				fargos.Call("AddSummon", 10.5f, "SGAmod", "TerrariacoCrateBase", (Func<bool>)(() => SGAWorld.downedCratrosity), Item.buyPrice(0, 0, 50, 0));
				fargos.Call("AddSummon", 11.25f, "SGAmod", "Mechacluskerf", (Func<bool>)(() => SGAWorld.downedTPD), Item.buyPrice(0, 7, 50, 0));
				fargos.Call("AddSummon", 11.85f, "SGAmod", "TruelySusEye", (Func<bool>)(() => SGAWorld.downedHarbinger), Item.buyPrice(0, 2, 50, 0));
				fargos.Call("AddSummon", 14.8f, "SGAmod", "WraithCoreFragment3", (Func<bool>)(() => SGAWorld.downedWraiths > 2), Item.buyPrice(0, 10, 0, 0));
				fargos.Call("AddSummon", 15f, "SGAmod", "SalvagedCrate", (Func<bool>)(() => SGAWorld.downedCratrosityPML), Item.buyPrice(0, 50, 0, 0));
				fargos.Call("AddSummon", 16f, "SGAmod", "Prettygel", (Func<bool>)(() => SGAWorld.downedSPinky), Item.buyPrice(0, 15, 0, 0));
				fargos.Call("AddSummon", 17.5f, "SGAmod", "HellionSummon", (Func<bool>)(() => SGAWorld.downedHellion > 0), Item.buyPrice(5, 0, 0, 0));
			}
			Mod bossList = ModLoader.GetMod("BossChecklist");
			if (bossList != null)
			{
				//bossList.Call("AddBoss", "TPD", 5.5f, (Func<bool>)(() => ExampleWorld.SGAWorld.downedTPD));
				//bossList.Call("AddBossWithInfo", "Copper Wraith", 0.05f, (Func<bool>)(() => (SGAWorld.downedWraiths > 0)), string.Format("Use a [i:{0}] at anytime, defeat this boss to unlock crafting the furnace,bars, and anything else made there", ItemType("WraithCoreFragment")));
				bossList.Call("AddBoss", 0.05f, ModContent.NPCType<CopperWraith>(), this, "Copper Wraith", (Func<bool>)(() => (SGAWorld.downedWraiths > 0)), ModContent.ItemType<WraithCoreFragment>(), new List<int>() { }, new List<int>() { ModContent.ItemType<WraithFragment>(), ModContent.ItemType<WraithFragment2>(), ItemID.CopperOre, ItemID.TinOre, ItemID.IronOre, ItemID.LeadOre, ItemID.SilverOre, ItemID.TungstenOre, ItemID.GoldOre, ItemID.PlatinumOre }, "Use a [i:" + ItemType("WraithCoreFragment") + "] at anytime, will also spawn should you craft too many bars at a furnace before beating it", "Copper Wraith makes a hasty retreat", "SGAmod/NPCs/Wraiths/CopperWraithLog");
				bossList.Call("AddMiniBoss", 2.9f, ModContent.NPCType<CaliburnGuardian>(), this, "The Caliburn Guardians", (Func<bool>)(() => SGAWorld.downedCaliburnGuardians > 2), new List<int>() { }, new List<int>() { }, new List<int>() { ModContent.ItemType<CaliburnTypeA>(), ModContent.ItemType<CaliburnTypeB>(), ModContent.ItemType<CaliburnTypeC>() }, "Find Caliburn Alters in Dank Shrines Underground and right click them to fight a Caliburn Spirit, after beating a spirit you can retrive your reward by breaking the Alter; each guardian is stronger than the previous", "The Caliburn Spirit returns to its slumber");
				bossList.Call("AddBoss", 3.5f, ModContent.NPCType<SpiderQueen>(), this, "Spider Queen", (Func<bool>)(() => SGAWorld.downedSpiderQueen), new List<int>() { }, new List<int>(ModContent.ItemType<AcidicEgg>()) { }, new List<int>() { ModContent.ItemType<VialofAcid>(), ModContent.ItemType<AmberGlowSkull>(), ModContent.ItemType<CorrodedShield>() }, "Use a [i: " + ItemType("AcidicEgg") + "] underground, anytime", "The Spider Queen will be feasting quite well tonight", "SGAmod/NPCs/SpiderQueen/SpiderQueenLog");
				bossList.Call("AddMiniBoss", 5.4f, ModContent.NPCType<BossFlyMiniboss1>(), this, "Killer Fly Swarm", (Func<bool>)(() => SGAWorld.downedMurk > 0), new List<int>() { ModContent.ItemType<RoilingSludge>() }, new List<int>() { }, new List<int>() { ModContent.ItemType<RoilingSludge>() }, "Use a [i:" + ItemType("RoilingSludge") + "] in the jungle");
				bossList.Call("AddBoss", 5.5f, ModContent.NPCType<Murk>(), this, "Murk", (Func<bool>)(() => SGAWorld.downedMurk > 1), new List<int>() { ModContent.ItemType<RoilingSludge>() }, new List<int>() { }, new List<int>() { ModContent.ItemType<MurkBossBag>(), ModContent.ItemType<MudAbsorber>(), ModContent.ItemType<MurkyGel>(), ModContent.ItemType<MurkFlail>(), ModContent.ItemType<Mudmore>(), ModContent.ItemType<Mossthorn>(), ModContent.ItemType<Landslide>(), ModContent.ItemType<SwarmGrenade>() }, "Use a [i:" + ItemType("RoilingSludge") + "] in the jungle after killing the fly swarm", "Murk slinks back into the depths of the jungle");

				bossList.Call("AddBoss", 6.4f, ModContent.NPCType<CobaltWraith>(), this, "Cobalt Wraith", (Func<bool>)(() => (SGAWorld.downedWraiths > 1)), ModContent.ItemType<WraithCoreFragment2>(), new List<int>() { }, new List<int>() { ModContent.ItemType<WraithFragment4>(), ItemID.Hellstone, ItemID.SoulofLight, ItemID.SoulofNight, ItemID.PalladiumOre, ItemID.CobaltOre, ItemID.MythrilOre, ItemID.OrichalcumOre, ItemID.AdamantiteOre, ItemID.TitaniumOre }, "Use a [i:" + ItemType("WraithCoreFragment2") + "] at anytime, defeat this boss to unlock crafting a hardmode anvil, as well as anything crafted at one", "Cobalt Wraith completes its mission", "SGAmod/NPCs/Wraiths/CobaltWraithLog");
				bossList.Call("AddBoss", 6.45f, ModContent.NPCType<Murk>(), this, "Murk-Lord of the Flies", (Func<bool>)(() => SGAWorld.GennedVirulent), new List<int>() { ModContent.ItemType<RoilingSludge>() }, new List<int>() { }, new List<int>() { ModContent.ItemType<MurkBossBag>(), ModContent.ItemType<MudAbsorber>(), ModContent.ItemType<MurkyGel>(), ModContent.ItemType<MurkFlail>(), ModContent.ItemType<Mudmore>(), ModContent.ItemType<Mossthorn>(), ModContent.ItemType<Landslide>(), ModContent.ItemType<SwarmGrenade>() }, "Use a [i:" + ItemType("RoilingSludge") + "] in the jungle during hardmode and after killing the fly swarm, defeating this buffed version causes a new ore to generate", "Empowered Murk slinks back into the depths of the jungle");
				bossList.Call("AddBoss", 6.5f, ModContent.NPCType<Cirno>(), this, "Cirno", (Func<bool>)(() => SGAWorld.downedCirno), new List<int>() { ModContent.ItemType<Nineball>() }, new List<int>() { }, new List<int>() { ModContent.ItemType<CirnoWings>(), ModContent.ItemType<CryostalBar>(), ModContent.ItemType<IceScepter>(), ModContent.ItemType<Snowfall>(), ModContent.ItemType<RubiedBlade>(), ModContent.ItemType<Starburster>(), ModContent.ItemType<IcicleFall>() }, "Use a [i:" + ItemType("Nineball") + "] in in the snow biome during the day", "Cirno retains their title of 'The Strongest'");
				bossList.Call("AddMiniBoss", 9.1f, ModContent.NPCType<CaliburnGuardianHardmode>(), this, "Wrath of Caliburn", (Func<bool>)(() => SGAWorld.downedCaliburnGuardianHardmode), new List<int>() { ModContent.ItemType<CaliburnCompess>() }, new List<int>() { }, new List<int>() { ModContent.ItemType<CaliburnTypeA>(), ModContent.ItemType<CaliburnTypeB>(), ModContent.ItemType<CaliburnTypeC>() }, "Use a [i:" + ItemType("CaliburnCompess") + "] in Dank Shrines in hardmode", "The Caliburn Spirit returns to its slumber");
				bossList.Call("AddBoss", 9.5f, ModContent.NPCType<SharkvernHead>(), this, "Sharkvern", (Func<bool>)(() => SGAWorld.downedSharkvern), new List<int>() { ModContent.ItemType<ConchHorn>() }, new List<int>() { }, new List<int>() { ModContent.ItemType<SerratedTooth>(), ModContent.ItemType<SharkTooth>(), ModContent.ItemType<Jaws>(), ModContent.ItemType<SnappyShark>(), ModContent.ItemType<SkytoothStorm>(), ModContent.ItemType<SharkBait>(), ItemID.Starfish, ItemID.Seashell, ItemID.Coral, ItemID.SharkFin, ItemID.SoulofFlight }, "Use a [i:" + ItemType("ConchHorn") + "] at the ocean", "The Sharkvern retreats back into seclusion","SGAmod/NPCs/Sharkvern/Sharkvern");
				bossList.Call("AddBoss", 10.5f, ModContent.NPCType<Cratrosity>(), this, "Cratrosity", (Func<bool>)(() => SGAWorld.downedCratrosity), new List<int>() { ModContent.ItemType<TerrariacoCrateBase>(), ItemID.GoldenKey, ItemID.NightKey, ItemID.LightKey }, new List<int>() { }, new List<int>() { ModContent.ItemType<IdolOfMidas>(), ModContent.ItemType<TerrariacoCrateKey>(), ModContent.ItemType<CrateBossWeaponMelee>(), ModContent.ItemType<CrateBossWeaponRanged>(), ModContent.ItemType<CrateBossWeaponMagic>(), ModContent.ItemType<CrateBossWeaponSummon>(), ModContent.ItemType<CrateBossWeaponThrown>(), ModContent.ItemType<TF2Emblem>() }, "Right Click a [i:" + ItemType("TerrariacoCrateBase") + "] while you have any of the listed keys in your inventory at night", "All players have paid up their lives to microtransactions", "SGAmod/NPCs/Cratrosity/CratrosityLog");
				bossList.Call("AddBoss", 11.25f, ModContent.NPCType<TPD>(), this, "Twin Prime Destroyers", (Func<bool>)(() => SGAWorld.downedTPD), new List<int>() { ModContent.ItemType<Mechacluskerf>() }, new List<int>() { }, new List<int>() { ItemID.ChlorophyteBar, ItemID.ShroomiteBar, ItemID.SpectreBar, ItemID.Ectoplasm, ModContent.ItemType<StarMetalMold>() }, "Use a [i:" + ItemType("Mechacluskerf") + "] anywhere at night");
				bossList.Call("AddBoss", 11.85f, ModContent.NPCType<Harbinger>(), this, "Doom Harbinger", (Func<bool>)(() => SGAWorld.downedHarbinger), new List<int>() { ModContent.ItemType<TruelySusEye>() }, new List<int>() { }, new List<int>() { }, "Can spawn randomly at the start of night after golem is beaten, the Old One's Army event is finished on tier 3, and the Martians are beaten, or use a [i:" + ItemType("TruelySusEye") + "]. Defeating him will allow the cultists to spawn (Single Player Only)");
				bossList.Call("AddBoss", 12.5f, ModContent.NPCType<LuminiteWraith>(), this, "Luminite Wraith", (Func<bool>)(() => (SGAWorld.downedWraiths > 2)), new List<int>() { ModContent.ItemType<WraithCoreFragment3>() }, new List<int>() { }, new List<int>() { ItemID.LunarCraftingStation }, "Use a [i:" + ItemType("WraithCoreFragment3") + "], defeat this boss to get the Ancient Manipulator.", "", "SGAmod/NPCs/Wraiths/LWraithLog");
				bossList.Call("AddMiniBoss", 14.5f, ModContent.NPCType<PrismBanshee>(), this, "Prismatic Banshee", (Func<bool>)(() => SGAWorld.downedPrismBanshee), new List<int>() { }, new List<int>() { }, new List<int>() { ModContent.ItemType<IlluminantEssence>() }, "Find it's seed spawning in underground in the Hallow after Moonlord's defeat, if the seed is not destroyed in time the Prism Banshee will hatch. Prismic Banshee's defeat makes the seed spawn far less often and allows Illuminant Essence to drop", "Banshee has left");
				bossList.Call("AddBoss", 14.8f, ModContent.NPCType<LuminiteWraith>(), this, "Luminite Wraith (Rematch)", (Func<bool>)(() => (SGAWorld.downedWraiths > 3)), new List<int>() { ModContent.ItemType<WraithCoreFragment3>() }, new List<int>() { }, new List<int>() { ModContent.ItemType<CosmicFragment>(), ItemID.FragmentNebula, ItemID.FragmentVortex, ItemID.FragmentStardust, ItemID.FragmentSolar, ItemID.LunarBar, ItemID.LunarOre }, "Use a [i:" + ItemType("WraithCoreFragment3") + "] after the first fight when Moonlord is defeated to issue a rematch; the true battle begins...", "", "SGAmod/NPCs/Wraiths/LWraithLog2");
				bossList.Call("AddBoss", 15f, ModContent.NPCType<Cratrogeddon>(), this, "Cratrogeddon", (Func<bool>)(() => SGAWorld.downedCratrosityPML), new List<int>() { ModContent.ItemType<SalvagedCrate>(), ItemID.TempleKey }, new List<int>() { }, new List<int>() { ModContent.ItemType<TerrariacoCrateKeyUber>(), ModContent.ItemType<MoneySign>() }, "Right Click a [i:" + ItemType("SalvagedCrate") + "] while you have a [i:" + ItemID.TempleKey + "] in your inventory at night", "All players have paid up their lives to microtransactions, again", "SGAmod/NPCs/Cratrosity/CratrosityLog");
				bossList.Call("AddBoss", 16f, ModContent.NPCType<SPinky>(), this, "Supreme Pinky", (Func<bool>)(() => SGAWorld.downedSPinky), new List<int>() { ModContent.ItemType<Prettygel>() }, new List<int>() { }, new List<int>() { ModContent.ItemType<LunarSlimeHeart>(), ModContent.ItemType<LunarRoyalGel>() }, "Use a [i:" + ItemType("Prettygel") + "] at night", "Supreme Pinky is content with the justice they have dealt");
				bossList.Call("AddBoss", 17.5f, ModContent.NPCType<Hellion>(), this, "Helon 'Hellion' Weygold", (Func<bool>)(() => SGAWorld.downedHellion > 1), new List<int>() { ModContent.ItemType<HellionSummon>() }, new List<int>() { }, new List<int>() { ModContent.ItemType<CodeBreakerHead>(), ModContent.ItemType<ByteSoul>(), ModContent.ItemType<DrakeniteBar>() }, "Use a [i:" + ItemType("HellionSummon") + "]. Talk to Draken when the time is right... (Expert Only)");

				//CaliburnCompess
				//bossList.Call("AddMiniBossWithInfo", "The Caliburn Guardians", 1.4f, (Func<bool>)(() => SGAWorld.downedCaliburnGuardians > 2), "Find Caliburn Alters in Dank Shrines Underground and right click them to fight a Caliburn Spirit, after beating a sprite you can retrive your reward by breaking the Alter; each guardian is stronger than the previous");
				//bossList.Call("AddBossWithInfo", "Spider Queen", 3.5f, (Func<bool>)(() => SGAWorld.downedSpiderQueen), string.Format("Use a [i:{0}] underground, anytime", ItemType("AcidicEgg")));
				//bossList.Call("AddMiniBossWithInfo", "Killer Fly Swarm", 5.4f, (Func<bool>)(() => SGAWorld.downedMurk > 0), string.Format("Use a [i:{0}] in the jungle", ItemType("RoilingSludge")));
				//bossList.Call("AddBossWithInfo", "Murk", 5.5f, (Func<bool>)(() => SGAWorld.downedMurk > 1), string.Format("Use a [i:{0}] in the jungle after killing the fly swarm", ItemType("RoilingSludge")));
				//bossList.Call("AddBossWithInfo", "Murk (Hardmode)", 6.5f, (Func<bool>)(() => SGAWorld.downedMurk > 1 && SGAWorld.GennedVirulent), string.Format("Use a [i:{0}] in the jungle in Hardmode (fly swarm must be defeated), defeating this buffed version causes a new ore to generate", ItemType("RoilingSludge")));
				//bossList.Call("AddBossWithInfo", "Cirno", 6.5f, (Func<bool>)(() => SGAWorld.downedCirno), string.Format("Use a [i:{0}] in the snow biome during the day", ItemType("Nineball")));
				//bossList.Call("AddBossWithInfo", "Cobalt Wraith", 6.6f, (Func<bool>)(() => (SGAWorld.downedWraiths > 1)), string.Format("Use a [i:{0}] at anytime, defeat this boss to unlock crafting a hardmode anvil, as well as anything crafted at one", ItemType("WraithCoreFragment2")));
				//bossList.Call("AddBossWithInfo", "Sharkvern", 9.5f, (Func<bool>)(() => SGAWorld.downedSharkvern), string.Format("Use a [i:{0}] at the ocean", ItemType("ConchHorn")));
				//bossList.Call("AddBossWithInfo", "Cratrosity", 10.5f, (Func<bool>)(() => SGAWorld.downedCratrosity), string.Format("Use any key that is not a [i:{1}] with a [i:{0}] at night, get a [i:{2}] from the merchant to allow enemies to drop [i:{0}], you can use different keys to get a customized boss", ItemType("TerrariacoCrateBase"), ItemType("TerrariacoCrateKey"), ItemType("PremiumUpgrade")));
				//bossList.Call("AddBossWithInfo", "Twin Prime Destroyers", 11.25f, (Func<bool>)(() => SGAWorld.downedTPD), string.Format("Use a [i:{0}] anywhere at night", ItemType("Mechacluskerf")));
				//bossList.Call("AddBossWithInfo", "Doom Harbinger", 11.85, (Func<bool>)(() => SGAWorld.downedHarbinger), string.Format("Can spawn randomly at the start of night after golem is beaten, the Old One's Army event is finished on tier 3, and the Martians are beaten, defeating him will allow the cultists to spawn (Single Player Only)", ItemType("Prettygel")));
				//bossList.Call("AddBossWithInfo", "Luminite Wraith", 12.5f, (Func<bool>)(() => (SGAWorld.downedWraiths > 2)), string.Format("Use a [i:{0}], defeat this boss to get the Ancient Manipulator", ItemType("WraithCoreFragment3")));
				//bossList.Call("AddBossWithInfo", "Luminite Wraith (Rematch)", 14.8f, (Func<bool>)(() => (SGAWorld.downedWraiths > 3)), string.Format("Use a [i:{0}] after the first fight when Moonlord is defeated to issue a rematch; the true battle begins...", ItemType("WraithCoreFragment3")));
				//bossList.Call("AddBossWithInfo", "Cratrogeddon", 15f, (Func<bool>)(() => SGAWorld.downedCratrosityPML), string.Format("Use a [i:{1}] with a [i:{0}] at night", ItemType("SalvagedCrate"), ItemID.TempleKey));
				//bossList.Call("AddBossWithInfo", "Supreme Pinky", 16f, (Func<bool>)(() => SGAWorld.downedSPinky), string.Format("Use a [i:{0}] anywhere at night", ItemType("Prettygel")));
				//bossList.Call("AddBossWithInfo", "Helon 'Hellion' Weygold", 17.5f, (Func<bool>)(() => Main.LocalPlayer.GetModPlayer<SGAPlayer>().downedHellion > 1), string.Format("Talk to Draken when the time is right... (Expert Only)"));

			}

			Mod yabhb = ModLoader.GetMod("FKBossHealthBar");
			if (yabhb != null)
			{
				yabhb.Call("hbStart");
				yabhb.Call("hbFinishMultiple", NPCType("BossCopperWraith"), NPCType("BossCopperWraith"));
				yabhb.Call("hbStart");
				yabhb.Call("hbFinishMultiple", NPCType("SPinkyClone"), NPCType("SPinkyClone"));
				yabhb.Call("hbStart");
				yabhb.Call("hbFinishMultiple", NPCType("Harbinger"), NPCType("Harbinger"));
				yabhb.Call("RegisterHealthBarMini", NPCType("PrismBanshee"));
				yabhb.Call("RegisterHealthBarMini", NPCType("CirnoHellion"));
			}

			SGAmod.EnchantmentCatalyst.Add(ItemID.Amethyst, new EnchantmentCraftingMaterial(2, 50, "Produces a weak, but stable enchantment"));
			SGAmod.EnchantmentCatalyst.Add(ItemID.Sapphire, new EnchantmentCraftingMaterial(3, 80, "Produces a balanced enchantment, water-related enchantments are more common"));
			SGAmod.EnchantmentCatalyst.Add(ItemID.Emerald, new EnchantmentCraftingMaterial(3, 80, "Produces a balanced enchantment, earthen-related enchantments are more common"));
			SGAmod.EnchantmentCatalyst.Add(ItemID.Ruby, new EnchantmentCraftingMaterial(3, 80, "Produces a balanced enchantment, fire-related enchantments are more common"));
			SGAmod.EnchantmentCatalyst.Add(ItemID.Topaz, new EnchantmentCraftingMaterial(5, 120, "Produces a stronger enchantment at a slightly higher cost, risk of bad enchantments are higher"));
			SGAmod.EnchantmentCatalyst.Add(ItemID.Amber, new EnchantmentCraftingMaterial(6, 75, "The enchantment from this one are powerful and fairly cheap, but highly errotic"));
			SGAmod.EnchantmentCatalyst.Add(ItemID.Diamond, new EnchantmentCraftingMaterial(7, 200, "Produces a stronger, more costly enchantment"));

			SGAmod.EnchantmentFocusCrystal.Add(ItemID.ShinyStone, new EnchantmentCraftingMaterial(20, 250, "guarantees one extra enchantment"));
			SGAmod.EnchantmentFocusCrystal.Add(ModContent.ItemType<EntropyTransmuter>(), new EnchantmentCraftingMaterial(15, 200, "One enchantment will always grant entropy bonuses"));
			SGAmod.EnchantmentFocusCrystal.Add(ModContent.ItemType<CalamityRune>(), new EnchantmentCraftingMaterial(20, 200, "Enchantments may grant bonuses to Apocalypticals"));

			//SGAWorld.downedCopperWraith==0 ? true : false)
			Idglib.AbsentItemDisc.Add(this.ItemType("Tornado"), "5% to drop from Wyverns after Golem");
			Idglib.AbsentItemDisc.Add(this.ItemType("Upheaval"), "20% to drop from Golem");
			Idglib.AbsentItemDisc.Add(this.ItemType("Powerjack"), "10% to drop from Wall of Flesh");
			Idglib.AbsentItemDisc.Add(this.ItemType("SwordofTheBlueMoon"), "10% (20% in expert mod) drop from Moon Lord");
			Idglib.AbsentItemDisc.Add(this.ItemType("Fieryheart"), "This item is a secret...");
			Idglib.AbsentItemDisc.Add(this.ItemType("Sunbringer"), "This item is not obtainable yet");
			Idglib.AbsentItemDisc.Add(this.ItemType("StoneBarrierStaff"), "Sold by the Dryad after defeating Spider Queen");
			Idglib.AbsentItemDisc.Add(this.ItemType("PeacekeepersDuster"), "Sold by the Traveling Merchant in Hardmode");
			Idglib.AbsentItemDisc.Add(this.ItemType("SecondCylinder"), "Sold by the Arms Dealer in a world with tin, or in Hardmode");
			Idglib.AbsentItemDisc.Add(this.ItemType("GunBarrelParts"), "Sold by the Arms Dealer in a world with copper, or in Hardmode");

			Idglib.AbsentItemDisc.Add(this.ItemType("PrimordialSkull"), "Sold by the Dergon (Draken)");
			Idglib.AbsentItemDisc.Add(this.ItemType("CaliburnCompess"), "Sold by the Dergon (Draken)");
			Idglib.AbsentItemDisc.Add(this.ItemType("EmptyCharm"), "Sold by the Dergon (Draken)");
			Idglib.AbsentItemDisc.Add(this.ItemType("SOATT"), "Sold by the Dergon (Draken)");
			Idglib.AbsentItemDisc.Add(this.ItemType("RedManaStar"), "Sold by the Dergon (Draken)");

			Idglib.AbsentItemDisc.Add(this.ItemType("OmegaSigil"), "Drops from Betsy");
			Idglib.AbsentItemDisc.Add(this.ItemType("SybariteGem"), "Drops from enemies worth 3 or more gold rarely, some greed infused enemies are more likely to drop it");
			Idglib.AbsentItemDisc.Add(this.ItemType("EntropyTransmuter"), "Sold By Draken");

			Idglib.AbsentItemDisc.Add(this.ItemType("FieryShard"), "Drops from Hell/Lava bats and Red Devils");
			Idglib.AbsentItemDisc.Add(this.ItemType("FrigidShard"), "Drops from icicle blocks");
			Idglib.AbsentItemDisc.Add(this.ItemType("VialofAcid"), "Drops from Spider Queen");
			Idglib.AbsentItemDisc.Add(this.ItemType("MurkyGel"), "Drops from Murk and Dank Slimes");
			Idglib.AbsentItemDisc.Add(this.ItemType("DankCore"), "Is found in Dank Shrines, including crates fished from there");
			Idglib.AbsentItemDisc.Add(this.ItemType("DankWood"), "Is found in Dank Shrines, including crates fished from there");
			Idglib.AbsentItemDisc.Add(this.ItemType("IceFairyDust"), "Drops from Ice Fairies, they spawn during the day on the surface biome in hardmode");
			Idglib.AbsentItemDisc.Add(this.ItemType("Entrophite"), "Is created via an Entropy Transmuter");
			Idglib.AbsentItemDisc.Add(this.ItemType("EldritchTentacle"), "Drops from Moonlord");
			Idglib.AbsentItemDisc.Add(this.ItemType("IlluminantEssence"), "Drops from Prismic Banshee, and after her defeat, enemies, the glowing ones... Obviously");
			Idglib.AbsentItemDisc.Add(this.ItemType("LunarRoyalGel"), "Drops from Supreme Pinky");
			Idglib.AbsentItemDisc.Add(this.ItemType("CosmicFragment"), "Drops from Luminite Wraith");
			Idglib.AbsentItemDisc.Add(this.ItemType("CryostalBar"), "Drops from Cirno");
			Idglib.AbsentItemDisc.Add(this.ItemType("MoneySign"), "Drops from Cratogeddon");
			Idglib.AbsentItemDisc.Add(this.ItemType("ByteSoul"), "Drops from Hellion Core's 'arms'");
			Idglib.AbsentItemDisc.Add(this.ItemType("StarMetalMold"), "Drops from Twin Prime Destroyers");
			Idglib.AbsentItemDisc.Add(this.ItemType("DrakeniteBar"), "Drops Hellion's 2nd form");

			/*SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("PrismalBooster"), "Recolor because lack of spriters");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("NovusCore"), "Don't complain about the placeholder sprite, my spriters wouldn't do jack shit about despite my efforts");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("NoviteCore"), "Don't complain about the placeholder sprite, my spriters wouldn't do jack shit about despite my efforts");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("NeckONerves"), "I made due with what I fucking had for sprites");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("RingOfRespite"), "I made due with what I fucking had for sprites");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("BrokenImmortalityCore"), "I made due with what I fucking had for sprites");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("AncientFabricItem"), "I made due with what I fucking had for sprites");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("NoviteChestplate"), "They didn't finish it, all they did was apploguise");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("NoviteLeggings"), "They didn't finish it, all they did was apploguise");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("BeserkerAuraStaff"), "I made due with what I fucking had for sprites");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("NonStationaryBunnyCannonLauncher"), "I made due with what I fucking had for sprites");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("NonStationarySnowballLauncher"), "I made due with what I fucking had for sprites");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("NonStationaryCannonLauncher"), "I made due with what I fucking had for sprites");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("GrippingGloves"), "Placeholder, fucking placeholder cus no one will help me with it! I can't sprite");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("HandlingGloves"), "Placeholder, fucking placeholder cus no one will help me with it! I can't sprite");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("HeartGuard"), "Placeholder, fucking placeholder cus no one will help me with it! I can't sprite");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("GoldenCog"), "Placeholder, fucking placeholder cus no one will help me with it! I can't sprite");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("JaggedOvergrownSpike"), "Placeholder, fucking placeholder cus no one will help me with it! I can't sprite");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("NadeFlail"), "Ugly ass piece of shit sprite edit");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("ClarityPotion"), "Placeholder, consider 'helping' rather than complaining about the sprite");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("RagnarokBrew"), "Placeholder, consider 'helping' rather than complaining about the sprite");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("RadCurePotion"), "Placeholder, consider 'helping' rather than complaining about the sprite");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("DragonsMightPotion"), "Placeholder, consider 'helping' rather than complaining about the sprite");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("TungstenBullet"), "Because I can't Get-C");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("AcidBullet"), "People to help me-A");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("BlazeBullet"), "At all, all they ever want-L");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("PrismicBullet"), "Is to be a part of winning team-A");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("PrismalBullet"), "The little guy doesn't matter-M");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("PortalBullet"), "They have no faith in him-I");
			SGAmod.StuffINeedFuckingSpritesFor.Add(ItemType("AimBotBullet"), "Only ever they want only to be a part of-TY");*/



		}
	}

}