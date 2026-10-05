using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace SoulSucker.Core
{
    public static class SceneRouter
    {
        public const string BootstrapSceneName = "Bootstrap";
        public const string MainMenuSceneName = "MainMenu";
        public const string GameplaySceneName = "Gameplay";

        private static bool isTransitioning;

        static SceneRouter()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        public static bool IsTransitioning => isTransitioning;

        public static void LoadMainMenu()
        {
            LoadScene(MainMenuSceneName);
        }

        public static void LoadGameplay()
        {
            LoadScene(GameplaySceneName);
        }

        public static void ReloadCurrentScene()
        {
            LoadScene(SceneManager.GetActiveScene().name);
        }

        private static void LoadScene(string sceneName)
        {
            if (isTransitioning)
            {
                return;
            }

            var activeScene = SceneManager.GetActiveScene();
            if (activeScene.IsValid() && activeScene.name == sceneName)
            {
                GameSession.EnsureInstance().SetCurrentRoute(sceneName);
                return;
            }

            isTransitioning = true;
            GameSession.EnsureInstance().SetCurrentRoute(sceneName);

            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
#if UNITY_EDITOR
                string editorPath = GetEditorScenePath(sceneName);
                if (!string.IsNullOrEmpty(editorPath))
                {
                    EditorSceneManager.LoadSceneInPlayMode(editorPath, new LoadSceneParameters(LoadSceneMode.Single));
                    return;
                }
#endif
                isTransitioning = false;
                return;
            }

            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            isTransitioning = false;

            if (GameSession.Instance != null)
            {
                GameSession.Instance.SetCurrentRoute(scene.name);
            }
        }

#if UNITY_EDITOR
        private static string GetEditorScenePath(string sceneName)
        {
            switch (sceneName)
            {
                case BootstrapSceneName:
                    return "Assets/Scenes/Bootstrap.unity";
                case MainMenuSceneName:
                    return "Assets/Scenes/MainMenu.unity";
                case GameplaySceneName:
                    return "Assets/Scenes/Gameplay.unity";
                default:
                    return string.Empty;
            }
        }
#endif
    }
}
