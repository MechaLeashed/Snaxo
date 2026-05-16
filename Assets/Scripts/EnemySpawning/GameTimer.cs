using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float timeRemaining = 60f;
    public TMP_Text timerText;
    public string sceneToLoad;

    void Update()
    {
        timeRemaining -= Time.deltaTime;

        if (timerText != null)
        {
            timerText.text = Mathf.Ceil(timeRemaining).ToString("0");
        }

        if (timeRemaining <= 0f)
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}