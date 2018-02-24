using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentMeter  : MonoBehaviour {
	
	[SerializeField] SimControl sim;
	[SerializeField] UIImage dialHand;
	[SerializeField] UILabel strengthValueLabel;

	void Update () {
		dialHand.transform.eulerAngles = new Vector3 (0, 0, sim.currentDirection - 90);//this -90 is because 0 degrees here is upwards.
		strengthValueLabel.SetString (sim.currentStrength);
	}
}
