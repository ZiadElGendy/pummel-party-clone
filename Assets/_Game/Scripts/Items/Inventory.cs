using System;
using System.Collections.Generic;
using PummelPartyClone;
using Sandbox.TileBehaviourDemo.Scripts.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace PummelPartyClone
{
    public class Inventory
    {
        public int Coins;
        public List<Item> Items;
        private GameEvent _onInventoryUpdate;

        public Inventory(int coins, List<Item> items, GameEvent onInventoryUpdate)
        {
            Coins = coins;
            Items = items;
            _onInventoryUpdate = onInventoryUpdate;
        }

        public List<Item> GetItems()
        {
            return Items;
        }

        public void AddSubCoins(int amount)
        {
            Coins += amount;
            Coins = Mathf.Max(0, Coins);
            OnInventoryUpdate();
        }

        public void AddItem(Item item)
        {
            Items.Add(item);
            Debug.Log("Added item: " + item);
            OnInventoryUpdate();
        }

        public void RemoveItem(Item item)
        {
            if (Items.Contains(item))
            {
                Debug.Log("Removed item: " + item);
                Items.Remove(item);
                OnInventoryUpdate();
            }
            else
            {
                Debug.LogError("Item not found");
            }
        }

        private void OnInventoryUpdate()
        {
            _onInventoryUpdate.Raise();
        }

        public Dictionary<Item, int> GetItemsQuantity()
        {
            Dictionary<Item, int> itemsQuantity = new Dictionary<Item, int>();

            foreach (Item item in Items)
            {
                if (itemsQuantity.ContainsKey(item))
                {
                    itemsQuantity[item]++;
                }
                else
                {
                    itemsQuantity.Add(item, 1);
                }
            }

            return itemsQuantity;
        }

        public override string ToString()
        {
            return $"Coins: {Coins}\nItems: {string.Join(", ", Items)}";
        }
    }
}
