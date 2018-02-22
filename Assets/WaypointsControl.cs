using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointsControl : MonoBehaviour {

	[SerializeField] SimControl sim;
	[SerializeField] GameObject waypointVisualPrefab;

	List<Vector2> waypoints = new List<Vector2> ();
	List<GameObject> waypointsVisuals = new List<GameObject> ();

	[System.NonSerialized]
	public bool editMode = false;

	void Update () {
		if (Input.GetKeyDown (KeyCode.Z)) {
			editMode = !editMode;
		}
		if (editMode) {
			Vector2 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			if (Input.GetKeyDown (KeyCode.Mouse0)) {
				waypoints.Add (mousePos);
				waypointsVisuals.Add ((GameObject)Instantiate (waypointVisualPrefab, mousePos, Quaternion.identity, transform));
			} else if (Input.GetKeyDown (KeyCode.Mouse1)) {
				for (int i = waypoints.Count - 1; i >= 0; i--) {
					if (Vector2.Distance (waypoints [i], mousePos) < 2) {
						GameObject t = waypointsVisuals [i];
						waypointsVisuals.RemoveAt (i);
						Destroy (t);
						waypoints.RemoveAt (i);
						break;
					}
				}
			}
			sim.SetWaypoints (waypoints.ToArray ());
		}
	}

}
