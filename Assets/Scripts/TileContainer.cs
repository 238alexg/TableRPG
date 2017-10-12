using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileContainer : MonoBehaviour {

    public TileClass Tile;
    public PawnClass Pawn;
    public SpriteRenderer SpriteRen;

    public static TileContainer SelectedTile;

    /// <summary>
    /// Performs selections necessary for when a user taps this tile
    /// </summary>
    public void OnTap() {
        StartCoroutine(DimSelection());

        Tile.Select();

        if (Pawn != null) {
            Pawn.SelectPawn();
        }

		// Check for existing actions including tile
		if (GameSelections.curAction != null)
		{
			if (GameSelections.curAction.tileOptions.Contains(this))
			{
				GameSelections.curAction.OnExecution(this);
				TouchManager.Instance.ClearSelectionsAndUI();
				TouchManager.Instance.ZoomToPoint(false);
			}
			// User taps outside action area, do nothing
			else
			{
				// TODO: Remove if nothing happens here
			}
		}

		// Check if pawn can move here, if moving Pawn
		else if (MovementManager.instance.isMovingPawn)
		{

			// If tile is within selected pawn's movement range, and pawn has movement left, move it
			if (MovementManager.instance.walkableTileOptions.Exists(wto => wto.tile == this))
			{
				StartCoroutine(MovementManager.instance.MovePawn(this));
				return;
			}
		}

		// Check for pawn on Tile
        else if (Pawn != null)
		{
			// If pawn belongs to player whose turn it is
            if (Pawn.ownership == GameStateManager.instance.turn)
			{

                if (GameSelections.hasMoved || Pawn == GameSelections.activePlayerPawn)
				{

				}
				else
				{
                    Pawn.SelectPawn();
				}
			}
			else
			{
				// TODO: Load enemy pawn into right UI for tiles/enemies?

			}
		}

		// User just taps tile
		else
		{
			// Select tile, clear UI
			TouchManager.Instance.ClearSelectionsAndUI();
		}

		if (GameSelections.selectedTile != this)
		{
			GameSelections.selectedTile = this;
		}
        StartCoroutine (DimSelection ());
	}

	// Dims the selected tile to show it has been selected
    public IEnumerator DimSelection()
	{
        const int dimSpeed = 10;
        Color32 dimColor = SpriteRen.color;
        dimColor.a = (byte)(SpriteRen.color.a * 255);

        while (dimColor.a < 255)
		{
            if (dimColor.a + dimSpeed > 255)
            {
                dimColor.a = 255;
            }
            else 
            {
                dimColor.a += dimSpeed;
            }
			SpriteRen.color = dimColor;
            yield return null;
		}

        while (dimColor.a > 0)
		{
			if (dimColor.a - dimSpeed < 0)
			{
				dimColor.a = 0;
			}
			else
			{
				dimColor.a -= dimSpeed;
			}
			SpriteRen.color = dimColor;
            yield return null;
		}
	}
}
