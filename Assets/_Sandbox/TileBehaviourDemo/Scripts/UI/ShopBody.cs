using PummelPartyClone;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Sandbox.TileBehaviourDemo.Scripts.UI
{
    public class ShopMenu : MonoBehaviour
    {
        public Shop Shop;
        public GameObject ItemPrefab;

        void Start()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            for (int i = 0; i < Shop.Items.Count; i++)
            {
                GameObject newItem = Instantiate(ItemPrefab, transform);
                
                Image itemImage = newItem.transform.GetChild(0).GetComponent<Image>();
                TextMeshProUGUI itemNameText = newItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI itemPriceText = newItem.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
                Button itemButton = newItem.transform.GetChild(2).GetComponent<Button>();

                itemImage.sprite = Shop.Items[i].Data.Icon;
                itemNameText.text = Shop.Items[i].Data.ItemName;
                itemPriceText.text = Shop.Items[i].Data.Price.ToString();
                int index = i; // Avoid the closure problem
                itemButton.onClick.AddListener(() => { Shop.BuyItem(Shop.Items[index]); });
            }
        }
    }
}