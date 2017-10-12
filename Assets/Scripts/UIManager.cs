using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager Instance;

    [Header("Scroll Bar UI")]
    public GameObject UIRoot;
	public GameObject MenuBar;
    public Transform ScrollbarContent;
    public List<UIBarButton> UIBarButtons;
    List<UIBarButton> CurrentAvailableButtons;
    UISelectionState UIState;

    public const float UIBarPercentHeight = 0.16f;

	void Awake() 
    {
		if (Instance == null) { Instance = this; } 
        else { Destroy (this); }
	}

    void Start() 
    {
        CurrentAvailableButtons = new List<UIBarButton>();
        UIState = UISelectionState.NoSelection;
    }

    /// <summary>
    /// Called when a user presses the action button with a pawn selected.
    /// </summary>
    public void ActionButtonPress() 
    {
        PawnClass pawn = GameSelections.ActivePlayerPawn;
	}

	public void StatsButtonPress() 
    {
		PawnClass pawn = GameSelections.ActivePlayerPawn;
	}

    public void LoadSelectedPawn(PawnClass pawn, bool isAlly)
    {
        RemovePreviousState();

        UIState = isAlly ? UISelectionState.AllyPawn : UISelectionState.EnemyPawn;

        // Movement and action buttons
        if (isAlly) {
            CurrentAvailableButtons.Add(Instantiate(FindButtonPrefab(UIBarButton.ButtonType.Move), Vector3.one, Quaternion.identity, ScrollbarContent));

            if (pawn.level >= PawnClass.Level.Novice && pawn.actions.Count > 0) 
            {
                CurrentAvailableButtons.Add(Instantiate(FindButtonPrefab(UIBarButton.ButtonType.Action), Vector3.one, Quaternion.identity, ScrollbarContent));
            }
            if (pawn.level >= PawnClass.Level.Veteran && pawn.actions.Count > 1)
            {
                CurrentAvailableButtons.Add(Instantiate(FindButtonPrefab(UIBarButton.ButtonType.Action), Vector3.one, Quaternion.identity, ScrollbarContent));
            }
            if (pawn.level == PawnClass.Level.Master && pawn.actions.Count > 2)
            {
                CurrentAvailableButtons.Add(Instantiate(FindButtonPrefab(UIBarButton.ButtonType.Action), Vector3.one, Quaternion.identity, ScrollbarContent));
            }
        }

        // Stats and End turn buttons
		CurrentAvailableButtons.Add(Instantiate(FindButtonPrefab(UIBarButton.ButtonType.Stats), Vector3.one, Quaternion.identity, ScrollbarContent));
		CurrentAvailableButtons.Add(Instantiate(FindButtonPrefab(UIBarButton.ButtonType.EndTurn), Vector3.one, Quaternion.identity, ScrollbarContent));

        InitializeButtonFunctions();
    }

    public void DeselectPawn() 
    {
        RemovePreviousState();

        CurrentAvailableButtons.Add(Instantiate(FindButtonPrefab(UIBarButton.ButtonType.Cards), Vector3.one, Quaternion.identity, ScrollbarContent));
        CurrentAvailableButtons.Add(Instantiate(FindButtonPrefab(UIBarButton.ButtonType.Pawns), Vector3.one, Quaternion.identity, ScrollbarContent));
        CurrentAvailableButtons.Add(Instantiate(FindButtonPrefab(UIBarButton.ButtonType.Shop), Vector3.one, Quaternion.identity, ScrollbarContent));
        CurrentAvailableButtons.Add(Instantiate(FindButtonPrefab(UIBarButton.ButtonType.Settings), Vector3.one, Quaternion.identity, ScrollbarContent));
        CurrentAvailableButtons.Add(Instantiate(FindButtonPrefab(UIBarButton.ButtonType.EndTurn), Vector3.one, Quaternion.identity, ScrollbarContent));

        InitializeButtonFunctions();

        UIState = UISelectionState.NoSelection;
    }

    public void RemovePreviousState() 
    {
        for (int i = 0; i < ScrollbarContent.childCount; i++) 
        {
            Destroy(ScrollbarContent.GetChild(i).gameObject);
        }
        CurrentAvailableButtons.Clear();
    }

    public void InitializeButtonFunctions() 
    {
        for (int i = 0; i < CurrentAvailableButtons.Count; i++) 
        {
            CurrentAvailableButtons[i].InitButtonFunctionality();
        }
    }

    UIBarButton FindButtonPrefab(UIBarButton.ButtonType bt) 
    {
        for (int i = 0; i < UIBarButtons.Count; i++)
        {
            if (UIBarButtons[i].Type == bt) {
                return UIBarButtons[i];
            }
        }
        return null;
    }


    enum UISelectionState {
        NoSelection,
        AllyPawn,
        EnemyPawn
    }
}
