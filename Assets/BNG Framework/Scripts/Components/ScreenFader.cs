using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BNG {
    public class ScreenFader : MonoBehaviour {

        [Tooltip("Should the screen fade in when a new level is loaded")]
        public bool FadeOnSceneLoaded = true;

        [Tooltip("Color of the fade. Alpha will be modified when fading in / out")]
        public Color FadeColor = Color.black;

        [Tooltip("How fast to fade in / out")]
        public float FadeSpeed = 6f;

        GameObject fadeObject;
        RectTransform fadeObjectRect;
        Canvas fadeCanvas;
        CanvasGroup canvasGroup;
        Image fadeImage;
        IEnumerator fadeRoutine;

        protected virtual void initialize() {
            // Create a Canvas that will be placed directly over the camera
            if(fadeObject == null) {
                fadeObject = new GameObject();
                fadeObject.transform.parent = Camera.main.transform;
                fadeObject.transform.localPosition = new Vector3(0, 0, 0.011f);
                fadeObject.transform.name = "ScreenFader";

                fadeCanvas = fadeObject.AddComponent<Canvas>();
                fadeCanvas.renderMode = RenderMode.WorldSpace;

                canvasGroup = fadeObject.AddComponent<CanvasGroup>();

                fadeImage = fadeObject.AddComponent<Image>();
                fadeImage.color = FadeColor;
                fadeImage.raycastTarget = false;

                // Stretch the image
                fadeObjectRect = fadeObject.GetComponent<RectTransform>();
                fadeObjectRect.anchorMin = new Vector2(1, 0);
                fadeObjectRect.anchorMax = new Vector2(0, 1);
                fadeObjectRect.pivot = new Vector2(0.5f, 0.5f);
                fadeObjectRect.sizeDelta = new Vector2(0.1f, 0.1f);
            }
        }

        void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            initialize();

            if (FadeOnSceneLoaded) {
                // Start screen at fade
                updateImageAlpha(FadeColor.a);
                DoFadeOut();
            }
        }

        /// <summary>
        /// Fade from transparent to solid color
        /// </summary>
        public virtual void DoFadeIn() {

            // Stop if currently running
            if(fadeRoutine != null) {
                StopCoroutine(fadeRoutine);
            }

            fadeRoutine = doFade(canvasGroup.alpha, 1);
            StartCoroutine(fadeRoutine);
        }

        /// <summary>
        /// Fade from solid color to transparent
        /// </summary>
        public virtual void DoFadeOut() {
            if (fadeRoutine != null) {
                StopCoroutine(fadeRoutine);
            }

            fadeRoutine = doFade(canvasGroup.alpha, 0);
            StartCoroutine(fadeRoutine);
        }

        public virtual void SetFadeLevel(float fadeLevel) {
            if (fadeRoutine != null) {
                StopCoroutine(fadeRoutine);
            }

            fadeRoutine = doFade(canvasGroup.alpha, fadeLevel);
            StartCoroutine(fadeRoutine);
        }

        IEnumerator doFade(float alphaFrom, float alphaTo) {

            float alpha = alphaFrom;

            updateImageAlpha(alpha);

            while (alpha != alphaTo) {

                if(alphaFrom < alphaTo) {
                    alpha += Time.deltaTime * FadeSpeed;
                    if(alpha > alphaTo) {
                        alpha = alphaTo;
                    }
                }
                else {
                    alpha -= Time.deltaTime * FadeSpeed;
                    if (alpha < alphaTo) {
                        alpha = alphaTo;
                    }
                }

                updateImageAlpha(alpha);

                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForEndOfFrame();
        }

        protected virtual void updateImageAlpha(float alphaValue) {

            // Canvas Group was Destroyed.
            if(canvasGroup == null) {
                return;
            }

            // Enable canvas if necessary
            if(!canvasGroup.gameObject.activeSelf) {
                canvasGroup.gameObject.SetActive(true);
            }

            canvasGroup.alpha = alphaValue;

            // Disable Canvas if we're done
            if (alphaValue == 0 && canvasGroup.gameObject.activeSelf) {
                canvasGroup.gameObject.SetActive(false);
            }
        }
    }
}

