using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelChanger : MonoBehaviour
{
    public Animator animator;

    private string level;

    public bool changeLevel = true;

    public bool fadeInOnStart = false;
    // Update is called once per frame
    private void Start()
    {
        if (fadeInOnStart) Fade();
    }

    public void Fade()
    {
        animator.SetTrigger("FadeIn");
    }

    public void FadeOut()
    {
        animator.SetTrigger("FadeOut");
    }

    public void FadeToLevel(string levelName)
    {
        animator.SetTrigger("FadeOut");
        level = levelName;
    }

    public void OnFadeComplite()
    {
        if(changeLevel) SceneManager.LoadScene(level);
    }
}
