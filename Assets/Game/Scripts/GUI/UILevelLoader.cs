using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILevelLoader : MonoBehaviour
{
    public string sceneName = "MainMenu";
    public bool runImmediately = false;

    private AsyncOperation nextScene;
    private bool isStartLoad = false;

    private float loadDelay = 3f;
    private float loadTimer;

    void Start()
    {
        
    }

    void Update()
    {
        if (runImmediately)
        {
            loadTimer += Time.deltaTime;
            if (loadTimer >= loadDelay) AllowNextScene();
        }
    }

    public void LoadNextScene()
    {
        if(!isStartLoad) StartCoroutine(LoadScene());
        isStartLoad = true;
        
    }

    public void AllowNextScene()
    {
        if (isStartLoad)
        {
            if (nextScene != null)
            {
                nextScene.allowSceneActivation = true;
            }
        }
    }

    public float LoadingPercent()
    {
        if (isStartLoad)
        {
            if (nextScene != null)
            {
                return nextScene.progress * 100f;
            }
            else
            {
                return 0f;
            }
        }
        else
        {
            return 0f;
        }
    }

    private IEnumerator LoadScene()
    {
        nextScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        nextScene.allowSceneActivation = false;
        yield return nextScene;

    }
}
