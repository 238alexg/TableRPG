﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager instance;

	public Animator actionBarUI, abilitiesBarUI, friendlyStatsUI;

	public Text actionBarPawnName, opponentPawnName;

	public Image actionBarPawnImage, friendlyStatImage;

	public Button moveButton, actionButton, friendlyStatButton;
	public Text moveButtonText, friendlyStatPawnName, friendlyStatText;

	public List<Button> actionButtons;
	public List<Text> actionButtonTexts;

	private List<GameObject> activeUI = new List<GameObject>();

	void Awake() {
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}
	}

	/// <summary>
	/// Called when a user presses the action button with a pawn selected.
	/// </summary>
	public void ActionButtonPress() {

		actionBarUI.SetBool ("SlideUp", true);
		abilitiesBarUI.SetBool ("SlideIn", true);

		PawnClass pawn = GameSelections.activePlayerPawn;

		actionButtonTexts[0].text = pawn.actions [0].actionName;
//		action2ButtonText.text = pawn.actions [1].actionName;
//		action3ButtonText.text = pawn.actions [2].actionName;

		actionButtons[0].onClick.AddListener (pawn.actions [0].OnButtonPress);

//		if (pawn.level > PawnClass.Level.Novice) {
//			action2Button.interactable = true;
//		} else if (pawn.level > PawnClass.Level.Veteran) {
//			action3Button.interactable = true;
//		}
	}

	public void ActionBackButtonPress() {
		actionBarUI.SetBool ("SlideUp", false);
		abilitiesBarUI.SetBool ("SlideIn", false);
	}

	public void StatsButtonPress() {
		PawnClass pawn = GameSelections.activePlayerPawn;

		friendlyStatText.text = pawn.GetPawnStats ();
		friendlyStatPawnName.text = pawn.pawnName;
		friendlyStatImage.sprite = pawn.UISprite;

		actionBarUI.SetBool ("SlideDown", true);
		friendlyStatsUI.SetBool ("SlideIn", true);
	}

	public void StatsBackButtonPress() {
		actionBarUI.SetBool ("SlideDown", false);
		friendlyStatsUI.SetBool ("SlideIn", false);
	}
		
	// Slide Action Bar UI in/out
	public void SlideActionBar(bool slideIn, PawnClass pawn = null) {
		if (actionBarUI.GetBool ("SlideIn") && slideIn) {
			// Just replace values in action bar
			// Do not reanimate action bar slide
			loadPawnIntoActionBar (pawn);
		} else if (slideIn) {
			loadPawnIntoActionBar (pawn);
			moveButtonText.text = GameSelections.hasMoved ? "DONE" : "MOVE";
			moveButton.interactable = !GameSelections.hasMoved;
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
