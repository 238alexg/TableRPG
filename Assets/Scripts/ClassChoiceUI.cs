using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassChoiceUI : MonoBehaviour {

	public ClassChoiceUI Inst;

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
		List<Card> DeckCards = DeckStorage.Inst.RequestClassCards (cardClass);
		DeckUI.Inst.SetCardLibraryBookmarks (DeckCards);
		gameObject.SetActiveIfChanged (false);
	}

	public void BackButtonPress()
	{
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


