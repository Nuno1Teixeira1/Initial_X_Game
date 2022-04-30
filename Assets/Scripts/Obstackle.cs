using UnityEngine;
using System.Collections;

public class Obstackle : MonoBehaviour {
	private TileManager.Tile m_Tile;
	TileManager tm;
	// Use this for initialization
	void Start () {
		
	}

	public void SetTile(TileManager.Tile tile){
		m_Tile = tile;
		tm = GameObject.Find ("Game Manager").GetComponent<TileManager> ();
		tm.ChangeState (m_Tile, 2);
		Invoke ("OnDestroy", 1.5f);
	}
	// Update is called once per frame
	void Update () {
	
	}

	void OnDestroy(){
		tm.ChangeState (m_Tile, 0);
		DestroyObject (gameObject);
	}
}
