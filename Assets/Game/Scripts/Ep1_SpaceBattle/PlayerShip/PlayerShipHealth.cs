using System.Collections;
using UnityEngine;
using Invector;
using System;

public class PlayerShipHealth : vHealthController
{
    public Action onActionBulletHit;

    private PlayerDeath playerDeath; 

    private void Awake()
    {
        playerDeath = GetComponent<PlayerDeath>();
    }
    private void FixedUpdate()
    {
        if (currentHealth <= 0)
        {
            playerDeath.CrossSceneDeadInfo("The ship was destroyed.");
            playerDeath.startDeath();
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            SimpleBullet sBullet = other.gameObject.GetComponent<SimpleBullet>();
            if (!tag.Contains(sBullet.creatorTag))
            {
                onActionBulletHit?.Invoke();

                currentHealth -= sBullet.damage;
                currentHealthRecoveryDelay = healthRecoveryDelay;
                if (!inHealthRecovery && canRecoverHealth)
                    StartCoroutine(RecoverHealth());
            }
        }
    }
    protected override IEnumerator RecoverHealth()
    {
        inHealthRecovery = true;
        while (canRecoverHealth)
        {
            HealthRecovery();
            yield return new WaitForSeconds(1);
        }
        inHealthRecovery = false;
    }

    protected override void HealthRecovery()
    {
        if (currentHealthRecoveryDelay > 0)
            currentHealthRecoveryDelay -= 1f;
        else
        {
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
            if (currentHealth < maxHealth)
                currentHealth += healthRecovery;
        }
    }

    protected override bool canRecoverHealth
    {
        get
        {
            return currentHealth > 0 && currentHealth < MaxHealth;
        }
    }
}
