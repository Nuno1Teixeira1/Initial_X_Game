using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    //--------------------------------------------------------
    // VARIÁVEIS

    public static int Level = 1;
    public static int lives = 3;
	public static int DistractionCount = 2;

	public enum GameState { Init, Game, Dead, Scores }
	public static GameState gameState;

    private GameObject GreenCar;
    private GameObject RedCar;
    private GameGUINavigation gui;

	public static bool scared;
    static public int score;
	static public int highscore;

	public float scareLength;
	private float _timeToCalm;

    public float SpeedPerLevel;
    
    //-------------------------------------------------------------------
    // singleton implementation
    private static GameManager _instance;

    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }

            return _instance;
        }
    }

    //-------------------------------------------------------------------
    // function definitions

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            if(this != _instance)   
                Destroy(this.gameObject);
        }

        AssignEnemys();
    }

	void Start () 
	{
		gameState = GameState.Init; //inicia o ambiente do jogo
	}

    void OnLevelWasLoaded() //quando o level leva é iniciado
    {
        if (Level == 0) lives = 3; //player começa com 3 vidas

        AssignEnemys(); //atribuir cada carro
        ResetVariables(); //reinicia as variáveis
    }

    private void ResetVariables()
    {
        _timeToCalm = 0.0f;
        scared = false;
        PlayerController.killstreak = 0;
    }

	/*void Update () 
	{
		if(scared && _timeToCalm <= Time.time)
			CalmEnemys();

	}*/

	public void ResetScene()
	{
        CalmEnemys();

		Car[] carList = GameObject.FindObjectsOfType<Car> () as Car[];
		foreach (Car car in carList) {
			car.Init ();
		}

        gameState = GameState.Init;  
        gui.H_ShowReadyScreen();

	}

	/*public void ToggleScare()
	{
		if(!scared)	ScareEnemys();
		else 		CalmEnemys();
	}*/

	/*public void ScareEnemys() //assustar os inimigos
	{
		scared = true;
		RedCar.GetComponent<EnemyMove>().Frighten();
		_timeToCalm = Time.time + scareLength;
	}*/

	public void CalmEnemys()
	{
		scared = false;
		RedCar.GetComponent<EnemyMove>().Calm();
	    PlayerController.killstreak = 0;
    }

    void AssignEnemys()
    {
        // descobre e atribui os inimigos
		RedCar = GameObject.Find("Red Car");
        GreenCar = GameObject.Find("Green Car");

		if (RedCar == null) {
			Debug.Log ("RedCar is NULL");
		}
		if (GreenCar == null) {
			Debug.Log ("GreenCar is NULL");
		}
		Time.timeScale = 1;
        gui = GameObject.FindObjectOfType<GameGUINavigation>();
    }

    public void LoseLife() //perde uma vida
    {
        lives--; //-1
        gameState = GameState.Dead; //Dead state
    
        // atualizar o UI
        UIScript ui = GameObject.FindObjectOfType<UIScript>();
        Destroy(ui.lives[ui.lives.Count - 1]);
        ui.lives.RemoveAt(ui.lives.Count - 1);
    }

    public static void DestroySelf()
    {
        score = 0;
        Level = 0;
        lives = 3;
        Destroy(GameObject.Find("Game Manager"));
    }
}
