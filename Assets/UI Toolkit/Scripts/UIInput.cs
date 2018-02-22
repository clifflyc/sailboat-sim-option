using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIInput : UILabel {
	
	InputField inputField;

	public override string stringElement { get { return inputField.text; } set { inputField.text = value; } }

	protected override void Awake () {
		base.Awake ();
		foreach (Graphic graphic in graphicElements) {
			InputField inputField = graphic.GetComponent<InputField> ();
			if (inputField) {
				if (this.inputField) {
					Debug.LogWarning ("Don't put multiple InputFields on a single UIInput");
				}
				this.inputField = inputField;
			}
		}
	}
}
