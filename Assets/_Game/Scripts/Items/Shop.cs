using System.Collections.Generic;
using Sandbox.TileBehaviourDemo.Scripts;
using UnityEngine;
using PummelPartyClone;

namespace PummelPartyClone
{
    public class Shop : MonoBehaviour
    {
        public List<Item> Items;

        public void BuyItem(Item item)
        {
            PlayerController player = TurnManager.Instance.CurrentPlayer;
            Inventory inventory = player.Inventory;

            if (inventory.Coins >= item.Data.Price)
            {
                inventory.AddItem(item);
                inventory.AddSubCoins(-item.Data.Price);
            }
            else
            {
                // TODO: Alert Message
                Debug.Log("You are POOOOOOOOOOOOR");
            }
        }
    }
}
