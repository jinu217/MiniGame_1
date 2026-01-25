using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void OnClickStartGame()
    {
        PlayerPrefs.SetInt("ShowStoryOnStage1", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Stage1");
    }
}
