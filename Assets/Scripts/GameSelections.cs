using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class containing static variables that change frequently across the course of a turn
/// </summary>
public static class GameSelections {
	[System.NonSerialized]
	public static PawnClass activePlayerPawn;
	public static byte pawnMovesLeft;
	public static bool hasMoved, hasCompletedTurn;
	public static ActionClass curAction;
    public static TileContainer selectedTile;
}
