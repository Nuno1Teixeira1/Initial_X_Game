using System;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class GameGUINavigation : MonoBehaviour {

	//------------------------------------------------------------------
	// Variáveis
	
	private bool _paused;
    private bool quit;
    private string _errorMsg;

	public float initialDelay;

	// Canvas
	public Canvas PauseCanvas;
	public Canvas QuitCanvas;
	public Canvas ReadyCanvas;
	public Canvas ScoreCanvas;
    public Canvas ErrorCanvas;
    public Canvas GameOverCanvas;
	
	// Buttons
	public Button MenuButton;

    //------------------------------------------------------------------
    // Funções para as definições

    //para cada função aparecer só quando perder e não ficar sempre a mostrar desde o inicio
    void Start () 
    {
		GameOverCanvas.enabled = false; 
		ScoreCanvas.enabled = false;
		StartCoroutine("ShowReadyScreen", initialDelay);
	}

	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Escape)) 
		{
			// se mostrar o score, vai para o menu
			if(GameManager.gameState == GameManager.GameState.Scores)
				Menu();

			// se ainda estiver a jogar toggle pause ou quit dialogue
			else
			{
				if(quit == true) 
					ToggleQuit(); 
				else
					TogglePause();
			}
		}
	}

    // os handles para fazer pop up de cada screen
	public void H_ShowReadyScreen()
	{
		StartCoroutine("ShowReadyScreen", initialDelay);
	}

    public void H_ShowGameOverScreen()
    {
        StartCoroutine("ShowGameOverScreen");
    }

	IEnumerator ShowReadyScreen(float seconds)
	{
		GameManager.gameState = GameManager.GameState.Init; //inicia o ambiente do jogo
		ReadyCanvas.enabled = true; //inicia ready
		yield return new WaitForSeconds(seconds); //espera
		ReadyCanvas.enabled = false;  //off ready
		GameManager.gameState = GameManager.GameState.Game; //inicia o jogo
	}

    
    IEnumerator ShowGameOverScreen() //same as before
    {
		GameOverCanvas.enabled = true;
        yield return new WaitForSeconds(2);
        Menu();
    }

	public void getScoresMenu() //pop up do menu de scores (high score)
	{
		Time.timeScale = 0f; // pára as animações
		GameManager.gameState = GameManager.GameState.Scores;
		MenuButton.enabled = false;
		ScoreCanvas.enabled = true;
	}

	//------------------------------------------------------------------
	// Funções Button

	public void TogglePause()
	{
		// se já estiver no menu da Pausa, então "Despausa"... KEK
		if(_paused)
		{
			Time.timeScale = 1;
			PauseCanvas.enabled = false;
			_paused = false;
			MenuButton.enabled = true;
		}
		
		// vai para uma pausa
		else
		{
			PauseCanvas.enabled = true;
			Time.timeScale = 0.0f;
			_paused = true;
			MenuButton.enabled = false;
		}
	}
	
	public void ToggleQuit()
	{
		if(quit) //sai do quit e vai para a pausa
        {
            PauseCanvas.enabled = true;
            QuitCanvas.enabled = false;
			quit = false;
		}
		
		else //vai para o quit
        {
            QuitCanvas.enabled = true;
			PauseCanvas.enabled = false;
			quit = true;
		}
	}

	public void Menu()
	{
		Application.LoadLevel("menu"); //carrega a scene
		//Time.timeScale = 1.0f; 
	    GameManager.DestroySelf(); // quando for para o menu, apagar o game manager para não haver conflitos com lvls sobrepostos
	}

	public void AddScore(string name, int score){ //uma maneira de guardar o high score
		PlayerPrefs.SetString ("HighUser", name);
		PlayerPrefs.SetInt ("HighScore", score);
		Application.LoadLevel ("score");
	}
    
    public void SubmitScores()
	{
        // Verifica o username e insere na base de dados se estiver tudo de acordo
	    int highscore = GameManager.score;
        string username = ScoreCanvas.GetComponentInChildren<InputField>().GetComponentsInChildren<Text>()[1].text;
        Regex regex = new Regex("^[a-zA-Z0-9]*$");

	    if (username == "")                 ToggleErrorMsg("Username cannot be empty");
        else if (!regex.IsMatch(username))  ToggleErrorMsg("Username can only consist alpha-numberic characters");
        else if (username.Length > 10)      ToggleErrorMsg("Username cannot be longer than 10 characters");
        else                                AddScore(username, highscore);
	    
	}

    public void LoadLevel() //os lvls a serem usados
    {
        GameManager.Level++;
        Application.LoadLevel("game");
    }

    public void ToggleErrorMsg(string errorMsg) // em caso se faltar algum item dar pop up dessas mensagens de erro
    {
        if (ErrorCanvas.enabled)
        {
            ScoreCanvas.enabled = true;
            ErrorCanvas.enabled = false;

        }
        else
        {
            ScoreCanvas.enabled = false;
            ErrorCanvas.enabled = true;
            ErrorCanvas.GetComponentsInChildren<Text>()[1].text = errorMsg;

        }
    }
}
