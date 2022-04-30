using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AI : MonoBehaviour {

	public Transform target;

	private List<TileManager.Tile> tiles = new List<TileManager.Tile>();
	private TileManager manager;
	public EnemyMove enemy;

	private List<Vector3> directionList;

	public TileManager.Tile nextTile = null;
	public TileManager.Tile targetTile;
	TileManager.Tile currentTile;
	bool bFlag = false;

	public GameObject markObject;

	void Awake()
	{
		manager = GameObject.Find("Game Manager").GetComponent<TileManager>();
		tiles = manager.tiles;

		if(enemy == null)	Debug.Log ("game object enemy not found");
		if(manager == null)	Debug.Log ("game object Game Manager not found");

		directionList = new List<Vector3> ();
		directionList.Add (Vector3.up);
		directionList.Add (Vector3.right);
		directionList.Add (-Vector3.up);
		directionList.Add (-Vector3.right);

	

	}

	public void AILogic()
	{
//		if (!bFlag) {
//			GetMinPath ();
//			bFlag = true;
//		}
		// get current tile
		Vector3 currentPos = new Vector3(transform.position.x + 2.0001f, transform.position.y + 2.001f);
		currentTile = tiles[manager.Index ((int)(currentPos.x), (int)(currentPos.y))];
		
		targetTile = GetTargetTilePerEnemy();

		// get the next tile according to direction
		if(enemy.direction.x > 0)	{
			nextTile = tiles [manager.Index ((int)(currentPos.x + 2), (int)currentPos.y)];
		}
		if (enemy.direction.x < 0) {
			nextTile = tiles [manager.Index ((int)(currentPos.x - 2), (int)currentPos.y)];
		}
		if (enemy.direction.y > 0) {
			nextTile = tiles [manager.Index ((int)currentPos.x, (int)(currentPos.y + 2))];
		}
		if (enemy.direction.y < 0) {
			nextTile = tiles [manager.Index ((int)currentPos.x, (int)(currentPos.y - 2))];
		}

		if(nextTile.occupied || currentTile.isIntersection)
		{
			//---------------------
			// IF WE BUMP INTO WALL
			if(nextTile.occupied && !currentTile.isIntersection)
			{
				Vector3 nextdirection = Vector3.zero;
				// if enemy moves to right or left and there is wall next tile
				if(enemy.direction.x != 0)
				{
					if (currentTile.up != null) {
						nextdirection = Vector3.up;
					} else if (currentTile.down != null) {
						nextdirection = Vector3.down;
					} 

				}
				
				// if enemy moves to up or down and there is wall next tile
				else if(enemy.direction.y != 0)
				{
					if (currentTile.left != null) {
						nextdirection = Vector3.left;
					}else if(currentTile.right != null){
						nextdirection = Vector3.right;
					}
				}

				if (nextdirection == Vector3.zero) {
					nextdirection = -Vector3.Normalize (enemy.direction);
				}
				enemy.direction = nextdirection;
			}
			
			//---------------------------------------------------------------------------------------
			// IF WE ARE AT INTERSECTION
			// calculate the distance to target from each available tile and choose the shortest one
			if(currentTile.isIntersection)
			{
				
				float dist1, dist2, dist3, dist4;
				dist1 = dist2 = dist3 = dist4 = 999999f;
				if(currentTile.up != null && !currentTile.up.occupied && !(enemy.direction.y < 0)) 		dist1 = manager.distance(currentTile.up, targetTile);
				if(currentTile.down != null && !currentTile.down.occupied &&  !(enemy.direction.y > 0)) 	dist2 = manager.distance(currentTile.down, targetTile);
				if(currentTile.left != null && !currentTile.left.occupied && !(enemy.direction.x > 0)) 	dist3 = manager.distance(currentTile.left, targetTile);
				if(currentTile.right != null && !currentTile.right.occupied && !(enemy.direction.x < 0))	dist4 = manager.distance(currentTile.right, targetTile);
				
				float min = Mathf.Min(dist1, dist2, dist3, dist4);
				if(min == dist1) enemy.direction = Vector3.up;
				if(min == dist2) enemy.direction = Vector3.down;
				if(min == dist3) enemy.direction = Vector3.left;
				if(min == dist4) enemy.direction = Vector3.right;
				
			}
			
		}
		
		// if there is no decision to be made, designate next waypoint for the enemy
		else
		{
			enemy.direction = enemy.direction;	// setter updates the waypoint
//			if (!Valid (direction)) {
//				for (int i = 0; i < directionList.Count; i++) {
//					direction = directionList [i];
//					if (Valid (direction)) {
//						break;
//					}
//				}
//			}

		}
	}

	void GetMinPath(){
		Vector3 currentPos = new Vector3(transform.position.x + 2.0001f, transform.position.y + 2.001f);
		TileManager.Tile curTile = tiles[manager.Index ((int)(currentPos.x), (int)(currentPos.y))];
		TileManager.Tile tarTile = GetTargetTilePerEnemy();
		List<int> minPath = new List<int> ();
		List<List<int>> paths = new List<List<int>> ();
		while (true) {
			if (paths.Count == 0) {
				List<int> newpath = new List<int> ();	
				newpath.Add (manager.Index (curTile));
				paths.Add (newpath);
			} else {
				bool FindFlag = false;
				List<List<int>> newpaths = new List<List<int>> ();
				foreach (List<int> path in paths) {
					curTile = tiles[path [path.Count - 1]];
					if (curTile.x == tarTile.x && curTile.y == tarTile.y) {
						FindFlag = true;
						minPath = path;
						break;
					}
					if (curTile.up != null) {
						List<int> newpath = new List<int> ();
						bool flag = false;
						foreach (int id in path) {
							if (id == manager.Index (curTile.up)) {
								flag = true;
								break;
							}
							newpath.Add (id);
						}
						if (!flag) {
							newpath.Add (manager.Index (curTile.up));
							newpaths.Add (newpath);
						}
					}
					if (curTile.down != null) {
						List<int> newpath = new List<int> ();
						bool flag = false;
						foreach (int id in path) {
							if (id == manager.Index (curTile.down)) {
								flag = true;
								break;
							}
							newpath.Add (id);
						}
						if (!flag) {
							newpath.Add (manager.Index (curTile.down));
							newpaths.Add (newpath);
						}
					}
					if (curTile.left != null) {
						List<int> newpath = new List<int> ();
						bool flag = false;
						foreach (int id in path) {
							if (id == manager.Index (curTile.left)) {
								flag = true;
								break;
							}
							newpath.Add (id);
						}
						if (!flag) {
							newpath.Add (manager.Index (curTile.left));
							newpaths.Add (newpath);
						}
					}
					if (curTile.right != null) {
						List<int> newpath = new List<int> ();
						bool flag = false;
						foreach (int id in path) {
							if (id == manager.Index (curTile.right)) {
								flag = true;
								break;
							}
							newpath.Add (id);
						}
						if (!flag) {
							newpath.Add (manager.Index (curTile.right));
							newpaths.Add (newpath);
						}
					}
				}
				if (FindFlag) {
					break;
				}
				paths = newpaths;
			}
		}
		foreach (int index in minPath) {
			Debug.Log ("MinPath" + index);
		}

	}
	TileManager.Tile GetTargetTilePerEnemy()
	{
		Vector3 targetPos;
		TileManager.Tile targetTile;
		targetPos = new Vector3 (target.position.x, target.position.y);
		targetTile = tiles[manager.Index((int)targetPos.x, (int)targetPos.y)];
		return targetTile;
	}

	void FixedUpdate(){
		Vector3 screenPoint = Camera.main.WorldToViewportPoint(gameObject.transform.position);
		bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
		if (onScreen == false) {
			screenPoint.x = Mathf.Clamp01 (screenPoint.x);
			screenPoint.y = Mathf.Clamp01 (screenPoint.y);
			screenPoint = Camera.main.ViewportToWorldPoint (screenPoint);
			currentTile = tiles [manager.Index ((int)((screenPoint.x + 2.001f)), (int)((screenPoint.y + 2.001f)))];
			markObject.transform.position = screenPoint;
		} else {
			markObject.transform.position = new Vector3(1000, 1000, 0);
		}
	}
}