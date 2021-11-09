using UnityEngine;

namespace Battlehub.SplineEditor
{
    public class SmoothFollow : MonoBehaviour
    {
        private bool m_wait;
        // The target we are following
        private Transform target;
        // The distance in the x-z plane to the target
        [SerializeField]
        private float distance = 10.0f;
        // the height we want the camera to be above the target
        [SerializeField]
        private float height = 5.0f;

        [SerializeField]
        private float rotationDamping;
        [SerializeField]
        private float heightDamping;

        // Use this for initialization
        void Start() { }

        public void SetTarget(Transform tr)
        {
            target = tr;
        }


        private System.Collections.IEnumerator ChangeTaget()
        {
            yield return new WaitForSeconds(1);
            m_wait = false;
            if (!target)
            {
                Paperplane paperPlane = FindObjectOfType<Paperplane>();
                if (paperPlane != null)
                {
                    SetTarget(paperPlane.transform);
                }
                else
                {
                    enabled = false;
                }
            }
        }

        void LateUpdate()
        {
            // Early out if we don't have a target
            if (!target)
            {
                if(!m_wait)
                {
                    StartCoroutine(ChangeTaget());
                    m_wait = true;
                }
                
                return;
            }
                

            // Calculate the current rotation angles
            var wantedRotationAngle = target.eulerAngles.y;
            var wantedHeight = target.position.y + height;

            var currentRotationAngle = transform.eulerAngles.y;
            var currentHeight = transform.position.y;

            // Damp the rotation around the y-axis
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

            // Damp the height
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

            // Convert the angle into a rotation
            var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

            // Set the position of the camera on the x-z plane to:
            // distance meters behind the target
            transform.position = target.position;
            transform.position -= currentRotation * Vector3.forward * distance;

            // Set the height of the camera
            transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

            // Always look at the target
            transform.LookAt(target);
        }
    }

}
