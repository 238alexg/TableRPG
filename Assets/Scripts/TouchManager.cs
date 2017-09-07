using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // TODO: Remove if unneeded after testing

public class TouchManager : MonoBehaviour {

	public GameObject selectionBorder;
	public static TouchManager instance;
	public int cameraOrthographicZoomSize;
	public bool isZoomedIn;

	private float worldViewLength, worldViewHeight, fieldMinX, fieldMaxX, fieldLength;
	private bool isZooming, isCentering;

	public Text xyPrintOut;

	private Vector2 touchOrigin, t1Origin, t2Origin;
	private float zoomTouchOriginMagnitude;
	private bool isMoving;
	private Vector3 cameraOrigin;

	// Animation increment count
	int incrementCount = 25;

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
				// Detect if user wants to move camera (only allow if zoomed in
				else if (isZoomedIn && Mathf.Abs (t.position.x - touchOrigin.x) > (Screen.width / 100) &&
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
		// Pinch zoom, adapted from the Unity Tutorial: 
		// https://unity3d.com/learn/tutorials/topics/mobile-touch/pinch-zoom
		else if (Input.touchCount == 2) {
			Touch t1 = Input.GetTouch (0);
			Touch t2 = Input.GetTouch (1);

			isMoving = false;

			// Set center touch origin
			if (t1.phase == TouchPhase.Began) {
				t1Origin = t1.position;
				t2Origin = t2.position;

				// Get distance between original touches
				zoomTouchOriginMagnitude = (t1Origin - t2Origin).magnitude;

				// Get central point between original 2 touches
				touchOrigin.x = (t1Origin.x + t2Origin.x) / 2;
				touchOrigin.y = (t1Origin.y + t2Origin.y) / 2;
			} else if (t1.phase == TouchPhase.Moved) {

				// Find the magnitude of the vector (the distance) between the touches
				float touchDeltaMag = (t1.position - t2.position).magnitude;

				// Find the difference in the distances between each frame.
				float deltaMagnitudeDiff = touchDeltaMag - zoomTouchOriginMagnitude;

				if (deltaMagnitudeDiff > (Screen.height / 20)) {
					ZoomToPoint (true, (t1Origin.x + t2Origin.x)/2, (t1Origin.y + t2Origin.y)/2);
				} else if (deltaMagnitudeDiff < -(Screen.height / 40)) {
					ZoomToPoint (false);
				}
			}
		}

		#if UNITY_EDITOR

		xyPrintOut.text = "X: " + (int)Input.mousePosition.x + ", Y: " + (int)Input.mousePosition.y;

		// If mouse clicked on screen
		if (Input.GetMouseButtonDown (0)) {
			ScreenTap (Input.mousePosition.x, Input.mousePosition.y);
		}
		if (Input.mouseScrollDelta.y < 0) {
			// Zoom out
			ZoomToPoint (false);
		} else if (Input.mouseScrollDelta.y > 0) {
			// Zoom in
			ZoomToPoint (true, Input.mousePosition.x, Input.mousePosition.y);
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

			// Check zoomed in UI touch
			if (UIManager.instance.actionBarUI.GetBool ("SlideIn") &&
				xPosRatio < 0.20f) {
				print ("Zoomed in UI Touch");
				return;
			}

			// Get the position of the touch in the world space, where (0,0) is the top left corner
			xTouchWorldPos = (Screen.width / 2) + cameraMinX + xPosRatio * (cameraMaxX - cameraMinX);
			yTouchWorldPos = worldViewHeight - ((Screen.height / 2) + cameraMinY + yPosRatio * (cameraMaxY - cameraMinY));
		} 
		// Get position of touch in zoomed out area
		else {
			xTouchWorldPos = x * worldViewLength / Screen.width;
			yTouchWorldPos = worldViewHeight - (y * worldViewHeight / Screen.height); // Y starts at top, increases going down

			// Check zoomed out UI touch
			if (UIManager.instance.actionBarUI.GetBool ("SlideIn") &&
			    xTouchWorldPos < 0.23f * worldViewLength) {
				print ("Zoomed out UI Touch");
				return;
			}
		}

		// Select a field tile
		if ((xTouchWorldPos < fieldMaxX)
		    && (xTouchWorldPos > fieldMinX)) {

			int row = Mathf.FloorToInt((xTouchWorldPos - fieldMinX) * (GameStateManager.instance.xSize)/ fieldLength);
			int col = Mathf.FloorToInt((yTouchWorldPos * GameStateManager.instance.ySize) / (worldViewHeight));

			TileClass selectedTile = GameStateManager.instance.tiles [row, col];

			selectedTile.TileTap ();
			selectionBorder.SetActive (true);
			selectionBorder.transform.position = new Vector3 (selectedTile.transform.position.x, selectedTile.transform.position.y, -10);

		} else { // Tapping out of bounds
			ClearSelectionsAndUI ();
		}
	}

	/// <summary>
	/// Clears tile selection and UI, removes selection border.
	/// </summary>
	public void ClearSelectionsAndUI () {
		// Remove all UI
		UIManager.instance.ClearAllActiveUI ();

		// Deselect tile
		GameSelections.selectedTile = null;

		// Remove selection border
		selectionBorder.SetActive (false);
	}

	// Takes 2 grid coordinates and converts them to world space coordinates
	public Vector2 GridToScreenCoordinates (int gridX, int gridY) {
		float screenX = (gridX * fieldLength / GameStateManager.instance.xSize) + fieldMinX;
		float screenY = worldViewHeight - (gridY * worldViewHeight / GameStateManager.instance.ySize);

		Vector2 screenCoord = new Vector2 (screenX, screenY);
		return screenCoord;
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
		StartCoroutine (AnimateZoom (zoomIn));
		StartCoroutine (AnimateCameraCenter (x, y));
	}

	// Zooms in or out on center 
	public IEnumerator AnimateZoom(bool zoomIn) {

		// Restrict multiple zooms
		if (isZooming) {
			yield break;
		} else {
			isZooming = true;
		}

		// If already zoomed in do nothing
		if (zoomIn && !isZoomedIn) {
			isZoomedIn = true;

			for (int i = 0; i < incrementCount; i++) {
				Camera.main.orthographicSize = (Screen.height / 2) - ((Screen.height * i) / (4 * incrementCount));
				yield return new WaitForSeconds (0.001f);
			}
			Camera.main.orthographicSize = Screen.height / 4; // Ensure the camera has correct orthographic size after animation
		} else if (!zoomIn && isZoomedIn) {
			isZoomedIn = false;

			for (int i = 0; i < incrementCount; i++) {
				Camera.main.orthographicSize = (Screen.height / 4) + ((Screen.height * i) / (4 * incrementCount));
				yield return new WaitForSeconds (0.001f);
			}
			Camera.main.orthographicSize = Screen.height / 2; // Ensure the camera has correct orthographic size after animation
		}

		isZooming = false;
	}

	// Animates the camera towards and centering on a point
	public IEnumerator AnimateCameraCenter(float x, float y) {

		// Restrict multiple centers
		if (isCentering) {
			yield break;
		} else {
			isCentering = true;
		}

		if (x < fieldMinX) {
			x = fieldMinX;
		} else if (x > fieldMaxX) {
			x = fieldMaxX;
		}

		if (y < 0) {
			y = 0;
		} else if (y > worldViewHeight) {
			y = worldViewHeight;
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

	// Zooms in or out without animating
	public void Zoom(bool zoomIn) {
		// If already zoomed in do nothing
		if (zoomIn && !isZoomedIn) {
			isZoomedIn = true;
			Camera.main.orthographicSize = Screen.height / 4;
		} else if (!zoomIn) {
			isZoomedIn = false;
			Camera.main.orthographicSize = Screen.height / 2;
		}
	}

	// Centers camera without animating
	public void CameraCenter(float x, float y) {
		if (x < fieldMinX) {
			x = fieldMinX;
		} else if (x > fieldMaxX) {
			x = fieldMaxX;
		}

		if (y < 0) {
			y = 0;
		} else if (y > worldViewHeight) {
			y = worldViewHeight;
		}

		x = x - (Screen.width /2);
		y = y - (Screen.height / 2);

		Camera.main.transform.position = new Vector3 (x, y, -100);
	}
}
