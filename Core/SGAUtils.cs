//#define WebmilioCommonsPresent
#define DEBUG
#define DefineHellionUpdate
#define Dimensions


using System;
using System.Linq;
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
using SGAmod.Items.Weapons.SeriousSam;
using ReLogic.Graphics;
using Terraria.Utilities;
using System.Reflection;
#if Dimensions
using SGAmod.Dimensions;
#endif

//using SubworldLibrary;

namespace SGAmod
{

	/*public class Blank : Subworld
	{
		public override int width => 800;
		public override int height => 400;
		public override ModWorld modWorld => SGAWorld.Instance;

		public override SubworldGenPass[] tasks => new SubworldGenPass[]
		{
		new SubworldGenPass("Loading", 1f, progress =>
		{
			progress.Message = "Loading"; //Sets the text above the worldgen progress bar
            Main.worldSurface = Main.maxTilesY - 42; //Hides the underground layer just out of bounds
            Main.rockLayer = Main.maxTilesY; //Hides the cavern layer way out of bounds
        })
		};

		public override void Load()
		{
			Main.dayTime = true;
			Main.time = 27000;
			Main.worldRate = 0;
		}
	}*/

	//please don't touch
	public class UncraftClass
	{
		Point16 location;
		List<Point> possibleItems;
		Item targetitem;
		public int stackSize;
		int recipeIndex = 0;
		public bool uncraftable;

		static public List<int> BlackListedItems;

		static UncraftClass()
		{
			BlackListedItems = new List<int>();
			BlackListedItems.Add(ItemID.FragmentNebula);
			BlackListedItems.Add(ItemID.FragmentSolar);
			BlackListedItems.Add(ItemID.FragmentStardust);
			BlackListedItems.Add(ItemID.FragmentVortex);
			BlackListedItems.Add(SGAmod.Instance.ItemType("StarMetalBar"));
			BlackListedItems.Add(SGAmod.Instance.ItemType("WraithArrow"));
			BlackListedItems.Add(SGAmod.Instance.ItemType("ShadowJavelin"));
			BlackListedItems.Add(SGAmod.Instance.ItemType("SPinkyBagFake"));
		}

		public UncraftClass(Point16 location, Item item, int recipeIndex = 0, int offsetter = 0)
		{
			uncraftable = true;
			this.location = location;
			this.recipeIndex = recipeIndex;
			this.possibleItems = GetUncraftData(item, offsetter);
			targetitem = item;
		}

		public List<Point> GetUncraftData(Item item, int receipeoffsetter = 0)
		{
			List<Point> items = new List<Point>();
			if (!item.IsAir)
			{
				RecipeFinder finder = new RecipeFinder();
				finder.SetResult(item.type);
				List<Recipe> reclist = finder.SearchRecipes();

				if (reclist != null && reclist.Count > 0)
				{
					Recipe recipe = reclist[recipeIndex % reclist.Count];//Only the first recipe, for now

					stackSize = recipe.createItem.stack;

					if (stackSize <= item.stack && BlackListedItems.FirstOrDefault(search => search == item.type) == default)
					{

						List<List<int>> isGroup = new List<List<int>>();

						if (recipe.acceptedGroups.Count > 0)
						{
							for (int k = 0; k < recipe.acceptedGroups.Count; k += 1)
							{
								List<int> recipeGroupItems = RecipeGroup.recipeGroups[recipe.acceptedGroups[k]].ValidItems;

								for (int kk = 0; kk < recipeGroupItems.Count; kk += 1)
								{
									isGroup.Add(recipeGroupItems);
								}
							}
						}

						for (int k = 0; k < recipe.requiredItem.Length; k += 1)
						{
							if (!recipe.requiredItem[k].IsAir)
							{
								int chosenitem = recipe.requiredItem[k].type;

								for (int kk = 0; kk < isGroup.Count; kk += 1)
								{
									UnifiedRandom rando = new UnifiedRandom(receipeoffsetter + kk);
									List<int> subgroup = isGroup[kk];
									if (subgroup.FirstOrDefault(itemx => itemx == chosenitem) != default)
									{
										chosenitem = subgroup[rando.Next(subgroup.Count)];
										//Main.NewText("test " + receipeoffsetter % subgroup.Count);
									}
								}
								items.Add(new Point(chosenitem, recipe.requiredItem[k].stack));
								uncraftable = false;
							}

						}
					}

				}
			}
			return items;
		}

		public void Uncraft(Player player, int uncraftChance = 75)
		{
			if (uncraftable)
				return;

			foreach (Point item in possibleItems)
			{
				int ammount = 1;
				if (item.Y > 1)
					ammount = Main.rand.Next((int)(item.Y * (uncraftChance / 100f)), item.Y + 1);

				if (Main.rand.Next(0, 100) < uncraftChance)
					player.QuickSpawnItem(item.X, ammount);
			}
			targetitem.stack -= stackSize;
			if (targetitem.stack < 1)
				targetitem.TurnToAir();

		}

		public void Draw()
		{
			Vector2 position = (location.ToVector2() * 16) - Main.screenPosition;

			position.X -= possibleItems.Count * 16;
			position.X += 16;

			foreach (Point data in possibleItems)
			{

				Texture2D tex = Main.itemTexture[data.X];

				DrawAnimation anim = Main.itemAnimations[data.X];
				int frame = 0;
				int height = tex.Height;
				Vector2 size = tex.Size() / 2f;
				if (anim != null)
				{
					frame = anim.Frame;
					height = tex.Height / anim.FrameCount;
					size = new Vector2(tex.Width, height) / 2f;
				}

				Item anitem = new Item();
				anitem.SetDefaults(data.X);

				Main.spriteBatch.Draw(tex, position, new Rectangle(0, height * frame, tex.Width, height), anitem.modItem?.GetAlpha(Color.White) ?? Color.White, 0f, size, 1f, SpriteEffects.None, 0);
				if (data.Y > 0)
				{
					string str = data.Y.ToString();
					Vector2 size2 = Main.fontDeathText.MeasureString(str);
					Main.spriteBatch.DrawString(Main.fontMouseText, str, position + new Vector2(4, 4), Color.White);
				}

				position.X += 32;

			}

		}

	}

	public class PostDrawCollection
	{
		public Vector3 light;

		public PostDrawCollection(Vector3 light)
		{
			this.light = light;
		}
	}
	public static class TextureExtension
	{
		//https://stackoverflow.com/questions/44760512/xna-make-a-new-texture2d-out-of-another-texture2d
		/// <summary>
		/// Creates a new texture from an area of the texture.
		/// </summary>
		/// <param name="graphics">The current GraphicsDevice</param>
		/// <param name="rect">The dimension you want to have</param>
		/// <returns>The partial Texture.</returns>
		public static Texture2D CreateTexture(this Texture2D src, GraphicsDevice graphics, Rectangle rect)
		{
			Texture2D tex = new Texture2D(graphics, rect.Width, rect.Height);
			int count = rect.Width * rect.Height;
			Color[] data = new Color[count];
			src.GetData(0, rect, data, 0, count);
			tex.SetData(data);
			return tex;
		}
	}

	public static class SGAUtils
	{
		/*static private readonly FieldInfo _firstTileX = typeof(Lighting).GetField("firstTileX", BindingFlags.NonPublic | BindingFlags.Instance);
		static private readonly FieldInfo _firstTileY = typeof(Lighting).GetField("firstTileY", BindingFlags.NonPublic | BindingFlags.Instance);
		static private readonly FieldInfo _offScreenTiles = typeof(Lighting).GetField("offScreenTiles", BindingFlags.NonPublic | BindingFlags.Instance);
		public static void AddLight(int i, int j, float R, float G, float B)
		{
			int firstTileX = (int)_firstTileX.GetValue(null);
			int firstTileY = (int)_firstTileY.GetValue(null);
			int offScreenTiles = (int)_offScreenTiles.GetValue(null);
			Dictionary<Point16, ColorTriplet> tempLights = 

			if (Main.gamePaused || Main.netMode == 2 || i - firstTileX + offScreenTiles < 0 || i - firstTileX + offScreenTiles >= Main.screenWidth / 16 + offScreenTiles * 2 + 10 || j - firstTileY + offScreenTiles < 0 || j - firstTileY + offScreenTiles >= Main.screenHeight / 16 + offScreenTiles * 2 + 10 || tempLights.Count == maxTempLights)
			{
				return;
			}
			Point16 key = new Point16(i, j);
			if (tempLights.TryGetValue(key, out var value))
			{
				if (RGB)
				{
					if (value.r < R)
					{
						value.r = R;
					}
					if (value.g < G)
					{
						value.g = G;
					}
					if (value.b < B)
					{
						value.b = B;
					}
					tempLights[key] = value;
				}
				else
				{
					float num = (R + G + B) / 3f;
					if (value.r < num)
					{
						tempLights[key] = new ColorTriplet(num);
					}
				}
			}
			else
			{
				value = (RGB ? new ColorTriplet(R, G, B) : new ColorTriplet((R + G + B) / 3f));
				tempLights.Add(key, value);
			}
		}*/


		//Again, from Joost, thanks man
		public static Vector2 PredictiveAim(float speed, Vector2 origin, Vector2 target, Vector2 targetVelocity, bool ignoreY)
		{
			Vector2 vel = (ignoreY ? new Vector2(targetVelocity.X, 0) : targetVelocity);
			Vector2 predictedPos = target + targetVelocity + (vel * (Vector2.Distance(target, origin) / speed));
			predictedPos = target + targetVelocity + (vel * (Vector2.Distance(predictedPos, origin) / speed));
			predictedPos = target + targetVelocity + (vel * (Vector2.Distance(predictedPos, origin) / speed));
			return predictedPos;
		}

		public static int ItemToMusic(int itemtype)
		{
			int value;
			if (SGAmod.itemToMusicReference.TryGetValue(itemtype, out value))
			{
				return value;
			}
			else
			{
				return -1;
			}
		}

		public static int MusicToItem(int itemtype)
		{
			int value;
			if (SGAmod.musicToItemReference.TryGetValue(itemtype, out value))
			{
				return value;
			}
			else
			{
				return -1;
			}
		}

		//Fancy closest enemy method with weights system
		public static List<NPC> ClosestEnemies(Vector2 Center, float maxdist, Vector2 Center2 = default, List<Point> AddedWeight = default, bool checkWalls = true, bool checkCanChase = true)
		{
			maxdist *= maxdist;
			if (Center2 == default)
				Center2 = Center;

			if (AddedWeight == default)
				AddedWeight = new List<Point>();

			//List<NPC> closestnpcs = Main.npc.Where(testnpc => testnpc.active && testnpc.friendly && !testnpc.townNPC && !testnpc.dontTakeDamage && testnpc.CanBeChasedBy() &&
			//Collision.CheckAABBvLineCollision(testnpc.position, new Vector2(testnpc.width, testnpc.height), testnpc.Center, Center) && (testnpc.Center - Center2).Length() < maxdist).ToList();

			List<NPC> closestnpcs = new List<NPC>();
			for (int i = 0; i < Main.maxNPCs; i += 1)
			{
				NPC npc = Main.npc[i];
				float distvectX = (Center2.X - npc.Center.X) * (Center2.X - npc.Center.X);
				float distvectY = (Center2.Y - npc.Center.Y) * (Center2.Y - npc.Center.Y);
				float squaredDist = Math.Abs((distvectX + distvectY));
				if (Main.npc[i].active)
				{
					bool colcheck = !checkWalls || (Collision.CheckAABBvLineCollision(Main.npc[i].position, new Vector2(Main.npc[i].width, Main.npc[i].height), Main.npc[i].Center, Center)
	&& Collision.CanHit(Main.npc[i].Center, 0, 0, Center, 0, 0));
					if (!Main.npc[i].friendly && !Main.npc[i].townNPC && !Main.npc[i].dontTakeDamage && (!checkCanChase || Main.npc[i].CanBeChasedBy()) && colcheck
					&& squaredDist < maxdist)
					{
						closestnpcs.Add(Main.npc[i]);
					}
				}
			}

			//Sorter delegate based on distance, weights are accounted for
			Func<NPC, float> sortbydistance = delegate (NPC npc)
			 {
				 float distvectX = (Center2.X - npc.Center.X) * (Center2.X - npc.Center.X);
				 float distvectY = (Center2.Y - npc.Center.Y) * (Center2.Y - npc.Center.Y);
				 float squaredDist = Math.Abs((distvectX + distvectY));

				 float score = squaredDist;
				 Point weightedscore = AddedWeight.FirstOrDefault(npcid => npcid.X == npc.whoAmI);
				 score += weightedscore != default ? weightedscore.Y * Math.Abs(weightedscore.Y) : 0;

				 //Values of weight over 1000000 are simply "removed" from the sorter as invalid
				 if (weightedscore != default && weightedscore.Y >= 1000000)
					 score = 100000000;

				 return score;

			 };

			if (closestnpcs.Count < 1)
			{
				return null;
			}
			else
			{
				closestnpcs = closestnpcs.ToArray().OrderBy(sortbydistance).ToList();//Closest
				if (AddedWeight != default)
					closestnpcs.RemoveAll(npc => (int)sortbydistance(npc) == 100000000);//Dups be gone

				return closestnpcs;
			}
		}

		public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
		{
			return listToClone.Select(item => (T)item.Clone()).ToList();
		}
		public static Vector3 ToVector3(this Vector2 vector)
		{
			return new Vector3(vector.X, vector.Y, 0);
		}
		public static SGAPlayer SGAPly(this Player player)
		{
			return player.GetModPlayer<SGAPlayer>();
		}
		public static SGAprojectile SGAProj(this Projectile proj)
		{
			return proj.GetGlobalProjectile<SGAprojectile>();
		}
		public static SGAnpcs SGANPCs(this NPC npc)
		{
			return npc.GetGlobalNPC<SGAnpcs>();
		}

		public static bool NoInvasion(NPCSpawnInfo spawnInfo)
		{
			return !spawnInfo.invasion && ((!Main.pumpkinMoon && !Main.snowMoon) || spawnInfo.spawnTileY > Main.worldSurface || Main.dayTime) && (!Main.eclipse || spawnInfo.spawnTileY > Main.worldSurface || !Main.dayTime);
		}

		public static float ArrowSpeed(this Player player)
		{
			return player.HasBuff(BuffID.Archery) ? 1.20f : 1f;
		}

		public static bool ConsumeItemRespectInfiniteAmmoTypes(this Player player,int item,bool reverseOrder = false)
		{
			if (item == ItemID.EndlessMusketPouch || item == ItemID.EndlessQuiver)
				return false;

			return player.ConsumeItem(item, reverseOrder);
		}

		public static void SpawnCoins(Vector2 where, int ammount2, float explodespeed = 0f)
		{
			int ammount = ammount2;
			int[] subanditem;
			while (ammount > 0)
			{
				subanditem = new int[] { ItemID.CopperCoin, 1 };
				if (ammount >= 100)
					subanditem = new int[] { ItemID.SilverCoin, 100 };
				if (ammount >= 10000)
					subanditem = new int[] { ItemID.GoldCoin, 10000 };
				if (ammount >= 1000000)
					subanditem = new int[] { ItemID.PlatinumCoin, 1000000 };

				int item = Item.NewItem(where, Vector2.Zero, subanditem[0]);
				Main.item[item].velocity = new Vector2(Main.rand.NextFloat(-2f, 2f) * explodespeed, Main.rand.NextFloat(-0.75f, 0.75f) * explodespeed);
				ammount -= subanditem[1];


			}

		}

		public static void DrawFishingLine(Vector2 start, Vector2 end, Vector2 Velocity, Vector2 offset, float reel)
		{
			float pPosX = start.X;
			float pPosY = start.Y;

			Vector2 value = new Vector2(pPosX, pPosY);
			float projPosX = end.X - value.X;
			float projPosY = end.Y - value.Y;
			Math.Sqrt((double)(projPosX * projPosX + projPosY * projPosY));
			float rotation2 = (float)Math.Atan2((double)projPosY, (double)projPosX) - 1.57f;
			bool flag2 = true;
			if (projPosX == 0f && projPosY == 0f)
			{
				flag2 = false;
			}
			else
			{
				float projPosXY = (float)Math.Sqrt((double)(projPosX * projPosX + projPosY * projPosY));
				projPosXY = 12f / projPosXY;
				projPosX *= projPosXY;
				projPosY *= projPosXY;
				value.X -= projPosX;
				value.Y -= projPosY;
				projPosX = end.X - value.X;
				projPosY = end.Y - value.Y;
			}
			while (flag2)
			{
				float num = 12f;
				float num2 = (float)Math.Sqrt((double)(projPosX * projPosX + projPosY * projPosY));
				float num3 = num2;
				if (float.IsNaN(num2) || float.IsNaN(num3))
				{
					flag2 = false;
				}
				else
				{
					if (num2 < 20f)
					{
						num = num2 - 8f;
						flag2 = false;
					}
					num2 = 12f / num2;
					projPosX *= num2;
					projPosY *= num2;
					value.X += projPosX;
					value.Y += projPosY;
					projPosX = end.X - value.X;
					projPosY = end.Y - value.Y;
					if (num3 > 12f)
					{
						float num4 = 0.3f;
						float num5 = Math.Abs(Velocity.X) + Math.Abs(Velocity.Y);
						if (num5 > 16f)
						{
							num5 = 16f;
						}
						num5 = 1f - num5 / 16f;
						num4 *= num5;
						num5 = num3 / 80f;
						if (num5 > 1f)
						{
							num5 = 1f;
						}
						num4 *= num5;
						if (num4 < 0f)
						{
							num4 = 0f;
						}
						num5 = 1f - reel / 100f;
						num4 *= num5;
						if (projPosY > 0f)
						{
							projPosY *= 1f + num4;
							projPosX *= 1f - num4;
						}
						else
						{
							num5 = Math.Abs(Velocity.X) / 3f;
							if (num5 > 1f)
							{
								num5 = 1f;
							}
							num5 -= 0.5f;
							num4 *= num5;
							if (num4 > 0f)
							{
								num4 *= 2f;
							}
							projPosY *= 1f + num4;
							projPosX *= 1f - num4;
						}
					}
					rotation2 = (float)Math.Atan2((double)projPosY, (double)projPosX) - 1.57f;
					Microsoft.Xna.Framework.Color color2 = Lighting.GetColor((int)value.X / 16, (int)(value.Y / 16f), Color.AliceBlue);

					Main.spriteBatch.Draw(Main.fishingLineTexture, new Vector2(value.X - Main.screenPosition.X + (float)Main.fishingLineTexture.Width * 0.5f, value.Y - Main.screenPosition.Y + (float)Main.fishingLineTexture.Height * 0.5f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, Main.fishingLineTexture.Width, (int)num)), color2, rotation2, new Vector2((float)Main.fishingLineTexture.Width * 0.5f, 0f), 1f, SpriteEffects.None, 0f);
				}
			}

		}



		//These next 3 methods came from a cheats forum lol, but hey if I can use them in Terraria... (https://www.unknowncheats.me/forum/battlefield-4-a/143104-absolutely-accurate-aiming-prediction-correction-erros-theory.html)
		private static uint SolveCubic(double[] coeff, ref double[] x)
		{
			/* Adjust coefficients */

			double a1 = coeff[2] / coeff[3];
			double a2 = coeff[1] / coeff[3];
			double a3 = coeff[0] / coeff[3];

			double Q = (a1 * a1 - 3 * a2) / 9;
			double R = (2 * a1 * a1 * a1 - 9 * a1 * a2 + 27 * a3) / 54;
			double Qcubed = Q * Q * Q;
			double d = Qcubed - R * R;

			/* Three real roots */

			if (d >= 0)
			{
				double theta = Math.Acos(R / Math.Sqrt(Qcubed));
				double sqrtQ = Math.Sqrt(Q);

				x[0] = -2 * sqrtQ * Math.Cos(theta / 3) - a1 / 3;
				x[1] = -2 * sqrtQ * Math.Cos((theta + 2 * Math.PI) / 3) - a1 / 3;
				x[2] = -2 * sqrtQ * Math.Cos((theta + 4 * Math.PI) / 3) - a1 / 3;

				return (3);
			}

			/* One real root */

			else
			{
				double e = Math.Pow(Math.Sqrt(-d) + Math.Abs(R), 1.0 / 3.0);

				if (R > 0)
				{
					e = -e;
				}

				x[0] = (e + Q / e) - a1 / 3.0;

				return (1);
			}
		}

		public static uint SolveQuartic(double a, double b, double c, double d, double e, ref double[] x)
		{
			/* Adjust coefficients */

			double a1 = d / e;
			double a2 = c / e;
			double a3 = b / e;
			double a4 = a / e;

			/* Reduce to solving cubic equation */

			double q = a2 - a1 * a1 * 3 / 8;
			double r = a3 - a1 * a2 / 2 + a1 * a1 * a1 / 8;
			double s = a4 - a1 * a3 / 4 + a1 * a1 * a2 / 16 - 3 * a1 * a1 * a1 * a1 / 256;

			double[] coeff_cubic = new double[4];
			double[] roots_cubic = new double[3];
			double positive_root = 0;

			coeff_cubic[3] = 1;
			coeff_cubic[2] = q / 2;
			coeff_cubic[1] = (q * q - 4 * s) / 16;
			coeff_cubic[0] = -r * r / 64;

			uint nRoots = SolveCubic(coeff_cubic, ref roots_cubic);

			for (int i = 0; i < nRoots; i++)
			{
				if (roots_cubic[i] > 0)
				{
					positive_root = roots_cubic[i];
				}
			}

			/* Reduce to solving two quadratic equations */

			double k = Math.Sqrt(positive_root);
			double l = 2 * k * k + q / 2 - r / (4 * k);
			double m = 2 * k * k + q / 2 + r / (4 * k);

			nRoots = 0;

			if (k * k - l > 0)
			{
				x[nRoots + 0] = -k - Math.Sqrt(k * k - l) - a1 / 4;
				x[nRoots + 1] = -k + Math.Sqrt(k * k - l) - a1 / 4;

				nRoots += 2;
			}

			if (k * k - m > 0)
			{
				x[nRoots + 0] = +k - Math.Sqrt(k * k - m) - a1 / 4;
				x[nRoots + 1] = +k + Math.Sqrt(k * k - m) - a1 / 4;

				nRoots += 2;
			}

			return nRoots;
		}

		public static Vector3 PredictAimingPos(Vector3 position, Vector3 targetPosition, Vector3 targetVelocity, float bulletVelocity, float bulletGravity)
		{
			Vector3 predictedAimingPosition = targetPosition;

			// trans target position relate to local player's view position for simplifying equations
			Vector3 p1 = targetPosition;

			// equations about predict time t
			//
			// unknowns
			// t: predict hit time
			// p: predict hit position at time 't'
			// v0: predict local player's bullet velocity vector which could hit target at at time 't' and position 'p'
			//
			// knowns
			// p1: target player's current position relate to local player's view position
			// v1: target player's velocity
			// |v0|: local player's bullet velocity
			// g: local player's bullet gravity
			//
			// =>
			//
			// vx0^2 + vy0^2 + vz0^2 = |v0|^2
			//
			// px = vx0*t
			// py = vy0*t + 0.5*g*t^2
			// pz = vz0*t
			//
			// px = px1 + vx1*t
			// py = py1 + vy1*t
			// pz = pz1 + vz1*t
			//
			// cause all positions are relate to local player's view position, so there is no p0 in above equations
			//
			// =>
			//
			// vx0 = px1/t + vx1
			// vy0 = py1/t + vy1 - 0.5g*t
			// vz0 = pz1/t + vz1
			//
			// with above three equations and the first equation about v0,  we got the final quartic equation about predict time 't'
			// (0.25*g^2)*(t^4) + (-g*vy1)*(t^3) + (vx1^2+vy1^2+vz1^2 - g*py1 - |v|^2)*(t^2) + 2*(px1*vx1+py1*vy1+pz1*vz1)*(t) + (px1^2+py1^2+pz1^2) = 0
			//
			// let's solve this problem...
			//
			double a = bulletGravity * bulletGravity * 0.25;
			double b = -bulletGravity * targetVelocity.Y;
			double c = targetVelocity.X * targetVelocity.X + targetVelocity.Y * targetVelocity.Y + targetVelocity.Z * targetVelocity.Z - bulletGravity * p1.Y - bulletVelocity * bulletVelocity;
			double d = 2.0 * (p1.X * targetVelocity.X + p1.Y * targetVelocity.Y + p1.Z * targetVelocity.Z);
			double e = p1.X * p1.X + p1.Y * p1.Y + p1.Z * p1.Z;

			// some unix guys will not afraid these two lines
			double[] roots = new double[4];
			uint num_roots = SolveQuartic(a, b, c, d, e, ref roots);

			if (num_roots > 0)
			{
				// find the best predict hit time
				// smallest 't' for guns, largest 't' for something like mortar with beautiful arcs
				double hitTime = 0.0;
				for (int i = 0; i < num_roots; ++i)
				{
					if (roots[i] > 0.0 && (hitTime == 0.0 || roots[i] < hitTime))
						hitTime = roots[i];
				}

				if (hitTime > 0.0)
				{
					// get predict bullet velocity vector at aiming direction
					double hitVelX = p1.X / hitTime + targetVelocity.X;
					double hitVelY = p1.Y / hitTime + targetVelocity.Y - 0.5 * bulletGravity * hitTime;
					double hitVelZ = p1.Z / hitTime + targetVelocity.Z;

					// finally, the predict aiming position in world space
					predictedAimingPosition.X = (float)(hitVelX * hitTime);
					predictedAimingPosition.Y = (float)(hitVelY * hitTime);
					predictedAimingPosition.Z = (float)(hitVelZ * hitTime);
				}
			}

			return predictedAimingPosition;
		}

		public static void DrawMoonlordHand(Vector2 drawHere, Vector2 drawThere)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			SpriteBatch spriteBatch = Main.spriteBatch;
			int facing = -1;



			float angleoffset = MathHelper.PiOver2;
			Texture2D armTex = Main.extraTexture[15];
			Texture2D TestTex = Main.extraTexture[19];

			Vector2 origin = new Vector2(armTex.Width / 2f, armTex.Height);

			float scale = 1;
			float armLength = armTex.Height;
			Vector2 dist = drawThere - drawHere;
			Vector2 normal = Vector2.Normalize(dist);
			Vector2 hand2loc = drawHere+normal*Math.Min(dist.Length(), (armLength*2)-1);

			Vector2 CirclePoint1, CirclePoint2;
			if (Idglib.FindCircleCircleIntersections(drawHere, armLength, hand2loc, armLength, out CirclePoint1, out CirclePoint2) > 0)
			{

				Vector2 elbowloc = CirclePoint1;
				Vector2 normal2 = elbowloc - drawHere;
				Vector2 normal3 = drawThere - elbowloc;

				spriteBatch.Draw(armTex, drawHere - Main.screenPosition, null, Color.White, normal2.ToRotation() + angleoffset, origin, scale, spriteEffects, 0f);
				spriteBatch.Draw(armTex, CirclePoint1 - Main.screenPosition, null, Color.White, normal3.ToRotation() + angleoffset, origin, scale, spriteEffects, 0f);

				//spriteBatch.Draw(TestTex, drawHere - Main.screenPosition, null, Color.White, 0, TestTex.Size() / 2f, scale, spriteEffects, 0f);
				//spriteBatch.Draw(TestTex, drawHere + dist1Tracker - Main.screenPosition, null, Color.White, 0, TestTex.Size() / 2f, scale, spriteEffects, 0f);
				//spriteBatch.Draw(TestTex, drawHere + dist1Tracker + dist2Tracker - Main.screenPosition, null, Color.White, 0, TestTex.Size() / 2f, scale, spriteEffects, 0f);
			}

			//spriteBatch.Draw(armTex, drawHere - Main.screenPosition, null, Color.White, angle + angleoffset, origin, scale, spriteEffects, 0f);

			//spriteBatch.Draw(armTex, arm2pos - Main.screenPosition, null, Color.White, angle2, origin, scale, spriteEffects, 0f);

			return;
		}


	}

	public class RippleBoom : ModProjectile
	{
		public float rippleSize = 1f;
		public float rippleCount = 1f;
		public float expandRate = 25f;
		public float opacityrate = 1f;
		public float size = 1f;
		int maxtime = 200;
		public override string Texture
		{
			get
			{
				return "SGAmod/MatrixArrow";
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			return false;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write((double)rippleSize);
			writer.Write((double)rippleCount);
			writer.Write((double)expandRate);
			writer.Write((double)size);
			writer.Write(maxtime);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			rippleSize = (float)reader.ReadDouble();
			rippleCount = (float)reader.ReadDouble();
			expandRate = (float)reader.ReadDouble();
			size = (float)reader.ReadDouble();
			maxtime = reader.ReadInt32();
		}

		public static void MakeShockwave(Vector2 position2, float rippleSize, float rippleCount, float expandRate, int timeleft = 200, float size = 1f, bool important = false)
		{
			if (!Main.dedServ)
			{
				if (!Filters.Scene["SGAmod:Shockwave"].IsActive() || important)
				{
					int prog = Projectile.NewProjectile(position2, Vector2.Zero, SGAmod.Instance.ProjectileType("RippleBoom"), 0, 0f);
					Projectile proj = Main.projectile[prog];
					RippleBoom modproj = proj.modProjectile as RippleBoom;
					modproj.rippleSize = rippleSize;
					modproj.rippleCount = rippleCount;
					modproj.expandRate = expandRate;
					modproj.size = size;
					proj.timeLeft = timeleft - 10;
					modproj.maxtime = timeleft;
					proj.netUpdate = true;
					Filters.Scene.Activate("SGAmod:Shockwave", proj.Center, new object[0]).GetShader().UseColor(rippleCount, rippleSize, expandRate).UseTargetPosition(proj.Center);
				}
			}

		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ripple Boom");
		}

		public override void SetDefaults()
		{
			projectile.width = 4;
			projectile.height = 4;
			projectile.friendly = true;
			projectile.alpha = 0;
			projectile.penetrate = -1;
			projectile.timeLeft = 200;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
		}

		public override void AI()
		{
			//float progress = (maxtime - (float)projectile.timeLeft);
			float progress = ((maxtime - (float)base.projectile.timeLeft) / 60f) * size;
			Filters.Scene["SGAmod:Shockwave"].GetShader().UseProgress(progress).UseOpacity(100f * ((float)base.projectile.timeLeft / (float)maxtime));
			projectile.localAI[1] += 1f;
		}

		public override void Kill(int timeLeft)
		{
			Filters.Scene["SGAmod:Shockwave"].Deactivate(new object[0]);
		}
	}

	public class ModdedDamage
	{
		public Player player;
		public float damage = 0;
		public int crit = 0;
		public ModdedDamage(Player player, float damage, int crit)
		{
			this.player = player;
			this.damage = damage;
			this.crit = crit;
		}

	}

	public class EnchantmentCraftingMaterial
	{
		public int value = 0;
		public int expertisecost = 0;
		public string text = "";
		public EnchantmentCraftingMaterial(int value, int expertisecost, string text)
		{
			this.value = value;
			this.expertisecost = expertisecost;
			this.text = text;
		}
	}

}