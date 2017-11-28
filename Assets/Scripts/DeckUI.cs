using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckUI : MonoBehaviour {

	public static DeckUI Inst;

	public GameObject CardDetailUI;
	public Image CardDetailBackground;
	public Text CardDetailName;
	public Text CardDetailDescription;

	public GameObject CardBookmarkPrefab;

	public Transform CardLibrary;
	[System.NonSerialized] public List<CardBookmark> CardLibraryBookmarks = new List<CardBookmark>();

	public Transform DeckList;
	[System.NonSerialized] public List<CardBookmark> DeckListBookmarks = new List<CardBookmark>();

	[System.NonSerialized] public Card CurrentSelectedCard;

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

	public void SetCardLibraryBookmarks(List<Card> cards) {

		for (int i = 0; i < cards.Count; i++)
		{
			CardBookmark bookmarkToUpdate = (i >= CardLibraryBookmarks.Count) 
				? CreateNewCardBookmark (i, CardLibrary) 
				: CardLibraryBookmarks [i];
			UpdateCardBookmark (cards[i], bookmarkToUpdate);
		}

		if (cards.Count < CardLibraryBookmarks.Count) {
			for (int i = cards.Count; i < CardLibraryBookmarks.Count; i++) 
			{
				CardLibraryBookmarks[i].gameObject.SetActiveIfChanged(false);
			}
		}
	}

	CardBookmark CreateNewCardBookmark(int index, Transform scrollTransform) 
	{
		GameObject newBookmarkGameObject = Instantiate (CardBookmarkPrefab, scrollTransform);
		CardBookmark newCardBookmark = newBookmarkGameObject.GetComponent <CardBookmark>();

		if (newCardBookmark == null) 
		{
			throw new UnityException ("Instantiated Card Bookmark without CardBookmark component attached!");
		} 
		else 
		{ 
			newCardBookmark.Initialize (index);
			CardLibraryBookmarks.Add (newCardBookmark); 
		}
		return newCardBookmark;
	}

	void UpdateCardBookmark(Card card, CardBookmark cardBookmark) 
	{
		cardBookmark.CardName.text = card.Name;
		cardBookmark.CardDescription.text = card.Description;
		cardBookmark.Card = card;

		cardBookmark.Button.onClick.AddListener (() => LoadCardIntoUI (card));
	}

	public void LoadCardIntoUI(Card card) {
		CardDetailName.text = card.Name;
		CardDetailDescription.text = card.Description;
		CardDetailBackground.color = GetColorFromCardClass (card.Class);

		CurrentSelectedCard = card;

		CardDetailUI.SetActiveIfChanged (true);
	}

	public void TryAddCardButtonPress()
	{
		if (DeckStorage.Inst.TryAddCardToDeck (CurrentSelectedCard)) 
		{
			AddCardToPlayerDeckList ();
		}
		else 
		{
			throw new UnityException("Maximum amount of card already present in deck!");
		}
	}

	void AddCardToPlayerDeckList()
	{
		List<Card> PlayerDeck = DeckStorage.isPlayerOnePicking ? DeckStorage.Inst.PlayerOneDeck : DeckStorage.Inst.PlayerTwoDeck;
		CardBookmark newBookmark = CreateNewCardBookmark (DeckList.childCount, DeckList);
		UpdateCardBookmark (CurrentSelectedCard, newBookmark);
	}

	Color GetColorFromCardClass(CardClass cardClass)
	{
		switch (cardClass) {
		case CardClass.Gold: 	return Color.yellow;
		case CardClass.Generic: return Color.grey;
		case CardClass.Knight: 	return Color.blue;
		case CardClass.Devil: 	return Color.magenta;
		case CardClass.Pirate: 	return Color.cyan;
		case CardClass.Nymph: 	return Color.green;
		case CardClass.Dwarf: 	return Color.red;
		default: 				return Color.white;
		}
	}
}