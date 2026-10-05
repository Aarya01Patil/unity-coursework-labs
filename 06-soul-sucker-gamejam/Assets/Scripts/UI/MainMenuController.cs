using SoulSucker.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoulSucker.UI
{
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button controlsButton;
        [SerializeField] private GameObject controlsPanel;

        private void Awake()
        {
            WireUpButtons();
            ApplyControlsState(false);
        }

        private void OnEnable()
        {
            WireUpButtons();
            ApplyControlsState(false);
            SelectDefaultButton();
        }

        private void OnDisable()
        {
            UnwireButtons();
        }

        public void HandlePlayClicked()
        {
            ApplyControlsState(false);
            GameSession.EnsureInstance().BeginCampaignRun();
            SceneRouter.LoadGameplay();
        }

        public void ToggleControlsPanel()
        {
            if (controlsPanel == null)
            {
                return;
            }

            controlsPanel.SetActive(!controlsPanel.activeSelf);
        }

        public void SetReferences(Button play, Button controls, GameObject panel)
        {
            playButton = play;
            controlsButton = controls;
            controlsPanel = panel;
            ApplyControlsState(false);
            WireUpButtons();
            SelectDefaultButton();
        }

        private void WireUpButtons()
        {
            if (playButton != null)
            {
                playButton.onClick.RemoveListener(HandlePlayClicked);
                playButton.onClick.AddListener(HandlePlayClicked);
            }

            if (controlsButton != null)
            {
                controlsButton.onClick.RemoveListener(ToggleControlsPanel);
                controlsButton.onClick.AddListener(ToggleControlsPanel);
            }
        }

        private void UnwireButtons()
        {
            if (playButton != null)
            {
                playButton.onClick.RemoveListener(HandlePlayClicked);
            }

            if (controlsButton != null)
            {
                controlsButton.onClick.RemoveListener(ToggleControlsPanel);
            }
        }

        private void ApplyControlsState(bool visible)
        {
            if (controlsPanel != null)
            {
                controlsPanel.SetActive(visible);
            }
        }

        private void SelectDefaultButton()
        {
            if (playButton == null || EventSystem.current == null)
            {
                return;
            }

            EventSystem.current.SetSelectedGameObject(playButton.gameObject);
        }
    }
}
