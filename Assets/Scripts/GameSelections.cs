using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class containing static variables that change frequently across the course of a turn
/// </summary>
public class GameSelections : MonoBehaviour {

	public static GameSelections instance;

	[System.NonSerialized]
	public static PawnClass activePlayerPawn;
	public static byte pawnMovesLeft;
	public static bool hasMoved;
	public static bool hasCompletedTurn;
	public static ActionClass curAction;
	public static TileClass selectedTile;
}
