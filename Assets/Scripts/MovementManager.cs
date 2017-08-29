using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour {

	public static MovementManager instance;

	private List<TileClass> searchedTiles = new List<TileClass>();
	private List<TileClass> walkableTiles = new List<TileClass>();

	void Awake () {
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}
	}

	public void ShowPawnMoveOptions() {

		walkableTiles.Clear ();
		searchedTiles.Clear ();
		searchedTiles.Add (TileClass.selectedTile);

		PawnClass pawn = ((WalkableTile)TileClass.selectedTile).curPawn;
		checkWalkableSides (pawn.moveCount - 1, pawn.x, pawn.y);

		print ("mc: " + pawn.moveCount + " x:"  + pawn.x + " y:" + pawn.y);
		print ("Walkable tiles: " + walkableTiles.Count);

//		// Display walkable spaces
//		foreach (TileClass tile in walkableTiles) {
//			tile.GetComponent <SpriteRenderer> ().color = Color.black;
//		}
	}

	/// <summary>
	/// Generates the walkable tile list.
	/// </summary>
	/// <param name="moveCount">Move count of the pawn.</param>
	/// <param name="x">The x coordinate of the tile.</param>
	/// <param name="y">The y coordinate of the tile.</param>
	public void GenerateWalkableTileList(int moveCount, int x, int y) {

		TileClass curTile = GameStateManager.instance.tiles [x, y];

		// If tile hasn't been searched and we have moves left
		if (!searchedTiles.Contains (curTile) &&  moveCount >= 0) {
			// If tile isn't walkable or tile has pawn on it
			if (curTile is NonWalkableTile ||
				(curTile is WalkableTile && ((WalkableTile)curTile).curPawn != null)) 
			{
				searchedTiles.Add (curTile);
				return;
			} else {
				searchedTiles.Add (curTile);
				walkableTiles.Add (curTile);

				// DEBUG: Set walkable tiles to black
				curTile.GetComponent <SpriteRenderer> ().color = Color.black;

				checkWalkableSides (moveCount - 1, x, y);
			}
		}
	}

	// Evaluates walkable tiles within move distance from all sides of a given tile (x,y)
	public void checkWalkableSides(int moveCount, int x, int y) {
		// Check left boundry
		if (x > 0) {
			GenerateWalkableTileList (moveCount, x - 1, y);
		}
		// Check right boundry
		if (x < GameStateManager.instance.xSize - 1) {
			GenerateWalkableTileList (moveCount, x + 1, y);
		}
		// Check top boundry
		if (y > 0) {
			GenerateWalkableTileList (moveCount, x, y - 1);
		}
		// Check bottom boundry
		if (y < GameStateManager.instance.ySize - 1) {
			GenerateWalkableTileList (moveCount, x, y + 1);
		}
	}


}
