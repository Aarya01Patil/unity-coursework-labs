using SoulSucker.Core;
using SoulSucker.Data;
using System;
using UnityEngine;

namespace SoulSucker.World
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LevelBuilder))]
    public sealed class LevelManager : MonoBehaviour
    {
        [SerializeField] private int startingLevelIndex;
        [SerializeField] private bool buildOnStart = true;

        private LevelBuilder levelBuilder;
        private GameObject currentRoomRoot;
        private GameObject victoryRoot;
        private int currentLevelIndex;
        private bool hasInitialized;
        private bool hasWonCampaign;

        public int CurrentLevelIndex => currentLevelIndex;
        public LevelBlueprint CurrentLevel => LevelCatalog.GetLevel(currentLevelIndex);
        public GameObject CurrentRoomRoot => currentRoomRoot;
        public bool HasWonCampaign => hasWonCampaign;
        public event Action<LevelBlueprint, GameObject> LevelBuilt;
        public event Action VictoryAchieved;

        private void Awake()
        {
            EnsureReferences();
            EnsureGameplayCamera();
        }

        private void Start()
        {
            GameSession.EnsureInstance().BeginCampaignRun();
            if (buildOnStart)
            {
                BuildCurrentLevel();
            }
        }

        public void BuildCurrentLevel()
        {
            EnsureReferences();
            BuildLevel(currentLevelIndex);
        }

        public void BuildLevel(int levelIndex)
        {
            EnsureReferences();
            hasWonCampaign = false;
            ClearVictoryRoot();
            currentLevelIndex = Mathf.Clamp(levelIndex, 0, LevelCatalog.Rooms.Count - 1);
            DestroyCurrentRoomRoot();
            LevelBlueprint blueprint = LevelCatalog.GetLevel(currentLevelIndex);
            currentRoomRoot = levelBuilder.BuildLevel(blueprint, currentLevelIndex);
            LevelBuilt?.Invoke(blueprint, currentRoomRoot);
            SnapCameraToCurrentTarget();
        }

        public void RebuildCurrentLevel()
        {
            EnsureReferences();
            BuildLevel(currentLevelIndex);
        }

        public void CompleteCurrentRoom()
        {
            EnsureReferences();
            if (hasWonCampaign)
            {
                return;
            }

            if (currentLevelIndex >= LevelCatalog.Rooms.Count - 1)
            {
                EnterVictoryState();
                return;
            }

            BuildLevel(currentLevelIndex + 1);
        }

        public void DestroyCurrentRoomRoot()
        {
            if (currentRoomRoot == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(currentRoomRoot);
            }
            else
            {
                DestroyImmediate(currentRoomRoot);
            }

            currentRoomRoot = null;
        }

        public Vector3 GetCurrentPlayerSpawnWorldPosition()
        {
            EnsureReferences();
            return LevelBuilder.CellToLocalPosition(CurrentLevel.playerStart, CurrentLevel.roomSize) + transform.position;
        }

        public Rect GetCurrentCameraBoundsWorld()
        {
            EnsureReferences();
            LevelBlueprint currentLevel = CurrentLevel;
            Vector2Int levelSize = currentLevel.levelSize;
            IntRect boundsRect = currentLevel.cameraBounds.width > 0 && currentLevel.cameraBounds.height > 0
                ? currentLevel.cameraBounds
                : new IntRect(0, 0, levelSize.x, levelSize.y);
            Rect localRect = LevelBuilder.RectToLocalRect(boundsRect, levelSize);
            localRect.position += (Vector2)transform.position;
            return localRect;
        }

        private void EnterVictoryState()
        {
            hasWonCampaign = true;
            GameSession.EnsureInstance().MarkVictory();
            ClearVictoryRoot();
            victoryRoot = CreateVictoryRoot();
            VictoryAchieved?.Invoke();
        }

        private void EnsureReferences()
        {
            if (levelBuilder == null)
            {
                levelBuilder = GetComponent<LevelBuilder>();
            }

            if (!hasInitialized)
            {
                currentLevelIndex = Mathf.Clamp(startingLevelIndex, 0, LevelCatalog.Rooms.Count - 1);
                hasInitialized = true;
            }
        }

        private GameObject CreateVictoryRoot()
        {
            var root = new GameObject("VictoryRoot");
            root.transform.SetParent(transform, false);

            var labelObject = new GameObject("VictoryLabel");
            labelObject.transform.SetParent(root.transform, false);
            labelObject.transform.position = new Vector3(0f, 4f, 0f);

            var textMesh = labelObject.AddComponent<TextMesh>();
            textMesh.text = $"Soul Sucker Complete\nLevels: {LevelCatalog.Rooms.Count}\nDeaths: {GameSession.EnsureInstance().TotalDeaths}\nTime: {GameSession.EnsureInstance().ElapsedTimeSeconds:0.0}s";
            textMesh.fontSize = 72;
            textMesh.characterSize = 0.18f;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.color = Color.white;

            return root;
        }

        private void ClearVictoryRoot()
        {
            if (victoryRoot == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(victoryRoot);
            }
            else
            {
                DestroyImmediate(victoryRoot);
            }

            victoryRoot = null;
        }

        private static void EnsureGameplayCamera()
        {
            Camera gameplayCamera = Camera.main;
            if (gameplayCamera == null)
            {
                return;
            }

            gameplayCamera.orthographic = true;
            gameplayCamera.orthographicSize = 9f;
            if (gameplayCamera.GetComponent<CameraFollow2D>() == null)
            {
                gameplayCamera.gameObject.AddComponent<CameraFollow2D>();
            }
        }

        private static void SnapCameraToCurrentTarget()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                return;
            }

            CameraFollow2D follow = mainCamera.GetComponent<CameraFollow2D>();
            if (follow != null)
            {
                follow.SnapToTarget();
            }
        }
    }
}
