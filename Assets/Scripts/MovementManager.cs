using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour {

	public static MovementManager instance;

	/// <summary>
	/// Whether the pawn is moving or not. Used in TileClass.TileTap() to determine outcome of a tap.
	/// </summary>
	public bool isMovingPawn;

	/// <summary>
	/// All of the walkable tiles from the origin of the selected pawn. Each tile has a reference
	/// to a preceding tile, creating a path to the origin tile.
	/// </summary>
	public List<walkableTileOption> walkableTileOptions = new List<walkableTileOption> ();

	/// <summary>
	/// The time it takes to move a pawn 1 tile.
	/// </summary>
	public float movementTime;

	void Awake () {
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}
		isMovingPawn = false;
	}

	/// <summary>
	/// Generates the possible paths for the pawn, and displays them to user.
	/// This is called when the user presses the "Move" button
	/// </summary>
	public void ShowPawnMoveOptions() {

		// User has already pressed move
		if (GameSelections.hasMoved) {
			UIManager.instance.moveButton.interactable = false;
			return;
		} 

		walkableTileOptions.Clear ();
		TileClass startTile = GameSelections.selectedTile;
		GameSelections.activePlayerPawn = startTile.curPawn;
		GameSelections.pawnMovesLeft = startTile.curPawn.moveCount;

		isMovingPawn = true;

		// Show movement UI
		Vector2 zoomCoord = TouchManager.instance.GridToScreenCoordinates (startTile.curPawn.x, startTile.curPawn.y);
		TouchManager.instance.ZoomToPoint (true, zoomCoord.x, zoomCoord.y);

		// Set origin tile (where pawn is starting from)
		walkableTileOption origin = new walkableTileOption(
			x: startTile.curPawn.x, 
			y: startTile.curPawn.y, 
			moveCount: startTile.curPawn.moveCount, 
			tile: startTile, 
			prevTile: null
		);

		// Add origin tile to the walkable tile set and start process of checking walkable sides.
		walkableTileOptions.Add (origin);
		checkWalkableSides (origin);

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
	public void GenerateWalkableTileList(int moveCount, int x, int y, walkableTileOption precedingTile) {

		TileClass curTile = GameStateManager.instance.tiles [x, y];

		// If tile isn't walkable, no moves left, or tile has pawn on it
		if (GameSelections.activePlayerPawn.PawnCanWalkOnTile (curTile) && moveCount >= 0) {
			walkableTileOption wto = walkableTileOptions.Find (w => (w.x == x && w.y == y));

			if (wto == null) {
				// Create wto
				wto = new walkableTileOption (
					x: x, y: y, moveCount: moveCount, tile: curTile, prevTile: precedingTile
				);

				walkableTileOptions.Add (wto);
				checkWalkableSides (wto);
			} else if (wto.moveCount < moveCount) {
				wto.moveCount = moveCount;
				wto.precedingTile = precedingTile;
				checkWalkableSides (wto);
			}
		}
	}

	// Evaluates walkable tiles within move distance from all sides of a given tile (x,y)
	public void checkWalkableSides(walkableTileOption tile) {
		// Check left boundry
		if (tile.x > 0) {
			GenerateWalkableTileList (tile.moveCount - 1, tile.x - 1, tile.y, tile);
		}
		// Check right boundry
		if (tile.x < GameStateManager.instance.xSize - 1) {
			GenerateWalkableTileList (tile.moveCount - 1, tile.x + 1, tile.y, tile);
		}
		// Check top boundry
		if (tile.y > 0) {
			GenerateWalkableTileList (tile.moveCount - 1, tile.x, tile.y - 1, tile);
		}
		// Check bottom boundry
		if (tile.y < GameStateManager.instance.ySize - 1) {
			GenerateWalkableTileList (tile.moveCount - 1, tile.x, tile.y + 1, tile);
		}
	}

	/// <summary>
	/// Moves the pawn to a destination tile.
	/// This is called when a user taps on a highlighted walkable Tile
	/// </summary>
	/// <returns>Yield returns each time a movement from one tile has completed</returns>
	/// <param name="destTile">Destination tile.</param>
	public IEnumerator MovePawn(TileClass destTile) {

		UIManager.instance.moveButtonText.text = "DONE";

		// Get pawn, destination, and path
		PawnClass pawn = GameSelections.activePlayerPawn;
		walkableTileOption destination = walkableTileOptions.Find (w => w.tile == destTile);

		walkableTileOption[] path = createWalkingPath (destination, pawn.moveCount);

		// Check to see if pawn has already moved
		if (GameSelections.hasMoved) {
			GameSelections.pawnMovesLeft -= (byte)path.Length;
		} else {
			GameSelections.pawnMovesLeft = (byte)(pawn.moveCount - path.Length);
			GameSelections.hasMoved = true;
		}

		// Move pawn 1 tile at a time, triggering all OnWalkOver effects of tiles en route
		for (int i = 0; i < path.Length; i++) {
			yield return StartCoroutine(movePawnToNextLocation (pawn.transform, endPos: path[i].tile.transform.position));
			path [i].precedingTile.tile.curPawn = null;

			// Update selected tile
			GameSelections.selectedTile = path [i].tile;

			// Set pawn location
			pawn.x = path [i].x;
			pawn.y = path [i].y;

			path [i].tile.curPawn = pawn;
			path [i].tile.OnWalkOver ();
		}

		walkableTileOptions.Clear ();

		// If pawn has moves still, generate paths from destination
		if (GameSelections.pawnMovesLeft > 0) {

			walkableTileOptions.Add (destination);
			checkWalkableSides (destination);

			// Display walkable spaces
			foreach (walkableTileOption wto in walkableTileOptions) {
				StartCoroutine (wto.tile.dimSelection (wto.tile.GetComponent <SpriteRenderer> ()));
			}
		} else {
			isMovingPawn = false;
			UIManager.instance.moveButton.interactable = false;
		}
	}

	/// <summary>
	/// Moves the pawn to next location.
	/// </summary>
	/// <param name="pawn">Transform of pawn.</param>.</param>
	/// <param name="endPos">Ending position of pawn.</param>
	IEnumerator movePawnToNextLocation(Transform pawn, Vector3 endPos) {

		Vector3 startPos = pawn.position;
		Vector3 curPos = pawn.position;

		float startTime = Time.time;
		float fracJourney = 0;

		while (fracJourney < 1) {
			curPos = Vector3.Lerp(startPos, endPos, fracJourney);

			pawn.position = curPos;

			if (TouchManager.instance.isZoomedIn) {
				curPos.z = -100;
				Camera.main.transform.position = curPos;
			}

			fracJourney += 0.04f;

			yield return new WaitForSeconds (0.01f * movementTime);
		}
		pawn.position = endPos;
	}

	private walkableTileOption[] createWalkingPath(walkableTileOption destination, int pawnMoveCount) {

		walkableTileOption[] path = new walkableTileOption[GameSelections.pawnMovesLeft - destination.moveCount];
		walkableTileOption curWTO = destination;

		for (int i = path.Length - 1; i >= 0; i--) {
			path [i] = curWTO;
			curWTO = curWTO.precedingTile;
		}
		return path;
	}


	public class walkableTileOption {
		public int x, y, moveCount;
		public TileClass tile;
		public walkableTileOption precedingTile;

		public walkableTileOption(int x, int y, int moveCount, TileClass tile, walkableTileOption prevTile) {
			this.x = x;
			this.y = y;
			this.moveCount = moveCount;
			this.tile = tile;
			this.precedingTile = prevTile;
		}
	}

	public enum MovementDirection {
		Left,
		Right,
		Up,
		Down
	}
}
