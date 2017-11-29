using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckStorage : MonoBehaviour {

	public static DeckStorage Inst;

	public List<Card> KnightCards = new List<Card>();
	public List<Card> DevilCards = new List<Card>();
	public List<Card> PirateCards = new List<Card>();
	public List<Card> NymphCards = new List<Card>();
	public List<Card> DwarfCards = new List<Card>();
	public List<Card> GoldCards = new List<Card>();
	public List<Card> GenericCards = new List<Card>();

	public List<Card> PlayerOneDeck = new List<Card>();
	public List<Card> PlayerTwoDeck = new List<Card>();
	public List<Card> PlayerOneHand = new List<Card>();
	public List<Card> PlayerTwoHand = new List<Card>();
	public List<Card> PlayerOneDiscard = new List<Card>();
	public List<Card> PlayerTwoDiscard = new List<Card>();

	public static bool isPlayerOnePicking = true;
	public List<Card> CurrentPlayerDeck 
	{
		get { return isPlayerOnePicking ? PlayerOneDeck : PlayerTwoDeck; }
	}

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

	public void Init() 
	{
		const int goldCount = 2;
		foreach (Card goldCard in GoldCards) 
		{
			PlayerOneDeck.Add (goldCard.Clone (goldCount));
			PlayerTwoDeck.Add (goldCard.Clone (goldCount));
		}

		DeckUI.Inst.SetCardLibraryBookmarks (KnightCards);
	}

	public Card TryAddCardToDeck(Card card) {

		const int multipleCardLimit = 2;

		Card existingCard = FindExistingCard (CurrentPlayerDeck, card);

		if (existingCard == null) { 
			Card newDeckCard = card.Clone (1);
			CurrentPlayerDeck.Add (newDeckCard);
			return newDeckCard;
		}

		else if (existingCard.Count < multipleCardLimit) 
		{
			Card newDeckCard = existingCard.Clone (++existingCard.Count);
			CurrentPlayerDeck.Add (newDeckCard);
			return newDeckCard;
		}
		else 
		{
			return null;
		}
	}

	public void RemoveCardFromDeck(Card card)
	{
		List<Card> deck = CurrentPlayerDeck;

		for (int i = 0; i < deck.Count; i++)
		{
			if (deck[i].Name == card.Name) 
			{
				if (deck[i].Count == 1) 
				{
					deck.RemoveAt (i);
				}
				else // Remove one of duplicate card
				{
					for (int j = i + 1; j < deck.Count; j++) 
					{
						if (deck [j].Name == deck [i].Name) 
						{
							deck [i].Count = 1;
							deck.RemoveAt (j);
						}
					}
				}
			}
		}
	}

	public Card FindExistingCard(List<Card> deck, Card card) 
	{
		foreach (Card deckCard in deck) 
		{
			if (deckCard.Name == card.Name) 
			{
				return deckCard;
			}
		}
		return null;
	}

	public List<Card> RequestClassCards(CardClass cardClass)
	{
		switch (cardClass) {
		case CardClass.Gold: 	return GoldCards;
		case CardClass.Generic: return GenericCards;
		case CardClass.Knight: 	return KnightCards;
		case CardClass.Devil: 	return DevilCards;
		case CardClass.Pirate: 	return PirateCards;
		case CardClass.Nymph: 	return NymphCards;
		case CardClass.Dwarf: 	return DwarfCards;
		default: 				return null;
		}
	}
}

public class Card 
{
	public string Name;
	public string Description;
	public CardClass Class;
	public int Count;
	public int Index;

	public Card Clone(int count = -1) 
	{
		Card clone = new Card();
		clone.Name = Name;
		clone.Description = Description;
		clone.Class = Class;
		clone.Count = (count == -1) ? Count : count;
		return clone;
	}
}

public enum CardClass 
{
	None,
	Gold,
	Generic,
	Knight,
	Devil,
	Pirate,
	Nymph,
	Dwarf,
	Count
}