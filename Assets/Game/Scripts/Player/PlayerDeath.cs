using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    public GameObject soundEffect;
    public string deathScene = "MainMenu";
    public GameObject playerCamera;
    public float glitchTime = 3f;
    private GlitchEffect glitchEffect;

    private float glitchDelta;
    private bool isDead;

    private bool isDeathStart;
    private bool canShowNextScene;
    private AsyncOperation nextScene;
    void Start()
    {
        glitchEffect = playerCamera.GetComponent<GlitchEffect>();
        if(glitchEffect == null)
        {
            playerCamera.AddComponent<GlitchEffect>();
        }

    }

    private IEnumerator LoadScene()
    {
        

        nextScene = SceneManager.LoadSceneAsync(deathScene);
        nextScene.allowSceneActivation = false;
        yield return nextScene;

    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            if (!isDeathStart)
            {
                StartCoroutine(Glitch());
                StartCoroutine(LoadScene());
                isDeathStart = true;
                if(soundEffect != null) Instantiate(soundEffect, transform.position, Quaternion.identity).GetComponent<AudioQueue>().Play();
            }
            if (nextScene != null && canShowNextScene)
            {
                nextScene.allowSceneActivation = true;
                Debug.Log("Scene is done");
            }
        }
    }

    public IEnumerator Glitch()
    {
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / glitchTime;
            float value  = Mathf.Lerp(0f, 1f, t);
            glitchEffect.intensity = value;
            glitchEffect.flipIntensity = value;
            glitchEffect.colorIntensity = value;
            yield return null;
        }
        canShowNextScene = true;
    }

    public void CrossSceneDeadInfo(string info)
    {
        CrossSceneInformation.DeadInfo = info;
    }

    public void startDeath()
    {
        isDead = true;
    }
}
