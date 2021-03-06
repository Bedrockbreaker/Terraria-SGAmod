using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SGAmod.HavocGear.Items.Weapons 
{
    public class SharkTooth : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shark Tooth");
            Tooltip.SetDefault("May be bought from the Arms Dealer if the player has a [i:" + mod.ItemType("SnappyShark") + "] in their inventory\nDrops from Sharks after Sharkvern is defeated");
        }

        public override void SetDefaults()
        {

            item.damage = 8;
            item.ranged = true;
            item.width = 16;
            item.height = 16;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 1.5f;
            item.value = 100;
            item.rare = 3;
            item.ammo = item.type;
        }
    }
}
