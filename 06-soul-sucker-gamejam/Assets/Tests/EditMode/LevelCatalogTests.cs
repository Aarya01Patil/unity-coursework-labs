using NUnit.Framework;
using SoulSucker.Data;
using SoulSucker.World;
using UnityEngine;

namespace SoulSucker.Tests.EditMode
{
    public sealed class LevelCatalogTests
    {
        [Test]
        public void LevelCatalog_HasExactlyEightLevels()
        {
            Assert.AreEqual(8, LevelCatalog.Rooms.Count);
        }

        [Test]
        public void EveryLevel_HasExitAndValidBounds()
        {
            foreach (LevelBlueprint level in LevelCatalog.Rooms)
            {
                Assert.IsTrue(level.HasExit(), $"Level '{level.id}' is missing an exit.");
                Assert.AreEqual(LevelCatalog.ExpectedLevelHeight, level.levelSize.y, $"Level '{level.id}' has the wrong height.");
                Assert.Greater(level.levelSize.x, 0, $"Level '{level.id}' has an invalid width.");
                Assert.Greater(level.cameraBounds.width, 0, $"Level '{level.id}' has invalid camera bounds.");
                Assert.Greater(level.cameraBounds.height, 0, $"Level '{level.id}' has invalid camera bounds.");
            }
        }

        [Test]
        public void LevelWidths_IncreaseAcrossCampaign()
        {
            int previousWidth = 0;
            foreach (LevelBlueprint level in LevelCatalog.Rooms)
            {
                Assert.Greater(level.levelSize.x, previousWidth, $"Level '{level.id}' did not increase campaign width.");
                previousWidth = level.levelSize.x;
            }
        }

        [Test]
        public void LevelBuilder_CanBuildLevelOneWithoutNullReferences()
        {
            var builderHost = new GameObject("LevelBuilderTestHost");

            try
            {
                var builder = builderHost.AddComponent<LevelBuilder>();
                GameObject roomRoot = builder.BuildLevel(LevelCatalog.GetLevel(0), 0);

                Assert.NotNull(roomRoot);
                Assert.NotNull(roomRoot.transform.Find("Environment"));
                Assert.NotNull(roomRoot.transform.Find("Solids"));
                Assert.NotNull(roomRoot.transform.Find("Hosts"));
                Assert.NotNull(roomRoot.transform.Find("Exit"));
            }
            finally
            {
                Object.DestroyImmediate(builderHost);
            }
        }
    }
}
