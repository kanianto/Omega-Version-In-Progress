using UnityEngine;
using System;
using System.Collections.Generic;

namespace Omega.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Demo Targeting", menuName = "Abilities/Targeting Demo", order = 0)]
    public class DemoTargeting : TargetingStrategy
    {
        public override void StartTargeting(AbilityData data, Action finished)
        {
            finished();
        }
    }
}