using System.IO;
using SoulSucker.Audio;
using SoulSucker.Core;
using SoulSucker.Player;
using SoulSucker.UI;
using SoulSucker.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoulSucker.Editor.Scaffold
{
    public static class ProjectScaffoldGenerator
    {
        public const string BootstrapScenePath = "Assets/Scenes/Bootstrap.unity";
        public const string MainMenuScenePath = "Assets/Scenes/MainMenu.unity";
        public const string GameplayScenePath = "Assets/Scenes/Gameplay.unity";

        private const string SceneDirectory = "Assets/Scenes";

        private static readonly Color MenuBackdropColor = new Color(0.03f, 0.04f, 0.06f, 1f);
        private static readonly Color MenuPanelColor = new Color(0.06f, 0.08f, 0.12f, 0.92f);
        private static readonly Color MenuAccentColor = new Color(0.12f, 0.84f, 0.8f, 1f);
        private static readonly Color MenuDangerColor = new Color(0.95f, 0.26f, 0.32f, 0.95f);
        private static readonly Color MenuTextColor = new Color(0.96f, 0.98f, 1f, 1f);
        private static readonly Color MenuMutedTextColor = new Color(0.73f, 0.81f, 0.89f, 1f);

        [MenuItem("Tools/Soul Sucker/Scaffold/Generate Or Repair Phase 1")]
        public static void GenerateOrRepairProjectScaffold()
        {
            EnsureFolderStructure();
            EnsureScenes();
            EnsureBuildSettings();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void EnsureFolderStructure()
        {
            EnsureFolder("Assets", "Scenes");
            EnsureFolder("Assets", "Scripts");
            EnsureFolder("Assets/Scripts", "Core");
            EnsureFolder("Assets/Scripts", "Data");
            EnsureFolder("Assets/Scripts", "Player");
            EnsureFolder("Assets/Scripts", "Hosts");
            EnsureFolder("Assets/Scripts", "World");
            EnsureFolder("Assets/Scripts", "UI");
            EnsureFolder("Assets/Scripts", "Audio");
            EnsureFolder("Assets/Scripts", "Utilities");
            EnsureFolder("Assets", "Editor");
            EnsureFolder("Assets/Editor", "Build");
            EnsureFolder("Assets/Editor", "Scaffold");
            EnsureFolder("Assets", "Tests");
            EnsureFolder("Assets/Tests", "EditMode");
            EnsureFolder("Assets/Tests", "PlayMode");
        }

        public static void EnsureScenes()
        {
            Directory.CreateDirectory(SceneDirectory);
            CreateBootstrapScene();
            CreateMainMenuScene();
            CreateGameplayScene();
        }

        public static void EnsureBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(BootstrapScenePath, true),
                new EditorBuildSettingsScene(MainMenuScenePath, true),
                new EditorBuildSettingsScene(GameplayScenePath, true),
            };
        }

        private static void CreateBootstrapScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = SceneRouter.BootstrapSceneName;

            var sessionObject = new GameObject(nameof(GameSession));
            sessionObject.AddComponent<GameSession>();

            var audioObject = new GameObject("AudioRoot");
            audioObject.AddComponent<AudioRouter>();

            var bootstrapObject = new GameObject("BootstrapRoot");
            bootstrapObject.AddComponent<Bootstrapper>();

            EditorSceneManager.SaveScene(scene, BootstrapScenePath);
        }

        private static void CreateMainMenuScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = SceneRouter.MainMenuSceneName;

            CreateMainCamera("Main Camera");

            var canvasObject = new GameObject("Canvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObject.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920f, 1080f);
            canvasObject.AddComponent<GraphicRaycaster>();

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();

            CreateMenuBackdrop(canvas.transform);
            CreateMenuFrame(canvas.transform);

            var menuRoot = new GameObject("MainMenuRoot");
            var menuController = menuRoot.AddComponent<MainMenuController>();

            CreateLabel(
                "Title",
                canvas.transform,
                new Vector2(0.5f, 0.85f),
                new Vector2(760f, 120f),
                "Soul Sucker",
                56,
                TextAnchor.MiddleCenter,
                MenuTextColor,
                FontStyle.Bold,
                true);

            CreateLabel(
                "Subtitle",
                canvas.transform,
                new Vector2(0.5f, 0.77f),
                new Vector2(620f, 52f),
                "Possess. Adapt. Escape the lab.",
                20,
                TextAnchor.MiddleCenter,
                MenuAccentColor,
                FontStyle.Bold,
                false);

            CreateLabel(
                "Tagline",
                canvas.transform,
                new Vector2(0.5f, 0.71f),
                new Vector2(560f, 36f),
                "A six-level possession puzzle-platformer.",
                15,
                TextAnchor.MiddleCenter,
                MenuMutedTextColor);

            var playButton = CreateButton("PlayButton", canvas.transform, new Vector2(0.5f, 0.53f), new Vector2(270f, 62f), "Play", MenuAccentColor);
            var controlsButton = CreateButton("ControlsButton", canvas.transform, new Vector2(0.5f, 0.43f), new Vector2(270f, 62f), "Controls", MenuPanelColor);
            var controlsPanel = CreateControlsPanel(canvas.transform);

            CreateLabel(
                "Footer",
                canvas.transform,
                new Vector2(0.5f, 0.12f),
                new Vector2(600f, 30f),
                "Web build ready  |  A/D Move  |  Space Ability  |  E Possess  |  J Shoot  |  R Restart",
                15,
                TextAnchor.MiddleCenter,
                MenuMutedTextColor);

            menuController.SetReferences(playButton, controlsButton, controlsPanel);

            EditorSceneManager.SaveScene(scene, MainMenuScenePath);
        }

        private static void CreateGameplayScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = SceneRouter.GameplaySceneName;

            GameObject gameplayCamera = CreateMainCamera("Main Camera");
            gameplayCamera.AddComponent<CameraFollow2D>();
            CreateGameplayEventSystem();

            var gameplayRoot = new GameObject("GameplayRoot");
            gameplayRoot.AddComponent<LevelBuilder>();
            gameplayRoot.AddComponent<LevelManager>();
            gameplayRoot.AddComponent<HostDetector>();
            gameplayRoot.AddComponent<PlayerBrain>();
            gameplayRoot.AddComponent<HUDController>();
            gameplayRoot.AddComponent<OverlayController>();

            new GameObject("WorldRoot");
            new GameObject("UIRoot");

            EditorSceneManager.SaveScene(scene, GameplayScenePath);
        }

        private static GameObject CreateMainCamera(string objectName)
        {
            var cameraObject = new GameObject(objectName);
            var camera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
            camera.orthographic = true;
            camera.orthographicSize = 9f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.04f, 0.05f, 0.08f);
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            return cameraObject;
        }

        private static void CreateGameplayEventSystem()
        {
            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        private static void CreateMenuBackdrop(Transform parent)
        {
            CreatePanel("Backdrop", parent, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1920f, 1080f), MenuBackdropColor);
            CreatePanel("LeftGlow", parent, new Vector2(0f, 0.5f), Vector2.zero, new Vector2(520f, 1080f), new Color(0.12f, 0.78f, 0.72f, 0.16f));
            CreatePanel("RightGlow", parent, new Vector2(1f, 0.5f), Vector2.zero, new Vector2(420f, 1080f), new Color(0.92f, 0.22f, 0.32f, 0.12f));
            CreatePanel("TopBand", parent, new Vector2(0.5f, 1f), new Vector2(0f, 0f), new Vector2(1920f, 160f), new Color(0.1f, 0.14f, 0.2f, 0.22f));
        }

        private static void CreateMenuFrame(Transform parent)
        {
            CreatePanel("FrameShadow", parent, new Vector2(0.5f, 0.5f), new Vector2(6f, -6f), new Vector2(840f, 690f), new Color(0f, 0f, 0f, 0.18f));
            var frame = CreatePanel("MenuFrame", parent, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(840f, 690f), MenuPanelColor);
            CreateAccentStrip(frame.transform, MenuAccentColor);
        }

        private static GameObject CreateControlsPanel(Transform parent)
        {
            var panel = CreatePanel("ControlsPanel", parent, new Vector2(0.5f, 0.5f), new Vector2(0f, -200f), new Vector2(660f, 190f), new Color(0.07f, 0.1f, 0.15f, 0.95f));
            CreateAccentStrip(panel.transform, MenuDangerColor);

            CreateLabel(
                "ControlsTitle",
                panel.transform,
                new Vector2(0.5f, 1f),
                new Vector2(0f, -18f),
                new Vector2(580f, 24f),
                "HOW TO SURVIVE",
                18,
                TextAnchor.UpperCenter,
                MenuDangerColor,
                FontStyle.Bold,
                true);

            CreateLabel(
                "ControlsText",
                panel.transform,
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, 8f),
                new Vector2(600f, 110f),
                "A / D Move\nE Possess / Release\nSpace Jump / Dash / Phase\nJ Fire (Shooter)\nR Restart",
                21,
                TextAnchor.MiddleCenter,
                MenuTextColor);

            panel.gameObject.SetActive(false);
            return panel.gameObject;
        }

        private static Image CreatePanel(string objectName, Transform parent, Vector2 anchor, Vector2 size, Color color)
        {
            return CreatePanel(objectName, parent, anchor, Vector2.zero, size, color);
        }

        private static Image CreatePanel(string objectName, Transform parent, Vector2 anchor, Vector2 offset, Vector2 size, Color color)
        {
            var panelObject = new GameObject(objectName);
            panelObject.transform.SetParent(parent, false);

            var rectTransform = panelObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = offset;
            rectTransform.sizeDelta = size;

            var image = panelObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static void CreateAccentStrip(Transform parent, Color color)
        {
            var strip = new GameObject("AccentStrip");
            strip.transform.SetParent(parent, false);

            var rect = strip.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(0f, 4f);

            var image = strip.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
        }

        private static Button CreateButton(string objectName, Transform parent, Vector2 anchor, Vector2 size, string buttonText, Color fillColor)
        {
            var buttonObject = new GameObject(objectName);
            buttonObject.transform.SetParent(parent, false);

            var rectTransform = buttonObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = size;

            var image = buttonObject.AddComponent<Image>();
            image.color = fillColor;

            var button = buttonObject.AddComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            var colors = button.colors;
            colors.normalColor = fillColor;
            colors.highlightedColor = new Color(Mathf.Min(fillColor.r + 0.06f, 1f), Mathf.Min(fillColor.g + 0.08f, 1f), Mathf.Min(fillColor.b + 0.1f, 1f), 1f);
            colors.pressedColor = new Color(fillColor.r * 0.78f, fillColor.g * 0.78f, fillColor.b * 0.78f, 1f);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = new Color(fillColor.r * 0.6f, fillColor.g * 0.6f, fillColor.b * 0.6f, 0.85f);
            colors.colorMultiplier = 1f;
            button.colors = colors;

            CreateLabel("Label", buttonObject.transform, new Vector2(0.5f, 0.5f), size, buttonText, 24, TextAnchor.MiddleCenter, MenuTextColor, FontStyle.Bold, true);
            return button;
        }

        private static GameObject CreateLabel(string objectName, Transform parent, Vector2 anchor, Vector2 offset, Vector2 size, string textValue, int fontSize, TextAnchor alignment, Color color, FontStyle fontStyle = FontStyle.Normal, bool addOutline = false)
        {
            GameObject labelObject = CreateLabel(objectName, parent, anchor, size, textValue, fontSize, alignment, color, fontStyle, addOutline);
            RectTransform rectTransform = labelObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = offset;
            return labelObject;
        }

        private static GameObject CreateLabel(string objectName, Transform parent, Vector2 anchor, Vector2 size, string textValue, int fontSize, TextAnchor alignment, Color color, FontStyle fontStyle = FontStyle.Normal, bool addOutline = false)
        {
            var labelObject = new GameObject(objectName);
            labelObject.transform.SetParent(parent, false);

            var rectTransform = labelObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = size;

            var text = labelObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = textValue;
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment = alignment;
            text.color = color;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            var shadow = labelObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.62f);
            shadow.effectDistance = new Vector2(1.4f, -1.4f);

            if (addOutline)
            {
                var outline = labelObject.AddComponent<Outline>();
                outline.effectColor = new Color(0f, 0f, 0f, 0.8f);
                outline.effectDistance = new Vector2(1f, -1f);
            }

            return labelObject;
        }

        private static void EnsureFolder(string parent, string child)
        {
            if (!AssetDatabase.IsValidFolder($"{parent}/{child}"))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }
    }
}
