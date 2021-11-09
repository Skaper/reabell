using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHelperTriggerEvent : MonoBehaviour
{
    // Start is called before the first frame update
    public string clipName;
    public bool destroyAfterEnter;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.AIHelper.GetComponent<AIHelper>().Play(clipName);
            if (destroyAfterEnter) Destroy(gameObject);
            
        }
    }
}
