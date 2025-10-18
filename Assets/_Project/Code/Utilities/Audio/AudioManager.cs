using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

using _Project.Code.Core.General;
using _Project.Code.Core.Audio;


namespace _Project.Code.Utilities.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        // Variables
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioMixerGroup musicMixer;
        [SerializeField] private AudioMixerGroup sfxMixer;

        [SerializeField] private AudioCue defaultMusic;
        private AudioCue _music;
        private AudioSource _musicSource;

        private List<AudioSource> _soundEffectSources = new List<AudioSource>();


        // Functions
        private void Start()
        {
            LoadVolume();
            PlayMusic(defaultMusic);
            SceneManager.activeSceneChanged += Reset;
        }

        #region Volume
        private void LoadVolume()
        {
            SetMixerVolume(AudioMixerKeys.MasterVolumeKey, PlayerPrefs.GetFloat(AudioMixerKeys.MasterVolumeKey));
            SetMixerVolume(AudioMixerKeys.MusicVolumeKey, PlayerPrefs.GetFloat(AudioMixerKeys.MusicVolumeKey));
            SetMixerVolume(AudioMixerKeys.SFXVolumeKey, PlayerPrefs.GetFloat(AudioMixerKeys.SFXVolumeKey));
        }

        public void SetMixerVolume(string key, float volume)
        {
            audioMixer.SetFloat(key, Mathf.Log10(volume) * 20);
        }
        #endregion

        public void StopMusic()
        {
            _musicSource.Stop();
        }

        private void Reset(Scene scene, Scene newScene)
        {
            foreach (AudioSource source in _soundEffectSources)
            {
                source.Stop();
            }
        }

        #region Plays
        public void ResumeMusic()
        {
            _musicSource.loop = true;
            _musicSource.Play();
        }

        private void PlayMusic(AudioCue music)
        {
            _music = music;

            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.outputAudioMixerGroup = musicMixer;

            SetupSource(ref _musicSource, music);

            _musicSource.loop = true;
            _musicSource.Play();
        }

        public void PlaySound(AudioCue sound)
        {
            AudioSource soundSource = GetAvailableSFXSource();

            SetupSource(ref soundSource, sound);

            soundSource.Play();
        }

        public AudioSource PlaySoundReturn(AudioCue sound)
        {
            AudioSource soundSource = GetAvailableSFXSource();

            SetupSource(ref soundSource, sound);

            soundSource.Play();

            return soundSource;
        }

        public void PlayRandomSound(List<AudioCue> listOfSFX)
        {
            PlaySound(listOfSFX[Random.Range(0, listOfSFX.Count)]);
        }
        #endregion

        private void SetupSource(ref AudioSource source, AudioCue sfx)
        {
            source.clip = sfx.Get();
            source.volume = sfx.Volume;
            source.pitch = sfx.Pitch;
        }

        private AudioSource GetAvailableSFXSource()
        {
            foreach (AudioSource soundEffectSource in _soundEffectSources)
            {
                if (!soundEffectSource.isPlaying)
                {
                    return soundEffectSource;
                }
            }

            AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
            newAudioSource.outputAudioMixerGroup = sfxMixer;
            _soundEffectSources.Add(newAudioSource);
            return newAudioSource;
        }
    }
}