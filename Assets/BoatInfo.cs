using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatInfo : MonoBehaviour {

	[SerializeField] SimControl sim;
	[SerializeField] UILabel boatStateLabel;
	[SerializeField] UILabel rudderLabel;
	[SerializeField] UILabel winchLabel;
	[SerializeField] UILabel boatKinematicsLabel;

	void Update () {
		rudderLabel.stringElement = "Rudder: " + sim.rudderValue;
		winchLabel.stringElement = "Winch: " + sim.winchValue;
		boatKinematicsLabel.stringElement = "Pos: " + sim.boatPos + "\n" +
		"Vel: " + sim.boatVel + "\n" +
		"Accel: " + sim.boatAccel + "\n" +
		"Heading: " + sim.boatHeading;
		if (sim.boatState != null) {
			boatStateLabel.stringElement = "Major: " + (SimControl.BoatState.Major)sim.boatState.major + "\n" +
			"Minor: " + (SimControl.BoatState.Minor)sim.boatState.minor;
		}
	}
}
