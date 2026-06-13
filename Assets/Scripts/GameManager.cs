using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SimpleSceneTransition : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Image fadeImage;

    [Header("Audio References")]
    [SerializeField] private AudioSource menuAudioSource;
    [SerializeField] private AudioClip gameplayMusic; // The track for the next scene

    [Header("Transition Settings")]
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private string targetSceneName;

    public void StartGameTransition()
    {
        StartCoroutine(FadeAndLoad());
    }

    /// <summary>
    /// Call this function from your Quit Button's OnClick() event.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quit application requested.");

        // If running in the Unity Editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // If running in a standalone build (PC, Mac, Mobile, etc.)
        Application.Quit();
        #endif
    }

    private IEnumerator FadeAndLoad()
    {
        fadeImage.raycastTarget = true;

        float elapsedTime = 0f;
        Color color = fadeImage.color;
        float startVolume = menuAudioSource.volume;

        // 1. Simultaneously fade visual screen to black AND audio track to 0
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeDuration;

            // Visual Fade
            color.a = Mathf.Clamp01(progress);
            fadeImage.color = color;

            // Audio Fade
            menuAudioSource.volume = Mathf.Lerp(startVolume, 0f, progress);

            yield return null;
        }

        // Snap values at the absolute finish line
        color.a = 1f;
        fadeImage.color = color;
        menuAudioSource.volume = 0f;

        // 2. Stop the old music track entirely
        menuAudioSource.Stop();

        // 3. Register a temporary event to swap tracks the exact instant the new scene loads
        SceneManager.sceneLoaded += OnGameplaySceneLoaded;

        // 4. Load the next scene
        SceneManager.LoadScene(targetSceneName);
    }

    // This runs automatically the moment the next scene is ready
    private void OnGameplaySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Unsubscribe immediately so it doesn't accidentally fire again later
        SceneManager.sceneLoaded -= OnGameplaySceneLoaded;

        // Find the main camera or audio listener in the new scene to play the track
        AudioSource gameplaySource = GameObject.FindObjectOfType<AudioSource>();

        if (gameplaySource != null && gameplayMusic != null)
        {
            gameplaySource.clip = gameplayMusic;
            gameplaySource.volume = 1.0f; // Reset target volume back up to full
            gameplaySource.loop = true;
            gameplaySource.Play();
        }
    }
}