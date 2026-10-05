using UnityEngine;

namespace SoulSucker.Audio
{
    public static class SfxGenerator
    {
        private const int SampleRate = 44100;

        public static AudioClip CreateTone(
            string clipName,
            float frequency,
            float durationSeconds,
            float amplitude = 0.18f,
            float attackSeconds = 0.006f,
            float releaseSeconds = 0.05f,
            float harmonicMix = 0.18f)
        {
            int sampleCount = Mathf.Max(1, Mathf.RoundToInt(SampleRate * durationSeconds));
            var clip = AudioClip.Create(clipName, sampleCount, 1, SampleRate, false);
            var samples = new float[sampleCount];
            float attackSamples = Mathf.Max(1f, attackSeconds * SampleRate);
            float releaseSamples = Mathf.Max(1f, releaseSeconds * SampleRate);

            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)SampleRate;
                float envelope = GetAttackReleaseEnvelope(i, sampleCount, attackSamples, releaseSamples);
                samples[i] = BuildToneSample(frequency, t, harmonicMix) * amplitude * envelope;
            }

            clip.SetData(samples, 0);
            clip.hideFlags = HideFlags.DontUnloadUnusedAsset;
            return clip;
        }

        public static AudioClip CreateTitleThemeLoop(string clipName = "SoulSucker_TitleTheme")
        {
            float[] motif =
            {
                164f, 196f, 185f, 147f,
                164f, 220f, 196f, 147f,
                131f, 164f, 147f, 123f,
                164f, 185f, 174f, 131f
            };

            return CreatePatternLoop(
                clipName,
                motif,
                stepDurationSeconds: 0.55f,
                leadAmplitude: 0.1f,
                bassFrequency: 41f,
                bassAmplitude: 0.18f,
                pulseFrequency: 58f,
                pulseAmplitude: 0.075f,
                harmonicMix: 0.14f,
                accentEverySteps: 4,
                accentStrength: 0.26f);
        }

        public static AudioClip CreateTitleSting(string clipName = "SoulSucker_TitleSting")
        {
            return CreateTitleThemeLoop(clipName);
        }

        public static AudioClip CreateGameplayThemeLoop(string clipName = "SoulSucker_GameplayTheme")
        {
            float[] motif =
            {
                131f, 147f, 156f, 185f,
                174f, 156f, 147f, 131f,
                117f, 131f, 147f, 174f,
                196f, 174f, 147f, 131f,
                117f, 131f, 147f, 156f,
                174f, 196f, 174f, 147f
            };

            return CreatePatternLoop(
                clipName,
                motif,
                stepDurationSeconds: 0.45f,
                leadAmplitude: 0.09f,
                bassFrequency: 52f,
                bassAmplitude: 0.2f,
                pulseFrequency: 58f,
                pulseAmplitude: 0.08f,
                harmonicMix: 0.12f,
                accentEverySteps: 2,
                accentStrength: 0.2f);
        }

        public static AudioClip CreateGameplayFallbackLoop(string clipName = "SoulSucker_GameplayFallback")
        {
            return CreateGameplayThemeLoop(clipName);
        }

        public static AudioClip CreateSequence(
            string clipName,
            float[] frequencies,
            float stepDurationSeconds,
            float leadAmplitude,
            float bassFrequency,
            float bassAmplitude,
            float pulseFrequency,
            float pulseAmplitude,
            float harmonicMix,
            int accentEverySteps,
            float accentStrength)
        {
            return CreatePatternLoop(
                clipName,
                frequencies,
                stepDurationSeconds,
                leadAmplitude,
                bassFrequency,
                bassAmplitude,
                pulseFrequency,
                pulseAmplitude,
                harmonicMix,
                accentEverySteps,
                accentStrength);
        }

        private static AudioClip CreatePatternLoop(
            string clipName,
            float[] frequencies,
            float stepDurationSeconds,
            float leadAmplitude,
            float bassFrequency,
            float bassAmplitude,
            float pulseFrequency,
            float pulseAmplitude,
            float harmonicMix,
            int accentEverySteps,
            float accentStrength)
        {
            if (frequencies == null || frequencies.Length == 0)
            {
                return CreateTone(clipName, bassFrequency, stepDurationSeconds, bassAmplitude, 0.01f, 0.08f, harmonicMix);
            }

            float durationSeconds = stepDurationSeconds * frequencies.Length;
            int sampleCount = Mathf.Max(1, Mathf.RoundToInt(SampleRate * durationSeconds));
            var clip = AudioClip.Create(clipName, sampleCount, 1, SampleRate, false);
            var samples = new float[sampleCount];
            float attackSamples = Mathf.Max(1f, 0.05f * SampleRate);
            float releaseSamples = Mathf.Max(1f, 0.08f * SampleRate);

            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)SampleRate;
                int stepIndex = Mathf.Min(frequencies.Length - 1, (int)(t / stepDurationSeconds));
                float stepStart = stepIndex * stepDurationSeconds;
                float stepProgress = Mathf.Clamp01((t - stepStart) / stepDurationSeconds);
                float stepEnvelope = GetAttackReleaseEnvelope(i - Mathf.RoundToInt(stepStart * SampleRate), Mathf.RoundToInt(stepDurationSeconds * SampleRate), attackSamples, releaseSamples);
                float note = BuildToneSample(frequencies[stepIndex], t, harmonicMix) * leadAmplitude * stepEnvelope;
                float bass = BuildToneSample(bassFrequency, t, harmonicMix * 0.35f) * bassAmplitude * stepEnvelope * (0.75f + 0.25f * Mathf.Sin(stepProgress * Mathf.PI));
                float pulse = BuildToneSample(pulseFrequency, t, 0.05f) * pulseAmplitude * stepEnvelope * GetPulseAccent(stepIndex, stepProgress, accentEverySteps, accentStrength);
                samples[i] = (note + bass + pulse) * 0.95f;
            }

            clip.SetData(samples, 0);
            clip.hideFlags = HideFlags.DontUnloadUnusedAsset;
            return clip;
        }

        private static float BuildToneSample(float frequency, float t, float harmonicMix)
        {
            float fundamental = Mathf.Sin(2f * Mathf.PI * frequency * t);
            float harmonic = Mathf.Sin(2f * Mathf.PI * frequency * 2f * t + 0.23f);
            float subHarmonic = Mathf.Sin(2f * Mathf.PI * frequency * 0.5f * t + 0.11f);
            float shimmer = Mathf.Sin(2f * Mathf.PI * frequency * 4f * t + 0.37f);

            return fundamental * (1f - harmonicMix) + harmonic * (harmonicMix * 0.55f) + subHarmonic * (harmonicMix * 0.25f) + shimmer * (harmonicMix * 0.12f);
        }

        private static float GetAttackReleaseEnvelope(int sampleIndex, int sampleCount, float attackSamples, float releaseSamples)
        {
            if (sampleCount <= 1)
            {
                return 1f;
            }

            float attack = sampleIndex / Mathf.Max(1f, attackSamples);
            float release = (sampleCount - 1 - sampleIndex) / Mathf.Max(1f, releaseSamples);
            float envelope = Mathf.Min(Mathf.Clamp01(attack), Mathf.Clamp01(release));
            return Mathf.SmoothStep(0f, 1f, envelope);
        }

        private static float GetPulseAccent(int stepIndex, float stepProgress, int accentEverySteps, float accentStrength)
        {
            if (accentEverySteps <= 0)
            {
                return 1f;
            }

            if (stepIndex % accentEverySteps != 0)
            {
                return 0.85f;
            }

            float hit = Mathf.Exp(-stepProgress * 10f);
            return 1f + hit * accentStrength;
        }
    }
}
