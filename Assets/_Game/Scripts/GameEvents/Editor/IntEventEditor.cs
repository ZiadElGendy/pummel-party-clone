using UnityEditor;
using UnityEngine;

namespace PummelPartyClone
{

    [CustomEditor(typeof(IntGameEvent), editorForChildClasses: true)]
    public class IntEventEditor : Editor
    {
        static int param = 0;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            IntGameEvent e = target as IntGameEvent;

            param = EditorGUILayout.IntField("Param", param);

            if (GUILayout.Button("Raise"))
                e.Raise(param);
        }
    }
}