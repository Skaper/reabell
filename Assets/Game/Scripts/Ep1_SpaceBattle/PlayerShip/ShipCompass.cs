using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ShipCompass : MonoBehaviour
{
    private Transform targetPointer;
    private TextMeshPro targetDistanceText;

    private Transform target;
    [HideInInspector]
    public float radius;

    
    void Start()
    {
        radius = GetComponent<SphereCollider>().radius / 3.25f;
        GameManager.QuestManagerEp1.onActionChangeTarget += ChangeTarget;

        targetPointer = transform.Find("TargetPointer").transform;
        targetDistanceText = targetPointer.Find("DistanceText").GetComponent<TextMeshPro>();

        

        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(target != null)
        {
            if (!targetPointer.gameObject.active) targetPointer.gameObject.SetActive(true);
            float dist = Vector3.Distance(transform.position, target.position);
            Vector3 targetVector = (target.position - transform.position).normalized * radius + transform.position;
            if (dist >= 5000)
            {
                targetDistanceText.text = "5000+";
            }
            else
            {
                targetDistanceText.text = ((int)dist).ToString();
            }

            targetPointer.position = targetVector;
        }
        else
        {
            if(targetPointer.gameObject.active) targetPointer.gameObject.SetActive(false);
        }
        

        
        
        
    }

    private void ChangeTarget(Transform t)
    {
        target = t;
    }

    

}
