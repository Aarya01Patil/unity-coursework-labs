using NUnit.Framework;
using SoulSucker.Data;
using SoulSucker.Player;
using SoulSucker.World;
using UnityEngine;

namespace SoulSucker.Tests.EditMode
{
    public sealed class CampaignProgressionTests
    {
        [Test]
        public void LevelOne_StartsWithJumperTutorial()
        {
            LevelBlueprint level = LevelCatalog.GetLevel(0);

            Assert.AreEqual(HostType.Jumper, level.hosts[0].type);
            Assert.Greater(level.exitCell.y, level.hosts[0].cell.y);
        }

        [Test]
        public void LevelTwo_UsesDasherAndHazardLane()
        {
            LevelBlueprint level = LevelCatalog.GetLevel(1);

            Assert.AreEqual(HostType.Dasher, level.hosts[0].type);
            Assert.IsTrue(level.hazards.Length > 0 || level.laserGates.Length > 0);
        }

        [Test]
        public void LevelFour_IsAnchorTutorial()
        {
            LevelBlueprint level = LevelCatalog.GetLevel(3);

            Assert.AreEqual(1, level.hosts.Length);
            Assert.AreEqual(HostType.Anchor, level.hosts[0].type);
            Assert.IsTrue(level.pressurePlates.Length > 0);
            Assert.IsTrue(level.heavyBlocks.Length > 0);
        }

        [Test]
        public void LevelFive_IsPhaseTutorial()
        {
            LevelBlueprint level = LevelCatalog.GetLevel(4);

            Assert.AreEqual(1, level.hosts.Length);
            Assert.AreEqual(HostType.Phase, level.hosts[0].type);
            Assert.IsTrue(level.phaseGates.Length > 0);
        }

        [Test]
        public void LevelSix_UsesThreeHostSequence()
        {
            LevelBlueprint level = LevelCatalog.GetLevel(5);

            CollectionAssert.AreEquivalent(
                new[] { HostType.Jumper, HostType.Shooter, HostType.Dasher },
                System.Array.ConvertAll(level.hosts, host => host.type));
        }

        [Test]
        public void LevelSeven_UsesFourHostChain()
        {
            LevelBlueprint level = LevelCatalog.GetLevel(6);

            CollectionAssert.AreEquivalent(
                new[] { HostType.Anchor, HostType.Phase, HostType.Shooter, HostType.Jumper },
                System.Array.ConvertAll(level.hosts, host => host.type));
        }

        [Test]
        public void FinalLevel_UsesAllFiveHostMechanisms()
        {
            LevelBlueprint level = LevelCatalog.GetLevel(LevelCatalog.Rooms.Count - 1);

            Assert.AreEqual(5, level.hosts.Length);
            CollectionAssert.AreEquivalent(
                new[] { HostType.Anchor, HostType.Phase, HostType.Shooter, HostType.Jumper, HostType.Dasher },
                System.Array.ConvertAll(level.hosts, host => host.type));
        }

        [Test]
        public void ShooterProjectile_DestroysBreakableBlock()
        {
            var breakableObject = new GameObject("Breakable");
            breakableObject.AddComponent<SpriteRenderer>();
            breakableObject.AddComponent<BoxCollider2D>().isTrigger = true;
            _ = breakableObject.AddComponent<BreakableBlock>();

            var projectileObject = new GameObject("Projectile");
            projectileObject.AddComponent<SpriteRenderer>();
            projectileObject.AddComponent<CircleCollider2D>().isTrigger = true;
            Projectile projectile = projectileObject.AddComponent<Projectile>();
            projectile.Initialize(Vector2.right, 10f);

            try
            {
                projectileObject.SendMessage("OnTriggerEnter2D", breakableObject.GetComponent<Collider2D>());
                Assert.IsTrue(breakableObject == null || breakableObject.Equals(null));
            }
            finally
            {
                if (projectileObject != null)
                {
                    Object.DestroyImmediate(projectileObject);
                }
            }
        }

        [Test]
        public void FinalLevel_CanTriggerVictoryState()
        {
            var root = CreateGameplayRoot(out LevelManager manager, out PlayerBrain brain);

            try
            {
                manager.BuildLevel(LevelCatalog.Rooms.Count - 1);
                manager.CompleteCurrentRoom();

                Assert.IsTrue(manager.HasWonCampaign);
                Assert.IsTrue(brain.HasWonCampaign);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        private static GameObject CreateGameplayRoot(out LevelManager manager, out PlayerBrain brain)
        {
            var root = new GameObject("GameplayRoot");
            root.AddComponent<LevelBuilder>();
            manager = root.AddComponent<LevelManager>();
            root.AddComponent<HostDetector>();
            brain = root.AddComponent<PlayerBrain>();
            _ = brain.ParasiteBody;
            return root;
        }
    }
}
