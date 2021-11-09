using System;
using UnityEngine;

using Battlehub.RTEditor;

namespace Battlehub.SplineEditor
{
    [ExecuteInEditMode]
    public class SplineControlPoint : MonoBehaviour
    {
        private MeshRenderer m_renderer;
        private SplineBase m_spline;
        private Vector3 m_localPosition;
        private Quaternion m_rotation;
        private bool m_updateAngle = true;
        

        [SerializeField, HideInInspector]
        private int m_index;
        public int Index
        {
            get { return m_index; }
            set
            {
                m_index = value;
                UpdateMaterial();
            }
        }

     
        private void OnEnable()
        {
            m_spline = GetComponentInParent<SplineBase>();
            if (m_spline == null)
            {
                return;
            }

            m_spline.ControlPointTwistChanged -= OnControlPointTwistChanged;
            m_spline.ControlPointTwistChanged += OnControlPointTwistChanged;
            m_spline.ControlPointThicknessChanged -= OnControlPointThicknessChanged;
            m_spline.ControlPointThicknessChanged += OnControlPointThicknessChanged;
            m_spline.ControlPointModeChanged -= OnControlPointModeChanged;
            m_spline.ControlPointModeChanged += OnControlPointModeChanged;
            m_spline.ControlPointPositionChanged -= OnControlPointPositionChanged;
            m_spline.ControlPointPositionChanged += OnControlPointPositionChanged;
            m_spline.ControlPointConnectionChanged -= OnControlPointConnectionChanged;
            m_spline.ControlPointConnectionChanged += OnControlPointConnectionChanged;

            UpdateRenderersState();
        }

        private void Start()
        {
            SplineRuntimeEditor.Created += OnRuntimeEditorCreated;
            SplineBase.ConvergingSplineChanged += OnIsConvergingChanged;

            CreateRuntimeComponents();
            if (m_spline == null)
            {
                m_spline = GetComponentInParent<SplineBase>();
                if (m_spline == null)
                {
                    Debug.LogError("Is not a child of gameobject with Spline or MeshDeformer component");
                    return;
                }

                m_spline.ControlPointTwistChanged -= OnControlPointTwistChanged;
                m_spline.ControlPointTwistChanged += OnControlPointTwistChanged;
                m_spline.ControlPointThicknessChanged -= OnControlPointThicknessChanged;
                m_spline.ControlPointThicknessChanged += OnControlPointThicknessChanged;
                m_spline.ControlPointModeChanged -= OnControlPointModeChanged;
                m_spline.ControlPointModeChanged += OnControlPointModeChanged;
                m_spline.ControlPointPositionChanged -= OnControlPointPositionChanged;
                m_spline.ControlPointPositionChanged += OnControlPointPositionChanged;
                m_spline.ControlPointConnectionChanged -= OnControlPointConnectionChanged;
                m_spline.ControlPointConnectionChanged += OnControlPointConnectionChanged;
            }

            m_localPosition = m_spline.GetControlPointLocal(m_index);
            transform.localPosition = m_localPosition;

            UpdateRenderersState();
            UpdateAngle(true);
            m_rotation = transform.rotation;

            Thickness thickness = m_spline.GetThickness(m_index);
            transform.localScale = thickness.Data;

            if (!m_spline.IsSelected)
            {
                gameObject.SetActive(false);
            }          
        }

        protected void OnDestroy()
        {
            if (m_spline != null)
            {
                m_spline.ControlPointTwistChanged -= OnControlPointTwistChanged;
                m_spline.ControlPointThicknessChanged -= OnControlPointThicknessChanged;
                m_spline.ControlPointModeChanged -= OnControlPointModeChanged;
                m_spline.ControlPointPositionChanged -= OnControlPointPositionChanged;
                m_spline.ControlPointConnectionChanged -= OnControlPointConnectionChanged;
            }

            SplineBase.ConvergingSplineChanged -= OnIsConvergingChanged;
            SplineRuntimeEditor.Created -= OnRuntimeEditorCreated;
        }

        private void Update()
        {
            if(m_spline == null)
            {
                return;
            }

            if (transform.localPosition != m_localPosition)
            {
                if(m_spline.SetControlPointLocal(m_index, transform.localPosition))
                {
                    m_localPosition = transform.localPosition;
                }
                else
                {
                    transform.localPosition = m_localPosition;
                }
            }


            if (transform.rotation != m_rotation)
            {
                if (m_index % 3 == 0)
                {
                    Vector3 v = Vector3.back;
                    int prevIndex = m_index - 1;
                    if (prevIndex < 0)
                    {
                        prevIndex = m_index + 1;
                        v = Vector3.forward;
                    }

                    Vector3 prevPt = m_spline.GetControlPoint(prevIndex);
                    Vector3 pt = m_spline.GetControlPoint(m_index);
                    Vector3 toPrev = (transform.rotation * v).normalized * (pt - prevPt).magnitude;

                    Twist twist = m_spline.GetTwist(m_index);

                    m_rotation = transform.rotation;
                    twist.Data = transform.eulerAngles.z; 

                    m_updateAngle = false;
                    m_spline.SetTwist(m_index, twist);
                    m_spline.SetControlPoint(prevIndex, pt + toPrev);
                    m_updateAngle = true;
                }
                else
                {
                    transform.rotation = m_rotation;
                }
            }

            Thickness thickness = m_spline.GetThickness(m_index);
            Vector3 thicknessData = thickness.Data;
            if (transform.localScale != thicknessData)
            {
                thickness.Data = transform.localScale;
                m_spline.SetThickness(m_index, thickness);
            }
        }

        private void OnControlPointThicknessChanged(int pointIndex)
        {
            if ((m_index + 1) / 3 == (pointIndex + 1) / 3)
            {
                transform.localScale = m_spline.GetThickness(pointIndex).Data;
            }
        }

        private void OnControlPointTwistChanged(int pointIndex)
        {
            if(m_updateAngle)
            {
                if ((m_index + 1) % 3 == (pointIndex + 1) % 3)
                {
                    UpdateAngle();
                }
            }
        }

        private void OnRuntimeEditorCreated(object sender, EventArgs e)
        {
            CreateRuntimeComponents();
        }

        private void OnIsConvergingChanged(object sender, EventArgs e)
        {
            if(m_spline.IsSelected)
            {
                UpdateRenderersState();
            }
        }

        private void OnControlPointModeChanged(int pointIndex)
        {
            if (pointIndex == m_index)
            {
                UpdateRenderersState();
            }
        }

        private void OnControlPointPositionChanged(int pointIndex)
        {
            if (m_spline == null)
            {
                return;
            }
            if (!m_updateAngle)
            {
                return;
            }

            if (pointIndex == m_index)
            {
                m_localPosition = m_spline.GetControlPointLocal(pointIndex);
                transform.localPosition = m_localPosition;
                UpdateAngle();
            }
            else if (pointIndex == m_index - 1 || pointIndex == m_index + 1)
            {
                UpdateAngle();
            }
        }

        private void OnControlPointConnectionChanged(int pointIndex)
        {
            if(pointIndex == m_index)
            {
                UpdateRenderersState();
            }
        }

        public void UpdateAngle(bool forceUpdateAngle = false)
        {
            if(m_spline == null)
            {
                return;
            }
            Twist twist = m_spline.GetTwist(m_index);
            int cIndex = m_index % 3;
            if (cIndex == 0)
            {
                int prevIndex = m_index - 1;
                if (prevIndex > 0)
                {
                    Vector3 ptPrev = m_spline.GetControlPoint(prevIndex);
                    Vector3 pt = m_spline.GetControlPoint(m_index);
                    m_rotation = Quaternion.AngleAxis(twist.Data, pt - ptPrev) * Quaternion.LookRotation(pt - ptPrev);
                    transform.rotation = m_rotation;
                }
                else
                {
                    int nextIndex = m_index + 1;
                    Vector3 ptPrev = m_spline.GetControlPoint(m_index);
                    Vector3 pt = m_spline.GetControlPoint(nextIndex);
                    m_rotation = Quaternion.AngleAxis(twist.Data, pt - ptPrev) * Quaternion.LookRotation(pt - ptPrev);
                    transform.rotation = m_rotation;
                }
            }
            else
            {
                bool updateAngle = true;
                #if UNITY_EDITOR
                if (UnityEditor.EditorWindow.focusedWindow == UnityEditor.SceneView.lastActiveSceneView)
                {
                    if (Event.current == null || Event.current.type != EventType.MouseUp)
                    {
                        updateAngle = false;
                    }
                }
                #endif

                if(updateAngle || forceUpdateAngle)
                {
                    if (cIndex == 1)
                    {
                        int prevIndex = m_index - 1;
                        Vector3 ptPrev = m_spline.GetControlPoint(prevIndex);
                        Vector3 pt = m_spline.GetControlPoint(m_index);
                        m_rotation = Quaternion.AngleAxis(twist.Data, pt - ptPrev) * Quaternion.LookRotation(pt - ptPrev);
                        transform.rotation = m_rotation;
                    }
                    else
                    {
                        int nextIndex = m_index + 1;
                        Vector3 ptPrev = m_spline.GetControlPoint(m_index);
                        Vector3 pt = m_spline.GetControlPoint(nextIndex);
                        m_rotation = Quaternion.AngleAxis(twist.Data, pt - ptPrev) * Quaternion.LookRotation(pt - ptPrev); 
                        transform.rotation = m_rotation;
                    }
                }
            } 
        }

        private void UpdateRenderersState()
        {
            if (m_index == 0 || m_index == 1)
            {
                if (m_spline.PrevSpline != null)
                {
                    if (m_renderer != null)
                    {
                        m_renderer.enabled = !m_spline.IsControlPointLocked(m_index);
                    }
                }
                else
                {
                    if (m_renderer != null && !m_renderer.enabled)
                    {
                        m_renderer.enabled = true;
                    }
                }
            }
            else if (m_index == m_spline.ControlPointCount - 1 || m_index == m_spline.ControlPointCount - 2)
            {
                if (m_spline.NextSpline != null)
                {
                    if (m_renderer != null)
                    {
                        m_renderer.enabled = !m_spline.IsControlPointLocked(m_index);
                    }
                }
                else
                {
                    if (m_renderer != null && !m_renderer.enabled)
                    {
                        m_renderer.enabled = true;
                    }
                }
            }
            else
            {
                if (m_renderer != null && !m_renderer.enabled)
                {
                    m_renderer.enabled = true;
                }
            }

            if (SplineBase.ConvergingSpline)
            {
                if (m_spline.Loop)
                {
                    if (m_index == 0 || m_index == m_spline.ControlPointCount - 1)
                    {
                        if (m_renderer != null)
                        {
                            m_renderer.enabled = false;
                        }
                    }
                }

                if (m_index % 3 != 0 || m_spline == SplineBase.ConvergingSpline)
                {
                    if (m_renderer != null)
                    {
                        m_renderer.enabled = false;
                    }
                }
            }

            UpdateMaterial();
        }

        private void UpdateMaterial()
        {
            if (m_renderer != null)
            {
                SplineRuntimeEditor runtimeEditor = SplineRuntimeEditor.Instance;
                if (runtimeEditor != null)
                {
                    if (m_index % 3 == 0)
                    {
                        if (m_spline.HasBranches(m_index))
                        {
                            m_renderer.sharedMaterial = runtimeEditor.ConnectedMaterial;
                        }
                        else
                        {
                            m_renderer.sharedMaterial = runtimeEditor.NormalMaterial;
                        }

                    }
                    else
                    {
                        if (m_index >= m_spline.ControlPointCount)
                        {
                            return;
                        }


                        ControlPointMode mode = m_spline.GetControlPointMode(m_index);
                        if (mode == ControlPointMode.Mirrored)
                        {
                            m_renderer.sharedMaterial = runtimeEditor.MirroredModeMaterial;
                        }
                        else if (mode == ControlPointMode.Aligned)
                        {
                            m_renderer.sharedMaterial = runtimeEditor.AlignedModeMaterial;
                        }
                        else
                        {
                            m_renderer.sharedMaterial = runtimeEditor.FreeModeMaterial;
                        }
                    }
                }
            }
        }

        private void CreateRuntimeComponents()
        {
            SplineRuntimeEditor runtimeEditor = SplineRuntimeEditor.Instance;
            if (runtimeEditor != null)
            {
                m_renderer = GetComponent<MeshRenderer>();
                if (!m_renderer)
                {
                    m_renderer = gameObject.AddComponent<MeshRenderer>();
                }

                MeshFilter filter = GetComponent<MeshFilter>();
                if(!filter)
                {
                    filter = gameObject.AddComponent<MeshFilter>();
                }

                if (!filter.sharedMesh)
                {
                    filter.sharedMesh = runtimeEditor.ControlPointMesh;
                    UpdateMaterial();
                }
                
                if (!gameObject.GetComponent<ExposeToEditor>())
                {
                    gameObject.AddComponent<ExposeToEditor>();
                }
            }
        }

        public void DestroyRuntimeComponents()
        {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if (renderer)
            {
                DestroyImmediate(renderer);
            }

            MeshFilter filter = GetComponent<MeshFilter>();
            if (filter)
            {
                DestroyImmediate(filter);
            }

            ExposeToEditor exposeToEditor = gameObject.GetComponent<ExposeToEditor>();
            if (exposeToEditor)
            {
                DestroyImmediate(exposeToEditor);
            }
        }
    }
}

