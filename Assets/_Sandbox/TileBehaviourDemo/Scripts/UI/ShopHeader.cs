using PummelPartyClone;
using UnityEngine;

namespace Sandbox.TileBehaviourDemo.Scripts.UI
{
    public class ShopHeader : MonoBehaviour
    {
        private PlayerController _player;
        private TMPro.TextMeshProUGUI _text;
        void Start()
        {
            _player = TurnManager.Instance.CurrentPlayer;
            _text = transform.GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        }

        void Update()
        {
            Inventory inventory = _player.Inventory;
            _text.text = $"Coins: {inventory.Coins}";
        }
    }
}
