using Random = UnityEngine.Random;
using Cysharp.Threading.Tasks;
using WendellLeao.Pooling;
using WendellLeao.ServiceLocator;
using UnityEngine;
using System;
using System.Threading;

namespace WendellLeao.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class SoundPlayer : MonoBehaviour, IPooledObject
    {
        public event Action<SoundPlayer> OnClipFinished;

        [SerializeField]
        private AudioSource audioSource;

        private AudioData _audioData;
        private Vector3 _targetPosition;
        private CancellationTokenSource _handleClipLengthCts;
        private bool _isEnabled;

        public string PoolId { get; set; }
        public AudioData Data => _audioData;

        public void Initialize(AudioData audioData, Vector3 position)
        {
            _audioData = audioData;
            _targetPosition = position;

            if (_isEnabled)
            {
                return;
            }

            _isEnabled = true;

            SetupAudioSource(_audioData);

            transform.position = _targetPosition;

            PlayAudioSource();
        }

        public void Shutdown()
        {
            if (!_isEnabled)
            {
                return;
            }

            _isEnabled = false;

            IPoolingService poolingService = Locator.Get<IPoolingService>();

            poolingService.ReleaseObjectToPool(this);

            DisposeHandleClipLengthCts();
        }

        private void PlayAudioSource()
        {
            audioSource.Play();

            if (audioSource.loop)
            {
                return;
            }

            _handleClipLengthCts = new CancellationTokenSource();

            HandleClipLengthAsync(_handleClipLengthCts.Token);
        }

        private async void HandleClipLengthAsync(CancellationToken token)
        {
            try
            {
                float clipDuration = audioSource.clip.length;

                await UniTask.Delay(TimeSpan.FromSeconds(clipDuration), cancellationToken: token);

                OnClipFinished?.Invoke(this);
            }
            catch (OperationCanceledException e)
            {
                Debug.LogWarning($"The operation was canceled when trying to deactivate the sound GameObject: {e}",
                    gameObject);
            }
            catch (Exception e)
            {
                Debug.LogError($"Unexpected error when trying to deactivate the sound GameObject: {e}", gameObject);
            }
            finally
            {
                DisposeHandleClipLengthCts();
            }
        }

        private void DisposeHandleClipLengthCts()
        {
            _handleClipLengthCts?.Cancel();
            _handleClipLengthCts?.Dispose();
            _handleClipLengthCts = null;
        }

        private void SetupAudioSource(AudioData audioData)
        {
            int randomIndex = Random.Range(0, audioData.AudioClips.Length);

            audioSource.clip = audioData.AudioClips[randomIndex];

            audioSource.volume = audioData.Volume;

            audioSource.pitch = audioData.Pitch;

            audioSource.spatialBlend = audioData.SpatialBlend;

            audioSource.loop = audioData.Loop;

            audioSource.outputAudioMixerGroup = audioData.AudioMixerGroup;

            if (audioData.PersistentSound)
            {
                transform.SetParent(null);

                DontDestroyOnLoad(audioSource.gameObject);
            }
        }
    }
}
