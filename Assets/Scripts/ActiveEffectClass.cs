using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveEffectClass : MonoBehaviour {

	public string effectName;
	public byte turnDuration;
	private byte turnsLeft;

	private Ownership turnStarted;

	public virtual void IncrementTurn() {
		if (turnDuration-- < 1) {
			Destroy (this);
		}
	}
}

public class MultiTurnTileEffect : ActiveEffectClass {
//	TileClass targetTile;
//	TileClass effectEndTile;

	// When tile effect finishes, trigger build completion funciton
	public override void IncrementTurn() {
		if (turnDuration-- < 1) {
			OnBuildCompletion ();
			Destroy (this);
		}
	}

	public virtual void OnBuildCompletion () {
//		print ("Tile " + targetTile.tileName + " has turned into tile " + effectEndTile.tileName);
	}
}