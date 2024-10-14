using Coffee.UIExtensions;
using UnityEngine;

public class DecorAdditionalParticles : AdditionalDecorEffect
{
    [SerializeField] private UIParticle particles;

    public override void Play()
    {
        particles.Play();
    }
}
