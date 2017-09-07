﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClass : MonoBehaviour {


	public string tileName;
	public Sprite sprite;
	public PawnClass curPawn = null;

	public virtual void TileTap() {

		// Check for existing actions including tile
		if (GameSelections.curAction != null) {
			if (GameSelections.curAction.tileOptions.Contains (this)) {
				GameSelections.curAction.OnExecution (this);
				TouchManager.instance.ClearSelectionsAndUI ();
				TouchManager.instance.ZoomToPoint (false);
			} 
			// User taps outside action area, do nothing
			else {
				// TODO: Remove if nothing happens here
			}
		}

		// Check if pawn can move here, if moving Pawn
		else if (MovementManager.instance.isMovingPawn) {

			// If tile is within selected pawn's movement range, and pawn has movement left, move it
			if (UIManager.instance.moveButton.interactable &&
				MovementManager.instance.walkableTileOptions.Exists (wto => wto.tile == this)) {
				StartCoroutine (MovementManager.instance.MovePawn (this));
				return;
			} 
		} 

		// Check for pawn on Tile
		else if (curPawn != null) {

			// If pawn belongs to player whose turn it is
			if (curPawn.ownership == GameStateManager.instance.turn) {

				if (GameSelections.hasMoved || curPawn == GameSelections.activePlayerPawn) {
					
					if (UIManager.instance.abilitiesBarUI.GetBool ("SlideIn")) {
						// Do nothing, maybe center camera on pawn?
						UIManager.instance.SlideActionBar (true, curPawn);
						print ("Action bar already up");
					} else {
						// Slide in action bar if it went away
						UIManager.instance.SlideActionBar (true, curPawn);
						print ("Action bar NOT already up");
					}
				} else {
					curPawn.SelectPawn ();
				}
			} else {
				// TODO: Load enemy pawn into right UI for tiles/enemies?
			}
		} 

		// User just taps tile
		else {
			// Select tile, clear UI
			TouchManager.instance.ClearSelectionsAndUI ();
		}

		if (GameSelections.selectedTile != this) {
			Select ();
			GameSelections.selectedTile = this;
		}
		StartCoroutine (dimSelection (GameSelections.selectedTile.GetComponent <SpriteRenderer> ()));
	}

	public virtual void Select() {
		print ("Selected a " + tileName + " tile");
	}

	// When pawn walks over tile, if effect happens
	public virtual void OnWalkOver () {
		print("Player walked on a " + tileName + " tile");
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
	

	// Load curPawn into UI
	public override void Select() {
		if (curPawn != null) {
			curPawn.SelectPawn ();
			UIManager.instance.SlideActionBar (slideIn: true, pawn: curPawn);
		} else {
			UIManager.instance.SlideActionBar (slideIn: false, pawn: curPawn);
		}
	}



	// Called when a pawn is spawned on this tile
	public virtual void PlacePawn (PawnClass placedPawn) {
		curPawn = placedPawn;
		curPawn.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
		curPawn.transform.localScale = transform.localScale;
	}
}

public class NonWalkableTile : TileClass {

	public override void Select() {
		UIManager.instance.SlideActionBar (slideIn: false);
	}
}
