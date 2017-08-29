using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClass : MonoBehaviour {

	public static TileClass selectedTile;
	public string tileName;
	public Sprite sprite;

	public virtual void TileTap() {
		if (selectedTile != this) {
			Select ();
			selectedTile = this;
		}
		StartCoroutine (dimSelection (selectedTile.GetComponent <SpriteRenderer> ()));
	}

	public virtual void Select() {
		print ("Selected a " + tileName + " tile");
	}

	// TODO: Add parameters for player class to determine if they can 
	public virtual void AttemptInteration() {
		print ("Attempting interaction with a " + tileName + " tile");
	}

	// Dims the selected tile to show it has been selected
	public IEnumerator dimSelection(SpriteRenderer sr) {

		Color dimColor = sr.color;

		while (sr.color.a > 0.5f) {
			dimColor.a -= 0.04f;
			sr.color = dimColor;
			yield return new WaitForSeconds (0.01f);
		}

		while (sr.color.a < 1) {
			dimColor.a += 0.04f;
			sr.color = dimColor;
			yield return new WaitForSeconds (0.01f);
		}
	}
}

public class WalkableTile : TileClass {
	public PawnClass curPawn = null;

	// Load curPawn into UI
	public override void Select() {
		if (curPawn != null) {
			curPawn.SelectPawn ();
			UIManager.instance.SlideActionBar (slideIn: true, pawn: curPawn);
		} else {
			UIManager.instance.SlideActionBar (slideIn: false, pawn: curPawn);
		}
	}

	// When pawn walks over tile, if effect happens
	public virtual void OnWalkOver () {
		print("Player walked on a " + tileName + " tile");
	}

	// Called when a pawn is spawned on this tile
	public virtual void PlacePawn (PawnClass placedPawn) {
		curPawn = placedPawn;
		curPawn.transform.position = transform.position;
		curPawn.transform.localScale = transform.localScale;
	}
}

public class NonWalkableTile : TileClass {

	public override void Select() {
		UIManager.instance.SlideActionBar (slideIn: false);
	}
}
