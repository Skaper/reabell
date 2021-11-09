using UnityEngine;
using BNG;
public class MenuSystem : MonoBehaviour
{
    public bool isMenuActive;
    public GameObject mainCanvas;

    public Transform parent;

    [SerializeField]
    private bool moveToPlayerMenuPoint = true;
    [SerializeField]
    private bool moveToPlayerMenuPointOnStart = false;

    private InputBridge input;

   
    private void Awake()
    {
        GameManager.OpenMenu += menuShower;
        GameManager.MenuSystem = this;
    }
    void Start()
    {
        
        if (moveToPlayerMenuPointOnStart)
        {
            transform.position = GameManager.PlayerMenuPoint.transform.position;
        }
        if (parent != null) transform.SetParent(parent);
        mainCanvas.SetActive(isMenuActive);


    }

    private void Update()
    {
        
    }

    private void menuShower(Transform _transform)
    {

        isMenuActive = !isMenuActive;
        mainCanvas.SetActive(isMenuActive);
        if (moveToPlayerMenuPoint)
        {
            transform.position = _transform.position;
            
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position) 
                * Quaternion.Euler(0f, 0f,  GameManager.Player.transform.eulerAngles.z);
        }
    }

    private void OnDestroy()
    {
        GameManager.OpenMenu -= menuShower;
    }

}
