using UnityEngine;
using System.Collections;

public class FlagTrigger : MonoBehaviour {
    int x;
    int y;
    Vector2 pos;

	void Awake(){
		TileManager.OnTileLoaded += OnReadyMakeFlag;
	}
    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        
    }

	public void MakeFlag(){
		TileManager TM = GameObject.FindObjectOfType<TileManager>();
		while (true) {
			x = Random.Range(1, 21);
			y = Random.Range(1, 22);
			if (!TM.isOccupied (x, y)) {
				break;
			}
		}
		pos = new Vector2((x-1)*2, (y-1)*2);
		transform.position = pos;
	}

	void OnReadyMakeFlag(){
		TileManager.OnTileLoaded -= OnReadyMakeFlag;
		MakeFlag ();
	}
}
