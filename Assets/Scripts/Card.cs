using System;
using UnityEngine;

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

	public Color ClassColor {
		get {
			switch (Class) {
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
}
