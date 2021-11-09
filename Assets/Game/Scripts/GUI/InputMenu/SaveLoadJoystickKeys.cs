using UnityEngine;

public class SaveLoadJoystickKeys : MonoBehaviour
{
    //public GameObject inputMenu;

    public void Save()
    {
        InputManager.OnActionSaveAllKeys?.Invoke();
        //inputMenu.SetActive(false);
    }
    public void Close()
    {
        InputManager.OnActionReloadAllKeys?.Invoke();
    }
}
