using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class containing static variables that change frequently across the course of a turn
/// </summary>
public static class GameSelections {
	[System.NonSerialized]
	public static PawnClass ActivePlayerPawn;
	public static byte PawnMovesLeft;
	public static bool HasMoved, HasCompletedTurn;
	public static ActionClass CurAction;
    public static TileContainer SelectedTile;
}
