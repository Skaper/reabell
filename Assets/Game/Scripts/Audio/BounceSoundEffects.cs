using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(VolumeControl))]
public class BounceSoundEffects : MonoBehaviour
{
    public AudioClip[] dropClips;
    public float delayBetweenSounds = 0.02f;

    public string[] ignoreTags = { "Player", "Player/GravityCollider", "Player/PlayerInSpace"};

    private float timer;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!ignoreTags.Contains(collision.gameObject.tag) && Time.timeSinceLevelLoad > 2f)
        {
            if (timer > delayBetweenSounds)
            {
                playSound();
                timer = 0f;
            }
        }
    }

    private void playSound()
    {
        if (dropClips.Length == 0) return;
        audioSource.clip = dropClips[Random.Range(0, dropClips.Length)];
        audioSource.Play();
    }
}
