﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using CalamityMod.CalPlayer;
using Terraria.Localization;
using CalamityMod.Items.Armor;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Pets;
using CalamityMod.Projectiles.Pets;
using CalamityMod.Buffs.Pets;

namespace FargowiltasSouls.Items.Accessories.Enchantments.Calamity
{
    public class DemonShadeEnchant : ModItem
    {
        private readonly Mod calamity = ModLoader.GetMod("CalamityMod");

        public override bool Autoload(ref string name)
        {
            return ModLoader.GetMod("CalamityMod") != null;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demonshade Enchantment");
            Tooltip.SetDefault(
@"'Demonic power emanates from you…'
All attacks inflict Demon Flames
Shadowbeams and Demon Scythes fall from the sky on hit
A friendly red devil follows you around
Press Y to enrage nearby enemies with a dark magic spell for 10 seconds
This makes them do 1.5 times more damage but they also take five times as much damage
Effects of Profaned Soul Crystal
Summons a Levi and Supreme Calamitas pet");
            DisplayName.AddTranslation(GameCulture.Chinese, "魔影魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"'你身上散发着恶魔之力...'
所有攻击造成恶魔之炎
被攻击时释放暗影射线和恶魔镰刀
一个友好的红魔鬼将跟随你
敌人触碰你时受到鬼魔伤害
站立不动你将吸收暗影能量并大幅增加生命恢复速度
按'Y'键用黑魔法激怒附近敌人10秒
这将使敌人造成1.5倍伤害, 同时受到5倍伤害");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = 10;
            item.value = 50000000;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(173, 52, 70);
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!Fargowiltas.Instance.CalamityLoaded) return;

            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>();
            //set bonus
            calamity.Call("SetSetBonus", player, "demonshade", true);

            if (SoulConfig.Instance.GetValue(SoulConfig.Instance.calamityToggles.RedDevilMinion))
            {
                modPlayer.redDevil = true;
                if (player.whoAmI == Main.myPlayer)
                {
                    if (player.FindBuffIndex(calamity.BuffType("RedDevil")) == -1)
                    {
                        player.AddBuff(calamity.BuffType("RedDevil"), 3600, true);
                    }
                    if (player.ownedProjectileCounts[calamity.ProjectileType("RedDevil")] < 1)
                    {
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, calamity.ProjectileType("RedDevil"), 10000, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
            }

            if (SoulConfig.Instance.GetValue(SoulConfig.Instance.calamityToggles.ProfanedSoulCrystal))
            {
                calamity.GetItem("ProfanedSoulCrystal").UpdateAccessory(player, hideVisual);
            }
                
            FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();
            fargoPlayer.DemonShadeEnchant = true;
            fargoPlayer.AddPet(SoulConfig.Instance.calamityToggles.LeviPet, hideVisual, ModContent.BuffType<LeviBuff>(), ModContent.ProjectileType<LeviPet>());
            fargoPlayer.AddPet(SoulConfig.Instance.calamityToggles.ScalPet, hideVisual, ModContent.BuffType<SCalPetBuff>(), ModContent.ProjectileType<SCalPet>());
        }

        public override void AddRecipes()
        {
            if (!Fargowiltas.Instance.CalamityLoaded) return;

            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DemonshadeHelm>());
            recipe.AddIngredient(ModContent.ItemType<DemonshadeBreastplate>());
            recipe.AddIngredient(ModContent.ItemType<DemonshadeGreaves>());
            recipe.AddIngredient(ModContent.ItemType<ProfanedSoulCrystal>());
            recipe.AddIngredient(ModContent.ItemType<Animus>());
            recipe.AddIngredient(ModContent.ItemType<Contagion>());
            recipe.AddIngredient(ModContent.ItemType<Megafleet>());
            recipe.AddIngredient(ModContent.ItemType<SomaPrime>());
            recipe.AddIngredient(ModContent.ItemType<Judgement>());
            recipe.AddIngredient(ModContent.ItemType<Apotheosis>());
            recipe.AddIngredient(ModContent.ItemType<Eternity>());
            recipe.AddIngredient(ModContent.ItemType<NanoblackReaperRogue>());
            recipe.AddIngredient(ModContent.ItemType<BrimstoneJewel>());
            recipe.AddIngredient(ModContent.ItemType<Levi>());

            recipe.AddTile(calamity, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
