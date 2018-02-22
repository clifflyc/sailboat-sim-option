using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindMeter : MonoBehaviour {

	[SerializeField] SimControl sim;
	[SerializeField] UIImage dialHand;
	[SerializeField] UILabel strengthValueLabel;

	void Update () {
		dialHand.transform.eulerAngles = new Vector3 (0, 0, sim.windDirection);
		strengthValueLabel.SetString (sim.windStrength);
	}
}
