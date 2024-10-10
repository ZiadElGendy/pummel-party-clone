using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Sandbox.TileBehaviourDemo.Scripts.UI
{
    public enum GameView
    {
        Game,
        Shop,
        Inventory
    }
    public class UIManager : MonoBehaviour
    {
        public GameObject[] views;
        public Button rollButton;
        public Button enterButton;

        void Start()
        {
            rollButton.gameObject.SetActive(true);
            enterButton.gameObject.SetActive(false);
        }

        public void SwitchView(GameView view)
        {
            foreach (var viewObj in views)
            {
                viewObj.SetActive(false);
            }

            views[(int)view].SetActive(true);
        }

        public void SwitchToShop()
        {
            SwitchView(GameView.Shop);
        }

        public void SwitchToGame()
        {
            SwitchView(GameView.Game);
        }

        public void SwitchToInventory()
        {
            SwitchView(GameView.Inventory);
        }

        public void DisplayEnterButton()
        {
            enterButton.gameObject.SetActive(true);
        }

        public void HideEnterButton()
        {
            enterButton.gameObject.SetActive(false);
        }
    }
}