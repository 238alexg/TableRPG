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
				? CreateNewCardBookmark (index: i, isLibrary: true) 
				: CardLibraryBookmarks [i];
			UpdateCardBookmark (cards[i], bookmarkToUpdate, isLibrary: true);
		}

		if (cards.Count < CardLibraryBookmarks.Count) {
			for (int i = cards.Count; i < CardLibraryBookmarks.Count; i++) 
			{
				CardLibraryBookmarks[i].gameObject.SetActiveIfChanged(false);
			}
		}
	}

	public void SetDeckStarterCards()
	{
		List<Card> deck = DeckStorage.Inst.CurrentPlayerDeck;
		for (int i = 0; i < deck.Count; i++) 
		{
			AddCardToPlayerDeckList (deck[i]);
		}
	}

	CardBookmark CreateNewCardBookmark(int index, bool isLibrary) 
	{
		Transform scrollTransform = isLibrary ? CardLibrary : DeckList;
		GameObject newBookmarkGameObject = Instantiate (CardBookmarkPrefab, scrollTransform);
		CardBookmark newCardBookmark = newBookmarkGameObject.GetComponent <CardBookmark>();

		if (newCardBookmark == null) 
		{
			throw new UnityException ("Instantiated Card Bookmark without CardBookmark component attached!");
		} 
		else 
		{ 
			newCardBookmark.Initialize (index);
			List<CardBookmark> bookmarkList = isLibrary ? CardLibraryBookmarks : DeckListBookmarks;
			bookmarkList.Add (newCardBookmark); 
		}
		return newCardBookmark;
	}

	void UpdateCardBookmark(Card card, CardBookmark cardBookmark, bool isLibrary) 
	{
		cardBookmark.CardName.text = card.Name;
		cardBookmark.CardDescription.text = card.Description;
		cardBookmark.Card = card;
		cardBookmark.Background.color = GetColorFromCardClass (card.Class);

		cardBookmark.Button.onClick.RemoveAllListeners ();
		if (isLibrary) 
		{
			cardBookmark.Button.onClick.AddListener (() => LoadCardIntoUI (card));
		} 
		else 
		{
			cardBookmark.Button.onClick.AddListener (() => RemoveCardFromPlayerDeckList (card));
		}

	}

	public void LoadCardIntoUI(Card card) {
		CardDetailName.text = card.Name;
		CardDetailDescription.text = card.Description;
		Color baseColor = GetColorFromCardClass (card.Class);
		baseColor.a = 0.3f;
		CardDetailBackground.color = baseColor;

		CurrentSelectedCard = card;

		CardDetailUI.SetActiveIfChanged (true);
	}

	public void TryAddCardButtonPress()
	{
		Card addedCard = DeckStorage.Inst.TryAddCardToDeck (CurrentSelectedCard);

		if (addedCard == null)
		{
			throw new UnityException("Maximum amount of card already present in deck!");
		}
		else if (addedCard.Count == 1) 
		{
			AddCardToPlayerDeckList (addedCard);
		}
		else 
		{
			foreach (CardBookmark bookmark in DeckListBookmarks) 
			{
				if (bookmark.CardName.text == addedCard.Name) 
				{
					bookmark.DoubleCount.gameObject.SetActiveIfChanged (true);
				}
			}

		}
	}

	CardBookmark FindOrCreateEmptyBookmark(bool isLibrary)
	{
		List<CardBookmark> scrollList = isLibrary ? CardLibraryBookmarks : DeckListBookmarks;

		foreach (CardBookmark bookmark in scrollList) 
		{
			if (!bookmark.gameObject.activeInHierarchy) {
				bookmark.gameObject.SetActive (true);
				return bookmark;
			}
		}

		return CreateNewCardBookmark (scrollList.Count, isLibrary);
	}

	void AddCardToPlayerDeckList(Card card)
	{
		CardBookmark newBookmark = FindOrCreateEmptyBookmark (false);
		UpdateCardBookmark (card, newBookmark, isLibrary: false);
	}

	void RemoveCardFromPlayerDeckList(Card card)
	{
		foreach (CardBookmark bookmark in DeckListBookmarks) 
		{
			if (bookmark.Card.Name == card.Name) 
			{
				if (bookmark.Card.Count > 1) 
				{
					bookmark.DoubleCount.gameObject.SetActiveIfChanged (false);
				} 
				else 
				{
					RemoveBookmarkFromList (bookmark, DeckListBookmarks);
				}
				DeckStorage.Inst.RemoveCardFromDeck (card);
				return;
			}
		}
	}

	void RemoveBookmarkFromList(CardBookmark bookmark, List<CardBookmark> list)
	{
		// Move to end of list
		list.Remove (bookmark);
		list.Add (bookmark);

		bookmark.transform.SetAsLastSibling ();
		bookmark.gameObject.SetActiveIfChanged (false);
	}

	Color GetColorFromCardClass(CardClass cardClass)
	{
		switch (cardClass) {
		case CardClass.Gold: 	return Color.white;
		case CardClass.Generic: return Color.grey;
		case CardClass.Knight: 	return Color.blue;
		case CardClass.Devil: 	return Color.magenta;
		case CardClass.Pirate: 	return Color.cyan;
		case CardClass.Nymph: 	return Color.green;
		case CardClass.Dwarf: 	return Color.yellow;
		default: 				return Color.white;
		}
	}
}