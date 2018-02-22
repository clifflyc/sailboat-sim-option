using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointsEditModeNotice : MonoBehaviour {

	[SerializeField] UIBase uiComponent;
	[SerializeField] WaypointsControl waypointsControl;

	void Update () {
		if (waypointsControl.editMode) {
			if (uiComponent.hidden) {
				uiComponent.Show ();
			}
		} else if (!uiComponent.hidden) {
			uiComponent.Hide ();
		}
	}
}
