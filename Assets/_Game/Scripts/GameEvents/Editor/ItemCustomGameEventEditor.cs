using UnityEditor;
using UnityEngine;

namespace PummelPartyClone
{
    [CustomEditor(typeof(ItemCustomGameEvent))]
    public class ItemCustomGameEventEditor : Editor
    {
        static int param = 0;
        
        static PlayerController player;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            ItemCustomGameEvent e = target as ItemCustomGameEvent;

            param = EditorGUILayout.IntField("Param", param);
            
            player = (PlayerController)EditorGUILayout.ObjectField("Player", player, typeof(PlayerController), true);

            if (GUILayout.Button("Raise"))
            {
                e.Raise(player, param);
            }
        }
    }
}