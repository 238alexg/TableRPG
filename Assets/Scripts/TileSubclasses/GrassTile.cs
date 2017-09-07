using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassTile : WalkableTile {

	public override void OnWalkOver () {
		print ("Walked on a grass tile!");
	}

	public override void Select() {
		base.Select ();
	}
}
