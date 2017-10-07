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
	public Ownership turn;
	public int turnCount, xSize, ySize;


	void Awake() {
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}
		Application.targetFrameRate = 60;
	}

	public void BeginGame() {
		turnCount = 0;
		turn = Ownership.Player1;
		DisplayTurnToken (turn);
	}

	// Increments the turn count, and flips the turn token
	public void IncrementTurn() {
		// Increment turn count and see whose turn it is
		if (++turnCount % 2 == 0) {
			turn = Ownership.Player1;
		} else {
			turn = Ownership.Player2;
		}

		DisplayTurnToken (turn);

		foreach (ActiveEffectClass aec in activeEffects) {
			aec.IncrementTurn ();
		}
	}

	// Ends the players turn
	public void EndTurn() {
		GameSelections.activePlayerPawn = null;
		GameSelections.hasMoved = false;
		GameSelections.curAction = null;

		TouchManager.Instance.ClearSelectionsAndUI ();
		IncrementTurn ();
	}

	public void DisplayTurnToken(Ownership turn) {
	}
}
