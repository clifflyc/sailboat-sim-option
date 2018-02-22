using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectIpAndSend : MonoBehaviour {
	[SerializeField] UIInput ipField;
	[SerializeField] SimControl simControl;

	public void InitiateConnection(){
		simControl.InitSimulation (ipField.stringElement);
	}
}
