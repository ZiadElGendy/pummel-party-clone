using System.Collections.Generic;
using System.Linq;
using PummelPartyClone;
using UnityEngine;
using UnityEngine.UI;

namespace Sandbox.TileBehaviourDemo.Scripts.UI
{
    public class InventoryBody : MonoBehaviour
    {
        private PlayerController _player;
        public GameObject ItemPrefab;
        private List<GameObject> _itemCache = new List<GameObject>();

        private void OnEnable()
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            Debug.Log("Updating Inventory UI");
            _player = TurnManager.Instance.CurrentPlayer;
            Inventory inventory = _player.Inventory;
            Dictionary<Item, int> itemQuantityDict = inventory.GetItemsQuantity();

            string[] itemNames = itemQuantityDict.Keys.Select(item => item.Data.ItemName).ToArray();
            int[] itemQuantities = itemQuantityDict.Values.ToArray();

            DeactivateItems();
            CreateNewItems(itemQuantityDict, itemNames, itemQuantities);
        }

        private void CreateNewItems(Dictionary<Item, int> itemsQuantity, string[] itemNames, int[] itemQuantities)
        {
            for (int i = 0; i < itemsQuantity.Count; i++)
            {
                GameObject newItem;

                // recycle item if possible
                if (i < _itemCache.Count)
                {
                    newItem = _itemCache[i];
                    newItem.SetActive(true);
                }
                else
                {
                    newItem = Instantiate(ItemPrefab, transform);
                    _itemCache.Add(newItem);
                }

                Image itemImage = newItem.transform.GetChild(0).GetComponent<Image>();
                TMPro.TextMeshProUGUI itemNameText = newItem.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
                TMPro.TextMeshProUGUI itemQuantityText =
                    newItem.transform.GetChild(2).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
                Button itemButton = newItem.transform.GetChild(2).GetComponent<Button>();
                Debug.Log("Created new item: " + itemNames[i]);

                itemImage.sprite = itemsQuantity.Keys.ElementAt(i).Data.Icon;
                itemNameText.text = itemNames[i];
                itemQuantityText.text = itemQuantities[i].ToString();
                int index = i; // Avoid the closure problem
                itemButton.onClick.RemoveAllListeners();
                Debug.Log("Listener uses:" + itemsQuantity.Keys.ElementAt(index).Data.ItemName);
                itemButton.onClick.AddListener(() => { _player.UseItem(itemsQuantity.Keys.ElementAt(index)); });
            }
        }

        private void DeactivateItems()
        {
            foreach (var item in _itemCache)
            {
                item.SetActive(false);
            }
        }
    }
}