using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatDisplayer : MonoBehaviour {
	[SerializeField] SimControl sim;
	[SerializeField] Transform boatVisual;
	[SerializeField] Transform rudderVisual;
	[SerializeField] Transform headingVisual;

	void Update () {
		boatVisual.transform.position = sim.boatPos;
		boatVisual.transform.eulerAngles = new Vector3 (0, 0, sim.boatHeading - 90); //this -90 is because 0 degrees here is upwards.
		rudderVisual.transform.localEulerAngles = new Vector3 (0, 0, sim.rudderValue - 90);//this -90 is because 90 degrees in rudder is considered 0 degrees offset from main boat.
		headingVisual.transform.eulerAngles = new Vector3 (0, 0, sim.targetHeading - 90);//this -90 is because 0 degrees here is upwards.
	}
}
