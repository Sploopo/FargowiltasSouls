using System;
using System.Collections.Generic;
using System.Linq;
using Fargowiltas.Projectiles;
using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.NPCs.Champions;
using FargowiltasSouls.Projectiles.Masomode;
using FargowiltasSouls.Projectiles.Minions;
using FargowiltasSouls.Projectiles.Souls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class FargoGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }

        private bool townNPCProj = false;
        private int counter;
        public bool IsRecolor = false;
        private int rainbowCounter = 0;
        public bool Rainbow = false;
        public int GrazeCD;

        //enchants
        public bool CanSplit = true;
        private int numSplits = 1;
        private bool ninjaTele;
        private bool stormBoosted = false;
        private int stormTimer;
        private int preStormDamage;
        public bool TungstenProjectile = false;
        private bool tikiMinion = false;
        private int tikiTimer = 300;
        public int shroomiteMushroomCD = 0;
        private int spookyCD = 0;
        public bool FrostFreeze = false;
        public bool SuperBee = false;
        public bool ChilledProj = false;
        public int ChilledTimer;

        public Func<Projectile, bool> GrazeCheck = projectile => projectile.Distance(Main.LocalPlayer.Center) < Math.Min(projectile.width, projectile.height) / 2 + Player.defaultHeight + 100 && Collision.CanHit(projectile.Center, 0, 0, Main.LocalPlayer.Center, 0, 0);

        private bool firstTick = true;
        private bool squeakyToy = false;
        public int TimeFrozen = 0;
        public bool TimeFreezeImmune;
        public bool TimeFreezeCheck;
        public bool HasKillCooldown;
        public bool ImmuneToMutantBomb;
        public bool ImmuneToGuttedHeart;

        public bool masobool;

        public int ModProjID;

        public override void SetDefaults(Projectile projectile)
        {
            if (FargoSoulsWorld.MasochistMode)
            {
                switch (projectile.type)
                {
                    case ProjectileID.SpiritHeal:
                        projectile.timeLeft = 180 * 4; //account for extraupdates
                        break;

                    case ProjectileID.DD2BetsyFlameBreath:
                        projectile.tileCollide = false;
                        projectile.penetrate = -1;
                        ImmuneToGuttedHeart = true;
                        break;

                    case ProjectileID.PhantasmalDeathray:
                    case ProjectileID.SaucerDeathray:
                    case ProjectileID.Sharknado:
                        ImmuneToGuttedHeart = true;
                        break;

                    case ProjectileID.ChlorophyteBullet:
                        projectile.extraUpdates = 1;
                        break;

                    case ProjectileID.CrystalBullet:
                    case ProjectileID.HolyArrow:
                        HasKillCooldown = true;
                        break;

                    case ProjectileID.StardustCellMinionShot:
                        projectile.minion = true; //allows it to hurt maso ML
                        break;

                    case ProjectileID.StardustGuardian:
                    case ProjectileID.StardustGuardianExplosion:
                        TimeFreezeImmune = true;
                        break;

                    case ProjectileID.SaucerLaser:
                        projectile.tileCollide = false;
                        break;

                    case ProjectileID.CultistBossFireBall:
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.cultBoss, NPCID.CultistBoss)
                            && Main.npc[EModeGlobalNPC.cultBoss].life < Main.npc[EModeGlobalNPC.cultBoss].lifeMax / 2)
                            projectile.timeLeft = 1;
                        break;

                    case ProjectileID.CultistBossFireBallClone:
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.cultBoss, NPCID.CultistBoss))
                            projectile.timeLeft = 1;
                        break;

                    case ProjectileID.CursedFlameHostile:
                        /*if (FargoSoulsGlobalNPC.BossIsAlive(ref FargoSoulsGlobalNPC.wallBoss, NPCID.WallofFlesh))
                        {
                            projectile.tileCollide = false;
                            projectile.timeLeft = 120;
                            projectile.extraUpdates = 1;
                        }*/
                        break;

                    case ProjectileID.SharknadoBolt:
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.fishBossEX, NPCID.DukeFishron))
                            projectile.extraUpdates++;
                        break;

                    case ProjectileID.FlamesTrap:
                        if (NPC.golemBoss != -1 && Main.npc[NPC.golemBoss].active && Main.npc[NPC.golemBoss].type == NPCID.Golem)
                            projectile.tileCollide = false;
                        break;

                    case ProjectileID.UnholyTridentHostile:
                        projectile.extraUpdates++;
                        break;

                    case ProjectileID.BulletSnowman:
                        projectile.tileCollide = false;
                        projectile.timeLeft = 600;
                        break;

                    case ProjectileID.CannonballHostile:
                        projectile.scale = 2f;
                        break;

                    case ProjectileID.EyeLaser:
                    case ProjectileID.EyeFire:
                        projectile.tileCollide = false;
                        break;

                    default:
                        break;
                }
            }

            if (projectile.type == ProjectileID.StardustGuardian || projectile.type == ProjectileID.StardustGuardianExplosion)
            {
                TimeFreezeImmune = true;
            }

            Fargowiltas.ModProjDict.TryGetValue(projectile.type, out ModProjID);
        }

        public static int[] noSplit = {
            ProjectileID.CrystalShard,
            ProjectileID.SandnadoFriendly,
            ProjectileID.LastPrism,
            ProjectileID.LastPrismLaser,
            ProjectileID.FlowerPetal,
            ProjectileID.BabySpider,
            ProjectileID.CrystalLeafShot,
            ProjectileID.Phantasm,
            ProjectileID.VortexBeater,
            ProjectileID.ChargedBlasterCannon,
            ProjectileID.MedusaHead,
            ProjectileID.WireKite,
            ProjectileID.DD2PhoenixBow,
            ProjectileID.LaserMachinegun
        };

        public override bool PreAI(Projectile projectile)
        {
            bool retVal = true;
            Player player = Main.player[Main.myPlayer];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            counter++;

            if (spookyCD > 0)
            {
                spookyCD--;
            }

            if (projectile.owner == Main.myPlayer)
            {
                if (firstTick)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];

                        if (npc.active && npc.townNPC && projectile.Hitbox.Intersects(npc.Hitbox))
                        {
                            townNPCProj = true;
                        }
                    }

                    if (modPlayer.TungstenEnchant && SoulConfig.Instance.GetValue(SoulConfig.Instance.TungstenProjSize) && modPlayer.TungstenCD == 0 && !townNPCProj && projectile.damage != 0 && !projectile.trap && !projectile.minion && projectile.aiStyle != 99 && projectile.type != ProjectileID.Arkhalis && projectile.friendly)
                    {
                        projectile.position = projectile.Center;
                        projectile.scale *= 2f;
                        projectile.width *= 2;
                        projectile.height *= 2;
                        projectile.Center = projectile.position;
                        TungstenProjectile = true;
                        modPlayer.TungstenCD = 30;

                        if (modPlayer.TerraForce || modPlayer.WizardEnchant)
                        {
                            modPlayer.TungstenCD /= 2;
                        }
                    }

                    if (modPlayer.TikiEnchant && modPlayer.TikiMinion && projectile.minion && projectile.minionSlots > 0)
                    {
                        tikiMinion = true;

                        if (projectile.type != ModContent.ProjectileType<EaterBody>() && projectile.type != ProjectileID.StardustDragon2 && projectile.type != ProjectileID.StardustDragon3)
                        {
                           tikiMinion = true;
                           tikiTimer = 300;

                            if (modPlayer.SpiritForce || modPlayer.WizardEnchant)
                            {
                                tikiTimer = 480;
                            }


                            /*for (int i = 0; i < 1000; i++)
                            {
                                Projectile pro = Main.projectile[i];

                                if (pro.type == ProjectileID.StardustDragon1 || pro.type == ProjectileID.StardustDragon4 || pro.type == ModContent.ProjectileType<EaterHead>() || pro.type == ModContent.ProjectileType<EaterTail>())
                                {
                                    pro.GetGlobalProjectile<FargoGlobalProjectile>().tikiMinion = true;
                                    pro.GetGlobalProjectile<FargoGlobalProjectile>().tikiTimer = 300;

                                    if (modPlayer.SpiritForce || modPlayer.WizardEnchant)
                                    {
                                        pro.GetGlobalProjectile<FargoGlobalProjectile>().tikiTimer = 480;
                                    }
                                }
                            }*/
                        }
                    }

                    if (modPlayer.StardustEnchant && projectile.type == ProjectileID.StardustGuardianExplosion)
                    {
                        projectile.damage *= 5;
                    }

                    if (!townNPCProj && modPlayer.AdamantiteEnchant && modPlayer.AdamantiteCD == 0 && CanSplit && projectile.friendly && !projectile.hostile && projectile.damage > 0 && !projectile.minion && projectile.aiStyle != 19 && projectile.aiStyle != 99
                        && SoulConfig.Instance.GetValue(SoulConfig.Instance.AdamantiteSplit) && Array.IndexOf(noSplit, projectile.type) <= -1
                        && !(projectile.type == ProjectileID.DD2BetsyArrow && projectile.ai[1] == -1))
                    {
                        modPlayer.AdamantiteCD = 60;

                        if (modPlayer.Eternity)
                        {
                            modPlayer.AdamantiteCD = 0;
                        }
                        else if (modPlayer.TerrariaSoul)
                        {
                            modPlayer.AdamantiteCD = 30;
                        }
                        else if (modPlayer.EarthForce || modPlayer.WizardEnchant)
                        {
                            modPlayer.AdamantiteCD = 45;
                        }

                        float damageRatio = 0.5f;

                        if (projectile.penetrate > 1)
                        {
                            damageRatio = 1;
                        }

                        SplitProj(projectile, 3, MathHelper.Pi / 16, damageRatio);
                    }

                    if (projectile.bobber && CanSplit)
                    {
                        /*if (modPlayer.FishSoul1)
                        {
                            SplitProj(projectile, 5);
                        }*/
                        if (modPlayer.FishSoul2)
                        {
                            SplitProj(projectile, 11, MathHelper.Pi / 3, 1);
                        }
                    }

                    if (modPlayer.BeeEnchant && (projectile.type == ProjectileID.GiantBee || projectile.type == ProjectileID.Bee || projectile.type == ProjectileID.Wasp))
                    {
                        projectile.usesLocalNPCImmunity = true;
                        projectile.localNPCHitCooldown = 5;
                        projectile.penetrate *= 2;
                        projectile.timeLeft *= 2;
                        projectile.scale *= 2.5f;
                        //projectile.damage = (int)(projectile.damage * 1.5);
                        SuperBee = true;

                    }
                }

                if (TungstenProjectile && (!modPlayer.TungstenEnchant || !SoulConfig.Instance.GetValue(SoulConfig.Instance.TungstenProjSize)))
                {
                    projectile.position = projectile.Center;
                    projectile.scale /= 2f;
                    projectile.width /= 2;
                    projectile.height /= 2;
                    projectile.Center = projectile.position;
                    TungstenProjectile = false;
                }

                if (tikiMinion)
                {
                    projectile.alpha = 120;

                    //dust
                    if (Main.rand.Next(4) < 2)
                    {
                        int dust = Dust.NewDust(new Vector2(projectile.position.X - 2f, projectile.position.Y - 2f), projectile.width + 4, projectile.height + 4, 44, projectile.velocity.X * 0.4f, projectile.velocity.Y * 0.4f, 100, Color.LimeGreen, .8f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 1.8f;
                        Dust expr_1CCF_cp_0 = Main.dust[dust];
                        expr_1CCF_cp_0.velocity.Y = expr_1CCF_cp_0.velocity.Y - 0.5f;
                        if (Main.rand.Next(4) == 0)
                        {
                            Main.dust[dust].noGravity = false;
                            Main.dust[dust].scale *= 0.5f;
                        }
                    }

                    tikiTimer--;

                    if (tikiTimer <= 0)
                    {
                        for (int num468 = 0; num468 < 20; num468++)
                        {
                            int num469 = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.width, projectile.height, 44, -projectile.velocity.X * 0.2f,
                                -projectile.velocity.Y * 0.2f, 100, Color.LimeGreen, 1f);
                            Main.dust[num469].noGravity = true;
                            Main.dust[num469].velocity *= 2f;
                            num469 = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.width, projectile.height, 44, -projectile.velocity.X * 0.2f,
                                -projectile.velocity.Y * 0.2f, 100, Color.LimeGreen, .5f);
                            Main.dust[num469].velocity *= 2f;
                        }

                        projectile.Kill();
                    }
                }

                if (SuperBee && (modPlayer.LifeForce || modPlayer.WizardEnchant))
                {
                    projectile.position += projectile.velocity;
                }

                //prob change in 1.4
                if (modPlayer.StardustEnchant && projectile.type == ProjectileID.StardustGuardian)
                {
                    projectile.localAI[0] = 0f;
                }

                //hook ai
                if (modPlayer.MahoganyEnchant && SoulConfig.Instance.GetValue(SoulConfig.Instance.MahoganyHook, false) && projectile.aiStyle == 7 && (modPlayer.WoodForce || modPlayer.WizardEnchant))
                {
                    projectile.extraUpdates = 1;
                }

                if (projectile.friendly && !projectile.hostile)
                {
                    if (modPlayer.ForbiddenEnchant && projectile.damage > 0 && projectile.type != ProjectileID.SandnadoFriendly && !stormBoosted)
                    {
                        Projectile nearestProj = null;

                        List<Projectile> projs = Main.projectile.Where(x => x.type == ProjectileID.SandnadoFriendly && x.active).ToList();

                        foreach (Projectile p in projs)
                        {
                            Vector2 stormDistance = p.Center - projectile.Center;

                            if (Math.Abs(stormDistance.X) < p.width / 2 && Math.Abs(stormDistance.Y) < p.height / 2)
                            {
                                nearestProj = p;
                                break;
                            }
                        }

                        if (nearestProj != null)
                        {
                            float multiplier = 1.3f;

                            if (modPlayer.SpiritForce || modPlayer.WizardEnchant)
                            {
                                multiplier = 1.6f;
                            }

                            preStormDamage = projectile.damage;

                            projectile.damage = (int)(projectile.damage * multiplier);

                            stormBoosted = true;
                            stormTimer = 240;
                        }
                    }

                    if (stormTimer > 0)
                    {
                        stormTimer--;

                        if (stormTimer == 0)
                        {
                            projectile.damage = preStormDamage;
                            stormBoosted = false;
                        }
                    }

                    if (modPlayer.Jammed && projectile.ranged && projectile.type != ProjectileID.ConfettiGun)
                    {
                        Projectile.NewProjectile(projectile.Center, projectile.velocity, ProjectileID.ConfettiGun, 0, 0f);
                        projectile.active = false;
                    }

                    if (modPlayer.Atrophied && projectile.thrown)
                    {
                        projectile.damage = 0;
                        projectile.position = new Vector2(Main.maxTilesX);
                        projectile.Kill();
                    }

                    if (modPlayer.ShroomEnchant && SoulConfig.Instance.GetValue(SoulConfig.Instance.ShroomiteShrooms) && player.stealth == 0 && projectile.damage > 0 && !townNPCProj && projectile.velocity.Length() > 1 && projectile.minionSlots == 0 && projectile.type != ModContent.ProjectileType<ShroomiteShroom>() && player.ownedProjectileCounts[ModContent.ProjectileType<ShroomiteShroom>()] < 50)
                    {
                        if (shroomiteMushroomCD <= 0)
                        {
                            shroomiteMushroomCD = 15;

                            if (modPlayer.NatureForce || modPlayer.WizardEnchant)
                            {
                                shroomiteMushroomCD = 10;
                            }

                            int p = Projectile.NewProjectile(projectile.position.X + (float)(projectile.width / 2), projectile.position.Y + (float)(projectile.height / 2), projectile.velocity.X, projectile.velocity.Y, ModContent.ProjectileType<ShroomiteShroom>(), projectile.damage / 5, 0f, projectile.owner, 0f, 0f);
                        }
                        shroomiteMushroomCD--;
                    }

                    if (modPlayer.SpookyEnchant && SoulConfig.Instance.GetValue(SoulConfig.Instance.SpookyScythes) 
                        && projectile.minion && projectile.minionSlots > 0 && spookyCD == 0)
                    {
                        float minDistance = 500f;
                        int npcIndex = -1;
                        for (int i = 0; i < 200; i++)
                        {
                            NPC target = Main.npc[i];

                            if (target.active && Vector2.Distance(projectile.Center, target.Center) < minDistance && Main.npc[i].CanBeChasedBy(projectile, false))
                            {
                                npcIndex = i;
                                minDistance = Vector2.Distance(projectile.Center, target.Center);
                            }
                        }

                        if (npcIndex != -1)
                        {
                            NPC target = Main.npc[npcIndex];

                            if (Collision.CanHit(projectile.position, projectile.width, projectile.height, target.position, target.width, target.height))
                            {
                                Vector2 velocity = Vector2.Normalize(target.Center - projectile.Center) * 20;

                                int p = Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<SpookyScythe>(), projectile.damage, 2, projectile.owner);

                                Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 62, 0.5f);

                                spookyCD = 30 + Main.rand.Next(player.maxMinions * 5);

                                if (modPlayer.ShadowForce || modPlayer.WizardEnchant)
                                {
                                    spookyCD -= 10;
                                }
                            }
                        }

                    }
                }
            }

            if (ChilledTimer > 0)
            {
                ChilledTimer--;

                if (ChilledTimer == 0)
                {
                    ChilledProj = false;
                }
            }

            if (TimeFrozen > 0 && !firstTick && !TimeFreezeImmune)
            {
                projectile.position = projectile.oldPosition;
                projectile.frameCounter--;
                projectile.timeLeft++;
                TimeFrozen--;
                retVal = false;
            }

            //masomode unicorn meme and pearlwood meme
            if (Rainbow)
            {
                Player p = Main.player[projectile.owner];

                projectile.tileCollide = false;

                counter++;
                if (counter >= 5)
                    projectile.velocity = Vector2.Zero;

                int deathTimer = 15;

                if (projectile.hostile)
                    deathTimer = 60;

                if (counter >= deathTimer)
                    projectile.Kill();
            }

            if (firstTick)
                firstTick = false;

            return retVal;
        }

        public static void SplitProj(Projectile projectile, int number, float maxSpread, float damageRatio)
        {
            if (projectile.type == ModContent.ProjectileType<SpawnProj>())
            {
                return;
            }

            //if its odd, we just keep the original 
            if (number % 2 != 0)
            {
                number--;
            }

            Projectile split;

            double spread = maxSpread / number;

            for (int i = 0; i < number / 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int factor = (j == 0) ? 1 : -1;
                    split = NewProjectileDirectSafe(projectile.Center, projectile.velocity.RotatedBy(factor * spread * (i + 1)), projectile.type, (int)(projectile.damage * damageRatio), projectile.knockBack, projectile.owner, projectile.ai[0], projectile.ai[1]);

                    if (split != null)
                    {
                        split.friendly = true;
                        split.GetGlobalProjectile<FargoGlobalProjectile>().numSplits = projectile.GetGlobalProjectile<FargoGlobalProjectile>().numSplits;
                        //split.GetGlobalProjectile<FargoGlobalProjectile>().firstTick = false;
                        split.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
                        split.GetGlobalProjectile<FargoGlobalProjectile>().TungstenProjectile = projectile.GetGlobalProjectile<FargoGlobalProjectile>().TungstenProjectile;
                    }
                }
            }
        }

        private void KillPet(Projectile projectile, Player player, int buff, bool enchant, bool toggle, bool minion = false)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (player.FindBuffIndex(buff) == -1)
            {
                if (player.dead || !(enchant || modPlayer.TerrariaSoul) || !SoulConfig.Instance.GetValue(toggle) || (!modPlayer.PetsActive && !minion))
                    projectile.Kill();
            }
        }

        public override void AI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            switch (projectile.type)
            {
                #region pets

                case ProjectileID.BabyHornet:
                    KillPet(projectile, player, BuffID.BabyHornet, modPlayer.BeeEnchant, SoulConfig.Instance.HornetPet);
                    break;

                case ProjectileID.Sapling:
                    KillPet(projectile, player, BuffID.PetSapling, modPlayer.ChloroEnchant, SoulConfig.Instance.SeedlingPet);
                    break;

                case ProjectileID.BabyFaceMonster:
                    KillPet(projectile, player, BuffID.BabyFaceMonster, modPlayer.CrimsonEnchant, SoulConfig.Instance.FaceMonsterPet);
                    break;

                case ProjectileID.CrimsonHeart:
                    KillPet(projectile, player, BuffID.CrimsonHeart, modPlayer.CrimsonEnchant, SoulConfig.Instance.CrimsonHeartPet);
                    break;

                case ProjectileID.MagicLantern:
                    KillPet(projectile, player, BuffID.MagicLantern, modPlayer.MinerEnchant, SoulConfig.Instance.MagicLanternPet);
                    break;

                case ProjectileID.MiniMinotaur:
                    KillPet(projectile, player, BuffID.MiniMinotaur, modPlayer.GladEnchant, SoulConfig.Instance.MinotaurPet);
                    break;

                case ProjectileID.BlackCat:
                    KillPet(projectile, player, BuffID.BlackCat, modPlayer.WizardEnchant, SoulConfig.Instance.BlackCatPet);
                    break;

                case ProjectileID.Wisp:
                    KillPet(projectile, player, BuffID.Wisp, modPlayer.SpectreEnchant, SoulConfig.Instance.WispPet);
                    break;

                case ProjectileID.CursedSapling:
                    KillPet(projectile, player, BuffID.CursedSapling, modPlayer.SpookyEnchant, SoulConfig.Instance.CursedSaplingPet);
                    break;

                case ProjectileID.EyeSpring:
                    KillPet(projectile, player, BuffID.EyeballSpring, modPlayer.SpookyEnchant, SoulConfig.Instance.EyeSpringPet);
                    break;

                case ProjectileID.Turtle:
                    KillPet(projectile, player, BuffID.PetTurtle, modPlayer.TurtleEnchant, SoulConfig.Instance.TurtlePet);
                    break;

                case ProjectileID.PetLizard:
                    KillPet(projectile, player, BuffID.PetLizard, modPlayer.TurtleEnchant, SoulConfig.Instance.LizardPet);
                    break;

                case ProjectileID.Truffle:
                    KillPet(projectile, player, BuffID.BabyTruffle, modPlayer.ShroomEnchant, SoulConfig.Instance.TrufflePet);
                    break;

                case ProjectileID.Spider:
                    KillPet(projectile, player, BuffID.PetSpider, modPlayer.SpiderEnchant, SoulConfig.Instance.SpiderPet);
                    break;

                case ProjectileID.Squashling:
                    KillPet(projectile, player, BuffID.Squashling, modPlayer.PumpkinEnchant, SoulConfig.Instance.SquashlingPet);
                    break;

                case ProjectileID.BlueFairy:
                    KillPet(projectile, player, BuffID.FairyBlue, modPlayer.HallowEnchant, SoulConfig.Instance.FairyPet);
                    break;

                case ProjectileID.StardustGuardian:
                    KillPet(projectile, player, BuffID.StardustGuardianMinion, modPlayer.StardustEnchant, SoulConfig.Instance.StardustGuardian, true);
                    break;

                case ProjectileID.TikiSpirit:
                    KillPet(projectile, player, BuffID.TikiSpirit, modPlayer.TikiEnchant, SoulConfig.Instance.TikiPet);
                    break;

                case ProjectileID.Penguin:
                    KillPet(projectile, player, BuffID.BabyPenguin, modPlayer.SnowEnchant, SoulConfig.Instance.PenguinPet);
                    break;

                case ProjectileID.BabySnowman:
                    KillPet(projectile, player, BuffID.BabySnowman, modPlayer.FrostEnchant, SoulConfig.Instance.SnowmanPet);
                    break;

                case ProjectileID.BabyGrinch:
                    KillPet(projectile, player, BuffID.BabyGrinch, modPlayer.FrostEnchant, SoulConfig.Instance.GrinchPet);
                    break;

                case ProjectileID.DD2PetGato:
                    KillPet(projectile, player, BuffID.PetDD2Gato, modPlayer.ShinobiEnchant, SoulConfig.Instance.GatoPet);
                    break;

                case ProjectileID.Parrot:
                    KillPet(projectile, player, BuffID.PetParrot, modPlayer.GoldEnchant, SoulConfig.Instance.ParrotPet);
                    break;

                case ProjectileID.Puppy:
                    KillPet(projectile, player, BuffID.Puppy, modPlayer.RedEnchant, SoulConfig.Instance.PuppyPet);
                    break;

                case ProjectileID.CompanionCube:
                    KillPet(projectile, player, BuffID.CompanionCube, modPlayer.VortexEnchant, SoulConfig.Instance.CompanionCubePet);
                    break;

                case ProjectileID.DD2PetDragon:
                    KillPet(projectile, player, BuffID.PetDD2Dragon, modPlayer.ValhallaEnchant, SoulConfig.Instance.DragonPet);
                    break;

                case ProjectileID.BabySkeletronHead:
                    KillPet(projectile, player, BuffID.BabySkeletronHead, modPlayer.NecroEnchant, SoulConfig.Instance.DGPet);
                    break;

                case ProjectileID.BabyDino:
                    KillPet(projectile, player, BuffID.BabyDinosaur, modPlayer.FossilEnchant, SoulConfig.Instance.DinoPet);
                    break;

                case ProjectileID.BabyEater:
                    KillPet(projectile, player, BuffID.BabyEater, modPlayer.ShadowEnchant, SoulConfig.Instance.EaterPet);
                    break;

                case ProjectileID.ShadowOrb:
                    KillPet(projectile, player, BuffID.ShadowOrb, modPlayer.ShadowEnchant, SoulConfig.Instance.ShadowOrbPet);
                    break;

                case ProjectileID.SuspiciousTentacle:
                    KillPet(projectile, player, BuffID.SuspiciousTentacle, modPlayer.CosmoForce, SoulConfig.Instance.SuspiciousEyePet);
                    break;

                case ProjectileID.DD2PetGhost:
                    KillPet(projectile, player, BuffID.PetDD2Ghost, modPlayer.DarkEnchant, SoulConfig.Instance.FlickerwickPet);
                    break;

                case ProjectileID.ZephyrFish:
                    KillPet(projectile, player, BuffID.ZephyrFish, modPlayer.FishSoul2, SoulConfig.Instance.ZephyrFishPet);
                    break;

                /*case ProjectileID.BabyGrinch:
                    if (player.FindBuffIndex(92) == -1)
                    {
                        if (!modPlayer.GrinchPet)
                        {
                            projectile.Kill();
                            return;
                        }
                    }
                    break;*/

                #endregion

                case ProjectileID.VampireHeal:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        if (!masobool)
                        {
                            masobool = true;
                            if (Main.rand.Next(2) == 0)
                                projectile.Kill();
                        }
                    }
                    break;

                case ProjectileID.DD2SquireSonicBoom:
                    projectile.position += projectile.velocity / 2f;
                    break;

                case ProjectileID.SpiritHeal:
                    if (FargoSoulsWorld.MasochistMode)
                        projectile.position -= projectile.velocity / 2;
                    break;

                case ProjectileID.WireKite:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        if (Main.player[projectile.owner].GetModPlayer<FargoPlayer>().LihzahrdCurse
                            && Framing.GetTileSafely(projectile.Center).wall == WallID.LihzahrdBrickUnsafe)
                        {
                            projectile.Kill();
                        }
                    }
                    break;

                case ProjectileID.NebulaSphere:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.cultBoss, NPCID.CultistBoss))
                            projectile.position += projectile.velocity / 4;
                    }
                    break;

                case ProjectileID.PhantasmalBolt:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.moonBoss, NPCID.MoonLordCore) && !FargoSoulsWorld.SwarmActive)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = -2; i <= 2; i++)
                                {
                                    Projectile.NewProjectile(projectile.Center,
                                        1.5f * Vector2.Normalize(projectile.velocity).RotatedBy(MathHelper.ToRadians(5 * i)),
                                        ModContent.ProjectileType<PhantasmalBolt2>(), projectile.damage, 0f, Main.myPlayer);
                                }
                                projectile.Kill();
                            }
                        }
                    }
                    break;

                case ProjectileID.DeathLaser:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.retiBoss, NPCID.Retinazer) && !FargoSoulsWorld.SwarmActive)
                        {
                            if (!masobool)
                            {
                                masobool = true;
                                projectile.velocity.Normalize();
                            }
                            
                            if (projectile.timeLeft % (projectile.extraUpdates + 1) == 0) //only run once per tick
                            {
                                if (++projectile.localAI[1] < 90)
                                {
                                    projectile.velocity *= 1.06f;
                                }
                            }
                        }
                    }
                    break;

                case ProjectileID.EyeBeam:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        if (EModeGlobalNPC.BossIsAlive(ref NPC.golemBoss, NPCID.Golem) && !FargoSoulsWorld.SwarmActive)
                        {
                            if (!masobool)
                            {
                                masobool = true;
                                projectile.velocity.Normalize();
                            }

                            if (projectile.timeLeft % (projectile.extraUpdates + 1) == 0) //only run once per tick
                            {
                                if (++projectile.localAI[1] < 90)
                                {
                                    projectile.velocity *= 1.05f;
                                }
                            }
                        }
                    }
                    break;

                case ProjectileID.JavelinHostile:
                case ProjectileID.FlamingWood:
                    if (FargoSoulsWorld.MasochistMode)
                        projectile.position += projectile.velocity * .5f;
                    break;

                case ProjectileID.VortexAcid:
                    if (FargoSoulsWorld.MasochistMode)
                        projectile.position += projectile.velocity * .25f;
                    break;

                case ProjectileID.CultistRitual:
                    if (FargoSoulsWorld.MasochistMode && !FargoSoulsWorld.SwarmActive)
                    {
                        if (projectile.ai[1] > -1 && projectile.ai[1] < Main.maxNPCs 
                            && Main.npc[(int)projectile.ai[1]].ai[3] == -1f && Main.npc[(int)projectile.ai[1]].ai[0] == 5)
                        {
                            projectile.Center = Main.player[Main.npc[(int)projectile.ai[1]].target].Center;
                        }

                        if (!masobool) //MP sync data to server
                        {
                            masobool = true;

                            if (projectile.ai[1] > -1 && projectile.ai[1] < Main.maxNPCs)
                            {
                                NPC cultist = Main.npc[(int)projectile.ai[1]];
                                if (Main.netMode == NetmodeID.MultiplayerClient)
                                {
                                    EModeGlobalNPC fargoCultist = cultist.GetGlobalNPC<EModeGlobalNPC>();

                                    var netMessage = mod.GetPacket();
                                    netMessage.Write((byte)10);
                                    netMessage.Write((byte)projectile.ai[1]);
                                    netMessage.Write(fargoCultist.Counter[0]);
                                    netMessage.Write(fargoCultist.Counter[1]);
                                    netMessage.Write(fargoCultist.Counter[2]);
                                    netMessage.Write(cultist.localAI[3]);
                                    netMessage.Send();

                                    fargoCultist.Counter[0] = 0; //clear client side data now
                                    fargoCultist.Counter[1] = 0;
                                    fargoCultist.Counter[2] = 0;
                                    cultist.localAI[3] = 0f;
                                }
                                else //refresh ritual
                                {
                                    for (int i = 0; i < Main.maxProjectiles; i++)
                                    {
                                        if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<CultistRitual>())
                                        {
                                            Main.projectile[i].Kill();
                                            break;
                                        }
                                    }
                                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<CultistRitual>(), 0, 0f, Main.myPlayer, 0f, projectile.whoAmI);
                                }
                            }

                            for (int i = 0; i < Main.maxProjectiles; i++) //purge spectre mask bolts and homing nebula spheres
                            {
                                if (Main.projectile[i].active && (Main.projectile[i].type == ProjectileID.SpectreWrath || Main.projectile[i].type == ProjectileID.NebulaSphere))
                                    Main.projectile[i].Kill();
                            }
                        }

                        if (Fargowiltas.Instance.MasomodeEXLoaded && projectile.ai[0] > 120f && projectile.ai[0] < 299f)
                            projectile.ai[0] = 299f;

                        bool dunk = false;

                        if (projectile.ai[1] == -1)
                        {
                            if (counter == 5)
                                dunk = true;
                        }
                        else
                        {
                            counter = 0;
                            if (projectile.ai[0] == 299f)
                                dunk = true;
                        }

                        if (dunk) //pillar dunk
                        {
                            int cult = -1;
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (Main.npc[i].active && Main.npc[i].type == NPCID.CultistBoss && Main.npc[i].ai[2] == projectile.whoAmI)
                                {
                                    cult = i;
                                    break;
                                }
                            }

                            if (cult != -1)
                            {
                                float ai0 = Main.rand.Next(4);

                                NPC cultist = Main.npc[cult];
                                EModeGlobalNPC fargoCultist = cultist.GetGlobalNPC<EModeGlobalNPC>();
                                int[] weight = new int[4];
                                weight[0] = fargoCultist.Counter[0];
                                weight[1] = fargoCultist.Counter[1];
                                weight[2] = fargoCultist.Counter[2];
                                weight[3] = (int)cultist.localAI[3];
                                fargoCultist.Counter[0] = 0;
                                fargoCultist.Counter[1] = 0;
                                fargoCultist.Counter[2] = 0;
                                cultist.localAI[3] = 0f;
                                int max = 0;
                                for (int i = 1; i < 4; i++)
                                    if (weight[max] < weight[i])
                                        max = i;
                                if (weight[max] > 0)
                                    ai0 = max;

                                if ((cultist.life < cultist.lifeMax / 2 || Fargowiltas.Instance.MasomodeEXLoaded) && Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(projectile.Center, Vector2.UnitY * -10f, ModContent.ProjectileType<CelestialPillar>(),
                                        75, 0f, Main.myPlayer, ai0);
                            }
                        }
                    }
                    break;

                case ProjectileID.MoonLeech:
                    if (FargoSoulsWorld.MasochistMode && projectile.ai[0] > 0f && !FargoSoulsWorld.SwarmActive)
                    {
                        Vector2 distance = Main.player[(int)projectile.ai[1]].Center - projectile.Center - projectile.velocity;
                        if (distance != Vector2.Zero)
                            projectile.position += Vector2.Normalize(distance) * Math.Min(16f, distance.Length());
                    }
                    break;

                case ProjectileID.DesertDjinnCurse:
                    if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<NPCs.Champions.SpiritChampion>())
                        && projectile.damage > 0)
                    {
                        projectile.damage = Main.npc[EModeGlobalNPC.championBoss].damage / 4;
                    }
                    break;

                case ProjectileID.SandnadoHostile:
                    if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.deviBoss, ModContent.NPCType<NPCs.DeviBoss.DeviBoss>())
                        && projectile.Distance(Main.npc[EModeGlobalNPC.deviBoss].Center) < 2000f)
                    {
                        projectile.damage = Main.npc[EModeGlobalNPC.deviBoss].damage / 4;
                        if (Main.npc[EModeGlobalNPC.deviBoss].ai[0] != 5 && projectile.timeLeft > 90)
                            projectile.timeLeft = 90;
                    }
                    else if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<NPCs.Champions.SpiritChampion>()))
                    {
                        projectile.damage = Main.npc[EModeGlobalNPC.championBoss].damage / 4;
                    }
                    else if (FargoSoulsWorld.MasochistMode && projectile.timeLeft == 1199 && NPC.CountNPCS(NPCID.SandShark) < 10 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int n = NPC.NewNPC((int)projectile.Center.X, (int)projectile.Center.Y, NPCID.SandShark);
                        if (n < 200)
                        {
                            Main.npc[n].velocity.X = Main.rand.NextFloat(-10, 10);
                            Main.npc[n].velocity.Y = Main.rand.NextFloat(-20, -10);
                            Main.npc[n].netUpdate = true;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                    }
                    break;

                case ProjectileID.GoldenShowerHostile:
                    /*if (FargoSoulsWorld.MasochistMode && Main.netMode != NetmodeID.MultiplayerClient && Main.rand.Next(6) == 0
                        && !(projectile.position.Y / 16 > Main.maxTilesY - 200 && FargoSoulsGlobalNPC.BossIsAlive(ref FargoSoulsGlobalNPC.wallBoss, NPCID.WallofFlesh)))
                    {
                        int p = Projectile.NewProjectile(projectile.Center, projectile.velocity, ProjectileID.CrimsonSpray, 0, 0f, Main.myPlayer, 8f);
                        if (p != 1000)
                            Main.projectile[p].timeLeft = 6;
                    }*/
                    break;

                case ProjectileID.RuneBlast:
                    if (FargoSoulsWorld.MasochistMode && projectile.ai[0] == 1f)
                    {
                        if (projectile.localAI[0] == 0f)
                        {
                            projectile.localAI[0] = projectile.Center.X;
                            projectile.localAI[1] = projectile.Center.Y;
                        }
                        Vector2 distance = projectile.Center - new Vector2(projectile.localAI[0], projectile.localAI[1]);
                        if (distance != Vector2.Zero && distance.Length() >= 300f)
                        {
                            projectile.velocity = distance.RotatedBy(Math.PI / 2);
                            projectile.velocity.Normalize();
                            projectile.velocity *= 8f;
                        }
                    }
                    break;
    
                case ProjectileID.PhantasmalEye:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.moonBoss, NPCID.MoonLordCore) && !FargoSoulsWorld.SwarmActive)
                        {
                            if (projectile.ai[0] == 2) //diving down and homing
                            {
                                if (++counter > 60)
                                {
                                    projectile.velocity.Y = 9;
                                }
                            }

                            projectile.position.X -= projectile.velocity.X * 0.75f;
                        }
                    }
                    break;

                case ProjectileID.PhantasmalSphere:
                    if (FargoSoulsWorld.MasochistMode && !FargoSoulsWorld.SwarmActive)
                    {
                        if (!masobool)
                        {
                            masobool = true;
                            int ai1 = (int)projectile.ai[1];
                            if (ai1 > -1 && ai1 < Main.maxNPCs && Main.npc[ai1].active && Main.npc[ai1].type == NPCID.MoonLordHand)
                            {
                                projectile.localAI[0] = 1;
                            }
                        }

                        if (projectile.ai[0] == -1 && projectile.localAI[0] > 0) //sent to fly, flagged as from hand
                        {
                            if (++projectile.localAI[1] < 150)
                                projectile.velocity *= 1.02f;

                            if (projectile.localAI[0] == 1 && projectile.velocity.Length() > 11) //only do this once
                            {
                                projectile.localAI[0] = 2;
                                projectile.velocity.Normalize();

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<PhantasmalSphereDeathray>(),
                                        0, 0f, Main.myPlayer, 0f, projectile.whoAmI);
                                }
                            }
                        }
                    }
                    break;

                case ProjectileID.BombSkeletronPrime: //needs to be set every tick
                    if (FargoSoulsWorld.MasochistMode && !FargoSoulsWorld.SwarmActive)
                        projectile.damage = 40;
                    break;

                case ProjectileID.DD2BetsyFireball:
                    if (FargoSoulsWorld.MasochistMode && !FargoSoulsWorld.SwarmActive)
                    {
                        if (!masobool)
                        {
                            masobool = true;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                bool phase2 = EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.betsyBoss, NPCID.DD2Betsy)
                                    && Main.npc[EModeGlobalNPC.betsyBoss].GetGlobalNPC<EModeGlobalNPC>().masoBool[3];
                                int max = phase2 ? 2 : 1;
                                for (int i = 0; i < max; i++)
                                {
                                    Vector2 speed = Main.rand.NextFloat(8, 12) * -Vector2.UnitY.RotatedByRandom(Math.PI / 2);
                                    float ai1 = phase2 ? 60 + Main.rand.Next(60) : 90 + Main.rand.Next(30);
                                    Projectile.NewProjectile(projectile.Center, speed, ModContent.ProjectileType<BetsyPhoenix>(),
                                        projectile.damage, 0f, Main.myPlayer, Player.FindClosest(projectile.Center, 0, 0), ai1);
                                }
                            }
                        }
                    }
                    break;

                case ProjectileID.DD2BetsyFlameBreath:
                    if (FargoSoulsWorld.MasochistMode && !FargoSoulsWorld.SwarmActive)
                    {
                        bool phase2 = EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.betsyBoss, NPCID.DD2Betsy)
                                    && Main.npc[EModeGlobalNPC.betsyBoss].GetGlobalNPC<EModeGlobalNPC>().masoBool[3];

                        if (++counter > (phase2 ? 2 : 4))
                        {
                            counter = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Main.PlaySound(SoundID.Item34, projectile.Center);
                                Vector2 projVel = projectile.velocity.RotatedBy((Main.rand.NextDouble() - 0.5) * Math.PI / 10);
                                projVel.Normalize();
                                projVel *= Main.rand.NextFloat(8f, 12f);
                                int type = ProjectileID.CultistBossFireBall;
                                if (!phase2 || Main.rand.Next(2) == 0)
                                {
                                    type = ModContent.ProjectileType<Champions.WillFireball>();
                                    projVel *= 2f;
                                    if (phase2)
                                        projVel *= 1.5f;
                                }
                                Projectile.NewProjectile(projectile.Center, projVel, type, projectile.damage, 0f, Main.myPlayer);
                            }
                        }
                    }
                    break;

                default:
                        break;
            }

            if (stormBoosted)
            {
                int dustId = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.GoldFlame, projectile.velocity.X, projectile.velocity.Y, 100, default(Color), 1.5f);
                Main.dust[dustId].noGravity = true;
            }

            if (ChilledProj)
            {
                int dustId = Dust.NewDust(projectile.Center, projectile.width, projectile.height, 76, projectile.velocity.X, projectile.velocity.Y, 100, default(Color), 1f);
                Main.dust[dustId].noGravity = true;

                projectile.position -= projectile.velocity * 0.33f;
            }

            if (projectile.bobber && modPlayer.FishSoul1)
            {
                if (projectile.wet && projectile.ai[0] == 0 && projectile.localAI[1] < 655) //ai0 = in water, localai1 = counter up to catching an item
                    projectile.localAI[1] = 655; //quick catch. not 660 and up (that breaks fishron summoning)
            }
        }

        public static void PrintAI(Projectile projectile)
        {
            Main.NewText(projectile.ai[0].ToString() + " " + projectile.ai[1].ToString() + " " + projectile.localAI[0].ToString() + " " + projectile.localAI[1].ToString());
        }

        public override void PostAI(Projectile projectile)
        {
            if (!TimeFreezeCheck)
            {
                TimeFreezeCheck = true;
                if (projectile.whoAmI == Main.player[projectile.owner].heldProj)
                    TimeFreezeImmune = true;
            }

            if (projectile.hostile && projectile.damage > 0 && Main.LocalPlayer.active && !Main.LocalPlayer.dead) //graze
            {
                FargoPlayer fargoPlayer = Main.LocalPlayer.GetModPlayer<FargoPlayer>();
                if (fargoPlayer.Graze && --GrazeCD < 0 && !Main.LocalPlayer.immune && Main.LocalPlayer.hurtCooldowns[0] <= 0 && Main.LocalPlayer.hurtCooldowns[1] <= 0)
                {
                    if (GrazeCheck(projectile))
                    {
                        double grazeCap = 0.3;
                        double grazeGain = 0.02;
                        if (fargoPlayer.CyclonicFin)
                        {
                            grazeCap += 0.2;
                        }
                        if (fargoPlayer.MutantEye)
                        {
                            grazeCap += 0.5;
                            grazeGain += 0.02;
                        }

                        GrazeCD = 60;
                        fargoPlayer.GrazeBonus += grazeGain;
                        if (fargoPlayer.GrazeBonus > grazeCap)
                            fargoPlayer.GrazeBonus = grazeCap;
                        fargoPlayer.GrazeCounter = -1; //reset counter whenever successful graze

                        if (!Main.dedServ) //is this check needed...?
                            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Graze").WithVolume(1f), Main.LocalPlayer.Center);

                        const int max = 30; //make some indicator dusts
                        for (int i = 0; i < max; i++)
                        {
                            Vector2 vector6 = Vector2.UnitY * 5f;
                            vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + Main.LocalPlayer.Center;
                            Vector2 vector7 = vector6 - Main.LocalPlayer.Center;
                            //changes color when bonus is maxed
                            int d = Dust.NewDust(vector6 + vector7, 0, 0, fargoPlayer.GrazeBonus >= grazeCap ? 86 : 228, 0f, 0f, 0, default(Color), 2f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity = vector7;
                        }
                    }
                    else
                    {
                        GrazeCD = 6; //don't check per tick ech
                    }
                }
            }
        }

        public override bool TileCollideStyle(Projectile projectile, ref int width, ref int height, ref bool fallThrough)
        {
            if (projectile.type == ProjectileID.SmokeBomb)
            {
                fallThrough = false;
            }

            if (TungstenProjectile)
            {
                width /= 2;
                height /= 2;
            }

            return base.TileCollideStyle(projectile, ref width, ref height, ref fallThrough);
        }

        public override Color? GetAlpha(Projectile projectile, Color lightColor)
        {
            if (IsRecolor)
            {
                if (projectile.type == ProjectileID.HarpyFeather)
                {
                    projectile.Name = "Vulture Feather";
                    return Color.Brown;
                }

                else if (projectile.type == ProjectileID.PineNeedleFriendly)
                {
                    return Color.GreenYellow;
                }

                else if(projectile.type == ProjectileID.Bone)
                {
                    return Color.SandyBrown;
                }

                else if (projectile.type == ProjectileID.DemonScythe)
                {
                    projectile.Name = "Blood Scythe";
                    return Color.Red;
                }
            }

            return null;
        }

        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (FrostFreeze)
            {
                FargoSoulsGlobalNPC globalNPC = target.GetGlobalNPC<FargoSoulsGlobalNPC>();

                globalNPC.frostCount++;

                if (globalNPC.frostCD == 0)
                {
                    globalNPC.frostCD = 30;
                }

                if (globalNPC.frostCount > 10)
                {
                    if (!target.HasBuff(ModContent.BuffType<TimeFrozen>()))
                    {
                        target.AddBuff(ModContent.BuffType<TimeFrozen>(), 60);
                        target.AddBuff(BuffID.Chilled, 300);
                    }
                    else
                    {
                        //full 30 icicles means 2 extra seconds of freeze pog
                        target.AddBuff(ModContent.BuffType<TimeFrozen>(), 3);
                    }
                    
                }
            }
        }

        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            Player player = Main.player[Main.myPlayer];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (modPlayer.NinjaEnchant && projectile.type == ProjectileID.SmokeBomb && !ninjaTele)
            {
                ninjaTele = true;

                Vector2 teleportPos = new Vector2(projectile.position.X, projectile.position.Y - 30);
                Vector2 originalPos = new Vector2(teleportPos.X, teleportPos.Y);


                //spiral out to find a save spot
                int count = 0;
                int increase = 10;
                while (Collision.SolidCollision(teleportPos, player.width, player.height))
                {
                    teleportPos = originalPos;

                    switch (count)
                    {
                        case 0:
                            teleportPos.X -= increase;
                            break;
                        case 1:
                            teleportPos.X += increase;
                            break;
                        case 2:
                            teleportPos.Y += increase;
                            break;
                        default:
                            teleportPos.Y -= increase;
                            increase += 10;
                            break;
                    }
                    count++;

                    if (count >= 4)
                    {
                        count = 0;
                    }

                    if (increase > 100)
                    {
                        return true;
                    }
                }

                if (teleportPos.X > 50 && teleportPos.X < (double)(Main.maxTilesX * 16 - 50) && teleportPos.Y > 50 && teleportPos.Y < (double)(Main.maxTilesY * 16 - 50))
                {
                    player.Teleport(teleportPos, 1);
                    NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, player.whoAmI, teleportPos.X, teleportPos.Y, 1);

                    player.AddBuff(ModContent.BuffType<FirstStrike>(), 60);

                    projectile.timeLeft = 120;
                }
            }

            
            if (FargoSoulsWorld.MasochistMode)
            {
                switch (projectile.type)
                {
                    case ProjectileID.SnowBallHostile:
                        projectile.active = false;
                        break;
                    case ProjectileID.SandBallFalling:
                        //antlion sand
                        if (projectile.ai[0] == 2f)
                        {
                            int num129 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 32, 0f, projectile.velocity.Y / 2f, 0, default(Color), 1f);
                            Dust expr_59B0_cp_0 = Main.dust[num129];
                            expr_59B0_cp_0.velocity.X = expr_59B0_cp_0.velocity.X * 0.4f;
                            projectile.active = false;
                            
                        }
                        break;
                }
                


            }

            return true;
        }

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref int damage, ref bool crit)
        {
            if(squeakyToy)
            {
                damage = 1;
                target.GetModPlayer<FargoPlayer>().Squeak(target.Center);
            }

            if (FargoSoulsWorld.MasochistMode)
            {
                switch (projectile.type)
                {
                    case ProjectileID.VortexLightning:
                        if (NPC.downedGolemBoss)
                            damage *= 2;
                        break;

                    default:
                        break;
                }
            }
        }

        public override void OnHitPlayer(Projectile projectile, Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.MasochistMode)
            {
                switch(projectile.type)
                {
                    case ProjectileID.JavelinHostile:
                        target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
                        if (!target.HasBuff(ModContent.BuffType<Stunned>()))
                            target.AddBuff(ModContent.BuffType<Stunned>(), 60);
                        break;

                    case ProjectileID.DemonSickle:
                        target.AddBuff(ModContent.BuffType<Shadowflame>(), 300);
                        break;

                    case ProjectileID.HarpyFeather:
                        target.AddBuff(ModContent.BuffType<ClippedWings>(), 300);
                        break;
                        
                    case ProjectileID.SandBallFalling:
                        if (!target.HasBuff(ModContent.BuffType<Stunned>()) && projectile.velocity.X != 0) //so only antlion sand and not falling sand 
                            target.AddBuff(ModContent.BuffType<Stunned>(), 90);
                        break;

                    case ProjectileID.Stinger:
                        target.AddBuff(ModContent.BuffType<Swarming>(), 300);
                        break;

                    case ProjectileID.Skull:
                        if (Main.rand.Next(2) == 0)
                            target.AddBuff(BuffID.Cursed, 60);
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.guardBoss, NPCID.DungeonGuardian))
                        {
                            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 300);
                            /*target.AddBuff(ModContent.BuffType<GodEater>(), 420);
                            target.AddBuff(ModContent.BuffType<FlamesoftheUniverse>(), 420);
                            target.immune = false;
                            target.immuneTime = 0;*/
                        }
                        break;

                    case ProjectileID.EyeLaser:
                    case ProjectileID.GoldenShowerHostile:
                    case ProjectileID.CursedFlameHostile:
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.wallBoss, NPCID.WallofFlesh))
                            target.AddBuff(BuffID.OnFire, 300);
                        break;

                    case ProjectileID.DeathSickle:
                        target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 300);
                        break;

                    case ProjectileID.DrManFlyFlask:
                        switch (Main.rand.Next(7))
                        {
                            case 0:
                                target.AddBuff(BuffID.Venom, 300);
                                break;
                            case 1:
                                target.AddBuff(BuffID.Confused, 300);
                                break;
                            case 2:
                                target.AddBuff(BuffID.CursedInferno, 300);
                                break;
                            case 3:
                                target.AddBuff(BuffID.OgreSpit, 300);
                                break;
                            case 4:
                                target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
                                break;
                            case 5:
                                target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
                                break;
                            case 6:
                                target.AddBuff(ModContent.BuffType<Purified>(), 600);
                                break;

                            default:
                                break;
                        }
                        target.AddBuff(BuffID.Stinky, 1200);
                        break;

                    case ProjectileID.SpikedSlimeSpike:
                        target.AddBuff(BuffID.Slimed, 120);
                        break;

                    case ProjectileID.CultistBossLightningOrb:
                    case ProjectileID.CultistBossLightningOrbArc:
                        target.AddBuff(BuffID.Electrified, 300);
                        break;

                    case ProjectileID.CultistBossIceMist:
                        if (!target.HasBuff(BuffID.Frozen))
                            target.AddBuff(BuffID.Frozen, 60);
                        target.AddBuff(BuffID.Chilled, 120);
                        break;

                    case ProjectileID.CultistBossFireBall:
                        target.AddBuff(BuffID.OnFire, 300);
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.betsyBoss, NPCID.DD2Betsy))
                        {
                            //target.AddBuff(BuffID.OnFire, 600);
                            //target.AddBuff(BuffID.Ichor, 600);
                            target.AddBuff(BuffID.WitheredArmor, Main.rand.Next(60, 300));
                            target.AddBuff(BuffID.WitheredWeapon, Main.rand.Next(60, 300));
                            target.AddBuff(BuffID.Burning, 300);
                        }
                        break;

                    case ProjectileID.CultistBossFireBallClone:
                        target.AddBuff(ModContent.BuffType<Shadowflame>(), 600);
                        break;

                    case ProjectileID.PaladinsHammerHostile:
                        if (!target.HasBuff(ModContent.BuffType<Stunned>()))
                            target.AddBuff(ModContent.BuffType<Stunned>(), 60);
                        break;

                    case ProjectileID.RuneBlast:

                        //target.AddBuff(ModContent.BuffType<Hexed>(), 240);
                        if (!EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.deviBoss, mod.NPCType("DeviBoss")))
                        {
                            target.AddBuff(ModContent.BuffType<FlamesoftheUniverse>(), 60);
                            target.AddBuff(BuffID.Suffocation, 240);
                        }  
                        
                        break;

                    case ProjectileID.ThornBall:
                    case ProjectileID.PoisonSeedPlantera:
                    case ProjectileID.SeedPlantera:
                        //target.AddBuff(BuffID.Poisoned, 300);
                        target.AddBuff(ModContent.BuffType<Infested>(), 180);
                        target.AddBuff(ModContent.BuffType<IvyVenom>(), 180);
                        break;

                    case ProjectileID.DesertDjinnCurse:
                        if (target.ZoneCorrupt)
                            target.AddBuff(BuffID.CursedInferno, 240);
                        else if (target.ZoneCrimson)
                            target.AddBuff(BuffID.Ichor, 240);
                        break;

                    case ProjectileID.BrainScramblerBolt:
                        target.AddBuff(ModContent.BuffType<Flipped>(), 60);
                        target.AddBuff(ModContent.BuffType<Unstable>(), 60);
                        break;

                    case ProjectileID.MartianTurretBolt:
                    case ProjectileID.GigaZapperSpear:
                        target.AddBuff(ModContent.BuffType<LightningRod>(), 300);
                        break;

                    case ProjectileID.RayGunnerLaser:
                        target.AddBuff(BuffID.VortexDebuff, 180);
                        break;

                    case ProjectileID.SaucerMissile:
                        target.AddBuff(ModContent.BuffType<ClippedWings>(), 300);
                        target.AddBuff(ModContent.BuffType<Crippled>(), 300);
                        break;

                    case ProjectileID.SaucerLaser:
                        target.AddBuff(BuffID.Electrified, 600);
                        break;

                    case ProjectileID.UFOLaser:
                    case ProjectileID.SaucerDeathray:
                        target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 180);
                        break;

                    case ProjectileID.FlamingWood:
                    case ProjectileID.GreekFire1:
                    case ProjectileID.GreekFire2:
                    case ProjectileID.GreekFire3:
                        target.AddBuff(ModContent.BuffType<Shadowflame>(), 180);
                        break;

                    case ProjectileID.VortexAcid:
                    case ProjectileID.VortexLaser:
                        target.AddBuff(ModContent.BuffType<LightningRod>(), 600);
                        target.AddBuff(ModContent.BuffType<ClippedWings>(), 300);
                        break;
                        
                    case ProjectileID.VortexLightning:
                        target.AddBuff(BuffID.Electrified, 300);
                        break;

                    case ProjectileID.LostSoulHostile:
                        //target.AddBuff(ModContent.BuffType<Hexed>(), 240);
                        target.AddBuff(ModContent.BuffType<ReverseManaFlow>(), 600);
                        break;

                    case ProjectileID.InfernoHostileBlast:
                    case ProjectileID.InfernoHostileBolt:
                        /*if (!EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.deviBoss, mod.NPCType("DeviBoss")))
                        {
                            if (Main.rand.Next(5) == 0)
                                target.AddBuff(ModContent.BuffType<Fused>(), 1800);
                        }*/
                        target.AddBuff(ModContent.BuffType<Jammed>(), 600);
                        break;

                    case ProjectileID.ShadowBeamHostile:
                        /*if (!EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.deviBoss, mod.NPCType("DeviBoss")))
                        {
                            target.AddBuff(ModContent.BuffType<Rotting>(), 1800);
                            target.AddBuff(ModContent.BuffType<Shadowflame>(), 300);
                        }*/
                        target.AddBuff(ModContent.BuffType<Atrophied>(), 600);
                        break;

                    case ProjectileID.PhantasmalDeathray:
                        target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 360);
                        break;

                    case ProjectileID.PhantasmalBolt:
                    case ProjectileID.PhantasmalEye:
                    case ProjectileID.PhantasmalSphere:
                        target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 360);
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<NPCs.MutantBoss.MutantBoss>()))
                        {
                            /*target.GetModPlayer<FargoPlayer>().MaxLifeReduction += 100;
                            target.AddBuff(ModContent.BuffType<OceanicMaul>(), 5400);*/
                            target.AddBuff(ModContent.BuffType<MutantFang>(), 180);
                        }
                        break;

                    case ProjectileID.RocketSkeleton:
                        target.AddBuff(BuffID.Dazed, 120);
                        target.AddBuff(ModContent.BuffType<Defenseless>(), 300);
                        break;

                    case ProjectileID.FlamesTrap:
                    case ProjectileID.GeyserTrap:
                    case ProjectileID.Fireball:
                    case ProjectileID.EyeBeam:
                        target.AddBuff(BuffID.OnFire, 300);
                        if (NPC.golemBoss != -1 && Main.npc[NPC.golemBoss].active && Main.npc[NPC.golemBoss].type == NPCID.Golem)
                        {
                            target.AddBuff(BuffID.BrokenArmor, 600);
                            target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
                            target.AddBuff(BuffID.WitheredArmor, 600);
                            if (Main.tile[(int)Main.npc[NPC.golemBoss].Center.X / 16, (int)Main.npc[NPC.golemBoss].Center.Y / 16] == null || //outside temple
                                Main.tile[(int)Main.npc[NPC.golemBoss].Center.X / 16, (int)Main.npc[NPC.golemBoss].Center.Y / 16].wall != WallID.LihzahrdBrickUnsafe)
                            {
                                target.AddBuff(BuffID.Burning, 120);
                            }
                        }

                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<EarthChampion>()))
                        {
                            target.AddBuff(BuffID.Burning, 300);
                        }

                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<TerraChampion>()))
                        {
                            target.AddBuff(BuffID.OnFire, 600);
                            target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
                        }
                        break;

                    case ProjectileID.SpikyBallTrap:
                        /*if (NPC.golemBoss != -1 && Main.npc[NPC.golemBoss].active && Main.npc[NPC.golemBoss].type == NPCID.Golem)
                        {
                            target.AddBuff(BuffID.BrokenArmor, 600);
                            target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
                            target.AddBuff(BuffID.WitheredArmor, 600);
                        }*/
                        break;

                    case ProjectileID.DD2BetsyFireball:
                    case ProjectileID.DD2BetsyFlameBreath:
                        //target.AddBuff(BuffID.OnFire, 600);
                        //target.AddBuff(BuffID.Ichor, 600);
                        target.AddBuff(BuffID.WitheredArmor, Main.rand.Next(60, 300));
                        target.AddBuff(BuffID.WitheredWeapon, Main.rand.Next(60, 300));
                        target.AddBuff(BuffID.Burning, 300);
                        break;

                    case ProjectileID.DD2DrakinShot:
                        target.AddBuff(ModContent.BuffType<Shadowflame>(), 600);
                        break;

                    case ProjectileID.NebulaSphere:
                        target.AddBuff(BuffID.VortexDebuff, 300);
                        break;

                    case ProjectileID.NebulaLaser:
                    case ProjectileID.NebulaBolt:
                        target.AddBuff(ModContent.BuffType<Hexed>(), 120);
                        break;

                    case ProjectileID.StardustJellyfishSmall:
                        target.AddBuff(BuffID.Frostburn, 180);
                        break;

                    case ProjectileID.StardustSoldierLaser:
                        target.AddBuff(BuffID.VortexDebuff, 120);
                        break;

                    case ProjectileID.Sharknado:
                        target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
                        target.AddBuff(ModContent.BuffType<OceanicMaul>(), 1800);
                        target.GetModPlayer<FargoPlayer>().MaxLifeReduction += EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.fishBossEX, NPCID.DukeFishron) ? 100 : 10;
                        break;

                    case ProjectileID.FlamingScythe:
                        target.AddBuff(BuffID.OnFire, 900);
                        target.AddBuff(ModContent.BuffType<LivingWasteland>(), 900);
                        break;

                    case ProjectileID.SnowBallHostile:
                        if (!target.HasBuff(BuffID.Frozen) && Main.rand.Next(2) == 0)
                            target.AddBuff(BuffID.Frozen, 60);
                        break;
                        
                    case ProjectileID.BulletSnowman:
                        target.AddBuff(BuffID.Chilled, 180);
                        break;

                    case ProjectileID.UnholyTridentHostile:
                        target.AddBuff(BuffID.Darkness, 300);
                        target.AddBuff(BuffID.Blackout, 300);
                        target.AddBuff(ModContent.BuffType<Shadowflame>(), 600);
                        //target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 180);
                        break;

                    case ProjectileID.BombSkeletronPrime:
                        target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
                        break;

                    case ProjectileID.DeathLaser:
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.retiBoss, NPCID.Retinazer))
                            target.AddBuff(BuffID.Ichor, 600);
                        break;

                    case ProjectileID.BulletDeadeye:
                    case ProjectileID.CannonballHostile:
                        target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
                        target.AddBuff(ModContent.BuffType<Midas>(), 900);
                        break;

                    case ProjectileID.AncientDoomProjectile:
                        target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 120);
                        target.AddBuff(ModContent.BuffType<Shadowflame>(), 300);
                        break;

                    case ProjectileID.SandnadoHostile:
                        if (!target.HasBuff(BuffID.Dazed))
                            target.AddBuff(BuffID.Dazed, 120);
                        break;

                    case ProjectileID.DD2OgreSmash:
                        target.AddBuff(BuffID.BrokenArmor, 300);
                        break;

                    case ProjectileID.DD2OgreStomp:
                        target.AddBuff(BuffID.Dazed, 120);
                        break;

                    case ProjectileID.DD2DarkMageBolt:
                        target.AddBuff(ModContent.BuffType<Hexed>(), 240);
                        break;

                    case ProjectileID.IceSpike:
                        //target.AddBuff(BuffID.Slimed, 120);
                        target.AddBuff(BuffID.Frostburn, 120);
                        break;

                    case ProjectileID.JungleSpike:
                        //target.AddBuff(BuffID.Slimed, 120);
                        target.AddBuff(ModContent.BuffType<Infested>(), 300);
                        break;

                    default:
                        break;
                }
            }
        }

        public override bool PreKill(Projectile projectile, int timeLeft)
        {
            if (FargoSoulsWorld.MasochistMode && projectile.owner == Main.myPlayer && HasKillCooldown)
            {
                if (Main.player[projectile.owner].GetModPlayer<FargoPlayer>().MasomodeCrystalTimer <= 60)
                {
                    Main.player[projectile.owner].GetModPlayer<FargoPlayer>().MasomodeCrystalTimer += 12;
                    return true;
                }
                else
                {
                    /*if (projectile.type == ProjectileID.CrystalBullet)
                    {
                        Main.PlaySound(0, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0.0f);
                        for (int index1 = 0; index1 < 5; ++index1) //vanilla dusts
                        {
                            int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 68, 0.0f, 0.0f, 0, new Color(), 1f);
                            Main.dust[index2].noGravity = true;
                            Dust dust1 = Main.dust[index2];
                            dust1.velocity = dust1.velocity * 1.5f;
                            Dust dust2 = Main.dust[index2];
                            dust2.scale = dust2.scale * 0.9f;
                        }
                    }
                    else if (projectile.type == ProjectileID.HolyArrow || projectile.type == ProjectileID.HallowStar)
                    {
                        Main.PlaySound(SoundID.Item10, projectile.position);
                        for (int index = 0; index < 10; ++index)
                            Dust.NewDust(projectile.position, projectile.width, projectile.height, 58, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 150, new Color(), 1.2f);
                        for (int index = 0; index < 3; ++index)
                            Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
                        if (projectile.type == 12 && projectile.damage < 500)
                        {
                            for (int index = 0; index < 10; ++index)
                                Dust.NewDust(projectile.position, projectile.width, projectile.height, 57, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 150, new Color(), 1.2f);
                            for (int index = 0; index < 3; ++index)
                                Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
                        }
                    }*/
                    return false;
                }
            }


            


            return true;
        }

        public override void Kill(Projectile projectile, int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (!townNPCProj && CanSplit && projectile.friendly && projectile.damage > 0 && !projectile.minion && projectile.aiStyle != 19)
            {
                if (modPlayer.CobaltEnchant)
                {
                    if (SoulConfig.Instance.GetValue(SoulConfig.Instance.CobaltShards) && modPlayer.CobaltCD == 0 && Main.rand.Next(4) == 0)
                    {
                        Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 27);

                        for (int i = 0; i < 5; i++)
                        {
                            float velX = -projectile.velocity.X * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                            float velY = -projectile.velocity.Y * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                            int p = Projectile.NewProjectile(projectile.position.X + velX, projectile.position.Y + velY, velX, velY, ProjectileID.CrystalShard, projectile.damage, 0f, projectile.owner);

                            Main.projectile[p].GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
                        }

                        if (modPlayer.TerrariaSoul)
                        {
                            modPlayer.CobaltCD = 30;
                        }
                        else if (modPlayer.EarthForce || modPlayer.WizardEnchant)
                        {
                            modPlayer.CobaltCD = 45;
                        }
                        else
                        {
                            modPlayer.CobaltCD = 60;
                        }
                    }
                }
                else if (modPlayer.AncientCobaltEnchant && !modPlayer.CobaltEnchant && SoulConfig.Instance.GetValue(SoulConfig.Instance.CobaltStingers) && modPlayer.CobaltCD == 0 && Main.rand.Next(5) == 0)
                {
                    Projectile[] projs = XWay(3, projectile.Center, ProjectileID.HornetStinger, 5f, projectile.damage, 0);

                    for (int i = 0; i < projs.Length; i++)
                    {
                        projs[i].penetrate = 3;
                        projs[i].timeLeft /= 2;
                    }

                    modPlayer.CobaltCD = 60;
                }
            }
        }

        public override void GrapplePullSpeed(Projectile projectile, Player player, ref float speed)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (modPlayer.MahoganyEnchant && SoulConfig.Instance.GetValue(SoulConfig.Instance.MahoganyHook, false))
            {
                float multiplier = 1.5f;

                if (modPlayer.WoodForce || modPlayer.WizardEnchant)
                {
                    multiplier = 2.5f;
                }

                speed *= multiplier;
            }
        }

        public override void GrappleRetreatSpeed(Projectile projectile, Player player, ref float speed)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (modPlayer.MahoganyEnchant && SoulConfig.Instance.GetValue(SoulConfig.Instance.MahoganyHook, false))
            {
                float multiplier = 1.5f;

                if (modPlayer.WoodForce || modPlayer.WizardEnchant)
                {
                    multiplier = 2.5f;
                }

                speed *= multiplier;
            }
        }

        public static Projectile[] XWay(int num, Vector2 pos, int type, float speed, int damage, float knockback)
        {
            Projectile[] projs = new Projectile[num];
            double spread = 2 * Math.PI / num;
            for (int i = 0; i < num; i++)
                projs[i] = NewProjectileDirectSafe(pos, new Vector2(speed, speed).RotatedBy(spread * i), type, damage, knockback, Main.myPlayer);
            return projs;
        }

        public static int CountProj(int type)
        {
            int count = 0;

            for (int i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].type == type)
                {
                    count++;
                }
            }

            return count;
        }

        public static Projectile NewProjectileDirectSafe(Vector2 pos, Vector2 vel, int type, int damage, float knockback, int owner = 255, float ai0 = 0f, float ai1 = 0f)
        {
            int p = Projectile.NewProjectile(pos, vel, type, damage, knockback, owner, ai0, ai1);
            return (p < 1000) ? Main.projectile[p] : null;
        }
    }
}
