using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Button playButton;
    public Button settingsButton;
    public Button exitButton;

    void Start()
    {
        Time.timeScale = 1f;
    }

    void Update()
    {
        
    }

    public void playGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
