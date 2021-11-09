using UnityEngine;
using UnityEditor;
using Battlehub.RTHandles;
using System.Linq;

namespace Battlehub.SplineEditor
{
    [CustomEditor(typeof(Spline))]
    public class SplineEditor : SplineBaseEditor
    {
        private Spline m_spline;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        protected override void OnInspectorGUIOverride()
        {
            if (m_spline == null)
            {
                m_spline = (Spline)GetTarget();
            }

            if (m_spline == null)
            {
                return;
            }

            EditorGUI.BeginChangeCheck();
            bool loop = EditorGUILayout.Toggle("Loop", m_spline.Loop);
            if (EditorGUI.EndChangeCheck())
            {
                ToggleLoop(loop);
            }

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                if (m_spline.NextSpline == null)
                {
                    if (GUILayout.Button("Append"))
                    {
                        Append(m_spline);
                    }
                }

                if (m_spline.PrevSpline == null)
                {
                    if (GUILayout.Button("Prepend"))
                    {
                        Prepend(m_spline);
                    }
                }
                if (GUILayout.Button("Set Free"))
                {
                    SetMode(m_spline, ControlPointMode.Free);
                }

                if (GUILayout.Button("Set Aligned"))
                {
                    SetMode(m_spline, ControlPointMode.Aligned);
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
             

                if (m_spline.NextSpline == null)
                {
                    if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera)
                    {
                        if (GUILayout.Button("To Cam"))
                        {
                            AppendThrough(m_spline, SceneView.lastActiveSceneView.camera.transform);
                        }
                    }
                }

                if (m_spline.PrevSpline == null)
                {
                    if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera)
                    {
                        if (GUILayout.Button("To Cam"))
                        {
                            PrependThrough(m_spline, SceneView.lastActiveSceneView.camera.transform);
                        }
                    }
                }

                if (GUILayout.Button("Set Mirrored"))
                {
                    SetMode(m_spline, ControlPointMode.Mirrored);
                }

                if (GUILayout.Button("Smooth"))
                {
                    Smooth(m_spline);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();            
        }

        protected override void SceneGUIOverride()
        {
            base.SceneGUIOverride();
        }

        protected override SplineBase GetTarget()
        {
            Spline spline = (Spline)target;
            return spline;
        }

        private void OnSceneGUI()
        {
            SceneGUIOverride();
        }

        
        protected override void ShowPointOverride(SplineBase spline, int index, Vector3 point, Quaternion handleRotation, float size)
        {
            if (!spline.Loop)
            {
                if (index == spline.ControlPointCount - 1)
                {
                    if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null)
                    {
                        if ((SceneView.lastActiveSceneView.camera.transform.position - point).magnitude > 4.0f)
                        {
                            Handles.color = new Color(0, 0, 0, 0);
                            
                            if (Handles.Button(point + spline.GetDirection(1.0f) * 1.5f, handleRotation, size * HandleSize, size * PickSize2, (id, p, r, s, e) => CapFunction(size, id, p, m_addButton, e)))
                            {
                                Append((Spline)spline);
                                Selection.activeGameObject = spline.GetSplineControlPoints().Last().gameObject;
                            }
                        }
                    }
                }
                else if (index == 0)
                {
                    if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null)
                    {
                        if ((SceneView.lastActiveSceneView.camera.transform.position - point).magnitude > 4.0f)
                        {
                            if (Handles.Button(point - spline.GetDirection(0.0f) * 1.5f, handleRotation, size * HandleSize, size * PickSize2, (id, p, r, s, e) => CapFunction(size, id, p, m_addButton, e)))
                            {
                                Prepend((Spline)spline);
                                Selection.activeGameObject = spline.GetSplineControlPoints().First().gameObject;
                            }
                        }
                    }
                }
            }
        }

    

        public static Spline CreateSpline(Vector3 position, Thickness thickness, Twist twist,  ControlPointMode mode = ControlPointMode.Mirrored, string undorecord = "Battlehub.Spline.Create")
        {
            GameObject spline = new GameObject();
            spline.name = "Spline";

            if (!FindObjectOfType<GLRenderer>())
            {
                GameObject go = new GameObject();
                go.name = "GLRenderer";
                go.AddComponent<GLRenderer>();
            }

            Undo.RegisterCreatedObjectUndo(spline, undorecord);

            Spline splineComponent = spline.AddComponent<Spline>();
            splineComponent.SetControlPointMode(mode);
            splineComponent.SetTwist(0, twist);
            splineComponent.SetTwist(3, twist);
            splineComponent.SetThickness(0, thickness);
            splineComponent.SetThickness(3, thickness);
            spline.transform.position = position;

            return splineComponent;
        }

        public static void AppendThrough(Spline spline, Transform transform)
        {
            RecordHierarchy(spline.Root, "Battlehub.Spline.Append");

            spline.AppendThorugh(transform);

            EditorUtility.SetDirty(spline);
        }

        public static void Append(Spline spline)
        {
            RecordHierarchy(spline.Root, "Battlehub.Spline.Append");
            spline.Append();
            EditorUtility.SetDirty(spline);
        }

        public static void Insert(Spline spline, int controlPointIndex)
        {
            RecordHierarchy(spline.Root, "Battlehub.Spline.Insert");
            spline.Insert((controlPointIndex + 2) / 3);
            EditorUtility.SetDirty(spline);
        }

        public static void PrependThrough(Spline spline, Transform transform)
        {
            RecordHierarchy(spline.Root, "Battlehub.Spline.Prepend");

            spline.PrependThrough(transform);

            EditorUtility.SetDirty(spline);
        }

        public static void Prepend(Spline spline)
        {
            RecordHierarchy(spline.Root, "Battlehub.Spline.Prepend");
            spline.Prepend();
            EditorUtility.SetDirty(spline);
        }
    }
}

