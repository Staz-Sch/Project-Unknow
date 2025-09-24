using System.Collections;
using UnityEngine;

public class LoadingScreen_Controller : MonoBehaviour
{
    public float FadeSpeed = 2f;

    private CanvasGroup _canvasGroup;

    private void OnEnable()
    {
        SceneLoader.OnSceneLoaded += FadeOut;
    }

    private void OnDisable()
    {
        SceneLoader.OnSceneLoaded -= FadeOut;
    }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f; // start hidden
    }

    public void FadeOut(SceneReference scene)
    {
        StartCoroutine(FadeRuntime(0f));
    }

    public IEnumerator FadeRuntime(float targetAlpha)
    {
        gameObject.SetActive(true);                
        _canvasGroup.blocksRaycasts = true;        

        while (Mathf.Abs(_canvasGroup.alpha - targetAlpha) > 0.01f)
        {
            _canvasGroup.alpha = Mathf.MoveTowards(
                _canvasGroup.alpha,
                targetAlpha,
                FadeSpeed * Time.deltaTime
            );
            yield return null;
        }

        _canvasGroup.alpha = targetAlpha;

        // If we faded out fully, hide the screen
        if (Mathf.Approximately(targetAlpha, 0f))
        {
            _canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        }
    }
}
