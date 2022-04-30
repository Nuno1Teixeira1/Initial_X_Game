using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{

    public float speed = 0.4f;
    Vector2 _dest = Vector2.zero;
    Vector2 _dir = Vector2.zero;
    Vector2 _nextDir = Vector2.zero;

    [Serializable]
    public class PointSprites
    {
        public GameObject[] pointSprites;
    }

    public PointSprites points;

    public static int killstreak = 0;
	private int direction = 0;
    // script handles
    private GameGUINavigation GUINav;
    private GameManager GM;
    private ScoreManager SM;
	private TileManager manager;
	private List<TileManager.Tile> tiles = new List<TileManager.Tile>();

	public GameObject DistractionObj;

    private bool _deadPlaying = false;
    // Use this for initialization
    void Start()
    {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        SM = GameObject.Find("Game Manager").GetComponent<ScoreManager>();
        GUINav = GameObject.Find("UI Manager").GetComponent<GameGUINavigation>();
		manager = GameObject.Find("Game Manager").GetComponent<TileManager>();
		tiles = manager.tiles;
		direction = 1;
        _dest = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameManager.gameState)
        {
            case GameManager.GameState.Game:
                ReadInputAndMove();
                break;

            case GameManager.GameState.Dead:
                if (!_deadPlaying)
                    StartCoroutine("PlayDeadAnimation");
                break;
        }
    }

    IEnumerator PlayDeadAnimation()
    {
        _deadPlaying = true;
//        GetComponent<Animator>().SetBool("Die", true);
        yield return new WaitForSeconds(1);
//        GetComponent<Animator>().SetBool("Die", false);
        _deadPlaying = false;

        if (GameManager.lives <= 0)
        {
			if (GameManager.score != 0 &&  GameManager.score >= GameManager.highscore)
                GUINav.getScoresMenu();
            else
                GUINav.H_ShowGameOverScreen();
        }

        else
            GM.ResetScene();
    }

    void Animate()
    {
//        Vector2 dir = _dest - (Vector2)transform.position;
		if (direction == 0) {
			return;
		}
		transform.eulerAngles = new Vector3 (0, 0, 90) * (direction - 1);
//		if (Vector2.Distance(_dest, (Vector2)transform.position) == 1f) {
//			transform.eulerAngles = new Vector3 (0, 0, 90) * (direction - 1);
//		}
    }

    bool Valid(Vector2 direction)
    {
        // cast line from 'next to pacman' to pacman
        // not from directly the center of next tile but just a little further from center of next tile
        Vector2 pos = transform.position;
        direction += new Vector2(direction.x * 0.45f, direction.y * 0.45f);
        RaycastHit2D hit = Physics2D.Linecast(pos + direction, pos);
        return hit.collider == GetComponent<Collider2D>();
    }

    public void ResetDestination()
    {
         _dest = new Vector2(20f, 20f);
		direction = 1;
		Animate ();
    }

    void ReadInputAndMove()
    {
        // move closer to destination
        Vector2 p = Vector2.MoveTowards(transform.position, _dest, speed);
        GetComponent<Rigidbody2D>().MovePosition(p);

        // get the next direction from keyboard
		if (Input.GetAxis ("Horizontal") > 0) {
			_nextDir = Vector2.right;
			direction = 4;
		}
		if (Input.GetAxis ("Horizontal") < 0) {
			_nextDir = -Vector2.right;
			direction = 2;
		}
		if (Input.GetAxis ("Vertical") > 0) {
			_nextDir = Vector2.up;
			direction = 1;
		}
		if (Input.GetAxis ("Vertical") < 0) {
			_nextDir = -Vector2.up;
			direction = 3;
		}

		if (Input.GetKeyDown (KeyCode.LeftControl)) {
			MakeDistraction ();
		}

        // if greenCar is in the center of a tile
        if (Vector2.Distance(_dest, transform.position) < 0.00001f)
        {
            if (Valid(_nextDir)) 
            {
                _dest = (Vector2)transform.position + _nextDir;
                _dir = _nextDir;
				Animate();
            }
            else   // if next direction is not valid
            {
                if (Valid(_dir))  // and the prev. direction is valid
				{
                    _dest = (Vector2)transform.position + _dir;   // continue on that direction
				}
            }
        }


    }

    public Vector2 getDir()
    {
        return _dir;
    }

    public void UpdateScore(Vector3 pos)
    {
		killstreak++;
        // limit killstreak at 4
        if (killstreak > 4) killstreak = 4;
		GameObject scoreObj = Instantiate(points.pointSprites[killstreak - 1], pos, Quaternion.identity) as GameObject;
		Destroy (scoreObj, 0.8f);
		GameManager.score += (int)Mathf.Pow(2, (killstreak - 1)) * 100;
		if (GameManager.score > GameManager.Level * 1000) {
			GUINav.LoadLevel ();
		}
    }

	public void Control(int dir){
		direction = dir;
		switch (dir) {
		case 1:
			_nextDir = Vector2.up;
			break;
		case 2:
			_nextDir = -Vector2.right;
			break;
		case 3:
			_nextDir =- Vector2.up;
			break;
		case 4:
			_nextDir = Vector2.right;
			break;
		default:
			break;
		}
		ReadInputAndMove ();
	}

	public void MakeDistraction(){
		if (GameManager.DistractionCount <= 0) {
			return;
		}
		Vector3 currentPos = new Vector3(transform.position.x + 0.0001f, transform.position.y + 0.001f);
		TileManager.Tile currentTile = tiles[manager.Index ((int)(currentPos.x), (int)(currentPos.y))];
		if (tiles [manager.Index (currentTile.x -(int) (getDir().x), currentTile.y - (int)(getDir().y))].occupied) {
			return;
		}
		GameObject obj = Instantiate (DistractionObj, new Vector3 ((currentTile.x-getDir().x) * 2, (currentTile.y- getDir().y) * 2), Quaternion.Euler (getDir())) as GameObject;
		obj.transform.localScale = Vector3.one * 1.2f;
		obj.GetComponent<Obstackle> ().SetTile (currentTile);
		GameManager.DistractionCount--;
	}

	void OnTriggerEnter2D(Collider2D co) {
		if(co.name == "Flag") {
			UpdateScore (co.transform.position);
			co.GetComponent<FlagTrigger>().MakeFlag();
		}
	}
}

