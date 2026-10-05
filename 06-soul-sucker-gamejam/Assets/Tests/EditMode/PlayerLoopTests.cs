using NUnit.Framework;
using SoulSucker.Audio;
using SoulSucker.Data;
using SoulSucker.Hosts;
using SoulSucker.Player;
using SoulSucker.World;
using UnityEngine;

namespace SoulSucker.Tests.EditMode
{
    public sealed class PlayerLoopTests
    {
        [Test]
        public void Possession_FailsWhenNoHostInRange()
        {
            GameObject root = CreateGameplayRoot(out PlayerBrain brain, out ParasiteBody parasite);
            parasite.TeleportTo(Vector3.zero);

            try
            {
                CreateHost(new Vector3(5f, 0f, 0f));
                bool didPossess = brain.TryPossessNearestHost();

                Assert.IsFalse(didPossess);
                Assert.AreEqual(PlayerBrain.PlayerState.ParasiteFree, brain.CurrentState);
            }
            finally
            {
                Object.DestroyImmediate(root);
                DestroyAllHosts();
                DestroyAllAudioRouters();
            }
        }

        [Test]
        public void Possession_SucceedsWhenHostIsInRange()
        {
            GameObject root = CreateGameplayRoot(out PlayerBrain brain, out ParasiteBody parasite);
            parasite.TeleportTo(Vector3.zero);

            try
            {
                HostBase host = CreateHost(new Vector3(0.75f, 0f, 0f));
                bool didPossess = brain.TryPossessNearestHost();

                Assert.IsTrue(didPossess);
                Assert.AreEqual(PlayerBrain.PlayerState.InsideHost, brain.CurrentState);
                Assert.AreEqual(host, brain.CurrentHost);
            }
            finally
            {
                Object.DestroyImmediate(root);
                DestroyAllHosts();
                DestroyAllAudioRouters();
            }
        }

        [Test]
        public void ReleasingFromHost_ReturnsToParasiteState()
        {
            GameObject root = CreateGameplayRoot(out PlayerBrain brain, out ParasiteBody parasite);
            parasite.TeleportTo(Vector3.zero);

            try
            {
                CreateHost(new Vector3(0.5f, 0f, 0f));
                Assert.IsTrue(brain.TryPossessNearestHost());

                brain.ReleaseHost();

                Assert.AreEqual(PlayerBrain.PlayerState.ParasiteFree, brain.CurrentState);
                Assert.IsNull(brain.CurrentHost);
                Assert.IsTrue(brain.ParasiteBody.IsActive);
            }
            finally
            {
                Object.DestroyImmediate(root);
                DestroyAllHosts();
                DestroyAllAudioRouters();
            }
        }

        [Test]
        public void AirTimer_KillsParasiteAtZero()
        {
            GameObject root = CreateGameplayRoot(out PlayerBrain brain, out ParasiteBody parasite);
            parasite.TeleportTo(Vector3.zero);

            try
            {
                brain.ConsumeAirTime(3.1f);

                Assert.AreEqual(PlayerBrain.PlayerState.Dead, brain.CurrentState);
                Assert.IsFalse(brain.ParasiteBody.IsActive);
            }
            finally
            {
                Object.DestroyImmediate(root);
                DestroyAllAudioRouters();
            }
        }

        [Test]
        public void Jumper_HighJumpIsStrongerThanParasiteHop()
        {
            GameObject root = CreateGameplayRoot(out PlayerBrain brain, out ParasiteBody parasite);
            _ = brain;

            try
            {
                JumperHost host = (JumperHost)CreateHost(new Vector3(0.5f, 0f, 0f));

                Assert.Greater(host.JumpImpulse, parasite.HopImpulse * 3f);
            }
            finally
            {
                Object.DestroyImmediate(root);
                DestroyAllHosts();
                DestroyAllAudioRouters();
            }
        }

        [Test]
        public void Restart_RebuildsCurrentRoom()
        {
            GameObject gameplayRoot = new GameObject("GameplayRoot");
            LevelBuilder builder = gameplayRoot.AddComponent<LevelBuilder>();
            LevelManager levelManager = gameplayRoot.AddComponent<LevelManager>();
            HostDetector detector = gameplayRoot.AddComponent<HostDetector>();
            PlayerBrain brain = gameplayRoot.AddComponent<PlayerBrain>();
            _ = builder;
            _ = detector;
            _ = brain.ParasiteBody;

            try
            {
                levelManager.BuildCurrentLevel();
                GameObject firstRoot = levelManager.CurrentRoomRoot;

                brain.RequestRestart();
                GameObject rebuiltRoot = levelManager.CurrentRoomRoot;

                Assert.NotNull(firstRoot);
                Assert.NotNull(rebuiltRoot);
                Assert.AreNotSame(firstRoot, rebuiltRoot);
            }
            finally
            {
                Object.DestroyImmediate(gameplayRoot);
                DestroyAllHosts();
                DestroyAllAudioRouters();
            }
        }

        private static GameObject CreateGameplayRoot(out PlayerBrain brain, out ParasiteBody parasite)
        {
            var root = new GameObject("GameplayRoot");
            root.AddComponent<HostDetector>();
            brain = root.AddComponent<PlayerBrain>();
            parasite = brain.ParasiteBody;
            return root;
        }

        private static HostBase CreateHost(Vector3 worldPosition)
        {
            var hostObject = new GameObject("TestHost");
            hostObject.transform.position = worldPosition;
            hostObject.AddComponent<SpriteRenderer>();
            hostObject.AddComponent<BoxCollider2D>().isTrigger = false;
            hostObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

            var releaseAnchor = new GameObject("ReleaseAnchor");
            releaseAnchor.transform.SetParent(hostObject.transform, false);
            releaseAnchor.transform.localPosition = Vector3.up;

            var host = hostObject.AddComponent<JumperHost>();
            host.Initialize(HostType.Jumper, releaseAnchor.transform);
            return host;
        }

        private static void DestroyAllHosts()
        {
            foreach (HostBase host in Object.FindObjectsByType<HostBase>(FindObjectsSortMode.None))
            {
                Object.DestroyImmediate(host.gameObject);
            }
        }

        private static void DestroyAllAudioRouters()
        {
            foreach (AudioRouter router in Object.FindObjectsByType<AudioRouter>(FindObjectsSortMode.None))
            {
                Object.DestroyImmediate(router.gameObject);
            }
        }
    }
}
