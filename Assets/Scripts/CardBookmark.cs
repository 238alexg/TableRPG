using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardBookmark : MonoBehaviour {
	public Button Button;
	public Text CardName;
	public Text CardDescription;
	public int Index;
	public Card Card;

	public void Initialize(int index) 
	{
		Index = index;
	}
}
