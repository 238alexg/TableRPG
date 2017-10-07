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

    public float OrthographicSize {
        get { return Camera.main.orthographicSize; }
        set { Camera.main.orthographicSize = value; }
    }

	public float HeightToWidthRatio {
		get { return Screen.height / Screen.width; }
	}

    Vector2 spriteSize;
    int xSize, ySize;
    const int SmallMapSize = 0, MediumMapSize = 1, LargeMapSize = 2;
    const int DefaultPawnSetup = 0;
    const int PPU = 16;

	[System.NonSerialized]
	public float horzExtent, xyGridRatio;

	void Awake() {
		if (GameSetup.instance == null) {
			GameSetup.instance = this;
		} else {
			Destroy (this);
		}
	}

	void Start () {
		PredefinedMapSize mapSize = mapSizes [SmallMapSize];
		startingPawnSetup pawnSetup = pawnSetups [DefaultPawnSetup];
		SetUpGame (mapSize, pawnSetup);
	}


	public void SetUpGame (PredefinedMapSize mapSize, startingPawnSetup pawnSetup) {

        spriteSize = tile.GetComponent<SpriteRenderer>().size;

		// Create the board and tiles
		CreateBoard (mapSize);

		// Create starting pawns on both sides
		CreateStartingPawns (pawnSetup);

        InitializePlayerCamera();

        GameStateManager.instance.BeginGame ();
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
        Vector2 scale = Vector2.one;
        scale.x *= 1 / spriteSize.x;
        scale.y *= 1 / spriteSize.y;

		// Make a x by y grid of tiles
		for (int x = 0; x < xSize; x++) {
			for (int y = 0; y < ySize; y++) {
                Vector3 tilePos = new Vector3(x, ySize - y);
				TileClass newTile = Instantiate(tile, tilePos, tile.transform.rotation);
                newTile.transform.localScale = scale;
				GameStateManager.instance.tiles[x, y] = newTile;
			}
		}

		GameStateManager.instance.xSize = xSize;
		GameStateManager.instance.ySize = ySize;
	}

	// Creates starting pawns for both players
	public void CreateStartingPawns(startingPawnSetup pawnSetup) {
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

        int xOffset = (xSize - p1Pawns.Count) / 2;

		for (int i = 0; i < p1Pawns.Count; i++) {
			WalkableTile p1Pos = (WalkableTile)GameStateManager.instance.tiles [i + xOffset, 0];
            WalkableTile p2Pos = (WalkableTile)GameStateManager.instance.tiles [xSize - (i + xOffset) - 1, ySize - 1];

			// Update pawn location information
            p1Pawns [i].x = i + xOffset;
            p1Pawns [i].y = 0;

            p2Pawns [i].y = ySize - 1;
            p2Pawns [i].x = xSize - (i + xOffset) - 1;

			p1Pos.PlacePawn (p1Pawns[i]);
			p2Pos.PlacePawn (p2Pawns[i]);
		}
	}

    void InitializePlayerCamera() {
        OrthographicSize = 0.25f * (float)(Screen.height / PPU);
        Camera.main.transform.position = new Vector3((float)xSize / 2, (float)ySize / 2, -100);
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


