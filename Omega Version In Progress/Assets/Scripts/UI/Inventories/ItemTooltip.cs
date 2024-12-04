using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Omega.Inventories;

namespace Omega.UI.Inventories
{
    /// <summary>
    /// Root of the tooltip prefab to expose properties to other classes.
    /// </summary>
    public class ItemTooltip : MonoBehaviour
    {
        // CONFIG DATA
        [SerializeField] Text titleText = null;
        [SerializeField] Text bodyText = null;

        // PUBLIC

        public void Setup(InventoryItem item)
        {
            titleText.text = item.GetDisplayName();
            bodyText.text = item.GetDescription();
        }
    }
}
