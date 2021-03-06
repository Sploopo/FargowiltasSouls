﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class ShroomiteShroom : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_131";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shroom");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Mushroom);
            aiType = ProjectileID.Mushroom;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;

            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 20;
        }

        public override void AI()
        {
            //dies thrice as fast
            projectile.alpha += 8;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 0);
        }
    }
}