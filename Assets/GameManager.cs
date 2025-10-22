using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;

    public float playTime = 0f;
    public int playerHp;

    public bool isGameOver = false;

    void Awake()
    {
        if(gameManager != null && gameManager != this)
        {
            Destroy(gameObject);
            return;
        }
        gameManager = this;


        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (isGameOver == true) return;

        playTime += Time.deltaTime;  


    }
}
