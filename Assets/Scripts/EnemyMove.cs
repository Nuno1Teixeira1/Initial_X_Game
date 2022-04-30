using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemyMove : MonoBehaviour {

    // ----------------------------
    // Navigation variables
	private Vector3 waypoint;			// AI-determined waypoint
	private Queue<Vector3> waypoints = new Queue<Vector3>();	// waypoints used on Init and Scatter states

	// direction is set from the AI component
	public Vector3 _direction = new Vector3(0, 1, 0);
	public Vector3 direction 
	{
		get
		{
			return _direction;
		}

		set
		{
			_direction = value;
			Vector3 pos = new Vector3((int)((transform.position.x+0.01f)/2), (int)((transform.position.y+0.01f)/2), (int)transform.position.z);
			RotateCar ();
			waypoint = pos*2 + _direction*2;
//			Debug.Log ("waypoint (" + waypoint.x + ", " + waypoint.y + ") set! _direction: " + _direction.x + ", " + _direction.y);
		
		}
	}

	public float speed = 0.3f;

    // ----------------------------
    // Enemy mode variables
	public float scatterLength = 5f;
	public float waitLength = 0.0f;
	public GameObject BangObj;

	private float timeToEndScatter;
	private float timeToEndWait;

	enum State { Wait, Init, Scatter, Chase, Run };
	State state;

    private Vector3 _startPos;
    private float _timeToWhite;
    private float _timeToToggleWhite;
    private float _toggleInterval;
    private bool isWhite = false;

	// handles
	public GameGUINavigation GUINav;
    public PlayerController GreenCar;
    private GameManager _gm;

	//-----------------------------------------------------------------------------------------
	// variables end, functions begin
	void Start()
	{
	    _gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _toggleInterval = _gm.scareLength * 0.33f * 0.20f;  

		InitializeEnemy();
	}

    public float DISTANCE;

	void FixedUpdate ()
	{
	    DISTANCE = Vector3.Distance(transform.position, waypoint);

		if(GameManager.gameState == GameManager.GameState.Game){
			switch(state)
			{
			case State.Wait:
				Wait();
				break;

			case State.Init:
				Init();
				break;

			case State.Scatter:
				Scatter();
				break;

			case State.Chase:
				ChaseAI();
				break;
			}
		}

	}

	//-----------------------------------------------------------------------------------
	// Start() functions

	public void InitializeEnemy()
	{
		waypoint = transform.position;	// to avoid flickering animation
		timeToEndWait = Time.time + waitLength + GUINav.initialDelay;
//		state = State.Wait;
		state = State.Chase;
	}

    public void InitializeEnemy(Vector3 pos)
    {
        transform.position = pos;
        waypoint = transform.position;	// to avoid flickering animation
		timeToEndWait = Time.time + waitLength + GUINav.initialDelay;
        state = State.Wait;
		state = State.Chase;
    }
	

    void OnTriggerEnter2D(Collider2D other)
	{
		if(other.name == "Green Car")
		{
			Debug.Log (other.name);
//			other.gameObject.transform.position = new Vector3 (1000, 1000);
			GameObject obj = Instantiate (BangObj, other.transform.position, Quaternion.identity) as GameObject;
			Destroy (obj, 1f);
	        _gm.LoseLife();
		}
	}

	bool Valid(Vector2 direction)
	{
		// cast line from 'next to pacman' to pacman
		// not from directly the center of next tile but just a little further from center of next tile
		Vector2 pos = transform.position;
		direction += new Vector2(direction.x*0.5f , direction.y*0.5f);
		RaycastHit2D hit = Physics2D.Raycast(pos + direction, direction, 0.1f);
		if (hit.collider == null) {
			return true;
		}
		return (hit.collider.tag != "obstackle");
	}

	//-----------------------------------------------------------------------------------
	// State functions
	void Wait()
	{
		if(Time.time >= timeToEndWait)
		{
			state = State.Init;
		    waypoints.Clear();
//			InitializeWaypoints(state);
		}

		// get the next waypoint and move towards it
		MoveToWaypoint(true);
	}

	void Init()
	{
	    _timeToWhite = 0;

		// if the Queue is cleared, do some clean up and change the state
		if(waypoints.Count == 0)
		{
			state = State.Scatter;

		    //get direction according to sprite name
			string name = GetComponent<SpriteRenderer>().sprite.name;
			if(name[name.Length-1] == '0' || name[name.Length-1] == '1')	direction = Vector3.right;
			if(name[name.Length-1] == '2' || name[name.Length-1] == '3')	direction = Vector3.left;
			if(name[name.Length-1] == '4' || name[name.Length-1] == '5')	direction = Vector3.up;
			if(name[name.Length-1] == '6' || name[name.Length-1] == '7')	direction = Vector3.down;

//			InitializeWaypoints(state);
			timeToEndScatter = Time.time + scatterLength;

			return;
		}

		// get the next waypoint and move towards it
		MoveToWaypoint();
	}

	void Scatter()
	{
		if(Time.time >= timeToEndScatter)
		{
			waypoints.Clear();
			state = State.Chase;
		    return;
		}

		// get the next waypoint and move towards it
		MoveToWaypoint(true);

	}

    void ChaseAI()
	{
        // if not at waypoint, move towards it
		float dist = Vector3.Distance(transform.position, waypoint);
		if (dist  > 0.001f)
		{
			if (dist >= speed) {
				dist = speed;
			}
			if (Valid (direction)) {
				Vector2 p = Vector2.MoveTowards (transform.position, waypoint, dist);
				GetComponent<Rigidbody2D> ().MovePosition (p);
			} else {
				GetComponent<AI>().AILogic();
			}

		}

		// if at waypoint, run AI module
		else GetComponent<AI>().AILogic();

	}

	//------------------------------------------------------------------------------
	// Utility functions
	void MoveToWaypoint(bool loop = false)
	{
		waypoint = waypoints.Peek();		// get the waypoint (CHECK NULL?)
        if (Vector3.Distance(transform.position, waypoint) > 0.000000000001)	// if its not reached
		{									                        // move towards it
			_direction = Vector3.Normalize(waypoint - transform.position);	// dont screw up waypoint by calling public setter
			Vector2 p = Vector2.MoveTowards(transform.position, waypoint, speed);
			GetComponent<Rigidbody2D>().MovePosition(p);
		}
		else 	// if waypoint is reached, remove it from the queue
		{
			if(loop)	waypoints.Enqueue(waypoints.Dequeue());
			else		waypoints.Dequeue();
		}
	}

	public void Frighten()
	{
		state = State.Run;
		_direction *= -1;

        _timeToWhite = Time.time + _gm.scareLength * 0.66f;
        _timeToToggleWhite = _timeToWhite;
        GetComponent<Animator>().SetBool("Run_White", false);

	}

	public void Calm()
	{
        // if the enemy is not running, do nothing
	    if (state != State.Run) return;

		waypoints.Clear ();
		state = State.Chase;
	    _timeToToggleWhite = 0;
	    _timeToWhite = 0;
        GetComponent<Animator>().SetBool("Run_White", false);
        GetComponent<Animator>().SetBool("Run", false);
	}

    public void ToggleBlueWhite()
    {
        isWhite = !isWhite;
        GetComponent<Animator>().SetBool("Run_White", isWhite);
        _timeToToggleWhite = Time.time + _toggleInterval;
    }

	void RotateCar(){
		int nDirection = 0;
		if (direction.x > 0) {
			nDirection = 4;
		}
		if (direction.x < 0) {
			nDirection = 2;
		}
		if (direction.y > 0) {
			nDirection = 1;
		}
		if (direction.y < 0) {
			nDirection = 3;
		}
		if (nDirection != 0) {
			transform.eulerAngles = new Vector3 (0, 0, 90) * (nDirection - 1);
		}
	}

}
