using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour {

	public static MovementManager instance;

	private List<walkableTileOption> walkableTileOptions = new List<walkableTileOption> ();

	void Awake () {
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}
	}

	public void ShowPawnMoveOptions() {
		
		walkableTileOptions.Clear ();
		PawnClass pawn = ((WalkableTile)TileClass.selectedTile).curPawn;

		// Show movement UI
		Vector2 zoomCoord = TouchManager.instance.GridToScreenCoordinates (pawn.x, pawn.y);
		TouchManager.instance.ZoomToPoint (true, zoomCoord.x, zoomCoord.y);

		checkWalkableSides (pawn.moveCount - 1, pawn.x, pawn.y);

		print ("mc: " + pawn.moveCount + " x:"  + pawn.x + " y:" + pawn.y);
		print ("Walkable tiles: " + walkableTileOptions.Count);

		// Display walkable spaces
		foreach (walkableTileOption wto in walkableTileOptions) {
			StartCoroutine (wto.tile.dimSelection (wto.tile.GetComponent <SpriteRenderer> ()));
		}
	}

	/// <summary>
	/// Generates the walkable tile list.
	/// </summary>
	/// <param name="moveCount">Move count of the pawn.</param>
	/// <param name="x">The x coordinate of the tile.</param>
	/// <param name="y">The y coordinate of the tile.</param>
	public void GenerateWalkableTileList(int moveCount, int x, int y) {

		TileClass curTile = GameStateManager.instance.tiles [x, y];

		// If tile isn't walkable, no moves left, or tile has pawn on it
		if (curTile is NonWalkableTile ||
			(curTile is WalkableTile && ((WalkableTile)curTile).curPawn != null) ||
			moveCount < 0) {
			return;
		} else {
			walkableTileOption existingWTO = walkableTileOptions.Find (wto => (wto.x == x && wto.y == y));

			if (existingWTO.tile == null) {
				// Create wto
				walkableTileOption wto = new walkableTileOption ();
				wto.moveCount = moveCount;
				wto.x = x;
				wto.y = y;
				wto.tile = curTile;

				walkableTileOptions.Add (wto);
			} else if (existingWTO.moveCount > moveCount) {
				existingWTO.moveCount = moveCount;
			}

			checkWalkableSides (moveCount - 1, x, y);
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

	public struct walkableTileOption {
		public int x, y, moveCount;
		public TileClass tile;
	}
}
