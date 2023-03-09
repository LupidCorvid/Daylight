using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderLayerSetter : MonoBehaviour
{
    public int newLayer = -100;
    public Color newColor;

    // Start is called before the first frame update
    public void setAllLayers()
    {
        List<SpriteRenderer> sprites = new List<SpriteRenderer>();
        sprites.AddRange(GetComponentsInChildren<SpriteRenderer>());
        foreach(SpriteRenderer sprite in sprites)
        {
            sprite.sortingOrder = newLayer;
            sprite.color = newColor;
        }

        List<ParticleSystemRenderer> particles = new List<ParticleSystemRenderer>();
        particles.AddRange(GetComponentsInChildren<ParticleSystemRenderer>());

        foreach (ParticleSystemRenderer system in particles)
        {
            system.sortingOrder = newLayer;
            system.sharedMaterial.color = newColor;
        }


        List<ParticleSystem> pSystems = new List<ParticleSystem>();
        pSystems.AddRange(GetComponentsInChildren<ParticleSystem>());

        foreach (ParticleSystem system in pSystems)
        {
            ParticleSystem.MainModule main = system.main;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
        }
    }
}
