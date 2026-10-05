using UnityEngine;

namespace SoulSucker.Core
{
    public sealed class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private bool loadMainMenuOnStart = true;

        private void Awake()
        {
            GameSession.EnsureInstance();
        }

        private void Start()
        {
            if (!loadMainMenuOnStart)
            {
                return;
            }

            SceneRouter.LoadMainMenu();
        }
    }
}
