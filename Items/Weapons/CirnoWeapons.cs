using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using SGAmod.NPCs;
using Idglibrary;
using AAAAUThrowing;


namespace SGAmod.Items.Weapons
{
	public class Snowfall : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Snowfall");
			Tooltip.SetDefault("Summon a cloud to rain hardened snowballs on your foes\nLimits 2 clouds at a time");
		}

		public override void SetDefaults()
		{
			item.summon = true;
			item.damage = 40;
			item.mana = 50;
			item.width = 44;
			item.height = 52;
			item.useTime = 30;
			item.useAnimation = 30;
			item.useStyle = 1;
			item.knockBack = 10;
			item.value = 100000;
			item.noMelee = true;
			item.rare = 5;
			item.shoot = 10;
			item.shootSpeed = 10f;
			item.UseSound = SoundID.Item60;
			item.autoReuse = true;
			item.useTurn = false;

		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{

			/*int projcount=player.ownedProjectileCounts[mod.ProjectileType("SnowfallCloud")]+player.ownedProjectileCounts[mod.ProjectileType("SnowCloud")];

			if (projcount>1){
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile him=Main.projectile[i];
				if (him.type==mod.ProjectileType("SnowfallCloud") || him.type==mod.ProjectileType("SnowCloud")){
				if (him.active && him.owner==projectile.owner){
				him.Kill();
				break;
			}}}}*/

			SGAPlayer.LimitProjectiles(player, 1, new ushort[] { (ushort)mod.ProjectileType("SnowfallCloud"), (ushort)mod.ProjectileType("SnowCloud") });

			int theproj = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("SnowfallCloud"), damage, knockBack, player.whoAmI);
			float num12 = (float)Main.mouseX + Main.screenPosition.X;
			float num13 = (float)Main.mouseY + Main.screenPosition.Y;
			HalfVector2 half = new HalfVector2(num12, num13);
			Main.projectile[theproj].ai[0] = ReLogic.Utilities.ReinterpretCast.UIntAsFloat(half.PackedValue);
			Main.projectile[theproj].netUpdate = true;
			return false;
		}


		public override void MeleeEffects(Player player, Rectangle hitbox)
		{

			for (int num475 = 0; num475 < 3; num475++)
			{
				int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, mod.DustType("HotDust"));
				Main.dust[dust].scale = 0.5f + (((float)num475) / 3.5f);
				Vector2 randomcircle = new Vector2(Main.rand.Next(-8000, 8000), Main.rand.Next(-8000, 8000)); randomcircle.Normalize();
				Main.dust[dust].velocity = randomcircle / 2f;
				Main.dust[dust].noGravity = true;
				//Main.dust[dust].velocity.Normalize();
				//Main.dust[dust].velocity+=new Vector2(player.velocity.X/4,0f);
				//Main.dust[dust].velocity*=(((float)Main.rand.Next(0,100))/30f);
			}
			Lighting.AddLight(player.position, 0.9f, 0.9f, 0f);
		}


	}


	public class SnowfallCloud : ModProjectile
	{

		public override void SetDefaults()
		{
			projectile.width = 30;
			projectile.height = 30;
			//projectile.aiStyle = 1;
			projectile.friendly = true;
			projectile.hostile = false;
			//projectile.magic = true;
			//projectile.penetrate = 1;
			projectile.timeLeft = 60 * 30;
			projectile.tileCollide = false;
		}

		public override string Texture
		{
			get { return "Terraria/Item_" + 5; }
		}

		public override bool? CanHitNPC(NPC target)
		{
			return false;
		}
		public override bool CanHitPlayer(Player target)
		{
			return false;
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			return false;
		}
		public override void AI()
		{

			Vector2 gohere = new HalfVector2() { PackedValue = ReLogic.Utilities.ReinterpretCast.FloatAsUInt(projectile.ai[0]) }.ToVector2();
			Vector2 anglevect = (gohere - projectile.position);
			float length = anglevect.Length();
			anglevect.Normalize();
			projectile.velocity = (10f * anglevect);
			bool reached = (length < projectile.velocity.Length() + 1f);
			for (int q = 0; q < (reached ? 40 : 4); q++)
			{
				Vector2 randomcircle = new Vector2(Main.rand.Next(-8000, 8000), Main.rand.Next(-8000, 8000)); randomcircle.Normalize();
				float reachfloat = reached ? 0f : 1f;
				int dust = Dust.NewDust(projectile.position - new Vector2(8, 0), 16, 16, DustID.Smoke, ((projectile.velocity.X * 0.75f) * reachfloat) + (randomcircle * (reached ? 12f : 0f)).X, ((projectile.velocity.Y * 0.75f) * reachfloat) + (randomcircle * (reached ? 4f : 0f)).Y, 100, Main.hslToRgb(0.6f, 0.8f, 0.8f), 3f);
				Main.dust[dust].noGravity = true;
			}
			if (reached)
			{
				int theproj = Projectile.NewProjectile(projectile.position.X + 16f, projectile.position.Y, 0f, 0f, mod.ProjectileType("SnowCloud"), projectile.damage, projectile.knockBack, projectile.owner);
				Main.projectile[theproj].friendly = true;
				Main.projectile[theproj].hostile = false;
				Main.projectile[theproj].timeLeft = projectile.timeLeft;
				Main.projectile[theproj].netUpdate = true;
				projectile.Kill();
			}
		}

	}

	public class CursedHail : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cursed Hail");
			Tooltip.SetDefault("Summon a corrupted cloud to rain cursed ice shards on your foes\nLimits 2 clouds at a time");
		}

		public override void SetDefaults()
		{
			item.summon = true;
			item.damage = 65;
			item.mana = 50;
			item.width = 44;
			item.height = 52;
			item.useTime = 30;
			item.useAnimation = 30;
			item.useStyle = 1;
			item.knockBack = 10;
			item.value = 10000;
			item.noMelee = true;
			item.rare = 5;
			item.shoot = 10;
			item.shootSpeed = 10f;
			item.UseSound = SoundID.Item60;
			item.autoReuse = true;
			item.useTurn = false;

		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{

			SGAPlayer.LimitProjectiles(player, 1, new ushort[] { (ushort)mod.ProjectileType("CursedHailProj"), (ushort)mod.ProjectileType("CursedHailCloud") });

			int theproj = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("CursedHailProj"), damage, knockBack, player.whoAmI);
			float num12 = (float)Main.mouseX + Main.screenPosition.X;
			float num13 = (float)Main.mouseY + Main.screenPosition.Y;
			HalfVector2 half = new HalfVector2(num12, num13);
			Main.projectile[theproj].ai[0] = ReLogic.Utilities.ReinterpretCast.UIntAsFloat(half.PackedValue);
			Main.projectile[theproj].netUpdate = true;
			return false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(mod.ItemType("Snowfall"), 1);
			recipe.AddIngredient(ItemID.CursedFlame, 15);
			recipe.AddIngredient(ItemID.BlizzardStaff, 1);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();

		}


	}

	public class CursedHailProj : SnowfallCloud
	{

		public override void SetDefaults()
		{
			projectile.width = 30;
			projectile.height = 30;
			//projectile.aiStyle = 1;
			projectile.friendly = true;
			projectile.hostile = false;
			//projectile.magic = true;
			//projectile.penetrate = 1;
			projectile.timeLeft = 60 * 30;
			projectile.tileCollide = false;
		}

		public override string Texture
		{
			get { return "Terraria/Item_" + 5; }
		}
		public override void AI()
		{

			Vector2 gohere = new HalfVector2() { PackedValue = ReLogic.Utilities.ReinterpretCast.FloatAsUInt(projectile.ai[0]) }.ToVector2();
			Vector2 anglevect = (gohere - projectile.position);
			float length = anglevect.Length();
			anglevect.Normalize();
			projectile.velocity = (10f * anglevect);
			bool reached = (length < projectile.velocity.Length() + 1f);
			for (int q = 0; q < (reached ? 40 : 4); q++)
			{
				Vector2 randomcircle = new Vector2(Main.rand.Next(-8000, 8000), Main.rand.Next(-8000, 8000)); randomcircle.Normalize();
				float reachfloat = reached ? 0f : 1f;
				int dust = Dust.NewDust(projectile.position - new Vector2(8, 0), 16, 16, DustID.Smoke, ((projectile.velocity.X * 0.75f) * reachfloat) + (randomcircle * (reached ? 12f : 0f)).X, ((projectile.velocity.Y * 0.75f) * reachfloat) + (randomcircle * (reached ? 4f : 0f)).Y, 100, Color.LimeGreen, 3f);
				Main.dust[dust].noGravity = true;
			}
			if (reached)
			{
				int theproj = Projectile.NewProjectile(projectile.position.X + 16f, projectile.position.Y, 0f, 0f, mod.ProjectileType("CursedHailCloud"), projectile.damage, projectile.knockBack, projectile.owner);
				Main.projectile[theproj].friendly = true;
				Main.projectile[theproj].hostile = false;
				Main.projectile[theproj].timeLeft = projectile.timeLeft;
				Main.projectile[theproj].netUpdate = true;
				projectile.Kill();

			}

		}

	}


	public class CursedHailCloud : SnowCloud
	{

		public override int projectileid => ModContent.ProjectileType<CursedHailProjectile>();
		public override Color colorcloud => Color.LimeGreen;
		public override int rate => 4;

		public override void SetDefaults()
		{
			projectile.width = 30;
			projectile.height = 30;
			//projectile.aiStyle = 1;
			projectile.friendly = false;
			projectile.hostile = true;
			//projectile.magic = true;
			//projectile.penetrate = 1;
			projectile.timeLeft = 600;
			projectile.tileCollide = false;
		}


	}


	public class CursedHailProjectile : ModProjectile
	{

		int fakeid = ProjectileID.FrostShard;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cursed Hail");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.CloneDefaults(fakeid);
			projectile.width = 8;
			projectile.height = 8;
			projectile.hostile = false;
			projectile.friendly = true;
			projectile.penetrate = 5;
			projectile.magic = true;
			projectile.extraUpdates = 2;
		}

		public override string Texture
		{
			get { return "Terraria/Projectile_" + fakeid; }
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (projectile.localAI[0] == 0)
				projectile.localAI[0] = Main.rand.Next(0, 3);
			Texture2D tex = Main.projectileTexture[fakeid];
			Vector2 drawOrigin = new Vector2(tex.Width, tex.Height / 5) / 2f;
			Vector2 drawPos = ((projectile.Center - Main.screenPosition)) + new Vector2(0f, 4f);
			int timing = (int)(projectile.localAI[0] - 1);
			timing %= 5;
			timing *= ((tex.Height) / 5);
			spriteBatch.Draw(tex, drawPos, new Rectangle(0, timing, tex.Width, (tex.Height - 1) / 5), lightColor, MathHelper.ToRadians(180) + projectile.velocity.X * -0.08f, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
			return false;
		}

		public override bool PreKill(int timeLeft)
		{
			projectile.type = fakeid;
			return true;
		}

		public override void AI()
		{
			if (projectile.ai[0] == 0)
			{
				projectile.ai[0] = 1;
				projectile.velocity += new Vector2(0, 10);
				projectile.velocity *= 0.75f;
			}
			int dust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 75);
			Main.dust[dust].scale = 1f;
			Main.dust[dust].noGravity = true;
			Main.dust[dust].velocity = projectile.velocity * (float)(Main.rand.NextFloat(0.1f, 0.25f));
			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.CursedInferno, 60 * 5);
			target.immune[projectile.owner] = 4;
		}

	}

	public class RubiedBlade : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rubied Blade");
			Tooltip.SetDefault("Rains down ruby bolts from the sky above enemies hit by the blade");
		}
		public override void SetDefaults()
		{
			item.damage = 42;
			item.melee = true;
			item.width = 32;
			item.height = 32;
			item.useTime = 15;
			item.useAnimation = 15;
			item.useStyle = 1;
			item.knockBack = 2;
			item.crit = 7;
			item.value = Item.sellPrice(0, 15, 0, 0);
			item.rare = 5;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			if (!Main.dedServ)
			{
				item.GetGlobalItem<ItemUseGlow>().glowTexture = mod.GetTexture("Items/GlowMasks/RubiedBlade_Glow");
			}

		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
		{
			Vector2 hereas = new Vector2(Main.rand.Next(-1000, 1000), Main.rand.Next(-1000, 1000)); hereas.Normalize();
			hereas *= Main.rand.NextFloat(50f, 100f);
			hereas += target.Center;
			hereas += new Vector2(0, -800);
			Vector2 gohere = ((hereas + new Vector2(Main.rand.NextFloat(-100f, 100f), 800f)) - hereas); gohere.Normalize(); gohere *= 15f;
			int proj = Projectile.NewProjectile(hereas, gohere, ProjectileID.RubyBolt, (int)(damage * 1), knockBack, player.whoAmI);
			Main.projectile[proj].melee = true;
			Main.projectile[proj].magic = false;
			Main.projectile[proj].timeLeft = 300;
			IdgProjectile.Sync(proj);
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{

			int dustIndex = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 32, 32, 113);
			Dust dust = Main.dust[dustIndex];
			Vector2 randomcircle = new Vector2(Main.rand.Next(-8000, 8000), Main.rand.Next(-8000, 8000)); randomcircle.Normalize();
			dust.velocity = (randomcircle / 2f) + (player.itemRotation.ToRotationVector2());
			dust.scale *= 1.5f + Main.rand.Next(-30, 31) * 0.01f;
			dust.fadeIn = 0f;
			dust.noGravity = true;
			dust.color = Color.LightSkyBlue;
		}

	}

	public class Starburster : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Star Burster");
			Tooltip.SetDefault("Fires 4 stars in bursts at the cost of 1, but requires a small ammount of mana\nuses Fallen Stars for Ammo, the stars cannot pass through platforms");
		}
		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.GoldenShower);
			item.damage = 38;
			item.useAnimation = 18;
			item.useTime = 5;
			item.reuseDelay = 20;
			item.knockBack = 6;
			item.value = 75000;
			item.rare = 5;
			item.magic = false;
			item.ranged = true;
			item.mana = 10;
			item.shoot = ProjectileID.StarAnise;
			item.shootSpeed = 9f;
			item.useAmmo = AmmoID.FallenStar;
		}

		public override bool CanUseItem(Player player)
		{
			return (player.CountItem(ItemID.FallenStar) > 0);
		}

		public override bool ConsumeAmmo(Player player)
		{
			if (player.itemAnimation > 6)
				return false;

			return base.ConsumeAmmo(player);
		}

		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			int Itd = type;
			Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(5));
			speedX = perturbedSpeed.X;
			speedY = perturbedSpeed.Y;
			float rander = Main.rand.Next(7000, 8000) / 2000;
			if (type == ProjectileID.StarAnise)
				Itd = 9;
			int probg = Terraria.Projectile.NewProjectile(position.X + (int)speedX * 4, position.Y + (int)speedY * 4, speedX * (rander), speedY * (rander), Itd, damage, knockBack, player.whoAmI);
			Main.projectile[probg].ranged = true;
			Main.projectile[probg].tileCollide = true;
			return false;
		}

	}

	public class Starfishburster : Starburster
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Star'Fish' Burster");
			Tooltip.SetDefault("Fires 4 starfish in bursts at the cost of 1, but requires a small ammount of mana\nStarfish bounce off walls and pierce\nuses Starfish as ammo");
		}
		public override bool CanUseItem(Player player)
		{
			return (player.CountItem(ItemID.Starfish) > 0);
		}
		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.GoldenShower);
			item.damage = 50;
			item.useAnimation = 18;
			item.useTime = 5;
			item.reuseDelay = 20;
			item.knockBack = 6;
			item.value = 250000;
			item.rare = 7;
			item.magic = false;
			item.ranged = true;
			item.mana = 10;
			//item.shoot = mod.ProjectileType("SunbringerFlare");
			item.shoot = mod.ProjectileType("StarfishProjectile");
			item.shootSpeed = 11f;
			item.useAmmo = 2626;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "StarfishBlaster", 1);
			recipe.AddIngredient(null, "Starburster", 1);
			recipe.AddIngredient(null, "WraithFragment4", 10);
			recipe.AddIngredient(null, "CryostalBar", 8);
			recipe.AddIngredient(null, "SharkTooth", 100);
			recipe.AddIngredient(ItemID.Coral, 8);
			recipe.AddIngredient(ItemID.Starfish, 5);
			recipe.AddIngredient(ItemID.HallowedBar, 6);
			recipe.AddTile(mod.TileType("ReverseEngineeringStation"));
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

	}

	public class IceScepter : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ice Scepter");
			Tooltip.SetDefault("Spews homing ice bolts with duo swirling ice shards\nShards fly off when the bolt hits something and do 50% of base damage\nAlt Fire summons a wall of ice blocks");
		}

		public override void SetDefaults()
		{
			item.damage = 36;
			item.magic = true;
			item.width = 34;
			item.mana = 8;
			item.height = 24;
			item.useTime = 17;
			item.useAnimation = 17;
			item.useStyle = 5;
			item.knockBack = 2;
			item.value = 50000;
			item.rare = 5;
			item.shootSpeed = 8f;
			item.noMelee = true;
			item.shoot = 14;
			item.shootSpeed = 11f;
			item.UseSound = SoundID.Item8;
			item.autoReuse = true;
			Item.staff[item.type] = true;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{

			if (player.altFunctionUse == 2)
			{
				if (player.statMana > 70)
				{
					speedX *= 2f;
					speedY *= 2f;
					position += Vector2.Normalize(new Vector2(speedX, speedY)) * 48f;
					for (int i = -4; i < 5; i += 1)
					{

						float rotation = MathHelper.ToRadians(32);
						Vector2 perturbedSpeed = (new Vector2(speedX, speedY)).RotatedBy(MathHelper.ToRadians(i * 5)) * 1.2f;
						Vector2 dister = perturbedSpeed;
						dister.Normalize();

						int proj = Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ProjectileID.IceBlock, (int)(damage / 5), knockBack, player.whoAmI, ((position + dister * 160f).X) / 16f, (position + dister * 160f).Y / 16f);
						Main.projectile[proj].friendly = true;
						Main.projectile[proj].hostile = false;
						Main.projectile[proj].timeLeft = 240;
						Main.projectile[proj].knockBack = item.knockBack;
						player.itemTime = 90;

					}
					player.statMana -= (int)(60f * player.manaCost);
					player.manaRegenDelay = 1200;
				}


			}
			else
			{

				float rotation = MathHelper.ToRadians(4);
				speedX *= 4f;
				speedY *= 4f;
				position += Vector2.Normalize(new Vector2(speedX, speedY)) * 48f;
				Vector2 perturbedSpeed = (new Vector2(speedX, speedY)).RotatedBy(MathHelper.Lerp(-rotation, rotation, (float)Main.rand.Next(0, 100) / 100f)) * .2f;

				int proj = Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, mod.ProjectileType("CirnoBoltPlayer"), damage, knockBack, player.whoAmI);
				Main.projectile[proj].friendly = true;
				Main.projectile[proj].hostile = false;
				Main.projectile[proj].timeLeft = 240;
				Main.projectile[proj].knockBack = item.knockBack;

			}
			return false;
		}

	}

	class IcicleFall : ModItem
	{

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			DisplayName.SetDefault("Icicle Fall");
			Tooltip.SetDefault("Throws an ice chunk that splits apart into 6 shards when falling downwards\nThese shards do full base damage");
		}

		public override void SetDefaults()
		{
			item.useStyle = 1;
			item.Throwing().thrown = true;
			item.damage = 20;
			item.crit = 8;
			item.knockBack = 0f;
			item.useTime = 25;
			item.useAnimation = 25;
			item.noUseGraphic = true;
			item.noMelee = true;
			item.autoReuse = true;
			item.maxStack = 1;
			item.shootSpeed = 12f;
			item.shoot = mod.ProjectileType("CirnoIceShardPlayerCore");
			item.value = Item.buyPrice(0, 0, 5, 0);
			item.rare = 6;
		}

	}

	public class CirnoIceShardPlayerCore : ModProjectile
	{

		int fakeid = ProjectileID.FrostShard;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Baka Ice Core");
		}

		public override void SetDefaults()
		{
			projectile.CloneDefaults(fakeid);
			projectile.width = 16;
			projectile.height = 16;
			projectile.hostile = false;
			projectile.friendly = true;
			projectile.penetrate = 1;
			projectile.Throwing().thrown = true;
			projectile.extraUpdates = 0;
			projectile.aiStyle = -1;
		}

		public override string Texture
		{
			get { return "Terraria/Item_" + ItemID.FrostCore; }
		}

		public override void AI()
		{

			projectile.rotation += projectile.velocity.X * 0.02f;
			projectile.velocity.Y += 0.2f;
			if (projectile.velocity.Y > 0)
			{
				Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 30, 1f, -0.5f);
				for (float xx = 0f; xx < 6f; xx += 1f)
				{
					int proj2 = Projectile.NewProjectile(projectile.Center, projectile.velocity + new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(0, 3f)), mod.ProjectileType("CirnoIceShardPlayer"), (int)((float)projectile.damage * 1f), 0f, projectile.owner);
					Main.projectile[proj2].friendly = false;
					Main.projectile[proj2].hostile = true;
					Main.projectile[proj2].netUpdate = true;
				}
				projectile.Kill();
			}
			for (float i = 0f; i < 4f; i += 1f)
			{
				int dust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Ice);
				Main.dust[dust].scale = 1.5f;
				Main.dust[dust].noGravity = true;
				Main.dust[dust].velocity = projectile.velocity * (float)(Main.rand.NextFloat(0.1f, 0.25f) * i);
			}
		}

	}

	public class CirnoIceShardPlayer : CirnoIceShard
	{

		int fakeid = ProjectileID.FrostShard;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Baka Ice Shard");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.CloneDefaults(fakeid);
			projectile.width = 8;
			projectile.height = 8;
			projectile.hostile = false;
			projectile.friendly = true;
			projectile.penetrate = 1;
			projectile.Throwing().thrown = true;
			projectile.extraUpdates = 0;
			projectile.aiStyle = -1;
		}

		public override string Texture
		{
			get { return "Terraria/Projectile_" + fakeid; }
		}

		public override void AI()
		{
			base.AI();
			projectile.hostile = false;
			projectile.friendly = true;
			if (projectile.ai[1]<=0)
			projectile.velocity.Y += 0.1f;
		}

	}

	public class CirnoIceShardPlayerOrbit : CirnoIceShardPlayer
	{

		int fakeid = ProjectileID.FrostShard;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Baka Ice Shard");
		}
		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.width = 8;
			projectile.height = 8;
			projectile.hostile = false;
			projectile.friendly = true;
			projectile.penetrate = 1;
			projectile.thrown = false;
			projectile.magic = true;
			projectile.extraUpdates = 0;
			projectile.aiStyle = -1;
		}

		public override void AI()
		{
			projectile.ai[0] += 0.1f;
			projectile.localAI[1] += 0.2f;
			Projectile parent = Main.projectile[(int)projectile.ai[1]];
			if (parent.active && parent.type == mod.ProjectileType("CirnoBoltPlayer") && projectile.ai[0]>-5000)
			{
				projectile.velocity = (projectile.ai[0] + MathHelper.ToRadians(90)).ToRotationVector2() * 7;
				projectile.Center = parent.Center + (projectile.ai[0]).ToRotationVector2()*Math.Min(projectile.localAI[1],12f); 
			}
			else
			{
				projectile.ai[0] = -10000;
			}

			base.AI();
		}

	}

}


namespace SGAmod.NPCs
{

	public class CirnoBoltPlayer : CirnoBolt
	{

		double keepspeed = 0.0;
		float homing = 0.05f;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cirno's Grace");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.hostile = false;
			projectile.friendly = true;
		}

		public override void AI()
		{
			base.AI();
			if (projectile.ai[0] == 2)
			{
				float rand = Main.rand.Next(0, 360);
				for (int i = 0; i < 360; i += 180)
				{
					int proj2 = Projectile.NewProjectile(projectile.Center, projectile.velocity + new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(0, 3f)), mod.ProjectileType("CirnoIceShardPlayerOrbit"), (int)((float)projectile.damage * 0.5f), 0f, projectile.owner);
					Main.projectile[proj2].ai[0] = MathHelper.ToRadians(i+ rand);
					Main.projectile[proj2].ai[1] = projectile.whoAmI;
					Main.projectile[proj2].netUpdate = true;
				}
			}

		}

		public override string Texture
		{
			get { return "Terraria/Projectile_" + ProjectileID.IceBolt; }
		}

	}

}