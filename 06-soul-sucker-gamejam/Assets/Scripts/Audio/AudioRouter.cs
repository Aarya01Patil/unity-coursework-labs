using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoulSucker.Audio
{
    [DisallowMultipleComponent]
    public sealed class AudioRouter : MonoBehaviour
    {
        private const string BootstrapSceneName = "Bootstrap";
        private const string MainMenuSceneName = "MainMenu";
        private const string GameplaySceneName = "Gameplay";

        private enum MusicState
        {
            None,
            Title,
            Gameplay
        }

        private enum SfxKind
        {
            Jump,
            Hover,
            Possess,
            Release,
            Shoot,
            Death,
            Exit,
            Victory
        }

        [Header("Music Clips")]
        [SerializeField, Tooltip("Optional title theme override. If empty, a generated menu loop is used.")]
        private AudioClip titleMusicOverride;

        [SerializeField, Tooltip("Optional gameplay theme override. If empty, the router loads Resources/Audio/bg-music first.")]
        private AudioClip gameplayMusicOverride;

        [Header("Mix")]
        [SerializeField, Range(0f, 1f)] private float musicVolume = 0.32f;
        [SerializeField, Range(0f, 1f)] private float sfxVolume = 0.88f;
        [SerializeField, Range(0.05f, 2f)] private float musicFadeSeconds = 0.45f;

        private AudioSource musicSource;
        private AudioSource sfxSource;

        private AudioClip titleMusicClip;
        private AudioClip gameplayMusicClip;
        private AudioClip jumpClip;
        private AudioClip hoverClip;
        private AudioClip possessClip;
        private AudioClip releaseClip;
        private AudioClip shootClip;
        private AudioClip deathClip;
        private AudioClip exitClip;
        private AudioClip victoryClip;

        private readonly Dictionary<SfxKind, float> lastPlayTimes = new();
        private Coroutine musicTransitionRoutine;
        private MusicState currentMusicState = MusicState.None;
        private bool hasLoggedMissingGameplayMusic;

        public static AudioRouter Instance { get; private set; }
        public float MusicVolume => musicVolume;
        public float SfxVolume => sfxVolume;
        public string CurrentMusicClipName => musicSource != null && musicSource.clip != null ? musicSource.clip.name : string.Empty;
        public AudioClip CurrentMusicClip => musicSource != null ? musicSource.clip : null;
        public float CurrentMusicTime => musicSource != null ? musicSource.time : 0f;
        public bool IsPlayingTitleMusic => currentMusicState == MusicState.Title && musicSource != null && musicSource.isPlaying;
        public bool IsPlayingGameplayMusic => currentMusicState == MusicState.Gameplay && musicSource != null && musicSource.isPlaying;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Instance = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BootstrapRuntime()
        {
            EnsureInstance();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Instance.CopySettingsFrom(this);
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSources();
            InitializeClips();
            EnsureSceneAudioListener();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void Start()
        {
            EnsureSceneAudioListener();
            SyncMusicToScene(SceneManager.GetActiveScene());
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public static AudioRouter EnsureInstance()
        {
            if (Instance != null)
            {
                return Instance;
            }

            var routerObject = new GameObject(nameof(AudioRouter));
            return routerObject.AddComponent<AudioRouter>();
        }

        public void PlayTitleMusic()
        {
            RequestMusic(MusicState.Title);
        }

        public void PlayGameplayMusic()
        {
            RequestMusic(MusicState.Gameplay);
        }

        public void PlayJump()
        {
            PlaySfx(SfxKind.Jump, jumpClip, 0.9f, 0.08f);
        }

        public void PlayHover()
        {
            PlaySfx(SfxKind.Hover, hoverClip, 0.55f, 0.12f);
        }

        public void PlayPossess()
        {
            PlaySfx(SfxKind.Possess, possessClip, 0.9f, 0.05f);
        }

        public void PlayRelease()
        {
            PlaySfx(SfxKind.Release, releaseClip, 0.85f, 0.05f);
        }

        public void PlayShoot()
        {
            PlaySfx(SfxKind.Shoot, shootClip, 0.8f, 0.04f);
        }

        public void PlayDeath()
        {
            PlaySfx(SfxKind.Death, deathClip, 1f, 0.12f);
        }

        public void PlayExit()
        {
            PlaySfx(SfxKind.Exit, exitClip, 0.85f, 0.08f);
        }

        public void PlayVictory()
        {
            PlaySfx(SfxKind.Victory, victoryClip, 0.95f, 0.18f);
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            EnsureSceneAudioListener();
            SyncMusicToScene(scene);
        }

        private void SyncMusicToScene(Scene scene)
        {
            if (!scene.IsValid())
            {
                return;
            }

            if (scene.name == GameplaySceneName)
            {
                PlayGameplayMusic();
                return;
            }

            if (scene.name == BootstrapSceneName || scene.name == MainMenuSceneName)
            {
                PlayTitleMusic();
            }
        }

        private void InitializeSources()
        {
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }

            ConfigureSource(musicSource, loop: true);
            ConfigureSource(sfxSource, loop: false);
            musicSource.volume = musicVolume;
            sfxSource.volume = sfxVolume;
        }

        private static void ConfigureSource(AudioSource source, bool loop)
        {
            source.playOnAwake = false;
            source.loop = loop;
            source.spatialBlend = 0f;
            source.dopplerLevel = 0f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = 1f;
            source.maxDistance = 20f;
            source.priority = loop ? 0 : 32;
        }

        private void InitializeClips()
        {
            titleMusicClip = titleMusicOverride != null ? titleMusicOverride : SfxGenerator.CreateTitleThemeLoop();
            jumpClip = SfxGenerator.CreateTone("Jump", 940f, 0.08f, 0.11f, 0.003f, 0.025f, 0.24f);
            hoverClip = SfxGenerator.CreateTone("Hover", 256f, 0.16f, 0.08f, 0.01f, 0.09f, 0.12f);
            possessClip = SfxGenerator.CreateTone("Possess", 520f, 0.13f, 0.13f, 0.004f, 0.05f, 0.2f);
            releaseClip = SfxGenerator.CreateTone("Release", 320f, 0.12f, 0.12f, 0.004f, 0.06f, 0.18f);
            shootClip = SfxGenerator.CreateTone("Shoot", 760f, 0.08f, 0.1f, 0.003f, 0.022f, 0.24f);
            deathClip = SfxGenerator.CreateTone("Death", 164f, 0.3f, 0.18f, 0.012f, 0.14f, 0.1f);
            exitClip = SfxGenerator.CreateTone("Exit", 640f, 0.16f, 0.14f, 0.005f, 0.06f, 0.2f);
            victoryClip = SfxGenerator.CreateTone("Victory", 784f, 0.32f, 0.16f, 0.006f, 0.1f, 0.22f);
        }

        private void CopySettingsFrom(AudioRouter other)
        {
            if (other == null)
            {
                return;
            }

            musicVolume = other.musicVolume;
            sfxVolume = other.sfxVolume;
            musicFadeSeconds = other.musicFadeSeconds;
            titleMusicOverride = other.titleMusicOverride;
            gameplayMusicOverride = other.gameplayMusicOverride;

            if (musicSource != null)
            {
                musicSource.volume = musicVolume;
            }

            if (sfxSource != null)
            {
                sfxSource.volume = sfxVolume;
            }
        }

        private void RequestMusic(MusicState targetState)
        {
            AudioClip targetClip = ResolveMusicClip(targetState);
            if (targetClip == null)
            {
                return;
            }

            if (currentMusicState == targetState && musicSource != null && musicSource.clip == targetClip && musicSource.isPlaying)
            {
                musicSource.volume = musicVolume;
                musicTransitionRoutine = null;
                return;
            }

            currentMusicState = targetState;

            if (!Application.isPlaying)
            {
                ApplyMusicImmediate(targetClip, musicVolume);
                return;
            }

            if (musicTransitionRoutine != null)
            {
                StopCoroutine(musicTransitionRoutine);
            }

            musicTransitionRoutine = StartCoroutine(TransitionMusic(targetClip, musicVolume));
        }

        private AudioClip ResolveMusicClip(MusicState targetState)
        {
            if (targetState == MusicState.Title)
            {
                return ResolveTitleMusicClip();
            }

            if (gameplayMusicOverride != null)
            {
                return gameplayMusicOverride;
            }

            if (gameplayMusicClip != null)
            {
                return gameplayMusicClip;
            }

            AudioClip loadedClip = LoadGameplayMusicFromResources();
            if (loadedClip != null)
            {
                gameplayMusicClip = loadedClip;
                return gameplayMusicClip;
            }

            if (!hasLoggedMissingGameplayMusic)
            {
                Debug.LogWarning("Soul Sucker: no gameplay music found in Resources, using generated fallback loop.");
                hasLoggedMissingGameplayMusic = true;
            }

            gameplayMusicClip = SfxGenerator.CreateGameplayFallbackLoop();
            return gameplayMusicClip;
        }

        private void ApplyMusicImmediate(AudioClip nextClip, float targetVolume)
        {
            if (musicSource == null || nextClip == null)
            {
                return;
            }

            musicSource.Stop();
            musicSource.clip = nextClip;
            musicSource.loop = true;
            musicSource.volume = targetVolume;
            musicSource.Play();
            musicTransitionRoutine = null;
        }

        private static AudioClip LoadGameplayMusicFromResources()
        {
            string[] resourcePaths =
            {
                "bg-music",
                "Audio/bg-music",
                "Audio/Music/bg-music",
                "Music/bg-music"
            };

            for (int i = 0; i < resourcePaths.Length; i++)
            {
                AudioClip clip = Resources.Load<AudioClip>(resourcePaths[i]);
                if (clip != null)
                {
                    return clip;
                }
            }

            return null;
        }

        private AudioClip ResolveTitleMusicClip()
        {
            if (titleMusicOverride != null)
            {
                return titleMusicOverride;
            }

            if (titleMusicClip == null)
            {
                titleMusicClip = SfxGenerator.CreateTitleThemeLoop();
            }

            return titleMusicClip;
        }

        private static void EnsureSceneAudioListener()
        {
            if (Object.FindFirstObjectByType<AudioListener>() != null)
            {
                return;
            }

            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Camera[] cameras = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
                if (cameras.Length > 0)
                {
                    mainCamera = cameras[0];
                }
            }

            if (mainCamera == null || mainCamera.GetComponent<AudioListener>() != null)
            {
                return;
            }

            mainCamera.gameObject.AddComponent<AudioListener>();
        }

        private IEnumerator TransitionMusic(AudioClip nextClip, float targetVolume)
        {
            if (musicSource == null || nextClip == null)
            {
                musicTransitionRoutine = null;
                yield break;
            }

            bool sameClip = musicSource.clip == nextClip && musicSource.isPlaying;
            if (sameClip)
            {
                musicSource.volume = targetVolume;
                musicTransitionRoutine = null;
                yield break;
            }

            float fadeTime = Mathf.Max(0.05f, musicFadeSeconds);

            if (musicSource.isPlaying)
            {
                float startingVolume = musicSource.volume;
                for (float elapsed = 0f; elapsed < fadeTime; elapsed += Time.unscaledDeltaTime)
                {
                    musicSource.volume = Mathf.Lerp(startingVolume, 0f, elapsed / fadeTime);
                    yield return null;
                }

                musicSource.Stop();
            }

            musicSource.clip = nextClip;
            musicSource.loop = true;
            musicSource.volume = 0f;
            musicSource.Play();

            for (float elapsed = 0f; elapsed < fadeTime; elapsed += Time.unscaledDeltaTime)
            {
                musicSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / fadeTime);
                yield return null;
            }

            musicSource.volume = targetVolume;
            musicTransitionRoutine = null;
        }

        private void PlaySfx(SfxKind kind, AudioClip clip, float clipVolumeScale, float cooldownSeconds)
        {
            if (sfxSource == null || clip == null)
            {
                return;
            }

            float now = Time.unscaledTime;
            if (cooldownSeconds > 0f && lastPlayTimes.TryGetValue(kind, out float lastPlayTime) && now - lastPlayTime < cooldownSeconds)
            {
                return;
            }

            lastPlayTimes[kind] = now;
            sfxSource.PlayOneShot(clip, Mathf.Clamp01(sfxVolume * clipVolumeScale));
        }
    }
}
