using UnityEngine;
using System.Collections;

public class TargetGizmo : MonoBehaviour {

	public GameObject enemy;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(enemy.GetComponent<AI>().targetTile != null)
		{
			Vector3 pos = new Vector3(enemy.GetComponent<AI>().targetTile.x, 
										enemy.GetComponent<AI>().targetTile.y, 0f);
			transform.position = pos;
		}
	}
}
