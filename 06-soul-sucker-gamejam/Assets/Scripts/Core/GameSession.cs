using UnityEngine;

namespace SoulSucker.Core
{
    public sealed class GameSession : MonoBehaviour
    {
        public static GameSession Instance { get; private set; }

        [SerializeField] private string currentRoute = string.Empty;
        [SerializeField] private int totalDeaths;
        [SerializeField] private float elapsedTimeSeconds;
        [SerializeField] private bool hasWon;

        public string CurrentRoute => currentRoute;
        public int TotalDeaths => totalDeaths;
        public float ElapsedTimeSeconds => elapsedTimeSeconds;
        public bool HasWon => hasWon;

        public static GameSession EnsureInstance()
        {
            if (Instance != null)
            {
                return Instance;
            }

            var sessionObject = new GameObject(nameof(GameSession));
            return sessionObject.AddComponent<GameSession>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (currentRoute == "Gameplay" && !hasWon)
            {
                elapsedTimeSeconds += Time.unscaledDeltaTime;
            }
        }

        public void SetCurrentRoute(string routeName)
        {
            currentRoute = routeName;
        }

        public void RegisterDeath()
        {
            totalDeaths++;
        }

        public void BeginCampaignRun()
        {
            totalDeaths = 0;
            elapsedTimeSeconds = 0f;
            hasWon = false;
        }

        public void MarkVictory()
        {
            hasWon = true;
        }
    }
}
