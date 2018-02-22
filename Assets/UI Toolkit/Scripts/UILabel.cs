using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UILabel : UIBase {

	[System.Serializable]
	public class UIEventString:UnityEvent<string> {

	}

	public UIEventString OnHoverString;
	public UIEventString OnUnhoverString;
	public UIEventString OnPressString;
	public UIEventString OnUnpressString;

	Text label;

	public virtual string stringElement { get { return label.text; } set { label.text = value; } }

	protected override void Awake () {
		base.Awake ();
		foreach (Graphic graphic in graphicElements) {
			if (graphic is Text) {
				if (label) {
					Debug.LogWarning ("Don't put multiple Texts on a single UILabel");
				}
				label = (Text)graphic;
				break;
			}
		}
	}

	public void SetString (string s) {
		stringElement = s;
	}

	public void SetString (float value) {
		stringElement = value.ToString ();
	}

	public override void OnPointerHover (GameObject source) {
		base.OnPointerHover (source);
		if (!hidden) {
			OnHoverString.Invoke (stringElement);
		}
	}


	public override void OnPointerUnhover (GameObject source) {
		base.OnPointerUnhover (source);
		if (!hidden) {
			OnUnhoverString.Invoke (stringElement);

		}
	}

	public override void OnPointerPress (GameObject source) {
		base.OnPointerPress (source);
		if (!hidden) {
			OnPressString.Invoke (stringElement);
		}
	}

	public override void OnPointerUnpress (GameObject source) {
		base.OnPointerUnpress (source);
		if (!hidden) {
			OnUnpressString.Invoke (stringElement);
		}
	}
}