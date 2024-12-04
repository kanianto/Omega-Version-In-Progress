using UnityEngine;
using UnityEngine.UI;
using System;

namespace Omega.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health healthComponent = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas rootCanvas = null;

        private void Update()
        {
            if(Mathf.Approximately(healthComponent.GetFraction(), 0)
            || Mathf.Approximately(healthComponent.GetFraction(), 1))
            {
                rootCanvas.enabled = false;
                return;
            }

            rootCanvas.enabled = true;

            foreground.localScale = new Vector3(healthComponent.GetFraction(), 1, 1);
        }
    }
}