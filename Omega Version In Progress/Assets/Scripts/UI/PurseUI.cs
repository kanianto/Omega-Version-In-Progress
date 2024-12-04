using UnityEngine;
using TMPro;
using Omega.UI.Inventories;

namespace Omega.UI
{
    public class PurseUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI balanceField;

        Purse playerPurse = null;

        private void Start()
        {
            playerPurse = GameObject.FindGameObjectWithTag("Player").GetComponent<Purse>(); 

            if(playerPurse != null)
            {
                playerPurse.onChange += RefreshUI;
            }

            RefreshUI();
        }

        private void RefreshUI()
        {
            balanceField.text = $"g{playerPurse.GetBalance()}";
        }
    }
}