using UnityEngine.Audio;
using UnityEngine;

namespace WendellLeao.Audio
{
    [CreateAssetMenu(menuName = "WendellLeao/Audio/Audio Data", fileName = "NewAudioData")]
    public sealed class AudioData : ScriptableObject
    {
        [SerializeField]
        private string id;
        [SerializeField]
        [Space(height: 10)]
        private AudioClip[] audioClips;
        [SerializeField]
        [Space(height: 10)]
        private AudioMixerGroup audioMixerGroup;
        [SerializeField]
        [Space(height: 10)]
        [Range(0f, 1f)]
        private float volume = 0.5f;
        [SerializeField]
        [Range(0f, 3f)]
        private float pitch = 1f;
        [SerializeField]
        [Range(0f, 1f)]
        private float spatialBlend = 0.5f;
        [SerializeField]
        [Space(height: 10)]
        private bool loop;
        [SerializeField]
        private bool persistentSound;

        public string Id => id;
        public AudioClip[] AudioClips => audioClips;
        public AudioMixerGroup AudioMixerGroup => audioMixerGroup;
        public float Volume => volume;
        public float Pitch => pitch;
        public float SpatialBlend => spatialBlend;
        public bool Loop => loop;
        public bool PersistentSound => persistentSound;
    }
}
