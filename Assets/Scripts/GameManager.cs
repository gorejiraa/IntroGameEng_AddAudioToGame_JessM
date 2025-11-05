using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public SFXManager sfxManager;
    public PlayerController playerController;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverScoreText;
    public int score  = 0;

    public TextMeshProUGUI shieldText;
    public int shield = 3;   

    [Header ("UI_Panels")]
    public GameObject MainMenuUI;
    public GameObject GameplayUI;
    public GameObject PausedMenuUI;
    public GameObject GameOverUI;

    private GameObject asteroidSpawner;

    private bool gameOver;
    private enum GameState { MainMenu, Gameplay, GameOver, Paused }

    private GameState gameState;
    //private GameState LastgameState;


    // Specifies this script and all its children as a singleton, Awake function below deletes any extra copies of this onbject so that there exists only a "Single" instance of itself
    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);  // will not destroy this object when changing scenes           
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        sfxManager.BGMusicMainMenu();        
    }


    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();
        shieldText.text = shield.ToString();

        if (shield < 0)
        {  
            gameOver = true;            
        }        

        switch (gameState)
        {
            case GameState.MainMenu:                

                MainMenuUI.SetActive(true);
                GameplayUI.SetActive(false);
                PausedMenuUI.SetActive(false);
                GameOverUI.SetActive(false);                
                break;

            case GameState.Gameplay:                
                MainMenuUI.SetActive(false);
                GameplayUI.SetActive(true);
                PausedMenuUI.SetActive(false);
                GameOverUI.SetActive(false);

                if (Input.GetKeyDown(KeyCode.Escape))
                {                    
                    gameState = GameState.Paused;
                }

                if (gameOver == true)
                {
                    playerController.PlayerDestroy();
                    gameState = GameState.GameOver;
                    gameOverScoreText.text = score.ToString();
                    asteroidSpawner = GameObject.Find("AsteroidSpawner");
                    asteroidSpawner.SetActive(false);                    
                }
                break;

            case GameState.GameOver:               

                MainMenuUI.SetActive(false);
                GameplayUI.SetActive(false);
                PausedMenuUI.SetActive(false);
                GameOverUI.SetActive(true);
                //ayerDestroy();
                break;


            case GameState.Paused:
                Time.timeScale = 0f;
                MainMenuUI.SetActive(false);
                GameplayUI.SetActive(false);
                PausedMenuUI.SetActive(true);
                GameOverUI.SetActive(false);

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    gameState = GameState.Gameplay;
                    Time.timeScale = 1f;
                }
                break;
        }
    }

    


    public void StartGame()
    {
        gameState = GameState.Gameplay;
        shield = 3;
        score = 0;
        sfxManager.BGMusicGameplay();
        SceneManager.LoadScene("Gameplay");
        player.SetActive(true);        
        gameOver = false;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        sfxManager.BGMusicMainMenu();
        gameState = GameState.MainMenu;
        gameOver = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    
}
