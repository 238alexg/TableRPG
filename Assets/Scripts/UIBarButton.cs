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
		Debug.Log("Card button press!");
	}

	void PawnButtonPress()
	{
		Debug.Log("Pawn button press!");
	}

	void ShopButtonPress()
	{
		Debug.Log("Shop button press!");
	}

	void SettingsButtonPress()
	{
		Debug.Log("Settings button press!");
	}

	void EndTurnButtonPress()
	{
		Debug.Log("End turn button press!");
	}

	void StatsButtonPress()
	{
		Debug.Log("Stats button press!");
	}

	void MoveButtonPress()
	{
        MovementManager.instance.ShowPawnMoveOptions();
	}

	void ActionButtonPress()
	{
		Debug.Log("Action button press!");
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
