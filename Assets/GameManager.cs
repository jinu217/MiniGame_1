using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;

    public float playTime = 0f;
    public float playerHp;

    [Header("Player Damage")]
    public int playerBaseDamage = 1;    
    public float damageMultiplier = 1f;

    public int CurrentPlayerDamage
        => Mathf.Max(1, Mathf.RoundToInt(playerBaseDamage * damageMultiplier));

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
