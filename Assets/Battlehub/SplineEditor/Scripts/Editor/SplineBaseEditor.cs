using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Battlehub.SplineEditor
{
    public class SplineBaseEditor : Editor
    {
        public const string UNDO_ADDCURVE = "Battlehub.MeshDeformer.AddCurve";
        public const string UNDO_MOVEPOINT = "Battlehub.MeshDeformer.MovePoint";
        public const string UNDO_CHANGEMODE = "Battlehub.MeshDeformer.ChangePointMode";
        public const string UNDO_TOGGLELOOP = "Battlehub.MeshDeformer.ToggleLoop";

        private const int StepsPerCurve = 5;
        private const float DirectionScale = 0.0f;
        protected const float TwistAngleScale = 0.25f;
        protected const float HandleSize = 0.045f;
        protected const float PickSize = 0.15f;
        protected const float PickSize2 = 0.09f;

        private int m_selectedIndex = -1;
        private SplineBase m_selectedSpline;
        private SplineBase m_splineBase;
        private GUIStyle m_greenLabelStyle = new GUIStyle();
        protected GUIStyle m_addButton;
        protected GUIStyle m_branchButton;
        private Color m_branchColor = new Color32(0xa5, 0x00, 0xff, 0xff);


        protected SplineBase SelectedSpline
        {
            get { return m_selectedSpline; }
        }

        protected int SelectedIndex
        {
            get { return m_selectedIndex; }
            set { m_selectedIndex = value; }
        }

        public static SplineBase ConvergingSpline
        {
            get { return SplineBase.ConvergingSpline; }
            set
            {
                SplineBase.ConvergingSpline = value;
                if (SplineBase.ConvergingSpline)
                {
                    Tools.hidden = true;
                    SceneView.RepaintAll();
                }
                else
                {
                    Tools.hidden = false;
                    SceneView.RepaintAll();
                }
            }
        }

        private static readonly Color[] ModeColors = {
            Color.yellow,
            Color.blue,
            Color.red,
        };

        private void OnEnable()
        {
            OnEnableOverride();

            SplineBase spline = GetTarget();
            if(spline)
            {
                spline.Select();
            }

            m_greenLabelStyle.normal.textColor = Color.green;
            m_addButton = new GUIStyle();
            m_addButton.alignment = TextAnchor.UpperLeft;
            m_addButton.contentOffset = new Vector2(-4, -10);
            m_addButton.normal.textColor = Color.green;


            m_branchButton = new GUIStyle();
            m_branchButton.alignment = TextAnchor.UpperLeft;
            m_branchButton.contentOffset = new Vector2(-4, -10);
            m_branchButton.normal.textColor = m_branchColor;

        }

        private void OnDisable()
        {
            OnDisableOverride();

            SplineBase spline = GetTarget();
            if(spline)
            {
                spline.Unselect();
            }
            
        }
        private void Awake()
        {
            AwakeOverride();
        }


        private void OnDestroy()
        {
            ConvergingSpline = null;
            OnDestroyOverride();
        }

        protected virtual void AwakeOverride()
        {

        }

        protected virtual void OnEnableOverride()
        {

        }

        protected virtual void OnDisableOverride()
        {

        }

     
        protected virtual void OnDestroyOverride()
        {

        }

        public override void OnInspectorGUI()
        {
            SerializedObject sObj = GetSerializedObject();
            sObj.Update();
            if (m_splineBase == null)
            {
                m_splineBase = GetTarget();
            }

            if (m_splineBase == null)
            {
                return;
            }


            if (m_selectedIndex >= 0 && m_selectedIndex < m_splineBase.ControlPointCount)
            {
                DrawSelectedPointInspector();
            }

            OnInspectorGUIOverride();


            if (target != null)
            {
                sObj.ApplyModifiedProperties();
            }
        }

        protected virtual void OnInspectorGUIOverride()
        {
            EditorGUI.BeginChangeCheck();
            bool loop = EditorGUILayout.Toggle("Loop", m_splineBase.Loop);
            if (EditorGUI.EndChangeCheck())
            {
                ToggleLoop(loop);
            }
        }

        private void OnSceneGUI()
        {
            SceneGUIOverride();
        }

        protected virtual void SceneGUIOverride()
        {
            if (m_splineBase == null)
            {
                m_splineBase = GetTarget();
            }

            if (m_splineBase == null)
            {
                return;
            }

            SplineBase root = m_splineBase.Root;
            ShowPointsRecursive(root);
        }

        private void ShowPointsRecursive(SplineBase spline)
        {
            ShowPoints(spline);
            if(spline.Children != null)
            {
                for (int i = 0; i < spline.Children.Length; ++i)
                {
                    SplineBase childSpline = spline.Children[i];
                    ShowPointsRecursive(childSpline);
                }
            }
        }

        private void ShowPoints(SplineBase spline)
        {
            if(!spline.IsSelected)
            {
                return;
            }
            Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                 spline.transform.rotation : Quaternion.identity;

            int firstPointIndex = 0;
            if(spline.IsControlPointLocked(firstPointIndex))
            {
                firstPointIndex++;
                if(spline.IsControlPointLocked(firstPointIndex))
                {
                    firstPointIndex++;
                }
            }
            int lastPointIndex = spline.ControlPointCount - 1;
            if(spline.IsControlPointLocked(lastPointIndex))
            {
                lastPointIndex--;
                if(spline.IsControlPointLocked(lastPointIndex))
                {
                    lastPointIndex--;
                }
            }

            if(ConvergingSpline)
            {
                if(spline.Loop)
                {
                    if(firstPointIndex == 0)
                    {
                        firstPointIndex++;
                    }
                    if(lastPointIndex == spline.ControlPointCount - 1)
                    {
                        lastPointIndex--;
                    }
                }

                for (int i = firstPointIndex; i <= lastPointIndex; i++)
                {
                    if(i % 3 == 0 && spline != m_splineBase)
                    {
                        Vector3 p = spline.GetControlPoint(i);
                        ShowPoint(spline, i, p, handleRotation);
                    }
                }
            }
            else
            {
                for (int i = firstPointIndex; i <= lastPointIndex; i++)
                {
                    Vector3 p = spline.GetControlPoint(i);
                    ShowPoint(spline, i, p, handleRotation);
                }
            }

            if(m_selectedIndex == -1 && spline == m_splineBase)
            {
                ShowSplineLength(m_splineBase, m_greenLabelStyle);
            }
        }

        protected virtual int GetStepsPerCurve()
        {
            return StepsPerCurve;
        }

        protected virtual Vector3 GetUpVector()
        {
            return Vector3.up;
        }

        protected virtual Vector3 GetSideVector()
        {
            return Vector3.forward;
        }

        private void ShowPoint(SplineBase spline, int index, Vector3 point, Quaternion handleRotation)
        {
            if (!CanShowPoint(index))
            {
                return;
            }

            bool hasBranches = spline.HasBranches(index);
            Handles.color = ModeColors[(int)spline.GetControlPointMode(index)];
            if (index % 3 == 0)
            {
                if (hasBranches)
                {
                    Handles.color = m_branchColor;
                }
                else
                {
                    Handles.color = Color.green;
                }
            }

            float size = HandleUtility.GetHandleSize(point);

            Handles.CapFunction dcf = Handles.DotHandleCap;

            if (index == 0)
            {
                if (!hasBranches)
                {
                    size *= 1.5f;
                }
            }

        
            if (Handles.Button(point, handleRotation, size * HandleSize, size * PickSize, dcf))
            {

                SplineBase unselectedSpline = m_selectedSpline;
                m_selectedSpline = spline;

                int unselectedIndex = m_selectedIndex;
                m_selectedIndex = index;

                if (OnControlPointClick(unselectedIndex, m_selectedIndex))
                {
                    SplineControlPoint controlPoint = spline.GetSplineControlPoints().Where(cpt => cpt.Index == index).FirstOrDefault();
                    if (controlPoint != null)
                    {
                        Selection.activeGameObject = controlPoint.gameObject;
                    }
                }
                else
                {
                    m_selectedIndex = unselectedIndex;
                    m_selectedSpline = unselectedSpline;
                }

                Repaint();
            }

            if (m_selectedIndex == index && spline == m_splineBase)
            {
                ShowLengths(spline, index, true);
            }

            ShowPointOverride(spline, index, point, handleRotation, size);

        }

        protected virtual void ShowPointOverride(SplineBase spline, int index, Vector3 point, Quaternion handleRotation, float size)
        {
            
        }

        private void ShowLengths(SplineBase spline, int index, bool allowRecursiveCall)
        {
            int curveIndex = (m_selectedIndex + 1) / 3;
            curveIndex = Math.Min(curveIndex, spline.CurveCount - 1);
            ShowLength(spline, curveIndex, m_greenLabelStyle);
        }

        private void ShowSplineLength(SplineBase spline, GUIStyle style)
        {
            float distance = spline.EvalDistance();
            float splineLength = spline.EvalSplineLength(GetStepsPerCurve());
            Handles.Label(spline.GetPoint(0.5f), string.Format("D: {0:0.00} m, S: {1:0.00} m", distance, splineLength), style);
        }

        private void ShowLength(SplineBase spline, int curveIndex, GUIStyle style)
        {
            float distance = spline.EvalDistance(curveIndex);
            float curveLength = spline.EvalCurveLength(curveIndex, GetStepsPerCurve());
            float splineLength = spline.EvalSplineLength(GetStepsPerCurve());
            Handles.Label(spline.GetPoint(0.5f, curveIndex), string.Format("D: {0:0.00} m, C: {1:0.00} m, S: {2:0.00}", distance, curveLength, splineLength), style);
        }

        protected virtual bool OnControlPointClick(int unselectedIndex, int selectedIndex)
        {
            return true;
        }

        protected virtual bool CanShowPoint(int index)
        {
            return true;
        }

        private void DrawSelectedPointInspector()
        {
            if (DrawSelectedPointInspectorOverride())
            {
                EditorGUI.BeginChangeCheck();
                ControlPointMode mode = (ControlPointMode)
                EditorGUILayout.EnumPopup("Mode", m_splineBase.GetControlPointMode(m_selectedIndex));
                if (EditorGUI.EndChangeCheck())
                {
                    SetMode(m_splineBase, mode, m_selectedIndex);
                }

                EditorGUI.BeginChangeCheck();

                int index = m_selectedIndex;
                bool isLast = (m_selectedIndex + 1) / 3 == m_splineBase.CurveCount;
                Twist twist = m_splineBase.GetTwist(index);
                EditorGUI.BeginChangeCheck();
                float twistAngle = EditorGUILayout.FloatField("Twist Angle", twist.Data);
                if (EditorGUI.EndChangeCheck())
                {
                    SetTwistAngle(m_splineBase, index, twistAngle);
                }


                if (m_splineBase.Loop || !isLast || m_splineBase.HasBranches(m_selectedIndex))
                {
                    float t1 = twist.T1;
                    float t2 = twist.T2;
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.MinMaxSlider(new GUIContent("Twist Offset"), ref t1, ref t2, 0.0f, 1.0f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        SetTwistOffset(m_splineBase, index, t1, t2);
                    }
                }

                Thickness thickness = m_splineBase.GetThickness(index);
                EditorGUI.BeginChangeCheck();
                Vector3 thicknessValue = EditorGUILayout.Vector3Field("Thickness", thickness.Data);
                if (EditorGUI.EndChangeCheck())
                {
                    SetThickness(m_splineBase, index, thicknessValue);
                }

                if (m_splineBase.Loop || !isLast ||  m_splineBase.HasBranches(m_selectedIndex))
                {
                    float t1 = thickness.T1;
                    float t2 = thickness.T2;
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.MinMaxSlider(new GUIContent("Thickness Offset"), ref t1, ref t2, 0.0f, 1.0f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        SetThicknessOffset(m_splineBase, index, t1, t2);
                    }
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();

                int index = m_selectedIndex;
                Twist twist = m_splineBase.GetTwist(index);
                EditorGUI.BeginChangeCheck();
                float twistAngle = EditorGUILayout.FloatField("Twist Angle", twist.Data);
                if (EditorGUI.EndChangeCheck())
                {
                    SetTwistAngle(m_splineBase, index, twistAngle);
                }

                Thickness thickness = m_splineBase.GetThickness(index);
                EditorGUI.BeginChangeCheck();
                Vector3 thicknessValue = EditorGUILayout.Vector3Field("Thickness", thickness.Data);
                if (EditorGUI.EndChangeCheck())
                {
                    SetThickness(m_splineBase, index, thicknessValue);
                }
            }
        }

        protected virtual bool DrawSelectedPointInspectorOverride()
        {
            return true;
        }

        protected virtual SplineBase GetTarget()
        {
            return (SplineBase)target;
        }

        protected virtual SerializedObject GetSerializedObject()
        {
            return serializedObject;
        }


        protected void ToggleLoop(bool loop)
        {
            do
            {
                if (loop)
                {
                    if (m_splineBase.PrevSpline != null || m_splineBase.NextSpline != null)
                    {
                        if (!EditorUtility.DisplayDialog("Creating Loop", "This spline is branch of another spline. This operation will break connection between them. Do you want to proceed?", "Yes", "No"))
                        {
                            break;
                        }

                    }

                    if (m_splineBase.HasBranches(0) || m_splineBase.HasBranches(m_splineBase.ControlPointCount - 1))
                    {
                        if (!EditorUtility.DisplayDialog("Creating Loop", "This spline has branches connected to it's ends. This operation will break connection between them. Do you want to proceed?", "Yes", "No"))
                        {
                            break;
                        }
                    }
                }

                RecordHierarchy(m_splineBase.Root, UNDO_TOGGLELOOP);
                EditorUtility.SetDirty(m_splineBase);
                m_splineBase.Loop = loop;
            }
            while (false);
        }

        public static void Smooth(SplineBase spline)
        {
            RecordHierarchy(spline.Root, "Battlehub.Spline.SetMode");
            spline.Root.Smooth();
            EditorUtility.SetDirty(spline);
        }

        public static void SetMode(SplineBase spline, ControlPointMode mode)
        {
            RecordHierarchy(spline.Root, "Battlehub.Spline.SetMode");
            spline.Root.SetControlPointMode(mode);
            EditorUtility.SetDirty(spline);
        }


        public static void SetMode(SplineBase spline, ControlPointMode mode, int controlPointIndex)
        {
            RecordHierarchy(spline.Root, UNDO_CHANGEMODE);
            EditorUtility.SetDirty(spline);
            spline.SetControlPointMode(controlPointIndex, mode);
        }

        public static void SetTwistAngle(SplineBase spline, int index, float twistAngle)
        {
            RecordHierarchy(spline.Root, "Battlehub.MeshDeformer2 Twist Angle");
            EditorUtility.SetDirty(spline);
            Twist twist = spline.GetTwist(index);
            twist.Data = twistAngle;
            spline.SetTwist(index, twist);
        }

        public static void SetTwistOffset(SplineBase spline, int index, float t1, float t2)
        {
            Twist twist = spline.GetTwist(index);
            RecordHierarchy(spline.Root, "Battlehub.MeshDeformer2 Twist Offset");
            EditorUtility.SetDirty(spline);
            twist.T1 = t1;
            twist.T2 = t2;
            spline.SetTwist(index, twist);
        }

        public static void SetThickness(SplineBase spline, int index, Vector3 thicknessValue )
        {
            Thickness thickness = spline.GetThickness(index);
            RecordHierarchy(spline.Root, "Battlehub.MeshDeformer2 Thickness");
            EditorUtility.SetDirty(spline);
            thickness.Data = thicknessValue;
            spline.SetThickness(index, thickness);
        }

        public static void SetThicknessOffset(SplineBase spline, int index, float t1, float t2)
        {
            Thickness thickness = spline.GetThickness(index);
            RecordHierarchy(spline.Root, "Battlehub.MeshDeformer2 Thickness Offset");
            EditorUtility.SetDirty(spline);
            thickness.T1 = t1;
            thickness.T2 = t2;
            spline.SetThickness(index, thickness);
        }

        public static void RecordHierarchy(SplineBase root, string name)
        {
            Undo.RecordObject(root, name);
            if (root.Children != null)
            {
                for (int i = 0; i < root.Children.Length; ++i)
                {
                    RecordHierarchy(root.Children[i], name);
                }
            }
        }

        protected void CapFunction(float size, int id, Vector3 p, GUIStyle style, EventType e)
        {
            if (e == EventType.Repaint)
            {
                Handles.Label(p, "+", style);
            }
            else if (e == EventType.Layout)
            {
                Layout(id, p, size * PickSize2);
            }
        }

        private void Layout(int id, Vector3 position, float pickSize)
        {
            Vector3 screenPosition = Handles.matrix.MultiplyPoint(position);

            Matrix4x4 cachedMatrix = Handles.matrix;
            Handles.matrix = Matrix4x4.identity;
            HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(screenPosition, pickSize));
            Handles.matrix = cachedMatrix;
        }


    }
}

