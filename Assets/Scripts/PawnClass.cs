using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnClass : MonoBehaviour {

	// Turn-inspecific stats
	public string pawnName, description;
	public float maxHealth, attack, defense;
	public byte moveCount;
	public Animator anim;
	public Sprite UISprite;
	public Level level;
	public int enemiesKilled, veteranKillsNeeded, masterKillsNeeded;

	public List<ActionClass> actions; // TODO: Implement the ActionClass class!
	public List<System.Type> tilesAllowedToWalkOver = new List<System.Type>() { typeof(WalkableTile) };

	// Turn-dynamic stats
	[System.NonSerialized]
	public float health, movesLeft;
	public int x, y;

	[System.NonSerialized]
	public Ownership ownership;

	protected virtual void Start() {
		health = maxHealth;
	}

	public void SelectPawn() {
		GameSelections.activePlayerPawn = this;
	}

	/// <summary>
	/// Hurts the pawn an amount of damange. Kills pawn at 0 health.
	/// </summary>
	/// <param name="damage">Damage.</param>
	public virtual void HurtPawn(float damage, PawnClass attacker) {
		health -= damage;

		if (health <= 0) {
			KillPawn ();
			attacker.UpdateKillCount (this);
		}
	}

	// TODO: Use modifiers to see the real value of stats with added abilities/effects, etc.
	/// <summary>
	/// Returns a string containing all the basic stat information of the pawn.
	/// </summary>
	public virtual string GetPawnStats() {
		string healthLine = "Health: " + (int)health + " / " + (int)maxHealth + System.Environment.NewLine;
		string atkLine = "Atk: " + (int)attack + System.Environment.NewLine;
		string defLine = "Def: " + (int)defense + System.Environment.NewLine;
		string abilitiesLine = "Abilities: " + System.Environment.NewLine;
		string actionsLine = "Actions: " + System.Environment.NewLine;

		foreach (ActionClass ac in actions) {
			if (ac is AttackAction) {
				abilitiesLine += ac.actionName + System.Environment.NewLine;
			} else {
				abilitiesLine += ac.actionName + System.Environment.NewLine;
			}
		}

		foreach (System.Type t in tilesAllowedToWalkOver) {
			if (t != typeof(WalkableTile)) {
				abilitiesLine += t.ToString () + " Walking" + System.Environment.NewLine;
			}
		}

		return (healthLine + atkLine + defLine + actionsLine + abilitiesLine);
	}

	/// <summary>
	/// Kills the pawn, and removes it from the scene
	/// </summary>
	public virtual void KillPawn() {
		GameStateManager.instance.tiles [x, y].curPawn = null;
		Destroy (this.gameObject);
	}

	/// <summary>
	/// Determines whether this pawn can move on the specified tile.
	/// </summary>
	/// <returns><c>true</c> if this instance can walk on the tile; otherwise, <c>false</c>.</returns>
	/// <param name="tile">Tile to be walked on.</param>
	public virtual bool PawnCanWalkOnTile(TileClass tile) {
		return (tile is WalkableTile && tile.curPawn == null);
	}

	/// <summary>
	/// Updates the kill count of the pawn, upgrading them if necessary
	/// </summary>
	/// <param name="killed">The killed pawn.</param>
	public virtual void UpdateKillCount(PawnClass killed) {
		switch (killed.level) {
		case Level.Novice:
			this.enemiesKilled++;
			break;
		case Level.Veteran:
			this.enemiesKilled += 2;
			break;
		case Level.Master:
			this.enemiesKilled += 5;
			break;
		}

		// Upgrade unit level if necessary
		if (level == Level.Novice && enemiesKilled >= veteranKillsNeeded) {
			this.level = Level.Veteran;
			// TODO: Medal/confetti animation
			// TODO: Add ability to list
		} else if (level == Level.Veteran && enemiesKilled >= masterKillsNeeded) {
			this.level = Level.Master;
			// TODO: Medal/confetti animation
			// TODO: Add ability to list
		}
	}

	public enum Level {
		Novice,
		Veteran,
		Master
	}
}

public enum Ownership {
	LoneAI,
	Player1,
	Player2,
}
