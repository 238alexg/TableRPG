﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClass : MonoBehaviour {

	public string tileName;
	public Sprite sprite;
	public PawnClass curPawn = null;

	public virtual void Select() {
		print ("Selected a " + tileName + " tile");

        if (curPawn != null) 
        {
            curPawn.SelectPawn();
        }
        else 
        {
            UIManager.Instance.DeselectPawn();
        }
	}

	// When pawn walks over tile, if effect happens
	public virtual void OnWalkOver () {
		print("Player walked on a " + tileName + " tile");
	}

	
}

public class WalkableTile : TileClass {
	

	// Load curPawn into UI
	public override void Select() {
        base.Select();
	}



	// Called when a pawn is spawned on this tile
	public virtual void PlacePawn (PawnClass placedPawn) {
		curPawn = placedPawn;
		curPawn.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
	}
}

public class NonWalkableTile : TileClass {

	public override void Select() {
	
    }
}
