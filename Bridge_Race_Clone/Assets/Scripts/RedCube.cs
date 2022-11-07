using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCube : CubeBase
{
    [SerializeField] private Transform[] trailHolders;
    public override void StopTrailRendereOnTop()
    {

        for (int i = 0; i < trailHolders.Length; i++)
        {
            TrailRenderer trail = trailHolders[i].GetComponent<TrailRenderer>();

            trail.enabled = false;
        }
    }
}
