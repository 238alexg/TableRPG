using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardBookmark : MonoBehaviour {
	public Button Button;
	public Text CardName;
	public Text CardDescription;
	public Text DoubleCount;
	public Image Background;
	public int Index;
	public Card Card;

	public void Initialize(int index) 
	{
		Index = index;
	}

	public void CopyFrom(Card card)
	{
		CardName.text = card.Name;
		CardDescription.text = card.Description;
		Card = card;
		Background.color = card.ClassColor;
	}
}
