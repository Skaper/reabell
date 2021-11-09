using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector;

public class EnemyShipAIHealth : vHealthController
{
    [vEditorToolbar("Settings", order = 200)]
    public ParticleSystem explosionFX;
    public Transform particleParent;
    public MeshRenderer[] renderers;

    private void Start()
    {
        this.onDead.AddListener((GameObject g) => StartCoroutine(ShipDeath()));
        
    }
    void Update()
    {


    }

	IEnumerator ShipDeath()
	{
        this.onDead.RemoveAllListeners();
        Debug.Log("ShipDeath");
		GetComponent<Renderer>().enabled = false;
		foreach (MeshRenderer ren in renderers)
		{
			ren.enabled = false;
		}
		
        if (!explosionFX.isPlaying) explosionFX.Play();
        while (explosionFX.isPlaying)
        {
            yield return null;
        }
        Destroy(gameObject);
    }

	
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Bullet"))
        {
            SimpleBullet sBullet = other.gameObject.GetComponent<SimpleBullet>();
            if (tag.Contains(sBullet.creatorTag)) return;
            currentHealth -= sBullet.damage;
                
        }
    }

}