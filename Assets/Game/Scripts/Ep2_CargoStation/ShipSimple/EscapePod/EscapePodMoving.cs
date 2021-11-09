using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Battlehub.SplineEditor
{
public class EscapePodMoving : MonoBehaviour
{

        public AudioClip enginSound;



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

        private bool isTimerStart = false;
        public float startMovingDelay = 20f;
        private float timer;


        private AudioSource audioSource;
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = enginSound;
            audioSource.loop = true;
            audioSource.playOnAwake = false;

            
            if (!Spline)
            {
                Debug.LogError("Set Spline Field!");
                enabled = false;
                return;
            }
            m_isCompleted = true;
            
        }

        private void Update()
        {
            //if (isTimerStart)
            //{
            //    timer += Time.deltaTime;
            //}
            //if (timer >= startMovingDelay)
            //{
            //    isTimerStart = false;
            //    timer = 0;
            //    IsRunning = true;
            //}


            if (IsRunning != m_isRunning)
            {
                if (m_isCompleted)
                {
                    Restart();
                }
                m_isRunning = IsRunning;
            }

            if (IsRunning)
            {
                Move();
                if (!audioSource.isPlaying) audioSource.Play();
            }
            else
            {
                audioSource.Stop();
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

                    if (m_spline.NextControlPointIndex > 0)
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



}
