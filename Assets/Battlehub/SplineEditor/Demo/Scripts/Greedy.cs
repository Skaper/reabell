using UnityEngine;

namespace Battlehub.SplineEditor
{
    public class Greedy : MonoBehaviour
    {
        public void OnFork(ForkEventArgs args)
        {
            float min = float.MaxValue;
            int minIndex = -1;
            if (args.NextCurveIndex != -1)
            {
                min = args.Spline.EvalCurveLength(args.NextCurveIndex);
            }

            for(int i = 0; i < args.Branches.Length; ++i)
            {
                SplineBase branch = args.Branches[i];
                float len = branch.EvalCurveLength(0);
                if(len < min)
                {
                    min = len;
                    minIndex = i; 
                }
            }

            args.SelectBranchIndex = minIndex;
        }
    }
}

