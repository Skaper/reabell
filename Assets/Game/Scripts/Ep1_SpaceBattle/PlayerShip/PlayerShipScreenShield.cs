using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerShipScreenShield : MonoBehaviour
{
    public PlayerShipHealth playerHealth;

    public GameObject shieldProgress;

    private TextMeshProUGUI shieldText;


    public Color dangerouColor;

    private Color normalColor;


    void Start()
    {
        shieldText = shieldProgress.GetComponent<TextMeshProUGUI>();
        normalColor = shieldText.color;
    }

    private void FixedUpdate()
    {
        if (Time.frameCount % 5 == 0)
        {
            float currentPercent = playerHealth.currentHealth / playerHealth.maxHealth * 100;
            shieldText.SetText(Mathf.RoundToInt(currentPercent) + "%");
            if (currentPercent < 30f) shieldText.color = dangerouColor;
            else shieldText.color = normalColor;
        }
    }
}
