using System.Runtime.Serialization;
using UnityEngine;
using Battlehub.RTEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System;
using System.Linq;

namespace Battlehub.SplineEditor
{
    public sealed class VersionDeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(typeName))
            {
                Type typeToDeserialize = null;

                assemblyName = Assembly.GetExecutingAssembly().FullName;

                // The following line of code returns the type. 
                typeToDeserialize = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));

                return typeToDeserialize;
            }

            return null;
        }
    }



    public class SplineRuntimeCmd : MonoBehaviour
    {
        public Spline m_spline;
        public SplineControlPoint m_controlPoint;

        private Spline GetSelectedSpline()
        {
            if(RuntimeSelection.activeGameObject == null)
            {
                return null;
            }

            return RuntimeSelection.activeGameObject.GetComponentInParent<Spline>();
        }

        private SplineControlPoint GetSelectedControlPoint()
        {
            if (RuntimeSelection.activeGameObject == null)
            {
                return null;
            }

            return RuntimeSelection.activeGameObject.GetComponentInParent<SplineControlPoint>();
        }

        public void Awake()
        {
            m_spline = GetSelectedSpline();
            RuntimeSelection.SelectionChanged += OnRuntimeSelectionChanged;
        }

        public void OnDestroy()
        {
            SplineBase.ConvergingSpline = null;
            RuntimeSelection.SelectionChanged -= OnRuntimeSelectionChanged;
        }

        private void OnRuntimeSelectionChanged(UnityEngine.Object[] unselectedObjects)
        {
            if(SplineBase.ConvergingSpline)
            {
                SplineControlPoint selectedControlPoint = GetSelectedControlPoint();
                Spline selectedSpline = GetSelectedSpline();
                if(selectedControlPoint == null || m_controlPoint == null || m_spline == null)
                {
                    SplineBase.ConvergingSpline = null;
                }
                else
                {
                    if (Converge(selectedSpline, m_spline, selectedControlPoint.Index, m_controlPoint.Index))
                    {
                        SplineBase.ConvergingSpline = null;
                        m_spline = selectedSpline;
                        m_controlPoint = selectedControlPoint;
                    }
                    else
                    {
                        SplineBase.ConvergingSpline = null;
                    }
                }
            }
            else
            {
                m_controlPoint = GetSelectedControlPoint();
                m_spline = GetSelectedSpline();
            }
        }

        public void RunAction<T>(Action<T, GameObject> action)
        {
            GameObject[] selectedObjects = RuntimeSelection.gameObjects;
            if(selectedObjects == null)
            {
                return;
            }

            for(int i = 0; i < selectedObjects.Length; ++i)
            {
                GameObject selectedObject = selectedObjects[i];
                if(selectedObject == null)
                {
                    continue;
                }

                T spline = selectedObject.GetComponentInParent<T>();
                if(spline == null)
                {
                    continue;
                }

                if(action != null)
                {
                    action(spline, selectedObject);
                }
            }
        }

        public virtual void Append()
        {
            RunAction<Spline>((spline, go) => 
            {
                if(spline.NextSpline == null)
                {
                    spline.Append();
                }
            });
        }

        public virtual void Insert()
        {
            RunAction<Spline>((spline, go) =>
            {
                if (go != null)
                {
                    SplineControlPoint ctrlPoint = go.GetComponent<SplineControlPoint>();
                    if (ctrlPoint != null)
                    {
                        spline.Insert((ctrlPoint.Index + 2) / 3);
                    }
                }
            });               
        }

        public virtual void Prepend()
        {
            RunAction<Spline>((spline, go) =>
            {
                if(spline.PrevSpline == null)
                {
                    spline.Prepend();
                }
            });
        }

        public virtual void Remove()
        {
            RunAction<Spline>((spline, go) =>
            {
                if (go != null)
                {
                    SplineControlPoint ctrlPoint = go.GetComponent<SplineControlPoint>();
                    if (ctrlPoint != null)
                    {
                        int curveIndex = Mathf.Min((ctrlPoint.Index + 1) / 3, spline.CurveCount - 1);
                        spline.Remove(curveIndex);
                    }
                    RuntimeSelection.activeObject = spline.gameObject;
                }
            });
        }

        public virtual void Smooth()
        {
            RunAction<SplineBase>((spline, go) => spline.Root.Smooth());           
        }

        public virtual void SetMirroredMode()
        {
            RunAction<SplineBase>((spline, go) => spline.Root.SetControlPointMode(ControlPointMode.Mirrored));
        }

        public virtual void SetAlignedMode()
        {
            RunAction<SplineBase>((spline, go) => spline.Root.SetControlPointMode(ControlPointMode.Aligned));
        }

        public virtual void SetFreeMode()
        {
            RunAction<SplineBase>((spline, go) => spline.Root.SetControlPointMode(ControlPointMode.Free));
        }

        public virtual void OutBranch()
        {
            throw new NotImplementedException("Implement after Save/Load enchancements");
        }

        public virtual void BranchIn()
        {
            throw new NotImplementedException("Implement after Save/Load enchancements");
        }

        public virtual void Converge()
        {
            SplineBase.ConvergingSpline = m_spline;
        }

        public virtual void Separate()
        {
            if(m_spline != null && m_controlPoint != null)
            {
                Separate(m_spline, m_controlPoint.Index);
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

                spline.SetBranch(branch, splineIndex, false);
                return true;
            }
            else if (branchIndex == branch.ControlPointCount - 1)
            {
                if (branch.NextSpline != null)
                {
                    return false;
                }

                spline.SetBranch(branch, splineIndex, true);
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
            spline.Unselect();
            spline.Disconnect(controlPointIndex);
            spline.Select();
        }

        public virtual void Load()
        {
            string dataAsString = PlayerPrefs.GetString("SplineEditorSave");
            if(string.IsNullOrEmpty(dataAsString))
            {
                return;
            }
            SplineBase[] splines = FindObjectsOfType<SplineBase>();
            SplineSnapshot[] snapshots = DeserializeFromString<SplineSnapshot[]>(dataAsString);

            //Should be replaced with more sophisticated load & save & validation logic
            if (splines.Length != snapshots.Length)
            {
                Debug.LogError("Wrong data in save file");
                return;  
                //throw new NotImplementedException("Wrong data in save file.");
            }

            for(int i = 0; i < snapshots.Length; ++i)
            {
                splines[i].Load(snapshots[i]);
            }
        }

        /// <summary>
        /// NOTE: THIS FUNCTION IS PROVIDED AS AN EXAMPLE AND DOES NOT SAVE ANY UNITY GAMEOBJECTS (ONLY SPLINE DATA).
        /// </summary>
        public virtual void Save()
        {
            SplineBase[] splines = FindObjectsOfType<SplineBase>();
            SplineSnapshot[] snapshots = new SplineSnapshot[splines.Length];
            for (int i = 0; i < snapshots.Length; ++i)
            {
                snapshots[i] = splines[i].Save();
            }
            string dataAsString = SerializeToString(snapshots);
            PlayerPrefs.SetString("SplineEditorSave", dataAsString);
        }


        private static TData DeserializeFromString<TData>(string settings)
        {
            byte[] b = Convert.FromBase64String(settings);
            using (var stream = new MemoryStream(b))
            {
                SurrogateSelector ss = new SurrogateSelector();
                Vector3SerializationSurrogate v3ss = new Vector3SerializationSurrogate();
                ss.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), v3ss);

                var formatter = new BinaryFormatter();
                formatter.SurrogateSelector = ss;
                stream.Seek(0, SeekOrigin.Begin);
                return (TData)formatter.Deserialize(stream);
            }
        }

        private static string SerializeToString<TData>(TData settings)
        {
            using (var stream = new MemoryStream())
            {
                SurrogateSelector ss = new SurrogateSelector();
                Vector3SerializationSurrogate v3ss = new Vector3SerializationSurrogate();
                ss.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), v3ss);

                var formatter = new BinaryFormatter();
                formatter.SurrogateSelector = ss;
                formatter.Serialize(stream, settings);
                stream.Flush();
                stream.Position = 0;
                return Convert.ToBase64String(stream.ToArray());
            }
        }
    }

}
