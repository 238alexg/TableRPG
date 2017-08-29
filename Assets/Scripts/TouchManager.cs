using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // TODO: Remove if unneeded after testing

public class TouchManager : MonoBehaviour {

	public GameObject selectionBorder;
	public static TouchManager instance;
	public int cameraOrthographicZoomSize;

	private float worldViewLength, worldViewHeight, fieldMinX, fieldMaxX, fieldLength;
	private bool isZoomedIn;

	public Text xyPrintOut;

	void Awake() {
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}
	}

	public void UpdateFieldMinMax() {
		worldViewHeight = Camera.main.orthographicSize * 2;
		worldViewLength = worldViewHeight * Screen.width / Screen.height;

		fieldLength = GameSetup.instance.horzExtent * 2;
		fieldMinX = (worldViewLength - fieldLength) / 2;
		fieldMaxX = worldViewLength - fieldMinX;

		SpriteRenderer borderSR = selectionBorder.GetComponent <SpriteRenderer> ();
		selectionBorder.transform.localScale = new Vector3 (
			fieldLength / (GameStateManager.instance.xSize * borderSR.size.x), 
			worldViewHeight / (GameStateManager.instance.ySize * borderSR.size.y)
		);
		selectionBorder.SetActive (false);
	}

	// Update is called once per frame
	void Update () {
		// If new touch was made
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) {
			Touch t = Input.GetTouch (0);
			ScreenTap (t.position.x, t.position.y);
		}

		#if UNITY_EDITOR

		xyPrintOut.text = "X: " + (int)Input.mousePosition.x + ", Y: " + (int)Input.mousePosition.y;

		// If mouse clicked on screen
		if (Input.GetMouseButtonDown (0)) {
			ScreenTap (Input.mousePosition.x, Input.mousePosition.y);
		}
		if (Input.mouseScrollDelta.y < 0) {
			// Zoom out
			Zoom (false, 0, 0);
			print ("Mouse wheel down");
		} else if (Input.mouseScrollDelta.y > 0) {
			// Zoom in
			print("Mouse wheel up");
			Zoom (true, Input.mousePosition.x, Input.mousePosition.y);
		}
		#endif
	}

	/// <summary>
	/// Called when user touches the screen. 
	/// Selects a tile if touch/click is in the bounds of the field.
	/// </summary>
	/// <param name="x">The x coordinate, in pixels.</param>
	/// <param name="y">The y coordinate, in pixels.</param>
	void ScreenTap(float x, float y) {

		float xTouchWorldPos, yTouchWorldPos;

		// Get position of touch in zoomed in area
		if (isZoomedIn) {
			
			// Get bounds of camera
			float cameraMinX = Camera.main.transform.position.x - (Camera.main.orthographicSize * Screen.width / Screen.height);
			float cameraMinY = Camera.main.transform.position.y - Camera.main.orthographicSize;
			float cameraMaxX = Camera.main.transform.position.x + (Camera.main.orthographicSize * Screen.width / Screen.height);
			float cameraMaxY = Camera.main.transform.position.y + Camera.main.orthographicSize;

			// Get ratio of x and y across screen space (center of screen would be 0.5 for both x and y)
			float xPosRatio = x / Screen.width;
			float yPosRatio = y / Screen.height;

			// Get the position of the touch in the world space, where (0,0) is the top left corner
			xTouchWorldPos = (Screen.width / 2) + cameraMinX + xPosRatio * (cameraMaxX - cameraMinX);
			yTouchWorldPos = worldViewHeight - ((Screen.height / 2) + cameraMinY + yPosRatio * (cameraMaxY - cameraMinY));
		} 
		// Get position of touch in zoomed out area
		else {
			xTouchWorldPos = x * worldViewLength / Screen.width;
			yTouchWorldPos = worldViewHeight - (y * worldViewHeight / Screen.height); // Y starts at top, increases going down
		}

		// User taps the UI window, do not select tiles beneath it
		if (UIManager.instance.actionBarUI.GetBool ("SlideIn") && 
			xTouchWorldPos < 0.23f * worldViewLength) {
			print ("UI Touch");
			return;
		}

		// Select a field tile
		if ((xTouchWorldPos < fieldMaxX)
		    && (xTouchWorldPos > fieldMinX)) {

			int row = Mathf.FloorToInt((xTouchWorldPos - fieldMinX) * (GameStateManager.instance.xSize)/ fieldLength);
			int col = Mathf.FloorToInt((yTouchWorldPos * GameStateManager.instance.ySize) / (worldViewHeight));

			TileClass selectedTile = GameStateManager.instance.tiles [row, col];

			selectedTile.TileTap ();
			selectionBorder.SetActive (true);
			selectionBorder.transform.position = new Vector3 (selectedTile.transform.position.x, selectedTile.transform.position.y, -1);

		} else { // Tapping out of bounds //
			
			// Remove all UI
			UIManager.instance.ClearAllActiveUI ();

			// Deselect tile
			TileClass.selectedTile = null;

			// Remove selection border
			selectionBorder.SetActive (false);
		}
	}

	// Zooms in or out on center 
	void Zoom(bool zoomIn, float x, float y) {
		// If already zoomed in do nothing
		if (zoomIn && !isZoomedIn) {
		
			if (x < fieldMinX) {
				x = fieldMinX;
			} else if (x > fieldMaxX) {
				x = fieldMaxX;
			}

			x = x - (Screen.width /2);
			y = y - (Screen.height / 2);

			isZoomedIn = true;
			Camera.main.orthographicSize = Screen.height / 4;
			Camera.main.transform.position = new Vector3 (x, y, -100);
		} else if (!zoomIn) {
			isZoomedIn = false;
			Camera.main.orthographicSize = Screen.height / 2;
			Camera.main.transform.position = new Vector3 (0, 0, -100);
		}
	}
}
