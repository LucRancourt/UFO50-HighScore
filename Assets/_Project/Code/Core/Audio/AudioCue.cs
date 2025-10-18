using UnityEngine;


namespace _Project.Code.Core.Audio
{
    [CreateAssetMenu(fileName = "NewAudioCue", menuName = "Scriptable Objects/Audio/Audio Cue")]
    public class AudioCue : ScriptableObject
    {
        [field: SerializeField] public AudioClip[] AudioClips { get; private set; }
        [field: SerializeField] public bool Loop { get; private set; } = false;
        [field: SerializeField, Range(0.0f, 1.0f)] public float Volume { get; private set; } = 1.0f;
        [field: SerializeField, Range(0.1f, 5.0f)] public float Pitch { get; private set; } = 1.0f;


        public AudioClip Get()
        {
            if (AudioClips == null || AudioClips.Length == 0)
            {
                Debug.LogError("AudioCue is Empty!");
                return null;
            }

            return AudioClips[Random.Range(0, AudioClips.Length)];
        }
    }
}