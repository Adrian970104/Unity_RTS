using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectedParticleBehaviour : MonoBehaviour
{
    private static readonly int _particlePerRadius = 300;

    public void SetRingRadius(float r)
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        var s = ps.shape;
        s.radius = r;

        var e = ps.emission;
        e.rateOverTime = r * _particlePerRadius;
    }

}
