using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIBarButton : MonoBehaviour
{
	public ButtonType Type;
	public Button Button;
	public Image Background;
	public Text Text;

	public void Initialize(ButtonType Type, Button b, Image bkgd, Color32 bkgdColor, Text t, GameObject parent)
	{
		Button = b;
		Background = bkgd;
		Text = t;

		SetBackgroundColor(bkgdColor);
		InitButtonFunctionality();
	}

	public void SetBackgroundColor(Color32 color)
	{
		Background.color = color;
	}

	public virtual void InitButtonFunctionality()
	{

		UnityAction UAction;

		UAction = MoveButtonPress;

		UAction = (this.Type == ButtonType.Move) ? (UnityAction)MoveButtonPress :
				(this.Type == ButtonType.Action) ? (UnityAction)ActionButtonPress :
				(this.Type == ButtonType.Stats) ? (UnityAction)StatsButtonPress :
				(this.Type == ButtonType.Settings) ? (UnityAction)SettingsButtonPress :
				(this.Type == ButtonType.Cards) ? (UnityAction)CardButtonPress :
				(this.Type == ButtonType.Pawns) ? (UnityAction)PawnButtonPress :
				(this.Type == ButtonType.Shop) ? (UnityAction)ShopButtonPress :
				(this.Type == ButtonType.EndTurn) ? (UnityAction)EndTurnButtonPress :
				() => Debug.Log("Unimplemented button functionality!");

		Button.onClick.AddListener(UAction);
	}

	void CardButtonPress()
	{
		Debug.Log("Move button press!");
	}

	void PawnButtonPress()
	{
		Debug.Log("Move button press!");
	}

	void ShopButtonPress()
	{
		Debug.Log("Move button press!");
	}

	void SettingsButtonPress()
	{
		Debug.Log("Move button press!");
	}

	void EndTurnButtonPress()
	{
		Debug.Log("Move button press!");
	}

	void StatsButtonPress()
	{
		Debug.Log("Move button press!");
	}

	void MoveButtonPress()
	{
		Debug.Log("Move button press!");
	}

	void ActionButtonPress()
	{
		Debug.Log("Move button press!");
	}

	public enum ButtonType
	{
		Move,
		Action,
		Stats,
		Settings,
		Cards,
		Pawns,
		Shop,
		EndTurn
	}
}
