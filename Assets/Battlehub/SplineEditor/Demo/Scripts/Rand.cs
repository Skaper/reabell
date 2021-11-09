using UnityEngine;

namespace Battlehub.SplineEditor
{
    public class Rand : MonoBehaviour
    {
        

        public void OnFork(ForkEventArgs args)
        {
            
            int r = Random.Range(args.NextCurveIndex == -1 ? 0 : -1, args.Branches.Length);
            args.SelectBranchIndex = r;
        }
    }
}

