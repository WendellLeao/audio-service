using UnityEngine;

namespace WendellLeao.Audio
{
    public interface IAudioService
    {
        public void PlaySound(string audioId, Vector3 position);
    }
}
