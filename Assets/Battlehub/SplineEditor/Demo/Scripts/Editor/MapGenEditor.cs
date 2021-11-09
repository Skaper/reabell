using UnityEngine;
using UnityEditor;
namespace Battlehub.SplineEditor
{
    [CustomEditor(typeof(MapGen))]
    public class MapGenEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate"))
            {
                MapGen gen = (MapGen)target;
                gen.Generate();
            }
        }
    }
}

