using UnityEditor;
using Battlehub.Utils;
namespace Battlehub.SplineEditor
{
    [CustomEditor(typeof(SplineRuntimeEditor))]
    public class SplineRuntimeEditorEditor : Editor
    {
        SplineRuntimeEditor m_instance;
        PropertyField[] m_fields;

        public void OnEnable()
        {
            m_instance = (SplineRuntimeEditor)target;
            m_fields = ExposeProperties.GetProperties(m_instance);
        }

        public override void OnInspectorGUI()
        {
            if (m_instance == null)
            {
                return;
            }

            DrawDefaultInspector();
            ExposeProperties.Expose(m_fields);
        }
    }
}
