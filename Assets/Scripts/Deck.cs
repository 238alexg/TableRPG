using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck {

	public const int DeckSizeLimit = 30;
	public const int MultipleCardLimit = 2;

	public List<Card> Cards;

	bool dirtyCachedSize = true;
	int cachedDeckSize = 0;
	public int Size { 
		get 
		{ 
			if (dirtyCachedSize) 
			{
				cachedDeckSize = GetCardCount ();
				dirtyCachedSize = false;
			}
			return cachedDeckSize;
		} 
	}

	public Deck()
	{
		Cards = new List<Card> (DeckSizeLimit);
	}

	int GetCardCount()
	{
		int deckSize = 0;
		foreach (Card card in Cards) 
		{
			deckSize += card.Count;
		}
		return deckSize;
	}

	public Card GetDrawCard()
	{
		int drawCardIndex = Random.Range (0, Cards.Count);
		Card drawCard = Cards [drawCardIndex];
		RemoveCardFromDeck (drawCardIndex);
		return drawCard;
	}

	public void AddRequiredCards()
	{
		Cards.AddRange(DeckStorage.GetRequiredCardClones());
	}

	public void Clear()
	{
		dirtyCachedSize = true;
		Cards.Clear ();
		AddRequiredCards ();
	}

	public Card FindExistingCard(Card card) 
	{
		foreach (Card deckCard in Cards) 
		{
			if (deckCard.Name == card.Name) 
			{
				return deckCard;
			}
		}
		return null;
	}

	public Card TryAddCardToDeck(Card card, bool force = false) {

		if (Size >= DeckSizeLimit) { return null; }

		Card existingCard = FindExistingCard (card);

		if (existingCard == null) { 
			Card newDeckCard = card.Clone (1);
			Cards.Add (newDeckCard);
			dirtyCachedSize = true;
			return newDeckCard;
		}

		else if (existingCard.Count < MultipleCardLimit || force) 
		{
			existingCard.Count++;
			dirtyCachedSize = true;
			return existingCard;
		}
		else 
		{
			return null;
		}
	}

	public void RemoveCardFromDeck(Card card)
	{
		for (int i = 0; i < Cards.Count; i++)
		{
			if (Cards[i].Name == card.Name) 
			{
				RemoveCardFromDeck (i);
			}
		}
	}

	public void RemoveCardFromDeck(int cardIndex)
	{
		if (Cards[cardIndex].Count > 1) { Cards [cardIndex].Count--; }
		else { Cards.RemoveAt (cardIndex); }
		dirtyCachedSize = true;
	}
}
