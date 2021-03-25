using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SneakyLunatic.Inventory.UI
{
    public class UIItem : MonoBehaviour
    {

        private int amount;
        public Image Icon { get; private set; }
        private TMP_Text tmpText;

        private void Awake()
        {
            Icon = GetComponentInChildren<Image>();
            tmpText = GetComponentInChildren<TMP_Text>();
        }

        public int GetAmount()
        {
            return amount;
        }

        public void SetAmount(int amount)
        {
            this.amount = amount;
            string textString = amount > 1 ? amount.ToString() : "";
            tmpText.text = textString;
        }
    }
}
