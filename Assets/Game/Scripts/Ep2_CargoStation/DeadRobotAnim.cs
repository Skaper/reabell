using UnityEngine;
using Invector.vCharacterController.AI;
using Invector;
public class DeadRobotAnim : MonoBehaviour
{
    public GameObject deadBody;
    private vControlAIShooter aiController;
    
    int count;
    void Start()
    {
        deadBody.SetActive(false);
        aiController = GetComponent<vControlAIShooter>();

    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ship") && count==1)
        {
            deadBody.SetActive(true);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ship"))
        {
            count = 1;
        }
    }
}

