using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameSetup : MonoBehaviour {

	public GameObject background;
	public TileClass tile;

	public PredefinedMapSize[] mapSizes;
	public startingPawnSetup[] pawnSetups;

	public static GameSetup instance;

	private int xSize, ySize;

	[System.NonSerialized]
	public float horzExtent, xyGridRatio;

	void Awake() {
		if (GameSetup.instance == null) {
			GameSetup.instance = this;
			Camera.main.orthographicSize = Screen.height / 2;
		} else {
			Destroy (this);
		}
	}

	void Start () {
		PredefinedMapSize mapSize = mapSizes [1];
		startingPawnSetup pawnSetup = pawnSetups [0];
		bool isSinglePlayer = false;

		SetUpGame (mapSize, pawnSetup, isSinglePlayer);
	}


	public void SetUpGame (PredefinedMapSize mapSize, startingPawnSetup pawnSetup, bool isSinglePlayer) {

		// Create the board and tiles
		CreateBoard (mapSize);

		// Create starting pawns on both sides
		CreateStartingPawns (pawnSetup, isSinglePlayer);

		// Begin the game
		GameStateManager.instance.BeginGame (isSinglePlayer);
	}

	/// <summary>
	/// Creates the board.
	/// </summary>
	/// <param name="mapSize">Map size struct.</param>
	/// Adapted from Ray Wenderlich's tile game tutorial:
	/// https://www.raywenderlich.com/152282/how-to-make-a-match-3-game-in-unity
	public void CreateBoard (PredefinedMapSize mapSize) {
		xSize = mapSize.x;
		ySize = mapSize.y;

		GameStateManager.instance.tiles = new TileClass[xSize, ySize];

		xyGridRatio = (float)xSize / (float)ySize;  // Ratio of grid, width:height
		float vertExtent = Camera.main.orthographicSize;

		// Get new ratio of horz extent: world space of camera height *  grid width/height
		horzExtent = vertExtent * xSize / ySize;

		// Get size of tile sprite
		Vector2 tileSpriteSize = tile.GetComponent <SpriteRenderer> ().size;

		// Get offset for each tile
		Vector3 tileOffset = new Vector3((horzExtent * 2) / xSize, (vertExtent * 2) / ySize);

		// Get localScale for each tile
		Vector3 tileSize = new Vector3 (tileOffset.x / tileSpriteSize.x, tileOffset.y / tileSpriteSize.y);

		Vector3 cameraPos = transform.position;

		// Get the upper left hand corner
		float startX = cameraPos.x - horzExtent;
		float startY = cameraPos.y + vertExtent;

		// Make a x by y grid of tiles
		for (int x = 0; x < xSize; x++) {
			for (int y = 0; y < ySize; y++) {
				Vector3 tilePos = new Vector3 (startX + (tileOffset.x * x), startY - (tileOffset.y * y), -1);

				TileClass newTile = Instantiate(tile, tilePos, tile.transform.rotation);
				newTile.transform.localScale = tileSize;
				GameStateManager.instance.tiles[x, y] = newTile;
			}
		}

		GameStateManager.instance.xSize = xSize;
		GameStateManager.instance.ySize = ySize;
		TouchManager.instance.UpdateFieldMinMax ();
	}

	// Creates starting pawns for both players
	public void CreateStartingPawns(startingPawnSetup pawnSetup, bool isSinglePlayer) {
		List<PawnClass> p1Pawns = new List<PawnClass> ();
		List<PawnClass> p2Pawns = new List<PawnClass> ();

		if (pawnSetup.lineupName.ToLower () == "random") {
			// Instantiate 6 random pawns into map
			for (int i = 0; i < 6; i++) {
				PawnClass randPawn = pawnSetup.startingPawns [Random.Range (0, pawnSetup.startingPawns.Count)];
				p1Pawns.Add (Instantiate (randPawn));
				p2Pawns.Add (Instantiate (randPawn));
			}
		} else {
			for (int i = 0; i < pawnSetup.startingPawns.Count; i++) {
				PawnClass p1Spawn = Instantiate(pawnSetup.startingPawns[i]);
				PawnClass p2Spawn = Instantiate(pawnSetup.startingPawns[i]);

				p1Spawn.ownership = Ownership.Player1;
				p2Spawn.ownership = Ownership.Player2;

				p1Pawns.Add (p1Spawn);
				p2Pawns.Add (p2Spawn);
			}
		}

		int yOffset = (ySize - p1Pawns.Count) / 2;

		for (int i = 0; i < p1Pawns.Count; i++) {
			WalkableTile p1Pos = (WalkableTile)GameStateManager.instance.tiles [0, i + yOffset];
			WalkableTile p2Pos = (WalkableTile)GameStateManager.instance.tiles [xSize - 1, ySize - (i + yOffset) - 1];

			// Update pawn location information
			p1Pawns [i].x = 0;
			p1Pawns [i].y = i + yOffset;
			p2Pawns [i].x = xSize - 1;
			p2Pawns [i].y = ySize - (i + yOffset) - 1;

			p1Pos.PlacePawn (p1Pawns[i]);
			p2Pos.PlacePawn (p2Pawns[i]);
		}
	}

	[System.Serializable]
	public struct PredefinedMapSize {
		public string sizeName;
		public int x;
		public int y;
	}

	[System.Serializable]
	public struct startingPawnSetup {
		public string lineupName;
		public List<PawnClass> startingPawns;
	}
}


