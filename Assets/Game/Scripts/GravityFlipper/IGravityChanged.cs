using UnityEngine;

namespace GravityFlipper
{
    public interface IGravityChanged
    {
        void OnGravityChanged(Vector3 gravity);
    }
}