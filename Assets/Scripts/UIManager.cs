using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager Instance;

    [Header("Scroll Bar UI")]
    public GameObject UIRoot;
	public GameObject MenuBar;
    public GameObject ScrollbarContent;
    public List<UIBarButton> UIBarButtons;

	void Awake() {
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy (this);
		}
	}

    void Start() {
        for (int i = 0; i < UIBarButtons.Count; i++) {
            UIBarButtons[i].transform.SetParent(ScrollbarContent.transform);
        }
    }

    /// <summary>
    /// Called when a user presses the action button with a pawn selected.
    /// </summary>
    public void ActionButtonPress() {
        PawnClass pawn = GameSelections.activePlayerPawn;
	}

	public void StatsButtonPress() {
		PawnClass pawn = GameSelections.activePlayerPawn;
	}
}

[System.Serializable]
public class UIBarButton : MonoBehaviour {
    public ButtonType Type;
    public Button Button;
    public Image Background;
    public Color32 BackgroundColor;
    public Text Text;

    public void Initialize(ButtonType Type, Button b, Image bkgd, Color32 bkgdColor, Text t, GameObject parent)
    {
        Button = b;
        Background = bkgd;
        BackgroundColor = bkgdColor;
        Text = t;

        Background.color = BackgroundColor;
        InitButtonFunctionality();
    }

    public virtual void InitButtonFunctionality() {

        UnityEngine.Events.UnityAction UAction;

        UAction = MoveButtonPress;

        UAction = (this.Type == ButtonType.MoveButton) ? (UnityEngine.Events.UnityAction)MoveButtonPress : ActionButtonPress;

        if (Type == ButtonType.MoveButton) {
            Button.onClick.AddListener(MoveButtonPress);
        } else if (Type == ButtonType.ActionButton) {
            Button.onClick.AddListener(MoveButtonPress);
        } else if (Type == ButtonType.SettingsButton) {
            
        }
    }

    void CardButtonPress() {
        Debug.Log("Move button press!");
    }

    void PawnButtonPress() {
        Debug.Log("Move button press!");
    }

    void ShopButtonPress() {
        Debug.Log("Move button press!");
    }

    void SettingsButtonPress() {
        Debug.Log("Move button press!");
    }

    void EndTurnButtonPress() {
        Debug.Log("Move button press!");
    }

    void StatsButtonPress() {
        Debug.Log("Move button press!");
    }

    void MoveButtonPress() {
        Debug.Log("Move button press!");
    }

    void ActionButtonPress() {
        Debug.Log("Move button press!");
    }

    public enum ButtonType {
        MoveButton,
        ActionButton,
        StatsButton,
        SettingsButton
    }
}
