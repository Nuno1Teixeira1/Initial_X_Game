using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class TileManager : MonoBehaviour
{
	
	public class Tile
	{
		public int x { get; set; }

		public int y { get; set; }

		public int type{ get; set; }

		public bool occupied { get; set; }

		public int adjacentCount { get; set; }

		public bool isIntersection { get; set; }

		public Tile left, right, up, down;

		public Tile (int x_in, int y_in)
		{
			x = x_in;
			y = y_in;
			type = 0;
			occupied = false;
			left = right = up = down = null;
		}
	};

	public static event Action OnTileLoaded = delegate{};

	public int n_RowCount = 30;
	public int n_ColumnCount = 30;
	public GameObject TileGroup;
	public GameObject BorderPrefab;
	public GameObject FloorPrefab;
	public List<Tile> tiles = new List<Tile> ();
	// Use this for initialization
	void Start ()
	{
		ReadTiles ();
		DrawTiles ();
		DrawNeighbors ();
	}

	// Update is called once per frame
	void Update ()
	{
//		DrawNeighbors();


	}
	
	//-----------------------------------------------------------------------
	void ReadTiles () //dados por tile: 1 = livre; 0 = wall
    {
        // LEVEL MAKER
        string data = @"111111111111111111111111111111
111111111111111111111111111111
111111111111111111111111111111
111000000001000010000000000111
111011011101011010110111110111
111000000000000010000000000111
111011111011011000110110110111
111011111010011101110010000111
111000000000111101111011011111
111011111011100000001010000111
111000010000101000101000111111
111111010111100010001010111111
111111010000101010101000111111
111111011101101111101010111111
111111010000000000000000000111
111000000111101111101111111111
111011111100001000100000000111
111010000001100010001111110111
111000111100111010111100000111
111111100001100010000001111111
111000001100101111101111000111
111011011110100000000000010111
111001000010111011111011110111
111101101010000000001000110111
111000001000101010100010100111
111011111110101010101110101111
111000000000101010100000000111
111111111111111111111111111111
111111111111111111111111111111
111111111111111111111111111111";

        int X = 1, Y = n_RowCount;
		using (StringReader reader = new StringReader (data)) {
			string line;
			while ((line = reader.ReadLine ()) != null) {
				X = 1; // para cada linha
				for (int i = 0; i < line.Length; ++i) {
					Tile newTile = new Tile (X, Y);
					newTile.type = int.Parse (line [i].ToString ());
					if (line [i] == '0') { // se o tile a ser lido é livre (é movivel?)
						if (i != 0 && line [i - 1] == '0') { //verificar esquerda e direita
                            // atribuir cada tile para o que corresponde ao tile de lado
							newTile.left = tiles [tiles.Count - 1];
							tiles [tiles.Count - 1].right = newTile;
                            // fazer a contagem por cada tile
							newTile.adjacentCount++;
							tiles [tiles.Count - 1].adjacentCount++;
						}
					}

                    else //se o tile está ocupado
						newTile.occupied = true;

                    // verificar as partes de cima e baixo (começa da 2º linha Y<30?)
					int upNeighbor = tiles.Count - line.Length; // up neighbor index
					if (Y < n_RowCount && !newTile.occupied && !tiles [upNeighbor].occupied) {
                        // atirbuir cada tile
                        tiles [upNeighbor].down = newTile;
						newTile.up = tiles [upNeighbor];
						// contagem
						newTile.adjacentCount++;
						tiles [upNeighbor].adjacentCount++;
					}            
					tiles.Add (newTile); //adiciona o tile
                    X++;
				}

				Y--;
			}
		}
    
		foreach (Tile tile in tiles) { // após ler todos os tiles, ler a intersecção de cada um
            if (tile.adjacentCount > 2)
				tile.isIntersection = true;
		}
		Debug.Log ("Loaded Tiles");
		OnTileLoaded (); //carrega

	}

	//-----------------------------------------------------------------------
	// Draw lines between neighbor tiles (debug)
	void DrawNeighbors ()
	{
		Debug.Log ("TilesCount :" + tiles.Count); 
		foreach (Tile tile in tiles) {
			Vector3 pos = new Vector3 (tile.x, tile.y, 0);
			Vector3 up = new Vector3 (tile.x + 2, tile.y + 2, 0);
			Vector3 down = new Vector3 (tile.x - 2, tile.y - 2, 0);
			Vector3 left = new Vector3 (tile.x - 2, tile.y + 2, 0);
			Vector3 right = new Vector3 (tile.x + 2, tile.y - 2, 0);
			
			if (tile.up != null)
				Debug.DrawLine (pos, up);
			if (tile.down != null)
				Debug.DrawLine (pos, down);
			if (tile.left != null)
				Debug.DrawLine (pos, left);
			if (tile.right != null)
				Debug.DrawLine (pos, right);
		}
		
	}


	//----------------------------------------------------------------------
	// returns the index in the tiles list of a given tile's coordinates
	public int Index (int X, int Y)
	{
		// the requsted index is in bounds
		X = X / 2;
		Y = Y / 2;
		X = Mathf.Clamp (X, 1, n_ColumnCount); 
		Y = Mathf.Clamp (Y, 1, n_RowCount);
		return (n_RowCount - Y) * n_ColumnCount + X - 1;
	}

	public int Index (Tile tile)
	{
		return (n_RowCount - tile.y) * n_ColumnCount + tile.x - 1;
	}

	//----------------------------------------------------------------------
	// returns the distance between two tiles
	public float distance (Tile tile1, Tile tile2)
	{
		return Mathf.Sqrt (Mathf.Pow (tile1.x - tile2.x, 2) + Mathf.Pow (tile1.y - tile2.y, 2));
	}

	void DrawTiles ()
	{
		foreach (Tile tile in tiles) {
			Vector3 pos = new Vector3 ((tile.x - 1) * 2, (tile.y - 1) * 2, 0);
			GameObject TilePrefab = GetMatchingTilePrefab (tile.type);
			if (TilePrefab != null) {
				GameObject tileObj = Instantiate (TilePrefab) as GameObject;
				tileObj.transform.SetParent (TileGroup.transform);
				tileObj.transform.localScale = Vector3.one;
				tileObj.transform.localPosition = pos;
				tileObj.name = string.Format ("Tile_{0:D2}_{1:D2}", tile.x, tile.y);
			}
		}
	}

	GameObject GetMatchingTilePrefab (int nId)
	{
		GameObject obj = null;
		switch (nId) {
		case 0:
			obj = FloorPrefab;
			break;
		case 1:
			obj = BorderPrefab;
			break;
		default:
			break;
		}
		return obj;
	}

	public bool isOccupied (int X, int Y)
	{
		foreach (Tile tile in tiles) {
			if (tile.x == X && tile.y == Y && tile.occupied) {
				return true;
			}
		}
		return false;
	}

	public void ChangeState (Tile tile, int nType)
	{
		tile.type = nType;
		if (nType == 0) {
			tile.occupied = false;
			if (tile.x > 1) {
				Tile leftTile = tiles [Index (tile.x - 1, tile.y)];
				if (!leftTile.occupied) {
					leftTile.right = tile;
					leftTile.adjacentCount++;
					tile.left = leftTile;
					tile.adjacentCount--;
				}
			}

			if (tile.x < n_ColumnCount) {
				Tile rightTile = tiles [Index (tile.x + 1, tile.y)];
				if (!rightTile.occupied) {
					rightTile.right = tile;
					rightTile.adjacentCount++;
					tile.right = rightTile;
					tile.adjacentCount--;
				}
			}

			if (tile.y > 1) {
				Tile upTile = tiles [Index (tile.x, tile.y - 1)];
				if (!upTile.occupied) {
					upTile.down = tile;
					upTile.adjacentCount++;
					tile.down = upTile;
					tile.adjacentCount--;
				}
			}

			if (tile.y < n_RowCount) {
				Tile downTile = tiles [Index (tile.x, tile.y + 1)];
				if (!downTile.occupied) {
					downTile.up = tile;
					downTile.adjacentCount++;
					tile.down = downTile;
					tile.adjacentCount--;
				}
			}

		} else {
			
			tile.occupied = true;
			tile.adjacentCount = 0;
			if (tile.up != null) {
				tile.up.down = null;
				tile.up.adjacentCount--;
			}
			if (tile.down != null) {
				tile.down.up = null;
				tile.down.adjacentCount--;
			}
			if (tile.left != null) {
				tile.left.right = null;
				tile.left.adjacentCount--;
			}
			if (tile.right != null) {
				tile.right.left = null;
				tile.right.adjacentCount--;
			}
			tile.up = tile.down = tile.left = tile.right = null;

		}
	}
}
