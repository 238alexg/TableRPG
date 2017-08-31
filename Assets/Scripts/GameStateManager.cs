using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour {

	// Game state data
	public static GameStateManager instance;

	public bool isSinglePlayer;

	public List<PawnClass> player1Pawns;
	public List<PawnClass> player2Pawns;

	public TileClass[,] tiles;

	// TODO: Implement these classes!
	public List<ActiveEffectClass> activeEffects;
//	public List<ItemClass> activeItems;
//
//	public List<CardClass> player1Cards;
//	public List<CardClass> player2Cards;

	// Turn state data
	[System.NonSerialized]
	public PawnClass.Ownership turn;
	public int turnCount, xSize, ySize;

	[System.NonSerialized]
	public int lastPlayer1Pawn, lastPlayer2Pawn; 

	void Awake() {
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}
		Application.targetFrameRate = 60;
	}

	public void BeginGame(bool isSinglePlayer) {
		turnCount = 0;
		turn = PawnClass.Ownership.Player1;
	}

	public void IncrementTurn() {
		// Increment turn count and see whose turn it is
		if (turnCount++ % 2 == 0) {
			turn = PawnClass.Ownership.Player1;
		} else {
			turn = PawnClass.Ownership.Player2;
		}

		foreach (ActiveEffectClass aec in activeEffects) {
			aec.IncrementTurn ();
		}
	}

}
