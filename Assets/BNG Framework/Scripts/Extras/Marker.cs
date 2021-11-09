using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {
    public class Marker : MonoBehaviour {

        public Material DrawMaterial;
        public Color DrawColor = Color.red;
        public float LineWidth = 0.02f;

        public Transform RaycastStart;
        public LayerMask DrawingLayers;

        public float RaycastLength = 0.01f;

        /// <summary>
        /// Minimum distance required from points to place drawing down
        /// </summary>
        public float MinDrawDistance = 0.02f;

        Transform lastDrawPoint;

        // Use this to store our Marker's LineRenderers
        Transform root;

        void Update() {
            RaycastHit hit;
            if(Physics.Raycast(RaycastStart.position, RaycastStart.up, out hit, RaycastLength, DrawingLayers, QueryTriggerInteraction.Ignore)) {                

                float tipDistance = Vector3.Distance(hit.point, RaycastStart.transform.position);
                float tipDercentage = tipDistance / RaycastLength;
                Vector3 drawStart = hit.point + (-RaycastStart.up * 0.0001f);
                Quaternion drawRotation = Quaternion.FromToRotation(Vector3.back, hit.normal);

                float lineWidth = LineWidth * (1 - tipDercentage);

                InitDraw(drawStart, drawRotation, lineWidth, DrawColor);
            }
            else {
                // No longer drawing, disconnect point
                lastDrawPoint = null;
            }
        }

        public virtual Transform InitDraw(Vector3 position, Quaternion rotation, float lineWidth, Color lineColor) {
            
            // Fresh draw
            if (lastDrawPoint == null) {
                lastDrawPoint = new GameObject().transform;
                lastDrawPoint.position = position;
            }
            // Already started drawing, connect lines
            else {
                float dist = Vector3.Distance(lastDrawPoint.position, position);
                if (dist > MinDrawDistance) {
                    lastDrawPoint = DrawPoint(lastDrawPoint, position, lineWidth, DrawColor);
                    lastDrawPoint.rotation = rotation;
                }
            }

            return DrawPoint(lastDrawPoint, position, lineWidth, lineColor);
        }

        public virtual Transform DrawPoint(Transform lastDrawPoint, Vector3 endPosition, float lineWidth, Color lineColor) {
            if (lastDrawPoint) {

                Transform newPoint = new GameObject().transform;
                newPoint.name = "DrawLine";

                // Make sure we have a root object to store our lines in
                if(root == null) {
                    root = new GameObject().transform;
                    root.name = "MarkerLineHolder";
                }

                newPoint.parent = root;
                newPoint.position = endPosition;

                LineRenderer lr = newPoint.gameObject.AddComponent<LineRenderer>();
                lr.startColor = lineColor;
                lr.endColor = lineColor;
                lr.startWidth = lineWidth;
                lr.endWidth = lineWidth;
                if (DrawMaterial) {
                    lr.material = DrawMaterial;
                }
                lr.numCapVertices = 5;
                lr.alignment = LineAlignment.TransformZ;

                lr.useWorldSpace = true;

                lr.SetPosition(0, lastDrawPoint.position);
                lr.SetPosition(1, endPosition);                

                return newPoint;
            }

            return null;
        }

        void OnDrawGizmosSelected() {
            // Show Grip Point
            Gizmos.color = Color.green;
            Gizmos.DrawLine(RaycastStart.position, RaycastStart.position + RaycastStart.up * RaycastLength);
        }
    }
}

