using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatDisplayer : MonoBehaviour {
	[SerializeField] SimControl sim;
	[SerializeField] Transform boatVisual;
	[SerializeField] Transform rudderVisual;

	void Update () {
		boatVisual.transform.position = sim.boatPos;
		boatVisual.transform.eulerAngles = new Vector3 (0, 0, sim.boatHeading);
		rudderVisual.transform.localEulerAngles = new Vector3 (0, 0, sim.rudderValue-90);
	}
}
