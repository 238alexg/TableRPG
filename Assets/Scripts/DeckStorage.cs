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

	public bool TryAddCardToDeck(Card card) {

		const int multipleCardLimit = 2;

		List<Card> PlayerCards = isPlayerOnePicking ? PlayerOneDeck : PlayerTwoDeck;

		Card existingCard = FindExistingCard (PlayerCards, card);

		if (existingCard == null) { 
			PlayerCards.Add (card.Clone (1));
			return true;
		}

		else if (existingCard.Count < multipleCardLimit) 
		{
			Card newDeckCard = existingCard.Clone (++existingCard.Count);
			PlayerCards.Add (newDeckCard);
			return true;
		}
		else 
		{
			return false;
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
