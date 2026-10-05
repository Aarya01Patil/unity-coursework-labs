using SoulSucker.Core;
using SoulSucker.Data;
using SoulSucker.Hosts;
using SoulSucker.World;
using UnityEngine;
using SoulSucker.Audio;

namespace SoulSucker.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(HostDetector))]
    public sealed class PlayerBrain : MonoBehaviour
    {
        public enum PlayerState
        {
            ParasiteFree = 0,
            InsideHost = 1,
            Dead = 2,
            TransitioningRoom = 3,
        }

        [SerializeField] private float parasiteAirLimitSeconds = 3f;

        private LevelManager levelManager;
        private HostDetector hostDetector;
        private InputReader inputReader;
        private ParasiteBody parasiteBody;
        private HostBase currentHost;
        private PlayerState currentState = PlayerState.ParasiteFree;
        private float airTimeRemaining = 3f;

        public PlayerState CurrentState => currentState;
        public HostBase CurrentHost => currentHost;
        public ParasiteBody ParasiteBody
        {
            get
            {
                EnsureReferences();
                return parasiteBody;
            }
        }
        public float AirTimeRemaining => airTimeRemaining;
        public bool IsParasiteFree => currentState == PlayerState.ParasiteFree;
        public bool HasWonCampaign => levelManager != null && levelManager.HasWonCampaign;
        public Transform CurrentCameraTarget
        {
            get
            {
                if (currentHost != null)
                {
                    return currentHost.transform;
                }

                return ParasiteBody != null ? ParasiteBody.transform : transform;
            }
        }

        private void Awake()
        {
            EnsureReferences();
        }

        private void OnEnable()
        {
            EnsureReferences();
            if (levelManager != null)
            {
                levelManager.LevelBuilt -= HandleLevelBuilt;
                levelManager.LevelBuilt += HandleLevelBuilt;
            }
        }

        private void OnDisable()
        {
            if (levelManager != null)
            {
                levelManager.LevelBuilt -= HandleLevelBuilt;
            }
        }

        private void Update()
        {
            EnsureReferences();
            InputFrame inputFrame = inputReader.Read();
            Tick(inputFrame, Time.deltaTime);
        }

        public void Tick(InputFrame inputFrame, float deltaTime)
        {
            EnsureReferences();

            if (HasWonCampaign)
            {
                return;
            }

            if (currentState == PlayerState.TransitioningRoom)
            {
                return;
            }

            if (inputFrame.restartPressed)
            {
                RequestRestart();
                return;
            }

            if (currentState == PlayerState.Dead)
            {
                return;
            }

            if (currentState == PlayerState.ParasiteFree)
            {
                parasiteBody.Tick(inputFrame);
                hostDetector.RefreshCandidate(parasiteBody.transform.position);
                ConsumeAirTime(deltaTime);

                if (inputFrame.possessPressed)
                {
                    TryPossessNearestHost();
                }

                return;
            }

            hostDetector.ClearCandidate();

            if (currentHost != null)
            {
                currentHost.TickControlled(inputFrame, deltaTime);
            }

            if (inputFrame.possessPressed)
            {
                ReleaseHost();
            }
        }

        public bool TryPossessNearestHost()
        {
            EnsureReferences();
            if (currentState != PlayerState.ParasiteFree)
            {
                return false;
            }

            HostBase host = hostDetector.FindNearestHostCandidate(parasiteBody.transform.position);
            if (host == null)
            {
                hostDetector.ClearCandidate();
                return false;
            }

            currentHost = host;
            currentHost.OnPossessed(this);
            parasiteBody.SetParasiteActive(false);
            parasiteBody.TeleportTo(currentHost.transform.position);
            currentState = PlayerState.InsideHost;
            hostDetector.ClearCandidate();
            AudioRouter.EnsureInstance().PlayPossess();
            return true;
        }

        public void ReleaseHost()
        {
            EnsureReferences();
            if (currentState != PlayerState.InsideHost || currentHost == null)
            {
                return;
            }

            Transform releasePoint = currentHost.ReleaseAnchor;
            currentHost.OnReleased(this);
            currentHost = null;
            currentState = PlayerState.ParasiteFree;
            ResetAirTimer();
            parasiteBody.TeleportTo(releasePoint.position);
            parasiteBody.SetParasiteActive(true);
            AudioRouter.EnsureInstance().PlayRelease();
        }

        public void KillPlayer()
        {
            EnsureReferences();
            if (currentState == PlayerState.Dead || currentState == PlayerState.TransitioningRoom)
            {
                return;
            }

            currentState = PlayerState.Dead;
            GameSession.EnsureInstance().RegisterDeath();

            if (currentHost != null)
            {
                currentHost.OnReleased(this);
                currentHost = null;
            }

            parasiteBody.SetParasiteActive(false);
            hostDetector.ClearCandidate();

            if (levelManager != null)
            {
                currentState = PlayerState.TransitioningRoom;
                levelManager.RebuildCurrentLevel();
            }
        }

        public void ConsumeAirTime(float deltaTime)
        {
            if (currentState != PlayerState.ParasiteFree)
            {
                return;
            }

            airTimeRemaining -= deltaTime;
            if (airTimeRemaining <= 0f)
            {
                airTimeRemaining = 0f;
                KillPlayer();
            }
        }

        public void RequestRestart()
        {
            EnsureReferences();
            if (levelManager == null)
            {
                return;
            }

            currentState = PlayerState.TransitioningRoom;

            if (currentHost != null)
            {
                currentHost.OnReleased(this);
                currentHost = null;
            }

            parasiteBody.SetParasiteActive(false);
            hostDetector.ClearCandidate();
            levelManager.RebuildCurrentLevel();
        }

        private void EnsureReferences()
        {
            if (levelManager == null)
            {
                levelManager = GetComponent<LevelManager>();
            }

            if (hostDetector == null)
            {
                hostDetector = GetComponent<HostDetector>();
            }

            if (inputReader == null)
            {
                inputReader = new InputReader();
            }

            if (parasiteBody == null)
            {
                ParasiteBody[] parasiteBodies = Object.FindObjectsByType<ParasiteBody>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
                if (parasiteBodies.Length > 0)
                {
                    parasiteBody = parasiteBodies[0];
                }

                if (parasiteBody == null)
                {
                    var parasiteObject = new GameObject("ParasiteBody");
                    parasiteObject.transform.SetParent(transform, false);
                    parasiteBody = parasiteObject.AddComponent<ParasiteBody>();
                }

                parasiteBody.Initialize(this);
            }

            ResetAirTimerIfNeeded();
        }

        private void HandleLevelBuilt(LevelBlueprint blueprint, GameObject roomRoot)
        {
            EnsureReferences();
            currentState = PlayerState.ParasiteFree;
            currentHost = null;
            ResetAirTimer();
            parasiteBody.SetParasiteActive(true);
            parasiteBody.TeleportTo(levelManager.GetCurrentPlayerSpawnWorldPosition());
            hostDetector.ClearCandidate();
        }

        private void ResetAirTimer()
        {
            airTimeRemaining = parasiteAirLimitSeconds;
        }

        private void ResetAirTimerIfNeeded()
        {
            if (airTimeRemaining <= 0f)
            {
                airTimeRemaining = parasiteAirLimitSeconds;
            }
        }
    }
}
