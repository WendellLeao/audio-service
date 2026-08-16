using System.Collections.Generic;
using WendellLeao.Pooling;
using WendellLeao.ServiceLocator;
using UnityEngine;

namespace WendellLeao.Audio
{
    [DisallowMultipleComponent]
    public sealed class AudioService : MonoBehaviour, IAudioService
    {
        [Header("Sound Player")]
        [SerializeField]
        private PoolData soundPlayerPool;

        [Header("Data")]
        [SerializeField]
        private AudioDataCollection audioDataCollection;

        private readonly Dictionary<string, AudioData> _audioDataDictionary = new();
        private readonly List<SoundPlayer> _allActiveSoundPlayers = new();
        private readonly List<AudioData> _playingAudios = new();

        public void PlaySound(string audioId, Vector3 position)
        {
            if (!_audioDataDictionary.TryGetValue(audioId, out AudioData audioData))
            {
                Debug.LogError($"Couldn't find any AudioData with id '{audioId}'!");
                return;
            }

            if (!CanPlaySound(audioData))
            {
                return;
            }

            if (!TryGetSoundPlayerFromPool(soundPlayerPool.Id, out SoundPlayer soundPlayer))
            {
                Debug.LogError($"Couldn't get a SoundPlayer from the pool with id '{soundPlayerPool.Id}'!");
                return;
            }

            InitializeSoundPlayer(position, soundPlayer, audioData);
        }

        private void Awake()
        {
            Locator.Register<IAudioService>(this);

            foreach (AudioData audioData in audioDataCollection.AudioData)
            {
                _audioDataDictionary.Add(audioData.Id, audioData);
            }
        }

        private void OnDestroy()
        {
            Locator.Unregister<IAudioService>();

            foreach (SoundPlayer activeSoundPlayer in _allActiveSoundPlayers.ToArray())
            {
                StopSoundPlayer(activeSoundPlayer);
            }
        }

        private void InitializeSoundPlayer(Vector3 position, SoundPlayer soundPlayer, AudioData audioData)
        {
            soundPlayer.OnClipFinished += StopSoundPlayer;

            soundPlayer.Initialize(audioData, position);

            _allActiveSoundPlayers.Add(soundPlayer);

            _playingAudios.Add(audioData);
        }

        private void StopSoundPlayer(SoundPlayer soundPlayer)
        {
            soundPlayer.OnClipFinished -= StopSoundPlayer;

            soundPlayer.Shutdown();

            _allActiveSoundPlayers.Remove(soundPlayer);

            _playingAudios.Remove(soundPlayer.Data);
        }

        private bool CanPlaySound(AudioData audioData)
        {
            if (!audioData.PersistentSound)
            {
                return true;
            }

            if (!_playingAudios.Contains(audioData))
            {
                return true;
            }

            return false;
        }

        private bool TryGetSoundPlayerFromPool(string poolId, out SoundPlayer soundPlayer)
        {
            IPoolingService poolingService = Locator.Get<IPoolingService>();

            return poolingService.TryGetObjectFromPool(poolId, parent: null, out soundPlayer);
        }
    }
}
