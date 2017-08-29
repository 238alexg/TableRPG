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

//	public List<ActionClass> actions; // TODO: Implement the ActionClass class!

	// Turn-dynamic stats
	[System.NonSerialized]
	public float health, XP, movesLeft;
	public int x, y;

	[System.NonSerialized]
	public Ownership ownership;

	public enum Ownership {
		LoneAI,
		Player1,
		Player2,
	}

	public void SelectPawn() {
		UIManager.instance.loadPawnIntoUI (this);
	}
}
