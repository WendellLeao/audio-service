using UnityEngine;

namespace WendellLeao.Audio
{
    public sealed class AudioDataCollection : ScriptableObject
    {
        [SerializeField] private AudioData[] audioData;

        public AudioData[] AudioData => audioData;
    }
}
