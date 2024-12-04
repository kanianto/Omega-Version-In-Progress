using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Omega.Inventories;

namespace Omega.UI.Inventories
{
    /// <summary>
    /// Allows the `ItemTooltipSpawner` to display the right information.
    /// </summary>
    public interface IItemHolder
    {
        InventoryItem GetItem();
    }
}