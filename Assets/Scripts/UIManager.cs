using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager instance;

	public Animator actionBarUI;

	public Text actionBarPawnName, opponentPawnName;

	public Image actionBarPawnImage;

	public Button moveButton, actionButton, statsButton, cardsButton, backButton, performMoveButton;

	private List<GameObject> activeUI = new List<GameObject>();

	void Awake() {
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}
	}
		
	// Slide Action Bar UI in/out
	public void SlideActionBar(bool slideIn, PawnClass pawn = null) {
		if (actionBarUI.GetBool ("SlideIn") && slideIn) {
			// Just replace values in action bar
			// Do not reanimate action bar slide
			loadPawnIntoActionBar (pawn);
		} else if (slideIn) {
			loadPawnIntoActionBar (pawn);
			actionBarUI.SetBool ("SlideIn", true);
			activeUI.Add (actionBarUI.gameObject);
		} else {
			actionBarUI.SetBool ("SlideIn", false);
			activeUI.Remove (actionBarUI.gameObject);
		}
	}

	// Loads pawn info into the UI
	public void loadPawnIntoUI(PawnClass pawn) {
		if (GameStateManager.instance.turn == pawn.ownership) {
			actionBarPawnName.text = pawn.pawnName;
		} else {
			actionBarPawnName.text = pawn.pawnName;
		}
	}

	// Clears all active UI
	public void ClearAllActiveUI() {
		foreach (GameObject ui in activeUI) {
			Animator anim = ui.GetComponent <Animator> ();

			if (anim == null) {
				Debug.Log (">> UI Animator not present!");
			} else {
				anim.SetBool ("SlideIn", false);
			}
		}
		activeUI.Clear ();
	}

	private void loadPawnIntoActionBar(PawnClass pawn) {
		actionBarPawnName.text = pawn.pawnName;
		actionBarPawnImage.sprite = pawn.UISprite;
	}
}
