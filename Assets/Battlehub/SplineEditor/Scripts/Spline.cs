using UnityEngine;

namespace Battlehub.SplineEditor
{
    [ExecuteInEditMode]
    public class Spline : SplineBase
    {
        private const float Mag = 5.0f;

        /// <summary>
        /// Append curve
        /// </summary>
        public void Append()
        {
            AppendCurve(Mag, false);
            #if UNITY_EDITOR
            TrackVersion();
            #endif
        }

        public void AppendThorugh(Transform t)
        {
            int pointsCount = 3;
            Vector3[] points = new Vector3[pointsCount];
            AlignWithEnding(points, CurveCount - 1, Mag);

            Vector3 from = GetPointLocal(1.0f);
            Vector3 pt = transform.InverseTransformPoint(t.position - t.forward);
            points[2] = pt;
            points[1] = pt - transform.InverseTransformVector(t.forward).normalized * (pt - from).magnitude * (1.0f / 3.0f);
            points[0] = pt - transform.InverseTransformVector(t.forward).normalized * (pt - from).magnitude * (2.0f / 3.0f);

            AppendCurve(points, false);
            #if UNITY_EDITOR
            TrackVersion();
            #endif
        }

        /// <summary>
        /// Insert curve
        /// </summary>
        /// <param name="curveIndex">[0, CurveCount]</param>
        public void Insert(int curveIndex)
        {
            PrependCurve(Mag, curveIndex, false, true);
            #if UNITY_EDITOR
            TrackVersion();
            #endif
        }

        /// <summary>
        /// Prepend curve
        /// </summary>
        public void Prepend()
        {
            if (!Loop)
            {
                const int curveIndex = 0;
                PrependCurve(Mag, curveIndex, false, false);
            }
            else
            {
                AppendCurve(Mag, false);
            }
            #if UNITY_EDITOR
            TrackVersion();
            #endif
        }

        public void PrependThrough(Transform t)
        {
            if (!Loop)
            {
                const int curveIndex = 0;
                const int pointsCount = 3;
                Vector3[] points = new Vector3[pointsCount];
                AlignWithBeginning(points, curveIndex, Mag);

                Vector3 from = GetPointLocal(0.0f);
                Vector3 pt = transform.InverseTransformPoint(t.position + t.forward);
                points[0] = pt;
                points[1] = pt + transform.InverseTransformVector(t.forward).normalized * (pt - from).magnitude * (1.0f / 3.0f);
                points[2] = pt + transform.InverseTransformVector(t.forward).normalized * (pt - from).magnitude * (2.0f / 3.0f);

                PrependCurve(points, curveIndex, Mag, false, false);
            }
            else
            {
                AppendThorugh(t);
            }
            #if UNITY_EDITOR
            TrackVersion();
            #endif
        }

        /// <summary>
        /// Remove by curveIndex
        /// </summary>
        /// <param name="curveIndex">[0, CurveCount - 1]</param>
        /// <returns></returns>
        public bool Remove(int curveIndex)
        {
            bool result = RemoveCurve(curveIndex);
            
            #if UNITY_EDITOR
            TrackVersion();
            #endif

            return result;
        }

        /// <summary>
        /// Load from snapshot
        /// </summary>
        /// <param name="snapshot">snapshot</param>
        public override void Load(SplineSnapshot snapshot)
        {
            LoadSpline(snapshot);

            #if UNITY_EDITOR
            TrackVersion();
            #endif
        }

        protected override void OnCurveChanged()
        {
            #if UNITY_EDITOR
            TrackVersion();
            #endif
        }

        #if UNITY_EDITOR
        protected override void AwakeOverride()
        {
            TrackVersion();
        }
        #endif

        protected override float GetMag()
        {
            return Mag;
        }

        private void AppendCurve(float mag, bool enforceNeighbour)
        {
            int pointsCount = 3;
            Vector3[] points = new Vector3[pointsCount];
            AlignWithEnding(points, CurveCount - 1, mag);
            AppendCurve(points, enforceNeighbour);
        }

        private void PrependCurve(float mag, int curveIndex, bool enforceNeighbour, bool shrinkPreceding)
        {
            const int pointsCount = 3;
            Vector3[] points = new Vector3[pointsCount];
            if (!shrinkPreceding)
            {
                AlignWithBeginning(points, curveIndex, mag);
            }

            PrependCurve(points, curveIndex, mag, enforceNeighbour, shrinkPreceding);
        }

        #if UNITY_EDITOR
        private void TrackVersion()
        {
            PersistentVersions[0]++;
            OnVersionChanged();
        }
        #endif

    }
}

