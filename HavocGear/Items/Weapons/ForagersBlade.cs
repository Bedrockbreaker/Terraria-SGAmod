using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SGAmod.HavocGear.Items.Weapons
{
	public class ForagersBlade : ModItem
	{
        public override void SetDefaults()
		{
			base.SetDefaults();

            item.damage = 14;
            item.width = 32;
			item.height = 32;
            item.melee = true;
            item.useTurn = true;
            item.rare = 0;
            item.useStyle = 1;
            item.useAnimation = 24;
           	item.knockBack = 5;
            item.useTime = 20;
            item.consumable = false;
            item.UseSound = SoundID.Item1;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forager's Blade");
            Tooltip.SetDefault("Dealing the killing blow will skin Leather off organic enemies");
        }

    }
}   
