﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    internal class BlenderProj2 : ModProjectile
    {
        public int Counter = 0;
        
        public override void SetDefaults()
        {
            projectile.width = 46;
            projectile.height = 46;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.scale = 1f;

            projectile.extraUpdates = 1;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile proj = Main.projectile[(int)projectile.localAI[0]];

            projectile.timeLeft++;
            projectile.rotation += 0.2f;

            if (!proj.active || proj.type != ModContent.ProjectileType<BlenderProj>())
            {
                projectile.Kill();
                return;
            }

            if (projectile.owner == Main.myPlayer)
            {
                //rotation mumbo jumbo
                float distanceFromPlayer = 32;

                projectile.position = proj.Center + new Vector2(distanceFromPlayer, 0f).RotatedBy(projectile.ai[1]);
                projectile.position.X -= projectile.width / 2;
                projectile.position.Y -= projectile.height / 2;

                float rotation = (float)Math.PI / 20;
                projectile.ai[1] += rotation;
                if (projectile.ai[1] > (float)Math.PI)
                {
                    projectile.ai[1] -= 2f * (float)Math.PI;
                    projectile.netUpdate = true;
                }

                projectile.rotation = (Main.MouseWorld - projectile.Center).ToRotation() - 5;
            }



            if (++Counter > 60)
            {
                Counter = 0;
                int proj2 = mod.ProjectileType("BlenderProj3");
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, proj2, projectile.damage, projectile.knockBack, Main.myPlayer);
            }
        }

        public override void PostAI()
        {
            /*if (Main.rand.Next(2) == 0)
            {
                int dustIndex = Dust.NewDust(projectile.position, projectile.width, projectile.height, 16, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].scale = 1.6f;
            }*/
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 6;
        }
    }
}