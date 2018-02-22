using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleUIOnKey : MonoBehaviour {

	[SerializeField] UIBase uiComponent;
	[SerializeField] KeyCode togglekey;

	void Update () {
		if (Input.GetKeyDown (togglekey)) {
			if (uiComponent.hidden) {
				uiComponent.Show ();
			} else {
				uiComponent.Hide ();
			}
		}
	}
}
