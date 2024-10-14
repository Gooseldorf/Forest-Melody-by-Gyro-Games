using Coffee.UIExtensions;
using UnityEngine;

public class TapEffectVisual : MonoBehaviour
{
    [SerializeField] private UIParticle tapEffect;
    private bool isPlaying;

    public void Play()
    {
        tapEffect.Play();
        isPlaying = true;
    }

    private void Release()
    {
        EffectVisualManager.Instance.TapEffectPool.Release(this);
        isPlaying = false;
    }

    private void Update()
    {
        if (!isPlaying) return;
        
        if(tapEffect.particles[0].IsAlive()) return;
        
        Release();
    }
}
