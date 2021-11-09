using UnityEngine;


namespace Battlehub.SplineEditor
{
    public class PaperplaneSpawn : MonoBehaviour
    {
        public float Interval = 2.0f;
        private float m_timeElapsed;

        public SmoothFollow SmoothFollow;
        public GameObject PaperplanePrefab;
        private SplineBase m_spline;
        public string SplineName = "Spline";
        private void Start()
        {
            if(m_spline == null)
            {
                GameObject go = GameObject.Find(SplineName);
                if(go != null)
                {
                    m_spline = go.GetComponent<SplineBase>();
                }

                if(m_spline == null)
                {
                    Debug.LogError("Unable to find spline " + m_spline);
                    enabled = false;
                    return;
                }
               
            }
            Spawn();
        }


        private void Update()
        {
            m_timeElapsed += Time.deltaTime;
            if (m_timeElapsed >= Interval)
            {
                Spawn();
                m_timeElapsed = 0;
            }
        }

        private void Spawn()
        {
            int index = 0;
            int nextIndex = index + 1;
            Twist twist = m_spline.GetTwist(index);
            Vector3 ptPrev = m_spline.GetControlPoint(index);
            Vector3 pt = m_spline.GetControlPoint(nextIndex);
            GameObject paperplaneGo = (GameObject)Instantiate(PaperplanePrefab, m_spline.GetPoint(0.0f), Quaternion.AngleAxis(twist.Data, pt - ptPrev) * Quaternion.LookRotation(pt - ptPrev));
            SplineFollow splineFollow = paperplaneGo.GetComponent<SplineFollow>();
            splineFollow.Spline = m_spline;

            if(!SmoothFollow.enabled)
            {
                SmoothFollow.SetTarget(paperplaneGo.transform);
                SmoothFollow.enabled = true;
            }
            
        }
    }
}

