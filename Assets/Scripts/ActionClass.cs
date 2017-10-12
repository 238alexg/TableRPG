using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for executing Pawn actions.
/// </summary>
public class ActionClass : MonoBehaviour {

	public string actionName;
	public PawnClass pawn;
    public List<TileContainer> tileOptions { get; protected set; }

	void Start() {
		tileOptions = new List<TileContainer> ();
	}

	public virtual void OnButtonPress() {
		GameSelections.CurAction = this;
		print("Action button pressed!");
	}

	public virtual void OnExecution(TileContainer tile) {
        print("Action executed on " + tile.Tile.name);

		GameSelections.HasMoved = true;
		GameSelections.PawnMovesLeft = 0;
		GameSelections.HasCompletedTurn = true;

	}

}

/// <summary>
/// Pawn attack action, subclass of ActionClass.
/// </summary>
public class AttackAction : ActionClass {

	public int damage;
	public float hitChance;

	/// <summary>
	/// Initializes a new instance of the <see cref="AttackAction"/> class.
	/// </summary>
	public virtual void OnHit() {

	}

	/// <summary>
	/// Checks if the hit lands.
	/// </summary>
	/// <returns><c>true</c>, if hit lands, <c>false</c> for misses.</returns>
	public virtual bool CheckHitChance() {
		return (hitChance > Random.Range (0, 101));
	}

}