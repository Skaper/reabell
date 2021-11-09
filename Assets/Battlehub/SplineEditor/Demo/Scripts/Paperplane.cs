using UnityEngine;
using System.Collections;


namespace Battlehub.SplineEditor
{
    public class Paperplane : MonoBehaviour
    {
        public GameObject ExplosionPrefab;

        public void Destroy()
        {
            Destroy(gameObject);
        }

        private void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit))
                {
                    if(hit.transform == transform)
                    {
                        Explode();
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider collision)
        {
            Explode();
        }

        private void Explode()
        {
            if (ExplosionPrefab != null)
            {
                GameObject explosion = (GameObject)Instantiate(ExplosionPrefab, transform.position, transform.rotation);
                Destroy(explosion, 3.0f);
            }


            Destroy(gameObject);
        }
    }
}

