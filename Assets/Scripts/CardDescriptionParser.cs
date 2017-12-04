using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CardDescriptionParser : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		StreamReader reader = File.OpenText("Assets/CardDescriptions.txt");
		string line;
		char [] seperators = new char[1]{':'};
		CardClass curClass = CardClass.None;

		while ((line = reader.ReadLine()) != null) {
			if (line.Length == 0) { continue; }

			if (line [0] == '<') 
			{
				if (curClass != CardClass.None) { curClass = CardClass.None; } 
				else { curClass = DetermineCardClass (line.Substring (1, line.Length - 2)); }
				continue;
			}

			Card newCard = new Card();
			string [] tokens = line.Split (seperators);
			newCard.Name = tokens[0];
			newCard.Description = tokens[1];
			newCard.Class = curClass;
			newCard.Count = -1;
			AddCardToClassList (newCard, curClass);
		}

		DeckStorage.Inst.Init ();
	}

	CardClass DetermineCardClass(string className) 
	{
		switch (className) 
		{
		case "Knight":
			return CardClass.Knight;
		case "Devil":
			return CardClass.Devil;
		case "Pirate":
			return CardClass.Pirate;
		case "Nymph":
			return CardClass.Nymph;
		case "Dwarf":
			return CardClass.Dwarf;
		case "Required":
			return CardClass.Gold;
		case "Generic":
			return CardClass.Generic;
		default:
			return CardClass.None;
		}
	}

	void AddCardToClassList(Card card, CardClass cardClass) {
		switch (cardClass) {
		case CardClass.Knight:
			DeckStorage.KnightCards.Add (card);
			return;
		case CardClass.Devil:
			DeckStorage.DevilCards.Add (card);
			return;
		case CardClass.Pirate:
			DeckStorage.PirateCards.Add (card);
			return;
		case CardClass.Nymph:
			DeckStorage.NymphCards.Add (card);
			return;
		case CardClass.Dwarf:
			DeckStorage.DwarfCards.Add (card);
			return;
		case CardClass.Generic:
			DeckStorage.GenericCards.Add (card);
			return;
		case CardClass.Gold:
			DeckStorage.GoldCards.Add (card);
			return;
		default:
			return;
		}
	}


}

