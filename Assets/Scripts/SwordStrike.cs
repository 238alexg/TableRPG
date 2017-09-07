using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordStrike : AttackAction {

	/// <summary>
	/// Locates targets for a sword strike.
	/// </summary>
	public override void OnButtonPress () {

		base.OnButtonPress ();

		List<TileClass> adjacentTiles = new List<TileClass> ();

		// Check left boundry
		if (pawn.x > 0) {
			adjacentTiles.Add (GameStateManager.instance.tiles[pawn.x-1, pawn.y]);
		}
		// Check right boundry
		if (pawn.x < GameStateManager.instance.xSize - 1) {
			adjacentTiles.Add (GameStateManager.instance.tiles[pawn.x+1, pawn.y]);
		}
		// Check top boundry
		if (pawn.y > 0) {
			adjacentTiles.Add (GameStateManager.instance.tiles[pawn.x, pawn.y-1]);
		}
		// Check bottom boundry
		if (pawn.y < GameStateManager.instance.ySize - 1) {
			adjacentTiles.Add (GameStateManager.instance.tiles[pawn.x, pawn.y+1]);
		}

		foreach (TileClass tc in adjacentTiles) {
			// There is a pawn here
			PawnClass pawn = tc.curPawn;

			if (pawn != null && pawn.ownership != GameStateManager.instance.turn) {
				tileOptions.Add (tc);
				pawn.GetComponent <SpriteRenderer> ().color = Color.red;
				print ("Sword Strike target found");
			}
		}

		if (tileOptions.Count == 0) {
			print ("No valid targets!");
		}
	}

	/// <summary>
	/// Raises the execution event.
	/// </summary>
	/// <param name="tile">Tile.</param>
	public override void OnExecution (TileClass tile) {

		bool didHit = CheckHitChance ();

		if (!didHit) {
			// TODO: Add miss animation here
			print(pawn.pawnName + " missed!");
		} else {
			PawnClass victim = tile.curPawn;
			victim.HurtPawn (this.damage, attacker: this.pawn);
		}
	}

}
