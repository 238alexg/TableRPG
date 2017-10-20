using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileContainer : MonoBehaviour {

    public TileClass Tile;
    public PawnClass Pawn;
    public SpriteRenderer SpriteRen;

    public int x, y;

    public static TileContainer SelectedTile;

    /// <summary>
    /// Performs selections necessary for when a user taps this tile
    /// </summary>
    public void OnTap() {
		if (GameSelections.SelectedTile != this)
		{
			GameSelections.SelectedTile = this;
		}

        StartCoroutine(DimSelection());

        Tile.Select();

        if (Pawn != null) {
            Pawn.SelectPawn();
        }

		// Check for existing actions including tile
		if (GameSelections.CurAction != null)
		{
			if (GameSelections.CurAction.tileOptions.Contains(this))
			{
				GameSelections.CurAction.OnExecution(this);
				TouchManager.Instance.ClearSelectionsAndUI();
                TouchManager.Instance.MoveCameraToPoint();
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
			if (MovementManager.instance.walkableTileOptions.Exists(wto => wto.Tile == this))
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

                if (GameSelections.HasMoved || Pawn == GameSelections.ActivePlayerPawn)
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

	// Called when a pawn is spawned on this tile
	public virtual void PlacePawn(PawnClass placedPawn)
	{
        Pawn = placedPawn;
        Pawn.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
	}
}
