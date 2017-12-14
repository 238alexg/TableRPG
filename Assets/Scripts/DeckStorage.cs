using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckStorage : MonoBehaviour {

	public static DeckStorage Inst;

	public const int MultipleCardLimit = 2;
	const int goldCount = 2;

	public static List<Card> KnightCards = new List<Card>();
	public static List<Card> DevilCards = new List<Card>();
	public static List<Card> PirateCards = new List<Card>();
	public static List<Card> NymphCards = new List<Card>();
	public static List<Card> DwarfCards = new List<Card>();
	public static List<Card> GoldCards = new List<Card>();
	public static List<Card> GenericCards = new List<Card>();

	public Deck PlayerOneDeck = new Deck ();
	public Deck PlayerTwoDeck = new Deck ();
	public List<Card> PlayerOneHand = new List<Card>();
	public List<Card> PlayerTwoHand = new List<Card>();
	public List<Card> PlayerOneDiscard = new List<Card>();
	public List<Card> PlayerTwoDiscard = new List<Card>();

	public static bool isPlayerOnePicking = true;
	public Deck CurrentPlayerDeck 
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
		PlayerOneDeck.AddRequiredCards ();
		PlayerTwoDeck.AddRequiredCards ();
	}

	public static List<Card> GetRequiredCardClones()
	{
		List<Card> requiredCards = new List<Card> (goldCount * GoldCards.Count);

		foreach (Card goldCard in GoldCards)
		{
			requiredCards.Add (goldCard.Clone (goldCount));
		}

		return requiredCards;
	}

	public void ResetCurrentDeck()
	{
		CurrentPlayerDeck.Clear ();
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