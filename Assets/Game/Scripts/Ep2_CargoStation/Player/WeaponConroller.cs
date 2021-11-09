using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponConroller : MonoBehaviour
{

    public GameObject[] weapons;
    public int defaultWeponID = 0;

    private int currentWepon;
    private int lastWeapon;
    void Start()
    {
        if (weapons.Length == 0) return;
        if(defaultWeponID >= weapons.Length)
        {
            defaultWeponID = weapons.Length - 1;
        }else if(defaultWeponID < 0)
        {
            defaultWeponID = 0;
        }
        currentWepon = defaultWeponID;
        lastWeapon = currentWepon;
        foreach (GameObject go in weapons)
        {
            go.SetActive(false);
        }
        weapons[defaultWeponID].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(InputManager.Next))
        {
            currentWepon += 1;
        }
        if (Input.GetKeyDown(InputManager.Previous))
        {
            currentWepon -= 1;
        }

        if(currentWepon < 0)
        {
            currentWepon = weapons.Length - 1;
        }
        else if(currentWepon >= weapons.Length)
        {
            currentWepon = 0;
        }

        if(currentWepon != lastWeapon)
        {
            GravityGun gravityGun = weapons[lastWeapon].GetComponent<GravityGun>();
            if (gravityGun != null)
            {
                gravityGun.DropObj();
            }
            changeWeapon();
        }
    }

    private void changeWeapon()
    {
        weapons[lastWeapon].SetActive(false);
        weapons[currentWepon].SetActive(true);
        lastWeapon = currentWepon;
    }
}
