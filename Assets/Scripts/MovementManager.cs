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
	public List<WalkableTileOption> walkableTileOptions = new List<WalkableTileOption> ();

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
		if (GameSelections.HasMoved) { return; }
		
        TileContainer startTile = GameSelections.SelectedTile;
        GameSelections.ActivePlayerPawn = startTile.Pawn;
        GameSelections.PawnMovesLeft = startTile.Pawn.moveCount;

		isMovingPawn = true;

		// Show movement UI
        Vector3 PawnWorldCoord = new Vector3 (startTile.x * GameStateManager.SpriteSize, (GameStateManager.instance.ySize - startTile.y) * GameStateManager.SpriteSize, -100);
        Camera.main.transform.position = PawnWorldCoord; //TouchManager.Instance.MoveCameraToPoint (zoomCoord.x, zoomCoord.y);

		// Set origin tile (where pawn is starting from)
		WalkableTileOption origin = new WalkableTileOption(
            x: startTile.x, 
            y: startTile.y, 
            moveCount: startTile.Pawn.moveCount, 
			tile: startTile, 
			prevTile: null
		);

		// Add origin tile to new walkable tile set and start process of checking walkable sides.
		walkableTileOptions.Clear();
        walkableTileOptions.Add (origin);
		CheckWalkableSides (origin);

		// Display walkable spaces
		foreach (WalkableTileOption wto in walkableTileOptions) {
            StartCoroutine(wto.Tile.DimSelection());
		}
	}

	/// <summary>
	/// Generates the walkable tile list.
	/// </summary>
	/// <param name="moveCount">Move count of the pawn.</param>
	/// <param name="x">The x coordinate of the tile.</param>
	/// <param name="y">The y coordinate of the tile.</param>
	public void GenerateWalkableTileList(int moveCount, int x, int y, WalkableTileOption precedingTile) {

        TileContainer curTile = GameStateManager.instance.tiles [x, y];

		// If tile isn't walkable, no moves left, or tile has pawn on it
        if (moveCount >= 0 && GameSelections.ActivePlayerPawn.PawnCanWalkOnTile (curTile)) {
			WalkableTileOption wto = walkableTileOptions.Find (w => (w.X == x && w.Y == y));

			if (wto == null) { // Create wto if it doesn't exist yet
				wto = new WalkableTileOption (x, y, moveCount, curTile, precedingTile);
				walkableTileOptions.Add (wto);
				CheckWalkableSides (wto);
			} 
            else if (wto.MoveCount < moveCount) {
				wto.MoveCount = moveCount;
				wto.PrecedingTile = precedingTile;
				CheckWalkableSides (wto);
			}
		}
	}

	// Evaluates walkable tiles within move distance from all sides of a given tile (x,y)
    public void CheckWalkableSides(WalkableTileOption tile) {
		// Check left boundry
		if (tile.X > 0) {
			GenerateWalkableTileList (tile.MoveCount - 1, tile.X - 1, tile.Y, tile);
		}
		// Check right boundry
		if (tile.X < GameStateManager.instance.xSize - 1) {
			GenerateWalkableTileList (tile.MoveCount - 1, tile.X + 1, tile.Y, tile);
		}
		// Check top boundry
		if (tile.Y > 0) {
			GenerateWalkableTileList (tile.MoveCount - 1, tile.X, tile.Y - 1, tile);
		}
		// Check bottom boundry
		if (tile.Y < GameStateManager.instance.ySize - 1) {
			GenerateWalkableTileList (tile.MoveCount - 1, tile.X, tile.Y + 1, tile);
		}
	}

	/// <summary>
	/// Moves the pawn to a destination tile.
	/// This is called when a user taps on a highlighted walkable Tile
	/// </summary>
	/// <returns>Yield returns each time a movement from one tile has completed</returns>
	/// <param name="destTile">Destination tile.</param>
    public IEnumerator MovePawn(TileContainer destTile) {

		// Get pawn, destination, and path
		PawnClass pawn = GameSelections.ActivePlayerPawn;
		WalkableTileOption destination = walkableTileOptions.Find (w => w.Tile == destTile);

		WalkableTileOption[] path = CreateWalkingPath (destination);

		// Check to see if pawn has already moved
		if (GameSelections.HasMoved) {
			GameSelections.PawnMovesLeft -= (byte)path.Length;
		} else {
			GameSelections.PawnMovesLeft = (byte)(pawn.moveCount - path.Length);
			GameSelections.HasMoved = true;
		}

		// Move pawn 1 tile at a time, triggering all OnWalkOver effects of tiles en route
		for (int i = 0; i < path.Length; i++) {
			yield return StartCoroutine(MovePawnToNextLocation (pawn.transform, endPos: path[i].Tile.transform.position));
            path [i].PrecedingTile.Tile.Pawn = null;

			// Update selected tile
			GameSelections.SelectedTile = path [i].Tile;

			// Set pawn location
			pawn.x = path [i].X;
			pawn.y = path [i].Y;

            path [i].Tile.Pawn = pawn;
            path [i].Tile.Tile.OnWalkOver ();
		}

		walkableTileOptions.Clear ();

		// If pawn has moves still, generate paths from destination
		if (GameSelections.PawnMovesLeft > 0) {

			walkableTileOptions.Add (destination);
			CheckWalkableSides (destination);

			// Display walkable spaces
			foreach (WalkableTileOption wto in walkableTileOptions) {
				StartCoroutine (wto.Tile.DimSelection ());
			}
		} else {
			isMovingPawn = false;
		}
	}

	/// <summary>
	/// Moves the pawn to next location.
	/// </summary>
	/// <param name="pawn">Transform of pawn.</param>
	/// <param name="endPos">Ending position of pawn.</param>
    IEnumerator MovePawnToNextLocation(Transform pawn, Vector3 endPos) {

		Vector3 startPos = pawn.position;
		Vector3 curPos = pawn.position;

		float startTime = Time.time;
		float fracJourney = 0;
        const float journeyIncrement = 0.05f;

		while (fracJourney < 1) {
			curPos = Vector3.Lerp(startPos, endPos, fracJourney);

			pawn.position = curPos;

            fracJourney += journeyIncrement;

            yield return null;
		}
		pawn.position = endPos;
	}

	private WalkableTileOption[] CreateWalkingPath(WalkableTileOption destination) {

		WalkableTileOption[] path = new WalkableTileOption[GameSelections.PawnMovesLeft - destination.MoveCount];
		WalkableTileOption curWTO = destination;

		for (int i = path.Length - 1; i >= 0; i--) {
			path [i] = curWTO;
			curWTO = curWTO.PrecedingTile;
		}
		return path;
	}


	public class WalkableTileOption {
        public int X, Y, MoveCount;
        public TileContainer Tile;
        public WalkableTileOption PrecedingTile;

		public WalkableTileOption(int x, int y, int moveCount, TileContainer tile, WalkableTileOption prevTile) {
			this.X = x;
			this.Y = y;
			this.MoveCount = moveCount;
			this.Tile = tile;
			this.PrecedingTile = prevTile;
		}
	}

	public enum MovementDirection {
		Left,
		Right,
		Up,
		Down
	}
}
