# Audio Service

Pooled 3D sound-playing service for Unity projects.

## Installation

Add the package via the Unity Package Manager using a git URL:

```
https://github.com/WendellLeao/audio-service.git
```

To pin a specific version, append `#v1.0.0` (or any tag) to the URL.

Depends on [WendellLeao.ServiceLocator](https://github.com/WendellLeao/service-locator), [WendellLeao.Pooling](https://github.com/WendellLeao/pooling-service) and [UniTask](https://github.com/Cysharp/UniTask).

## Usage

1. Create a `SoundPlayer` prefab: add an `AudioSource` and a `SoundPlayer` component.
2. Create a `PoolData` asset for that prefab (`Create > WendellLeao > Pooling > Pool Data`) and add it to a `PoolDataCollection` assigned to a `PoolingService` in your scene.
3. Create an `AudioData` asset per sound (`Create > WendellLeao > Audio > Audio Data`), setting its clips, volume, pitch, and spatial blend.
4. Create an `AudioDataCollection` asset and assign the `AudioData` entries to it.
5. Add an `AudioService` component to a persistent GameObject, assigning the sound player `PoolData` and the `AudioDataCollection`.

```csharp
using WendellLeao.Audio;
using WendellLeao.ServiceLocator;

IAudioService audioService = Locator.Get<IAudioService>();

audioService.PlaySound(audioId: "Explosion", position: transform.position);
```

`AudioData` marked as `persistentSound` won't retrigger while already playing, and survives scene loads. `AudioService` registers itself as `IAudioService` on `Awake` and unregisters on `OnDestroy`. Requires a `PoolingService` already registered, since sound players are drawn from its pool.
