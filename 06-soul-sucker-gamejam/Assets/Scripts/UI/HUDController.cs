using SoulSucker.Data;
using SoulSucker.Hosts;
using SoulSucker.Player;
using SoulSucker.World;
using UnityEngine;
using UnityEngine.UI;

namespace SoulSucker.UI
{
    [DisallowMultipleComponent]
    public sealed class HUDController : MonoBehaviour
    {
        private const float ParasiteAirLimitSeconds = 3f;

        private static readonly Color PanelColor = new Color(0.05f, 0.08f, 0.12f, 0.88f);
        private static readonly Color PanelShadowColor = new Color(0f, 0f, 0f, 0.22f);
        private static readonly Color PanelAccent = new Color(0.14f, 0.84f, 0.8f, 0.95f);
        private static readonly Color SafeColor = new Color(0.32f, 0.96f, 0.68f, 1f);
        private static readonly Color WarningColor = new Color(1f, 0.82f, 0.26f, 1f);
        private static readonly Color DangerColor = new Color(0.98f, 0.28f, 0.32f, 1f);
        private static readonly Color TextColor = new Color(0.95f, 0.98f, 1f, 1f);
        private static readonly Color MutedTextColor = new Color(0.62f, 0.76f, 0.86f, 1f);

        private PlayerBrain playerBrain;
        private LevelManager levelManager;
        private HostDetector hostDetector;
        private SoulSucker.Core.GameSession gameSession;

        private Canvas hudCanvas;
        private Image infoPanel;
        private Image survivalPanel;
        private Image survivalFill;
        private Text roomLabelText;
        private Text roomValueText;
        private Text hostLabelText;
        private Text hostValueText;
        private Text deathText;
        private Text timerText;
        private Text survivalLabelText;
        private Text survivalValueText;
        private Text survivalStateText;
        private Text survivalHintText;
        private Image hostInfoPanel;
        private Image hostInfoAccentStrip;
        private Text hostInfoLabelText;
        private Text hostInfoNameText;
        private Text hostInfoSummaryText;
        private Text hostInfoControlsText;
        private RectTransform promptRow;
        private Image interactPromptChip;
        private Image abilityPromptChip;
        private Image restartPromptChip;
        private Text interactPromptText;
        private Text abilityPromptText;
        private Text restartPromptText;
        private string currentPrompt = string.Empty;

        public string CurrentHostLabel => hostValueText != null ? hostValueText.text : string.Empty;
        public string CurrentPrompt => currentPrompt;
        public float CurrentAirFillAmount => survivalFill != null ? survivalFill.fillAmount : 0f;
        public Color CurrentAirColor => survivalFill != null ? survivalFill.color : Color.clear;
        public string CurrentHostSummary => hostInfoSummaryText != null ? hostInfoSummaryText.text : string.Empty;
        public string CurrentHostControls => hostInfoControlsText != null ? hostInfoControlsText.text : string.Empty;

        private void Awake()
        {
            EnsureReferences();
            EnsureHudCanvas();
            Refresh();
        }

        private void Update()
        {
            EnsureReferences();
            Refresh();
        }

        public void Refresh()
        {
            if (playerBrain == null || levelManager == null || gameSession == null)
            {
                return;
            }

            int roomIndex = Mathf.Clamp(levelManager.CurrentLevelIndex + 1, 1, Mathf.Max(1, LevelCatalog.Rooms.Count));
            roomLabelText.text = "LEVEL";
            roomValueText.text = $"{roomIndex}/{LevelCatalog.Rooms.Count}";
            hostLabelText.text = "HOST";
            hostValueText.text = playerBrain.CurrentHost != null ? playerBrain.CurrentHost.HostType.ToString() : "Parasite";
            deathText.text = $"Deaths {gameSession.TotalDeaths}";
            timerText.text = $"Run {FormatRunTime(gameSession.ElapsedTimeSeconds)}";

            float airRatio = Mathf.Clamp01(playerBrain.AirTimeRemaining / ParasiteAirLimitSeconds);
            survivalFill.fillAmount = airRatio;
            survivalValueText.text = $"{playerBrain.AirTimeRemaining:0.0}s";

            UpdateSurvivalTheme(airRatio);
            UpdateHostInfoCard();
            UpdatePromptChips();
        }

        private void EnsureReferences()
        {
            if (playerBrain == null)
            {
                playerBrain = FindFirstObjectByType<PlayerBrain>();
            }

            if (levelManager == null)
            {
                levelManager = FindFirstObjectByType<LevelManager>();
            }

            if (hostDetector == null)
            {
                hostDetector = FindFirstObjectByType<HostDetector>();
            }

            if (gameSession == null)
            {
                gameSession = SoulSucker.Core.GameSession.EnsureInstance();
            }
        }

        private void EnsureHudCanvas()
        {
            if (hudCanvas != null)
            {
                return;
            }

            var canvasObject = new GameObject("HUDCanvas");
            canvasObject.transform.SetParent(transform, false);

            hudCanvas = canvasObject.AddComponent<Canvas>();
            hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            hudCanvas.overrideSorting = true;
            hudCanvas.sortingOrder = 100;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();

            infoPanel = CreateCard("InfoPanel", hudCanvas.transform, new Vector2(0f, 1f), new Vector2(24f, -24f), new Vector2(340f, 156f), new Vector2(0f, 1f));
            CreateAccentStrip(infoPanel.transform, PanelAccent);
            roomLabelText = CreateLabel("RoomLabel", infoPanel.transform, new Vector2(0f, 1f), new Vector2(20f, -16f), new Vector2(140f, 22f), TextAnchor.UpperLeft, 14, MutedTextColor, FontStyle.Bold);
            roomValueText = CreateLabel("RoomValue", infoPanel.transform, new Vector2(0f, 1f), new Vector2(20f, -38f), new Vector2(160f, 36f), TextAnchor.UpperLeft, 32, TextColor, FontStyle.Bold);
            hostLabelText = CreateLabel("HostLabel", infoPanel.transform, new Vector2(0f, 1f), new Vector2(20f, -76f), new Vector2(140f, 20f), TextAnchor.UpperLeft, 13, MutedTextColor, FontStyle.Bold);
            hostValueText = CreateLabel("HostValue", infoPanel.transform, new Vector2(0f, 1f), new Vector2(20f, -96f), new Vector2(200f, 28f), TextAnchor.UpperLeft, 22, PanelAccent, FontStyle.Bold);
            deathText = CreateLabel("DeathText", infoPanel.transform, new Vector2(0f, 0f), new Vector2(20f, 14f), new Vector2(160f, 22f), TextAnchor.LowerLeft, 16, MutedTextColor, FontStyle.Bold);
            timerText = CreateLabel("TimerText", infoPanel.transform, new Vector2(1f, 0f), new Vector2(-20f, 14f), new Vector2(160f, 22f), TextAnchor.LowerRight, 16, MutedTextColor, FontStyle.Bold);

            survivalPanel = CreateCard("SurvivalPanel", hudCanvas.transform, new Vector2(1f, 1f), new Vector2(-24f, -24f), new Vector2(460f, 164f), new Vector2(1f, 1f));
            CreateAccentStrip(survivalPanel.transform, PanelAccent);
            survivalLabelText = CreateLabel("SurvivalLabel", survivalPanel.transform, new Vector2(0f, 1f), new Vector2(20f, -16f), new Vector2(180f, 22f), TextAnchor.UpperLeft, 14, MutedTextColor, FontStyle.Bold);
            survivalLabelText.text = "SURVIVAL";
            survivalStateText = CreateLabel("SurvivalState", survivalPanel.transform, new Vector2(0f, 1f), new Vector2(20f, -40f), new Vector2(240f, 26f), TextAnchor.UpperLeft, 20, SafeColor, FontStyle.Bold);
            survivalHintText = CreateLabel("SurvivalHint", survivalPanel.transform, new Vector2(0f, 1f), new Vector2(20f, -68f), new Vector2(340f, 22f), TextAnchor.UpperLeft, 14, MutedTextColor);
            survivalHintText.text = "Outside a host, the timer becomes lethal.";
            survivalValueText = CreateLabel("SurvivalValue", survivalPanel.transform, new Vector2(1f, 1f), new Vector2(-20f, -16f), new Vector2(180f, 34f), TextAnchor.UpperRight, 30, TextColor, FontStyle.Bold);

            GameObject barRoot = new GameObject("SurvivalBar");
            barRoot.transform.SetParent(survivalPanel.transform, false);
            RectTransform barRect = barRoot.AddComponent<RectTransform>();
            barRect.anchorMin = new Vector2(0f, 0f);
            barRect.anchorMax = new Vector2(1f, 0f);
            barRect.pivot = new Vector2(0.5f, 0f);
            barRect.anchoredPosition = new Vector2(0f, 18f);
            barRect.sizeDelta = new Vector2(-36f, 28f);
            Image barBackground = barRoot.AddComponent<Image>();
            barBackground.color = new Color(0f, 0f, 0f, 0.42f);

            GameObject fillObject = new GameObject("Fill");
            fillObject.transform.SetParent(barRoot.transform, false);
            RectTransform fillRect = fillObject.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = new Vector2(3f, 3f);
            fillRect.offsetMax = new Vector2(-3f, -3f);
            survivalFill = fillObject.AddComponent<Image>();
            survivalFill.type = Image.Type.Filled;
            survivalFill.fillMethod = Image.FillMethod.Horizontal;
            survivalFill.fillOrigin = 0;
            survivalFill.fillAmount = 1f;
            survivalFill.color = SafeColor;

            GameObject promptRowObject = new GameObject("PromptRow");
            promptRowObject.transform.SetParent(hudCanvas.transform, false);
            promptRow = promptRowObject.AddComponent<RectTransform>();
            promptRow.anchorMin = new Vector2(0.5f, 0f);
            promptRow.anchorMax = new Vector2(0.5f, 0f);
            promptRow.pivot = new Vector2(0.5f, 0f);
            promptRow.anchoredPosition = new Vector2(0f, 28f);
            promptRow.sizeDelta = new Vector2(960f, 64f);

            interactPromptChip = CreateChip("InteractPrompt", promptRow, new Vector2(240f, 58f), PanelAccent, out interactPromptText);
            abilityPromptChip = CreateChip("AbilityPrompt", promptRow, new Vector2(260f, 58f), new Color(0.63f, 0.50f, 0.96f, 1f), out abilityPromptText);
            restartPromptChip = CreateChip("RestartPrompt", promptRow, new Vector2(200f, 58f), new Color(0.76f, 0.82f, 0.90f, 1f), out restartPromptText);

            hostInfoPanel = CreateCard("HostInfoPanel", hudCanvas.transform, new Vector2(0.5f, 0f), new Vector2(0f, 112f), new Vector2(760f, 128f), new Vector2(0.5f, 0f));
            hostInfoAccentStrip = CreateAccentStrip(hostInfoPanel.transform, PanelAccent);
            hostInfoLabelText = CreateLabel("HostInfoLabel", hostInfoPanel.transform, new Vector2(0f, 1f), new Vector2(20f, -16f), new Vector2(220f, 20f), TextAnchor.UpperLeft, 14, MutedTextColor, FontStyle.Bold);
            hostInfoLabelText.text = "EQUIPPED HOST";
            hostInfoNameText = CreateLabel("HostInfoName", hostInfoPanel.transform, new Vector2(0f, 1f), new Vector2(20f, -36f), new Vector2(220f, 30f), TextAnchor.UpperLeft, 24, PanelAccent, FontStyle.Bold);
            hostInfoSummaryText = CreateLabel("HostInfoSummary", hostInfoPanel.transform, new Vector2(0f, 1f), new Vector2(20f, -66f), new Vector2(520f, 34f), TextAnchor.UpperLeft, 16, TextColor, FontStyle.Bold);
            hostInfoSummaryText.horizontalOverflow = HorizontalWrapMode.Wrap;
            hostInfoSummaryText.verticalOverflow = VerticalWrapMode.Truncate;
            hostInfoControlsText = CreateLabel("HostInfoControls", hostInfoPanel.transform, new Vector2(0f, 0f), new Vector2(20f, 14f), new Vector2(700f, 22f), TextAnchor.LowerLeft, 15, MutedTextColor);
        }

        private void UpdateSurvivalTheme(float airRatio)
        {
            Color stateColor;
            string stateLabel;
            if (airRatio <= 0.28f)
            {
                stateColor = DangerColor;
                stateLabel = "DANGER";
            }
            else if (airRatio <= 0.58f)
            {
                stateColor = WarningColor;
                stateLabel = "WARNING";
            }
            else
            {
                stateColor = SafeColor;
                stateLabel = "STABLE";
            }

            survivalFill.color = stateColor;
            survivalStateText.text = stateLabel;
            survivalStateText.color = stateColor;
            survivalValueText.color = stateColor;
            hostValueText.color = playerBrain.CurrentHost != null ? PanelAccent : new Color(0.78f, 0.98f, 0.44f, 1f);
        }

        private void UpdatePromptChips()
        {
            string interactPrompt = string.Empty;
            string abilityPrompt = string.Empty;
            string restartPrompt = levelManager != null && !levelManager.HasWonCampaign ? "R Restart" : string.Empty;

            if (playerBrain.CurrentState == PlayerBrain.PlayerState.Dead || playerBrain.CurrentState == PlayerBrain.PlayerState.TransitioningRoom)
            {
                restartPrompt = "R Restart";
            }
            else if (playerBrain.CurrentHost != null)
            {
                interactPrompt = "E Release";
                abilityPrompt = GetHostActionPrompt(playerBrain.CurrentHost.HostType);
            }
            else
            {
                HostBase candidate = GetCandidate();
                if (candidate != null && playerBrain.IsParasiteFree)
                {
                    interactPrompt = "E Possess";
                }
            }

            SetChip(interactPromptChip, interactPromptText, interactPrompt, PanelAccent);
            SetChip(abilityPromptChip, abilityPromptText, abilityPrompt, new Color(0.70f, 0.58f, 1f, 1f));
            SetChip(restartPromptChip, restartPromptText, restartPrompt, new Color(0.78f, 0.84f, 0.92f, 1f));
            LayoutVisibleChips();
            currentPrompt = ComposePromptString(interactPrompt, abilityPrompt, restartPrompt);
        }

        private void UpdateHostInfoCard()
        {
            HostInfoCardData hostInfo = GetHostInfoCardData(playerBrain.CurrentHost != null ? playerBrain.CurrentHost.HostType : HostType.None);
            hostInfoNameText.text = hostInfo.name;
            hostInfoSummaryText.text = hostInfo.summary;
            hostInfoControlsText.text = hostInfo.controls;
            hostInfoNameText.color = hostInfo.accent;
            if (hostInfoAccentStrip != null)
            {
                hostInfoAccentStrip.color = hostInfo.accent;
            }
        }

        private void LayoutVisibleChips()
        {
            Image[] chips = { interactPromptChip, abilityPromptChip, restartPromptChip };
            int visibleCount = 0;
            float totalWidth = 0f;
            const float spacing = 16f;

            foreach (Image chip in chips)
            {
                if (chip.gameObject.activeSelf)
                {
                    totalWidth += chip.rectTransform.sizeDelta.x;
                    visibleCount++;
                }
            }

            if (visibleCount > 1)
            {
                totalWidth += spacing * (visibleCount - 1);
            }

            float cursor = -totalWidth * 0.5f;
            foreach (Image chip in chips)
            {
                if (!chip.gameObject.activeSelf)
                {
                    continue;
                }

                RectTransform rect = chip.rectTransform;
                cursor += rect.sizeDelta.x * 0.5f;
                rect.anchoredPosition = new Vector2(cursor, 0f);
                cursor += (rect.sizeDelta.x * 0.5f) + spacing;
            }
        }

        private static string ComposePromptString(params string[] prompts)
        {
            string composed = string.Empty;
            for (int index = 0; index < prompts.Length; index++)
            {
                if (string.IsNullOrEmpty(prompts[index]))
                {
                    continue;
                }

                if (composed.Length > 0)
                {
                    composed += " | ";
                }

                composed += prompts[index];
            }

            return composed;
        }

        private static Image CreateCard(string name, Transform parent, Vector2 anchor, Vector2 offset, Vector2 size, Vector2 pivot)
        {
            CreatePanel($"{name}Shadow", parent, anchor, offset + new Vector2(6f, -6f), size, PanelShadowColor, pivot);
            return CreatePanel(name, parent, anchor, offset, size, PanelColor, pivot);
        }

        private static Image CreateChip(string name, Transform parent, Vector2 size, Color accentColor, out Text text)
        {
            Image chip = CreatePanel(name, parent, new Vector2(0.5f, 0.5f), Vector2.zero, size, PanelColor, new Vector2(0.5f, 0.5f));
            RectTransform rect = chip.rectTransform;
            rect.pivot = new Vector2(0.5f, 0.5f);

            CreateAccentStrip(chip.transform, accentColor);
            text = CreateLabel("Label", chip.transform, new Vector2(0.5f, 0.5f), Vector2.zero, size - new Vector2(28f, 16f), TextAnchor.MiddleCenter, 20, TextColor, FontStyle.Bold);
            chip.gameObject.SetActive(false);
            return chip;
        }

        private static void SetChip(Image chip, Text text, string value, Color accentColor)
        {
            bool isVisible = !string.IsNullOrEmpty(value);
            chip.gameObject.SetActive(isVisible);
            if (!isVisible)
            {
                return;
            }

            text.text = value;
            text.color = TextColor;
            Transform stripTransform = chip.transform.Find("AccentStrip");
            if (stripTransform != null)
            {
                Image strip = stripTransform.GetComponent<Image>();
                if (strip != null)
                {
                    strip.color = accentColor;
                }
            }
        }

        private static Text CreateLabel(string name, Transform parent, Vector2 anchor, Vector2 offset, Vector2 size, TextAnchor alignment, int fontSize, Color color, FontStyle fontStyle = FontStyle.Normal)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);

            RectTransform rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = offset;
            rect.sizeDelta = size;

            Text text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
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

        private static Image CreatePanel(string name, Transform parent, Vector2 anchor, Vector2 offset, Vector2 size, Color color, Vector2 pivot)
        {
            var panelObject = new GameObject(name);
            panelObject.transform.SetParent(parent, false);

            RectTransform rect = panelObject.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.anchoredPosition = offset;
            rect.sizeDelta = size;

            Image image = panelObject.AddComponent<Image>();
            image.color = color;
            return image;
        }

        private static Image CreateAccentStrip(Transform parent, Color color)
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
            return image;
        }

        private HostBase GetCandidate()
        {
            return hostDetector != null ? hostDetector.CurrentCandidate : null;
        }

        private static string GetHostActionPrompt(HostType hostType)
        {
            switch (hostType)
            {
                case HostType.Jumper:
                    return "Space Jump";
                case HostType.Dasher:
                    return "Space Dash";
                case HostType.Shooter:
                    return "J Shoot";
                case HostType.Phase:
                    return "Space Phase";
                case HostType.Anchor:
                    return "Push Block";
                default:
                    return string.Empty;
            }
        }

        private static string FormatRunTime(float elapsedSeconds)
        {
            elapsedSeconds = Mathf.Max(0f, elapsedSeconds);
            int minutes = Mathf.FloorToInt(elapsedSeconds / 60f);
            int seconds = Mathf.FloorToInt(elapsedSeconds % 60f);
            return $"{minutes:00}:{seconds:00}";
        }

        private static HostInfoCardData GetHostInfoCardData(HostType hostType)
        {
            switch (hostType)
            {
                case HostType.Jumper:
                    return new HostInfoCardData("Jumper", "High vertical burst. Weak air control.", "A/D Move | Space Jump | E Release", new Color(0.24f, 0.85f, 1f));
                case HostType.Dasher:
                    return new HostInfoCardData("Dasher", "Fixed horizontal dash. No height gain.", "A/D Move | Space Dash | E Release", new Color(0.58f, 0.96f, 0.36f));
                case HostType.Shooter:
                    return new HostInfoCardData("Shooter", "Fire to break walls and hit switches.", "A/D Move | J Shoot | E Release", new Color(1f, 0.72f, 0.30f));
                case HostType.Phase:
                    return new HostInfoCardData("Phase", "Phase briefly through spirit barriers.", "A/D Move | Space Phase | E Release", new Color(0.76f, 0.56f, 1f));
                case HostType.Anchor:
                    return new HostInfoCardData("Anchor", "Push blocks. Hold plates. Ignore fans.", "A/D Move | Push Block | E Release", new Color(0.44f, 0.86f, 0.42f));
                default:
                    return new HostInfoCardData("Parasite", "Weak form. Survive 3s outside a host.", "A/D Move | E Possess | R Restart", new Color(0.78f, 0.98f, 0.44f, 1f));
            }
        }

        private readonly struct HostInfoCardData
        {
            public readonly string name;
            public readonly string summary;
            public readonly string controls;
            public readonly Color accent;

            public HostInfoCardData(string name, string summary, string controls, Color accent)
            {
                this.name = name;
                this.summary = summary;
                this.controls = controls;
                this.accent = accent;
            }
        }
    }
}
