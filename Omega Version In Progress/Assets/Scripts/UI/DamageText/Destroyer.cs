using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.UI.DamageText
{
    public class Destroyer : MonoBehaviour
    {
        [SerializeField] GameObject targetToDestroy = null;

        public void DestroyTarget()
        {
            Destroy(targetToDestroy);
        }
    }
}
