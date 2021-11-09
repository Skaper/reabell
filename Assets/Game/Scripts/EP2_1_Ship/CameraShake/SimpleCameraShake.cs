using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SimpleCameraShake : MonoBehaviour
{
    public float magnitudeTarget = 1;
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0f;
        Debug.Log("Shake");
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }

    float timer;
    private void Start()
    {
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > 5f)
        {
            EZCameraShake.CameraShaker.Instance.ShakeOnce(4f, 4f, 1f, 1f);
            //StartCoroutine(Shake(2f, magnitudeTarget));
            timer = 0f;
        }
    }
}
