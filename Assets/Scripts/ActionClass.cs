using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for executing Pawn actions.
/// </summary>
public class ActionClass : MonoBehaviour {

	public string actionName;
	public PawnClass pawn;
	public List<TileClass> tileOptions { get; protected set; }

	void Start() {
		tileOptions = new List<TileClass> ();
	}

	public virtual void OnButtonPress() {
		GameSelections.curAction = this;
		UIManager.instance.moveButton.interactable = false;
		UIManager.instance.moveButtonText.text = "DONE";
		print("Action button pressed!");
	}

	public virtual void OnExecution(TileClass tile) {
		print("Action executed on " + tile.tileName);
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