using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Battlehub.SplineEditor
{
    
    [CustomEditor(typeof(SplineControlPoint))]
    //[CanEditMultipleObjects]
    public class SplineControlPointEditor : SplineBaseEditor
    {
        private Spline m_spline;

        public override void OnInspectorGUI()
        {
            InitActiveSpline();

            base.OnInspectorGUI();
        }

        private void InitActiveSpline()
        {
            if (m_spline == null)
            {
                m_spline = GetTarget() as Spline;

                SplineControlPoint controlPoint = (SplineControlPoint)target;
                SelectedIndex = controlPoint.Index;
                SplineBase.ActiveControlPointIndex = controlPoint.Index;
                SplineBase.ActiveSpline = m_spline;
            }

        }

        protected override void OnEnableOverride()
        {
            base.OnEnableOverride();
        }

        protected override void AwakeOverride()
        {
            Repaint();
        }

        protected override void OnDestroyOverride()
        {
            SplineBase.ActiveControlPointIndex = -1;
            SplineBase.ActiveSpline = null;
        }

        protected override void OnInspectorGUIOverride()
        {
            if (m_spline == null)
            {
                return;
            }

            if (ConvergingSpline)
            {
                if (GUILayout.Button("Cancel"))
                {
                    ConvergingSpline = null;
                }
                return;
            }
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                int curveIndex = (SelectedIndex - 1) / 3;

                if (GUILayout.Button("OUT -> Branch"))
                {
                    CreateBranch(m_spline, SelectedIndex, false);
                }

            

                if (curveIndex == m_spline.CurveCount - 1 && m_spline.NextSpline == null)
                {
                    if (m_spline.NextSpline == null)
                    {
                        if (GUILayout.Button("Append"))
                        {
                            SplineEditor.Append(m_spline);
                            Selection.activeGameObject = m_spline.GetSplineControlPoints().Last().gameObject;
                        }
                    }
                }

                if (curveIndex == 0 && m_spline.PrevSpline == null)
                {
                    if (m_spline.PrevSpline == null)
                    {
                        if (GUILayout.Button("Prepend"))
                        {
                            SplineEditor.Prepend(m_spline);
                            Selection.activeGameObject = m_spline.GetSplineControlPoints().First().gameObject;
                        }
                    }
                }

                if (GUILayout.Button("Insert"))
                {
                    SplineEditor.Insert(m_spline, SelectedIndex);
                    Selection.activeGameObject = m_spline.GetSplineControlPoints().ElementAt(SelectedIndex + 3).gameObject;
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical();

                if (GUILayout.Button("Branch -> IN"))
                {
                    CreateBranch(m_spline, SelectedIndex, true);
                }

                if (curveIndex == m_spline.CurveCount - 1 && m_spline.NextSpline == null)
                {
                    if (m_spline.NextSpline == null)
                    {
                        if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera)
                        {
                            if (GUILayout.Button("To Cam"))
                            {
                                SplineEditor.AppendThrough(m_spline, SceneView.lastActiveSceneView.camera.transform);
                                Selection.activeGameObject = m_spline.GetSplineControlPoints().Last().gameObject;
                            }
                        }
                    }
                }

                if (curveIndex == 0 && m_spline.PrevSpline == null)
                {
                    if (m_spline.PrevSpline == null)
                    {
                        if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera)
                        {
                            if (GUILayout.Button("To Cam"))
                            {
                                SplineEditor.PrependThrough(m_spline, SceneView.lastActiveSceneView.camera.transform);
                                Selection.activeGameObject = m_spline.GetSplineControlPoints().First().gameObject;
                            }
                        }
                    }
                }

                if (SelectedIndex >= 0 && curveIndex < m_spline.CurveCount)
                {
                    if (GUILayout.Button("Remove"))
                    {
                        Remove(m_spline, SelectedIndex);
                    }
                }

                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            if (SelectedIndex == 0 && m_spline.PrevSpline == null ||
               SelectedIndex == m_spline.ControlPointCount - 1 && m_spline.NextSpline == null)
            {
                if (!m_spline.HasBranches(SelectedIndex))
                {
                    if (GUILayout.Button("Converge"))
                    {
                        if (m_spline.Loop)
                        {
                            EditorUtility.DisplayDialog("Unable to converge", "Unable to converge. Selected spline has loop.", "OK");
                        }
                        else
                        {
                            ConvergingSpline = m_spline;
                        }
                    }
                }
            }
            if (SelectedIndex < m_spline.ControlPointCount && SelectedIndex >= 0)
            {
                if (m_spline.HasBranches(SelectedIndex))
                {
                    if (GUILayout.Button("Separate"))
                    {
                        Separate(m_spline, SelectedIndex);
                    }
                }
            }

            if (GUILayout.Button("Align View To Point"))
            {
                if(SceneView.lastActiveSceneView != null)
                {
                    SplineControlPoint controlPoint = (SplineControlPoint)target;


                    SceneView.lastActiveSceneView.AlignViewToObject(controlPoint.transform);
                    SceneView.lastActiveSceneView.Repaint();
                }
            }

            base.OnInspectorGUIOverride();
        }

        protected override void SceneGUIOverride()
        {
            base.SceneGUIOverride();
        }

        protected override void ShowPointOverride(SplineBase spline, int index, Vector3 point, Quaternion handleRotation, float size)
        {
            if (!spline.Loop)
            {
                if (index == spline.ControlPointCount - 1)
                {
                    if(SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null)
                    {
                        if((SceneView.lastActiveSceneView.camera.transform.position - point).magnitude > 4.0f)
                        {
                            if (Handles.Button(point + spline.GetDirection(1.0f) * 1.5f, handleRotation, size * HandleSize, size * PickSize2,  (id, p, r, s, e) => CapFunction(size, id, p, m_addButton, e)))
                            {
                                SplineEditor.Append((Spline)spline);
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
                                SplineEditor.Prepend((Spline)spline);
                                Selection.activeGameObject = spline.GetSplineControlPoints().First().gameObject;
                            }
                        }
                    }
                }
            }
        }


        protected override bool OnControlPointClick(int unselectedIndex, int selectedIndex)
        {
            if(ConvergingSpline)
            {
                if(Converge(SelectedSpline, m_spline, selectedIndex, unselectedIndex))
                {
                    ConvergingSpline = null;
                    return true;
                }
                return false;
            }
            
            return  base.OnControlPointClick(unselectedIndex, selectedIndex);
        }

        protected override SplineBase GetTarget()
        {
            SplineControlPoint controlPoint = (SplineControlPoint)target;
            if(controlPoint)
            {
                SplineBase spline = controlPoint.GetComponentInParent<SplineBase>();
                return spline;
            }
            return null;
        }

        private void OnSceneGUI()
        {
            InitActiveSpline();
            SplineControlPoint controlPoint = (SplineControlPoint)target;
            SelectedIndex = controlPoint.Index;
            SceneGUIOverride();

            if (EditorWindow.focusedWindow == SceneView.lastActiveSceneView)
            {
                if (Event.current != null && (Event.current.type == EventType.MouseUp))
                {
                    SplineControlPoint[] controlPoints = m_spline.GetSplineControlPoints();
                    for(int i = 0; i < controlPoints.Length; ++i)
                    {
                        controlPoints[i].UpdateAngle();
                    }
                }
            }
        }

        public static void CreateBranch(Spline spline, int connectionPointIndex, bool isInbound)
        {
            Spline branch = SplineEditor.CreateSpline(
                                    spline.transform.position,
                                    spline.GetThickness(connectionPointIndex),
                                    spline.GetTwist(connectionPointIndex),
                                    spline.GetControlPointMode(connectionPointIndex),
                                    "Battlehub.Spline.CreateBranch");
            branch.name = "Branch";
            RecordHierarchy(spline, "Battlehub.Spline.CreateBranch");

            spline.SetBranch(branch, connectionPointIndex, isInbound);
            if(isInbound)
            {
                branch.AlignWithNextSpline();
                Selection.activeGameObject = branch.GetSplineControlPoints().First().gameObject;
            }
            else
            {
                branch.AlignWithPrevSpline();
                Selection.activeGameObject = branch.GetSplineControlPoints().Last().gameObject;
            }
            
            EditorUtility.SetDirty(spline);
        }

        public static void Remove(Spline spline, int controlPointIndex)
        {
            int curveIndex = Mathf.Min((controlPointIndex + 1) / 3, spline.CurveCount - 1);
            RecordHierarchy(spline.Root, "Battlehub.Spline.Remove");
            if (!spline.Remove(curveIndex))
            {
                EditorUtility.DisplayDialog("Action cancelled", "Unable to remove last curve in spline. If you want to remove whole spline or branch, please select it in hierarchy and press delete button", "OK");
            }
            else
            {
                EditorUtility.SetDirty(spline);
            }
        }

        public static bool Converge(SplineBase spline, SplineBase branch, int splineIndex, int branchIndex)
        {
            if (spline == branch)
            {
                return false;
            }

            if (branch.PrevSpline != null && branch.NextSpline != null)
            {
                return false;
            }

            if (branchIndex == 0)
            {
                if (branch.PrevSpline != null)
                {
                    return false;
                }

                RecordHierarchy(spline.Root, "Battlehub.Spline.Converge");
                spline.SetBranch(branch, splineIndex, false);
                EditorUtility.SetDirty(branch);
                EditorUtility.SetDirty(spline);
                return true;
            }
            else if (branchIndex == branch.ControlPointCount - 1)
            {
                if (branch.NextSpline != null)
                {
                    return false;
                }

                RecordHierarchy(spline.Root, "Battlehub.Spline.Converge");
                spline.SetBranch(branch, splineIndex, true);
                EditorUtility.SetDirty(branch);
                EditorUtility.SetDirty(spline);
                return true;
            }
            else
            {
                Debug.LogError("branchIndex should be equal to 0 or branch.ControlPointCount - 1");
                return false;
            }
        }

        public static void Separate(SplineBase spline, int controlPointIndex)
        {
            RecordHierarchy(spline.Root, "Battlehub.Spline.Separate");
            spline.Unselect();
            spline.Disconnect(controlPointIndex);
            spline.Select();
            EditorUtility.SetDirty(spline.Root);
        }
    }
}
