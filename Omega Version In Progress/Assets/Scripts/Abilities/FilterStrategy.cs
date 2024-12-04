using UnityEngine;
using System;
using System.Collections.Generic;

namespace Omega.Abilities
{
    public abstract class FilterStrategy : ScriptableObject
    {
        public abstract IEnumerable<GameObject> Filter(IEnumerable<GameObject> objectsToFilter);
    }
}