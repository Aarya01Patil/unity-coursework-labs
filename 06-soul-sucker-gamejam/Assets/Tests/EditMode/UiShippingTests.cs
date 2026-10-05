using NUnit.Framework;
using SoulSucker.Audio;
using SoulSucker.Core;
using SoulSucker.Data;
using SoulSucker.Player;
using SoulSucker.UI;
using SoulSucker.World;
using UnityEngine;

namespace SoulSucker.Tests.EditMode
{
    public sealed class UiShippingTests
    {
        [Test]
        public void MainMenu_PlayStartsGameplayRoute()
        {
            GameSession session = GameSession.EnsureInstance();
            var controllerObject = new GameObject("MainMenuController");
            MainMenuController controller = controllerObject.AddComponent<MainMenuController>();

            try
            {
                controller.HandlePlayClicked();
                Assert.AreEqual("Gameplay", session.CurrentRoute);
            }
            finally
            {
                Object.DestroyImmediate(controllerObject);
                if (GameSession.Instance != null)
                {
                    Object.DestroyImmediate(GameSession.Instance.gameObject);
                }
            }
        }

        [Test]
        public void Hud_UpdatesWhenChangingHost()
        {
            GameObject root = CreateGameplayRoot(out PlayerBrain brain, out LevelManager manager);
            _ = manager;

            try
            {
                HUDController hud = root.AddComponent<HUDController>();
                HostLoopHelper.CreateJumperHost(new Vector3(0.5f, 0f, 0f));
                brain.ParasiteBody.TeleportTo(Vector3.zero);
                Assert.IsTrue(brain.TryPossessNearestHost());

                hud.Refresh();

                StringAssert.Contains("Jumper", hud.CurrentHostLabel);
                StringAssert.Contains("High vertical burst", hud.CurrentHostSummary);
                StringAssert.Contains("Space Jump", hud.CurrentHostControls);
            }
            finally
            {
                HostLoopHelper.DestroyAllHosts();
                Object.DestroyImmediate(root);
                DestroySession();
            }
        }

        [Test]
        public void Hud_AirTimerRespondsToCountdown()
        {
            GameObject root = CreateGameplayRoot(out PlayerBrain brain, out LevelManager manager);
            _ = manager;

            try
            {
                HUDController hud = root.AddComponent<HUDController>();
                hud.Refresh();
                float initial = hud.CurrentAirFillAmount;

                brain.ConsumeAirTime(1.5f);
                hud.Refresh();

                Assert.Less(hud.CurrentAirFillAmount, initial);
            }
            finally
            {
                Object.DestroyImmediate(root);
                DestroySession();
            }
        }

        [Test]
        public void VictoryOverlay_AppearsAfterFinalRoom()
        {
            GameObject root = CreateGameplayRoot(out PlayerBrain brain, out LevelManager manager);
            OverlayController overlay = root.AddComponent<OverlayController>();
            _ = brain;

            try
            {
                manager.BuildLevel(LevelCatalog.Rooms.Count - 1);
                manager.CompleteCurrentRoom();

                Assert.IsTrue(overlay.IsVictoryVisible);
            }
            finally
            {
                Object.DestroyImmediate(root);
                DestroySession();
            }
        }

        [Test]
        public void BuildScript_EntryPointsExist()
        {
            var type = typeof(SoulSucker.Editor.Build.BuildScript);

            Assert.NotNull(type.GetMethod("BuildWebDevelopment"));
            Assert.NotNull(type.GetMethod("BuildWebRelease"));
        }

        private static GameObject CreateGameplayRoot(out PlayerBrain brain, out LevelManager manager)
        {
            GameSession.EnsureInstance().SetCurrentRoute("Gameplay");

            var root = new GameObject("GameplayRoot");
            root.AddComponent<LevelBuilder>();
            manager = root.AddComponent<LevelManager>();
            root.AddComponent<HostDetector>();
            root.AddComponent<AudioRouter>();
            brain = root.AddComponent<PlayerBrain>();
            _ = brain.ParasiteBody;
            manager.BuildLevel(0);
            return root;
        }

        private static void DestroySession()
        {
            if (GameSession.Instance != null)
            {
                Object.DestroyImmediate(GameSession.Instance.gameObject);
            }
        }

        private static class HostLoopHelper
        {
            public static void CreateJumperHost(Vector3 worldPosition)
            {
                var hostObject = new GameObject("TestHost");
                hostObject.transform.position = worldPosition;
                hostObject.AddComponent<SpriteRenderer>();
                hostObject.AddComponent<BoxCollider2D>().isTrigger = false;
                hostObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

                var releaseAnchor = new GameObject("ReleaseAnchor");
                releaseAnchor.transform.SetParent(hostObject.transform, false);
                releaseAnchor.transform.localPosition = Vector3.up;

                var host = hostObject.AddComponent<SoulSucker.Hosts.JumperHost>();
                host.Initialize(HostType.Jumper, releaseAnchor.transform);
            }

            public static void DestroyAllHosts()
            {
                foreach (SoulSucker.Hosts.HostBase host in Object.FindObjectsByType<SoulSucker.Hosts.HostBase>(FindObjectsSortMode.None))
                {
                    Object.DestroyImmediate(host.gameObject);
                }
            }
        }
    }
}
