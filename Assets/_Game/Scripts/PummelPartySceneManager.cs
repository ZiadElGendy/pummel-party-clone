using System.Collections.Generic;
using UnityEngine;

namespace PummelPartyClone
{
    public class PummelPartySceneManager : PersistentSingleton<PummelPartySceneManager>
    {
        public List<PlayerController> players;
        
        public void SwitchScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

        public void Update()
        {
            if (Input.GetKeyDown("space"))
            {
                SwitchScene("TashMaTashMinigame");
            }
        }
        
    }
}