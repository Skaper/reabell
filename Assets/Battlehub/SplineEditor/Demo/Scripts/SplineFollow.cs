using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Battlehub.SplineEditor
{
    [Serializable]
    public class ForkEventArgs
    {
        private SplineBase[] m_branches;
        public SplineBase[] Branches
        {
            get { return m_branches; }
        }

        private SplineBase m_spline;
        public SplineBase Spline
        {
            get { return m_spline; }
        }

        private int m_nextCurveIndex;

        /// <summary>
        /// -1 if end of the spline reached, otherwise [0, Spline.CurveCount - 1]
        /// </summary>
        public int NextCurveIndex
        {
            get { return m_nextCurveIndex; }
        }


        /// <summary>
        /// -1 will force SplineFollow not to choose any branch. (To choose branch set SelectedBranchIndex to [0, Branches.Length - 1])
        /// </summary>
        public int SelectBranchIndex
        {
            get;
            set;
        }

        public ForkEventArgs(SplineBase spline, int pointIndex)
        {
            m_spline = spline;
            m_nextCurveIndex = pointIndex / 3;  
            SplineBranch[] branches = spline.GetBranches(pointIndex);
            if (branches == null || branches.Length == 0)
            {
                m_branches = new SplineBase[0];
            }
            else
            {
                List<SplineBase> branchList = new List<SplineBase>();
                for (int i = 0; i < branches.Length; ++i)
                {
                    SplineBranch branch = branches[i];
                    if(!branch.Inbound)
                    {
                        branchList.Add(spline.BranchToSpline(branch));
                    }
                }
                m_branches = branchList.ToArray();
            }

            if (m_nextCurveIndex >= spline.CurveCount)
            {
                if(m_branches.Length > 0)
                {
                    SelectBranchIndex = 0;
                }
                SelectBranchIndex = -1;
                m_nextCurveIndex = -1;
            }
            else
            {
                SelectBranchIndex = -1;
            }
        }
    }

    [Serializable]
    public class ForkEvent : UnityEvent<ForkEventArgs> { }

    public class SplineFollow : MonoBehaviour
    {
        public float Speed = 5.0f;
        public SplineBase Spline;
        public float Offset;
        public bool IsRunning = true;
        public bool IsLoop = false;
        public ForkEvent Fork;
        public UnityEvent Completed;

        private SplineBase m_spline;
        private bool m_isRunning;
        private bool m_isCompleted;

        private float m_t;
        private int m_curveIndex;

        private void Start()
        {
            if(!Spline)
            {
                Debug.LogError("Set Spline Field!");
                enabled = false;
                return;
            }
            m_isCompleted = true;
        }

        private void Update()
        {
            if(IsRunning != m_isRunning)
            {
                if(m_isCompleted)
                {
                    Restart();
                }
                m_isRunning = IsRunning;
            }

            if(IsRunning)
            {
                Move();
            }
            
        }

        private void Restart()
        {
            m_spline = Spline;
            m_t = Offset % 1;
            m_curveIndex = Spline.ToCurveIndex(m_t);
            m_isCompleted = false;
            IsRunning = true;
        }

        private void Move()
        {
            int curveIndex = m_spline.ToCurveIndex(m_t);
            if (m_curveIndex != curveIndex || m_t >= 1.0f)
            {
                CheckBranches(curveIndex);
            }

            float t = m_t;
            UpdatePosition(t);

            float v = m_spline.GetVelocity(t).magnitude;
            v *= m_spline.CurveCount;
            if (m_t >= 1.0f)
            {
                if (m_spline.NextSpline != null)
                {
                    int nextControlPointIndex = m_spline.NextControlPointIndex;
                    m_curveIndex = nextControlPointIndex / 3;
                    m_spline = m_spline.NextSpline;

                    if(m_spline.NextControlPointIndex > 0)
                    {
                        m_t = ((float)m_curveIndex) / m_spline.CurveCount;
                        m_curveIndex++;
                    }
                    else
                    {
                        m_t = ((float)m_curveIndex) / m_spline.CurveCount;
                    }
                    Debug.Log("Next Spline " + m_curveIndex);
                    CheckBranches(m_curveIndex);    
                }
                else
                {
                    m_t = (m_t - 1.0f) + (Time.deltaTime * Speed) / v;
                    if (!m_spline.Loop && !IsLoop)
                    {
                        m_t = 1.0f;
                        m_isCompleted = true;
                        IsRunning = false;
                        m_isRunning = false;
                        Completed.Invoke();
                    }

                    if (IsLoop)
                    {
                        if (m_spline != Spline)
                        {
                            Restart();
                        }
                    }
                }
            }
            else
            {
                m_t += (Time.deltaTime * Speed) / v;
            }
        }

        private void CheckBranches(int curveIndex)
        {
            int pointIndex = curveIndex * 3;
            if (m_t >= 1.0f)
            {
                pointIndex += 3;
            }
            m_curveIndex = curveIndex;
            if (m_spline.HasBranches(pointIndex))
            {
                ForkEventArgs args = new ForkEventArgs(m_spline, pointIndex);
                Fork.Invoke(args);
                if (args.SelectBranchIndex > -1 && args.SelectBranchIndex < args.Branches.Length)
                {
                    Debug.Log("CurveIndex " + m_curveIndex);
                    Debug.Log("Selected Branch " + args.SelectBranchIndex);
                    m_spline = args.Branches[args.SelectBranchIndex];
                    m_t = 0.0f;
                    m_curveIndex = 0;
                }
            }
        }

        private void UpdatePosition(float t)
        {
            Vector3 position = m_spline.GetPoint(t);
            Vector3 dir = m_spline.GetDirection(t);
            float twist = m_spline.GetTwist(t);

            transform.position = position;
            transform.LookAt(position + dir);
            transform.RotateAround(position, dir, twist);
        }
    }

    //public class SplineFollow : MonoBehaviour
    //{
    //    public float Speed = 1.0f;
    //    public float Duration = 15.0f;
    //    public SplineBase Spline;
    //    public float Offset;
    //    private float m_t;

    //    private float Wrap(float t)
    //    {
    //        return (Duration + t % Duration) % Duration;
    //    }

    //    private void Update()
    //    {
    //        Move();
    //    }

    //    private void Move()
    //    {
    //        float t = Wrap(m_t + Offset * Duration / 50.0f);
    //        float v = Spline.GetVelocity(t / Duration).magnitude / 5.0f;

    //        if (m_t >= Duration)
    //        {
    //            m_t = (m_t - Duration) + Time.deltaTime / v * Speed;

    //        }
    //        else
    //        {
    //            m_t += Time.deltaTime / v * Speed;
    //        }

    //        Vector3 position = Spline.GetPoint(t / Duration);
    //        Vector3 dir = Spline.GetDirection(t / Duration);
    //        float twist = Spline.GetTwist(t / Duration);

    //        transform.position = position;
    //        transform.LookAt(position + dir);
    //        transform.RotateAround(position, dir, twist);
    //    }

    //}

}
