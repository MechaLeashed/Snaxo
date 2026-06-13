using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float timeRemaining = 60f;
    public TMP_Text timerText;
    public string sceneToLoad;

    [Header("Transition UI")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.0f;

    [Header("Transition Audio")]
    [SerializeField] private AudioSource currentAudioSource;
    [SerializeField] private AudioClip nextSceneMusic;

    private bool isTransitioning = false; 

    void Update()
    {
        if (!isTransitioning)
        {
            timeRemaining -= Time.deltaTime;

            if (timerText != null)
            {
                timerText.text = Mathf.Ceil(Mathf.Max(0f, timeRemaining)).ToString("0");
            }

            // Lose Condition: Time runs out
            if (timeRemaining <= 0f)
            {
                StartSceneTransition();
            }
        }
    }

    // Made public so your Inventory script can trigger it when the player wins
    public void StartSceneTransition()
    {
        if (isTransitioning) return; // Prevent multiple triggers
        isTransitioning = true;
        StartCoroutine(FadeAndLoad());
    }

    private IEnumerator FadeAndLoad()
    {
        if (fadeImage != null) fadeImage.raycastTarget = true;

        float elapsedTime = 0f;
        float startVolume = currentAudioSource != null ? currentAudioSource.volume : 1f;
        Color color = fadeImage != null ? fadeImage.color : Color.black;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeDuration;

            if (fadeImage != null)
            {
                color.a = Mathf.Clamp01(progress);
                fadeImage.color = color;
            }

            if (currentAudioSource != null)
            {
                currentAudioSource.volume = Mathf.Lerp(startVolume, 0f, progress);
            }

            yield return null;
        }

        if (fadeImage != null)
        {
            color.a = 1f;
            fadeImage.color = color;
        }
        
        if (currentAudioSource != null)
        {
            currentAudioSource.volume = 0f;
            currentAudioSource.Stop();
        }

        SceneManager.sceneLoaded += OnNextSceneLoaded;
        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnNextSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnNextSceneLoaded;
        AudioSource nextSceneSource = GameObject.FindObjectOfType<AudioSource>();

        if (nextSceneSource != null && nextSceneMusic != null)
        {
            nextSceneSource.clip = nextSceneMusic;
            nextSceneSource.volume = 1.0f;
            nextSceneSource.loop = true;
            nextSceneSource.Play();
        }
    }
}