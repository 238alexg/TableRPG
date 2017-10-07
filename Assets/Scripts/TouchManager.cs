using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // TODO: Remove if unneeded after testing

public class TouchManager : MonoBehaviour {

	public GameObject selectionBorder;
	public static TouchManager Instance;

	private bool isCentering;

	public Text xyPrintOut;

	private Vector2 touchOrigin, t1Origin, t2Origin;
	private float zoomTouchOriginMagnitude;
	private bool isMoving;
	private Vector3 cameraOrigin;

	// Animation increment count
	int incrementCount = 25;

	void Awake() {
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy (this);
		}
	}

	// Update is called once per frame
	void Update () {
		// Single touch
		if (Input.touchCount == 1) {
			
			Touch t = Input.GetTouch (0);

			// Save single touch origin
			if (t.phase == TouchPhase.Began) {
				touchOrigin = Input.GetTouch (0).position;

			} else if (t.phase == TouchPhase.Moved) {
				if (isMoving) {
					CameraCenter (cameraOrigin.x + (Screen.width / 2) + ((touchOrigin.x - t.position.x) / 2), 
						cameraOrigin.y + (Screen.height / 2) + ((touchOrigin.y - t.position.y) / 2));
				} 
				// Detect if user wants to move camera
				else if (Mathf.Abs (t.position.x - touchOrigin.x) > (Screen.width / 100) &&
					Mathf.Abs (t.position.y - touchOrigin.y) > (Screen.height / 100)) {
					isMoving = true;
					cameraOrigin = Camera.main.transform.position;
				} 
			} else if (t.phase == TouchPhase.Ended) {
				if (!isMoving && Mathf.Abs (t.position.x - touchOrigin.x) < (Screen.width / 100) &&
				    Mathf.Abs (t.position.y - touchOrigin.y) < (Screen.height / 100)) {
					// Touch is a tap
					ScreenTap (t.position.x, t.position.y);
				} 
				isMoving = false;
			}
		}

		#if UNITY_EDITOR

		xyPrintOut.text = "X: " + (int)Input.mousePosition.x + ", Y: " + (int)Input.mousePosition.y;

        float moveDist = 0.08f;

		// If mouse clicked on screen
		if (Input.GetMouseButtonDown (0)) {
			ScreenTap (Input.mousePosition.x, Input.mousePosition.y);
		}

        // Move camera
        if (Input.GetKey(KeyCode.W)) {
            Vector3 pos = Camera.main.transform.position;
            pos.y += moveDist;
            Camera.main.transform.position = pos;
		}
		else if (Input.GetKey(KeyCode.S))
		{
			Vector3 pos = Camera.main.transform.position;
			pos.y -= moveDist;
			Camera.main.transform.position = pos;
		}

        if (Input.GetKey(KeyCode.A)) {
			Vector3 pos = Camera.main.transform.position;
            pos.x -= moveDist;
			Camera.main.transform.position = pos;
        }

        else if (Input.GetKey(KeyCode.D))
		{
			Vector3 pos = Camera.main.transform.position;
			pos.x += moveDist;
			Camera.main.transform.position = pos;
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
        Vector3 screenPos = Camera.main.ScreenToWorldPoint(new Vector3(x, y));

        if (screenPos.x < 0 || screenPos.x > GameStateManager.instance.xSize ||
            screenPos.y < 0 || screenPos.y > GameStateManager.instance.ySize) {
            // TODO: Change Bar UI to unselected state
            return;
        }

        int tileX = Mathf.FloorToInt(screenPos.x);
        int tileY = Mathf.FloorToInt(screenPos.y);

        TileClass tile = GameStateManager.instance.tiles[tileX, GameStateManager.instance.ySize - 1 - tileY];

        tile.Select();

        Debug.Log("Screen tapped at world location: " + screenPos.ToString());
	}

	/// <summary>
	/// Clears tile selection and UI, removes selection border.
	/// </summary>
	public void ClearSelectionsAndUI () {
		// Deselect tile
		GameSelections.selectedTile = null;

		// Remove selection border
		selectionBorder.SetActive (false);
	}

	// Takes 2 grid coordinates and converts them to world space coordinates
	public Vector2 GridToScreenCoordinates (int gridX, int gridY) {
        //float screenX = (gridX * fieldLength / GameStateManager.instance.xSize) + fieldMinX;
        //float screenY = worldViewHeight - (gridY * worldViewHeight / GameStateManager.instance.ySize);

        //Vector2 screenCoord = new Vector2 (screenX, screenY);
        //return screenCoord;
        return Vector2.zero;
	}

	/// <summary>
	/// Zooms in/out to point (x, y). For zoom out, just enter false as parameter to zoom out to screen center.
	/// </summary>
	/// <param name="zoomIn">If set to <c>true</c> zoom in. Else, zoom out.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public void ZoomToPoint(bool zoomIn, float x = 0, float y = 0) {
		// Always zoom out to screen center
		if (!zoomIn) {
			x = Screen.width / 2;
			y = Screen.height / 2;
		}
		StartCoroutine (AnimateCameraCenter (x, y));
	}

	// Animates the camera towards and centering on a point
	public IEnumerator AnimateCameraCenter(float x, float y) {

		// Restrict multiple centers
		if (isCentering) {
			yield break;
		} else {
			isCentering = true;
		}

		// Get position to move camera to
		x = x - (Screen.width / 2);
		y = y - (Screen.height / 2);

		Vector3 cameraOrigin = Camera.main.transform.position;
		Vector3 currentPos = new Vector3 (cameraOrigin.x, cameraOrigin.y, cameraOrigin.z);

		float xIncrement = (x - cameraOrigin.x) / incrementCount;
		float yIncrement = (y - cameraOrigin.y) / incrementCount;

		for (int i = 0; i < incrementCount; i++) {
			currentPos.x = cameraOrigin.x + xIncrement * i;
			currentPos.y = cameraOrigin.y + yIncrement * i;

			Camera.main.transform.position = currentPos;

			yield return new WaitForSeconds (0.001f);
		}

		// Ensure the camera is in the correct position after animation
		currentPos.x = x;
		currentPos.y = y;
		Camera.main.transform.position = currentPos;

		isCentering = false;
	}

	// Centers camera without animating
	public void CameraCenter(float x, float y) {

		x = x - (Screen.width /2);
		y = y - (Screen.height / 2);

		Camera.main.transform.position = new Vector3 (x, y, -100);
	}
}
