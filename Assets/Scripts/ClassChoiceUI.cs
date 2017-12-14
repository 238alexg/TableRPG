using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassChoiceUI : MonoBehaviour {

	public static ClassChoiceUI Inst;
	public Text ClassChoicePlayerHeader;

	void Awake() 
	{
		if (Inst == null) 
		{
			Inst = this;
		} 
		else 
		{
			Destroy (this);
		}
	}

	public void ChooseCardClass(string classString)
	{
		CardClass cardClass = StringToCardClass (classString);
		List<Card> classCards = DeckStorage.Inst.RequestClassCards (cardClass);

		List<Card> LibraryCards = new List<Card> (DeckStorage.GenericCards.Count + classCards.Count);
		LibraryCards.AddRange (DeckStorage.GenericCards);
		LibraryCards.AddRange (classCards);

		DeckUI.Inst.InitializeBookmarks (LibraryCards, isLibrary: true); // Initialize library
		DeckUI.Inst.InitializeBookmarks (DeckStorage.Inst.CurrentPlayerDeck.Cards, isLibrary: false); // Initialize deck
		gameObject.SetActiveIfChanged (false);
	}

	public void BackButtonPress()
	{
		ClassChoicePlayerHeader.text = "Player " + (DeckStorage.isPlayerOnePicking ? "1" : "2") + "'s Class";
		DeckStorage.Inst.CurrentPlayerDeck.Clear ();
		DeckUI.Inst.CardDetailUI.SetActiveIfChanged (false);
		DeckUI.Inst.UpdateDeckSizeCount ();
		gameObject.SetActiveIfChanged (true);
	}

	CardClass StringToCardClass(string cardClass)
	{
		switch (cardClass) 
		{
		case "Knight": return CardClass.Knight;
		case "Devil": return CardClass.Devil;
		case "Pirate": return CardClass.Pirate;
		case "Nymph": return CardClass.Nymph;
		case "Dwarf": return CardClass.Dwarf;
		case "Generic": return CardClass.Generic;
		case "Gold": return CardClass.Gold;
		default: return CardClass.None;
		}
	}
}


