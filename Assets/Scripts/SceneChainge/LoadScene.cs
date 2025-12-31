using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void OnClickStartGame()
    {
        SceneManager.LoadScene("Stage1");
    }
}
