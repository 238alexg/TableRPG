using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckUI : MonoBehaviour {

	public static DeckUI Inst;

	public GameObject CardDetailUI;
	public Image CardDetailBackground;
	public Text CardDetailName, CardDetailDescription, DeckSizeCount;
	public Button FinishDeckButton;

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

	public void InitializeBookmarks(List<Card> cards, bool isLibrary) 
	{
		List<CardBookmark> CardBookmarks = isLibrary ? CardLibraryBookmarks : DeckListBookmarks;

		for (int i = 0; i < cards.Count; i++)
		{
			CardBookmark bookmarkToUpdate = (i >= CardBookmarks.Count) 
				? CreateNewCardBookmark (i, isLibrary) 
				: CardBookmarks [i];
			UpdateCardBookmark (cards[i], bookmarkToUpdate, isLibrary);
		}

		if (cards.Count < CardBookmarks.Count) {
			for (int i = cards.Count; i < CardBookmarks.Count; i++) 
			{
				CardBookmarks[i].gameObject.SetActiveIfChanged(false);
			}
		}

		if (!isLibrary)
		{
			UpdateDeckSizeCount();
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
		cardBookmark.Background.color = card.ClassColor;

		cardBookmark.Button.onClick.RemoveAllListeners ();
		if (isLibrary) 
		{
			cardBookmark.Button.onClick.AddListener (() => LoadCardIntoUI (card));
		} 
		else 
		{
			cardBookmark.Button.onClick.AddListener (() => RemoveCardFromPlayerDeckList (card));
		}

		cardBookmark.DoubleCount.gameObject.SetActiveIfChanged (card.Count > 1);
	}

	public void LoadCardIntoUI(Card card) {
		CardDetailName.text = card.Name;
		CardDetailDescription.text = card.Description;
		Color baseColor = card.ClassColor;
		baseColor.a = 0.3f;
		CardDetailBackground.color = baseColor;

		CurrentSelectedCard = card;

		CardDetailUI.SetActiveIfChanged (true);
	}

	public void TryAddCardButtonPress()
	{
		Card addedCard = DeckStorage.Inst.CurrentPlayerDeck.TryAddCardToDeck (CurrentSelectedCard);

		if (addedCard == null)
		{
			throw new UnityException("Card limit reached! Duplicate card already exists or deck is full.");
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
					break;
				}
			}
		}
		UpdateDeckSizeCount ();
	}

	public void UpdateDeckSizeCount()
	{
		int deckSize = DeckStorage.Inst.CurrentPlayerDeck.Size;
		string deckCountText = "Deck Count: " + deckSize + "/" + Deck.DeckSizeLimit;
		if (DeckSizeCount.text != deckCountText) 
		{
			DeckSizeCount.text = deckCountText;
		}
		FinishDeckButton.interactable = (deckSize == Deck.DeckSizeLimit);
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
		if (card.Class == CardClass.Gold) { return; }

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
				DeckStorage.Inst.CurrentPlayerDeck.RemoveCardFromDeck (card);
				UpdateDeckSizeCount ();
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

	public void FinishBuildingDeck()
	{
		DeckStorage.isPlayerOnePicking = !DeckStorage.isPlayerOnePicking;
		ClassChoiceUI.Inst.BackButtonPress ();
	}
}