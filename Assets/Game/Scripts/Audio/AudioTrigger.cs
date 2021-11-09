using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public AudioClip clip;
    public string targetTag = "Player";
    private bool hasPlayed = false;
    public AudioClip Clip {
        get
        {
            return clip;
        }
        set
        {
            clip = value;
        }
    }
    public GameObject audioEffector = null;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(targetTag) && !hasPlayed)
        {
            GameObject soundEffector = Instantiate(audioEffector, transform.position, transform.rotation);
            AudioSource _audioSource = soundEffector.GetComponent<AudioSource>();
            _audioSource.clip = clip;
            _audioSource.Play();
            hasPlayed = true;
        }
        
    }
}
