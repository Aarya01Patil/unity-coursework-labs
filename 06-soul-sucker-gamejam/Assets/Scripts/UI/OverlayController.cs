using SoulSucker.Audio;
using SoulSucker.Core;
using SoulSucker.Data;
using SoulSucker.Player;
using SoulSucker.World;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoulSucker.UI
{
    [DisallowMultipleComponent]
    public sealed class OverlayController : MonoBehaviour
    {
        private static readonly Color PanelColor = new Color(0.05f, 0.08f, 0.12f, 0.88f);
        private static readonly Color ShadowColor = new Color(0f, 0f, 0f, 0.24f);
        private static readonly Color AccentColor = new Color(0.14f, 0.84f, 0.8f, 1f);
        private static readonly Color VictoryColor = new Color(0.43f, 0.94f, 0.36f, 1f);
        private static readonly Color DangerColor = new Color(0.98f, 0.28f, 0.32f, 1f);
        private static readonly Color SpiritColor = new Color(0.70f, 0.58f, 1f, 1f);
        private static readonly Color TextColor = new Color(0.95f, 0.98f, 1f, 1f);
        private static readonly Color MutedTextColor = new Color(0.76f, 0.84f, 0.92f, 1f);

        private const float IntroFadeInSeconds = 0.2f;
        private const float IntroHoldSeconds = 2.3f;
        private const float IntroFadeOutSeconds = 0.4f;

        private LevelManager levelManager;
        private GameSession gameSession;
        private PlayerBrain playerBrain;
        private Canvas overlayCanvas;
        private Image flashImage;
        private Image introPanel;
        private GameObject introContainer;
        private CanvasGroup introCanvasGroup;
        private Text introHeaderText;
        private Text introTitleText;
        private Text hintText;
        private Image victoryPanel;
        private GameObject victoryContainer;
        private Text victoryHeaderText;
        private Text victoryTitleText;
        private Text victoryStatsText;
        private Button replayButton;
        private Button menuButton;
        private float introTimer;
        private float flashTimer;
        private float flashDuration;
        private Color flashColor = Color.white;
        private float flashBaseAlpha;
        private int previousDeaths;
        private PlayerBrain.PlayerState previousState = PlayerBrain.PlayerState.ParasiteFree;

        public bool IsVictoryVisible => victoryContainer != null && victoryContainer.activeSelf;
        public string CurrentHintText => hintText != null ? hintText.text : string.Empty;
        public bool IsIntroVisible => introContainer != null && introContainer.activeSelf && introCanvasGroup != null && introCanvasGroup.alpha > 0.01f;

        private void Awake()
        {
            EnsureReferences();
            EnsureOverlayCanvas();
        }

        private void OnEnable()
        {
            EnsureReferences();
            if (levelManager != null)
            {
                levelManager.LevelBuilt -= HandleLevelBuilt;
                levelManager.LevelBuilt += HandleLevelBuilt;
                levelManager.VictoryAchieved -= HandleVictory;
                levelManager.VictoryAchieved += HandleVictory;
            }
        }

        private void OnDisable()
        {
            if (levelManager != null)
            {
                levelManager.LevelBuilt -= HandleLevelBuilt;
                levelManager.VictoryAchieved -= HandleVictory;
            }
        }

        private void Update()
        {
            EnsureReferences();
            UpdateIntro();
            UpdateFlash();
            DetectDeathFlash();
            DetectStateChangeFlash();
        }

        private void HandleLevelBuilt(LevelBlueprint blueprint, GameObject roomRoot)
        {
            ShowIntroCard(blueprint);
            previousState = playerBrain != null ? playerBrain.CurrentState : PlayerBrain.PlayerState.ParasiteFree;
            if (victoryContainer != null)
            {
                victoryContainer.SetActive(false);
            }
        }

        private void HandleVictory()
        {
            AudioRouter.EnsureInstance().PlayVictory();
            if (introContainer != null)
            {
                introContainer.SetActive(false);
            }

            TriggerFlash(Color.white, 0.58f, 0.35f);

            if (victoryContainer != null)
            {
                victoryContainer.SetActive(true);
                victoryTitleText.text = "Campaign Complete";
                victoryStatsText.text = $"All {LevelCatalog.Rooms.Count} levels cleared\nDeaths: {gameSession.TotalDeaths}\nTime: {gameSession.ElapsedTimeSeconds:0.0}s";
                SelectVictoryButton();
            }
        }

        public void ReplayCampaign()
        {
            if (levelManager == null)
            {
                return;
            }

            gameSession.BeginCampaignRun();
            levelManager.BuildLevel(0);
        }

        public void ReturnToMainMenu()
        {
            SceneRouter.LoadMainMenu();
        }

        private void EnsureReferences()
        {
            if (levelManager == null)
            {
                levelManager = FindFirstObjectByType<LevelManager>();
            }

            if (playerBrain == null)
            {
                playerBrain = FindFirstObjectByType<PlayerBrain>();
            }

            if (gameSession == null)
            {
                gameSession = GameSession.EnsureInstance();
                previousDeaths = gameSession.TotalDeaths;
            }
        }

        private void EnsureOverlayCanvas()
        {
            if (overlayCanvas != null)
            {
                return;
            }

            var canvasObject = new GameObject("OverlayCanvas");
            canvasObject.transform.SetParent(transform, false);

            overlayCanvas = canvasObject.AddComponent<Canvas>();
            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.overrideSorting = true;
            overlayCanvas.sortingOrder = 110;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();

            GameObject flashObject = new GameObject("Flash");
            flashObject.transform.SetParent(overlayCanvas.transform, false);
            RectTransform flashRect = flashObject.AddComponent<RectTransform>();
            flashRect.anchorMin = Vector2.zero;
            flashRect.anchorMax = Vector2.one;
            flashRect.offsetMin = Vector2.zero;
            flashRect.offsetMax = Vector2.zero;
            flashImage = flashObject.AddComponent<Image>();
            flashImage.color = new Color(1f, 1f, 1f, 0f);
            flashImage.raycastTarget = false;

            introPanel = CreateCard("IntroPanel", overlayCanvas.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, 180f), new Vector2(780f, 148f));
            introContainer = introPanel.transform.parent.gameObject;
            introCanvasGroup = introContainer.AddComponent<CanvasGroup>();
            CreateAccentStrip(introPanel.transform, AccentColor);
            introHeaderText = CreateText("IntroHeader", introPanel.transform, new Vector2(0.5f, 1f), new Vector2(0f, -16f), new Vector2(680f, 22f), TextAnchor.UpperCenter, 15, MutedTextColor, FontStyle.Bold);
            introHeaderText.text = "LEVEL";
            introTitleText = CreateText("IntroTitle", introPanel.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, 12f), new Vector2(680f, 36f), TextAnchor.MiddleCenter, 32, AccentColor, FontStyle.Bold);
            hintText = CreateText("HintText", introPanel.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -24f), new Vector2(680f, 44f), TextAnchor.MiddleCenter, 20, TextColor);
            introContainer.SetActive(false);
            introCanvasGroup.alpha = 0f;

            victoryPanel = CreateCard("VictoryPanel", overlayCanvas.transform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(600f, 400f));
            victoryContainer = victoryPanel.transform.parent.gameObject;
            CreateAccentStrip(victoryPanel.transform, VictoryColor);
            victoryHeaderText = CreateText("VictoryHeader", victoryPanel.transform, new Vector2(0.5f, 1f), new Vector2(0f, -18f), new Vector2(480f, 24f), TextAnchor.UpperCenter, 16, MutedTextColor, FontStyle.Bold);
            victoryHeaderText.text = "CAMPAIGN CLEAR";
            victoryTitleText = CreateText("VictoryTitle", victoryPanel.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, 30f), new Vector2(480f, 48f), TextAnchor.MiddleCenter, 38, VictoryColor, FontStyle.Bold);
            victoryStatsText = CreateText("VictoryStats", victoryPanel.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -24f), new Vector2(480f, 120f), TextAnchor.MiddleCenter, 22, MutedTextColor);
            Text victoryFooter = CreateText("VictoryFooter", victoryPanel.transform, new Vector2(0.5f, 0f), new Vector2(0f, 100f), new Vector2(480f, 28f), TextAnchor.LowerCenter, 17, MutedTextColor);
            victoryFooter.text = "Replay restarts from level 1.";

            replayButton = CreateButton("ReplayButton", victoryPanel.transform, new Vector2(0.5f, 0f), new Vector2(0f, 42f), new Vector2(260f, 56f), "Replay");
            menuButton = CreateButton("MenuButton", victoryPanel.transform, new Vector2(0.5f, 0f), new Vector2(0f, -22f), new Vector2(260f, 56f), "Main Menu");
            replayButton.onClick.AddListener(ReplayCampaign);
            menuButton.onClick.AddListener(ReturnToMainMenu);
            victoryContainer.SetActive(false);

            flashImage.transform.SetAsLastSibling();
        }

        private void ShowIntroCard(LevelBlueprint blueprint)
        {
            if (introPanel == null)
            {
                return;
            }

            introTitleText.text = blueprint.title;
            hintText.text = blueprint.tutorialHints != null && blueprint.tutorialHints.Length > 0
                ? blueprint.tutorialHints[0]
                : "Reach the exit to advance.";
            introTimer = 0f;
            introCanvasGroup.alpha = 0f;
            introContainer.SetActive(true);
        }

        private void DetectDeathFlash()
        {
            if (gameSession == null || gameSession.TotalDeaths == previousDeaths)
            {
                return;
            }

            previousDeaths = gameSession.TotalDeaths;
            TriggerFlash(DangerColor, 0.52f, 0.24f);
            AudioRouter.EnsureInstance().PlayDeath();
        }

        private void DetectStateChangeFlash()
        {
            if (playerBrain == null)
            {
                return;
            }

            PlayerBrain.PlayerState currentState = playerBrain.CurrentState;
            if (currentState == previousState)
            {
                return;
            }

            if (currentState == PlayerBrain.PlayerState.InsideHost)
            {
                TriggerFlash(SpiritColor, 0.22f, 0.18f);
            }
            else if (previousState == PlayerBrain.PlayerState.InsideHost && currentState == PlayerBrain.PlayerState.ParasiteFree)
            {
                TriggerFlash(AccentColor, 0.18f, 0.16f);
            }

            previousState = currentState;
        }

        private void UpdateIntro()
        {
            if (introPanel == null || !introContainer.activeSelf || introCanvasGroup == null)
            {
                return;
            }

            introTimer += Time.deltaTime;
            float totalDuration = IntroFadeInSeconds + IntroHoldSeconds + IntroFadeOutSeconds;

            if (introTimer < IntroFadeInSeconds)
            {
                introCanvasGroup.alpha = Mathf.Clamp01(introTimer / IntroFadeInSeconds);
            }
            else if (introTimer < IntroFadeInSeconds + IntroHoldSeconds)
            {
                introCanvasGroup.alpha = 1f;
            }
            else if (introTimer < totalDuration)
            {
                float fadeOutElapsed = introTimer - IntroFadeInSeconds - IntroHoldSeconds;
                introCanvasGroup.alpha = 1f - Mathf.Clamp01(fadeOutElapsed / IntroFadeOutSeconds);
            }
            else
            {
                introCanvasGroup.alpha = 0f;
                introContainer.SetActive(false);
            }
        }

        private void UpdateFlash()
        {
            if (flashImage == null)
            {
                return;
            }

            if (flashTimer <= 0f)
            {
                flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);
                return;
            }

            flashTimer -= Time.deltaTime;
            float ratio = Mathf.Clamp01(flashTimer / Mathf.Max(0.01f, flashDuration));
            flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, flashBaseAlpha * ratio);
        }

        private void TriggerFlash(Color color, float alpha, float duration)
        {
            flashColor = color;
            flashBaseAlpha = alpha;
            flashDuration = duration;
            flashTimer = duration;
        }

        private static Image CreatePanel(string name, Transform parent, Vector2 anchor, Vector2 offset, Vector2 size, Color color)
        {
            var panel = new GameObject(name);
            panel.transform.SetParent(parent, false);

            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = offset;
            rect.sizeDelta = size;

            Image image = panel.AddComponent<Image>();
            image.color = color;
            return image;
        }

        private static Image CreateCard(string name, Transform parent, Vector2 anchor, Vector2 offset, Vector2 size)
        {
            var container = new GameObject($"{name}Container");
            container.transform.SetParent(parent, false);
            RectTransform containerRect = container.AddComponent<RectTransform>();
            containerRect.anchorMin = anchor;
            containerRect.anchorMax = anchor;
            containerRect.pivot = new Vector2(0.5f, 0.5f);
            containerRect.anchoredPosition = offset;
            containerRect.sizeDelta = size;

            CreatePanel($"{name}Shadow", container.transform, new Vector2(0.5f, 0.5f), new Vector2(6f, -6f), size, ShadowColor);
            Image main = CreatePanel(name, container.transform, new Vector2(0.5f, 0.5f), Vector2.zero, size, PanelColor);
            return main;
        }

        private static void CreateAccentStrip(Transform parent, Color color)
        {
            var strip = new GameObject("AccentStrip");
            strip.transform.SetParent(parent, false);

            RectTransform rect = strip.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(0f, 4f);

            Image image = strip.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
        }

        private static Text CreateText(string name, Transform parent, Vector2 anchor, Vector2 offset, Vector2 size, TextAnchor alignment, int fontSize, Color color, FontStyle fontStyle = FontStyle.Normal)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);

            RectTransform rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = offset;
            rect.sizeDelta = size;

            Text text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = string.Empty;
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment = alignment;
            text.color = color;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            var shadow = textObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.62f);
            shadow.effectDistance = new Vector2(1.5f, -1.5f);
            return text;
        }

        private static Button CreateButton(string name, Transform parent, Vector2 anchor, Vector2 offset, Vector2 size, string label)
        {
            var buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);

            RectTransform rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = offset;
            rect.sizeDelta = size;

            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.14f, 0.20f, 0.30f, 1f);

            Button button = buttonObject.AddComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            ColorBlock colors = button.colors;
            colors.normalColor = image.color;
            colors.highlightedColor = new Color(0.19f, 0.28f, 0.42f, 1f);
            colors.pressedColor = new Color(0.09f, 0.14f, 0.22f, 1f);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = new Color(0.12f, 0.14f, 0.18f, 0.75f);
            colors.colorMultiplier = 1f;
            button.colors = colors;

            CreateText("Label", buttonObject.transform, new Vector2(0.5f, 0.5f), Vector2.zero, size, TextAnchor.MiddleCenter, 24, TextColor, FontStyle.Bold).text = label;
            return button;
        }

        private void SelectVictoryButton()
        {
            if (replayButton == null || EventSystem.current == null)
            {
                return;
            }

            EventSystem.current.SetSelectedGameObject(replayButton.gameObject);
        }
    }
}
