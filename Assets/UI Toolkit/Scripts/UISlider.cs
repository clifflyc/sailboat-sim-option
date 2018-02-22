using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISlider : UIBase {
	[System.Serializable]
	public class UIEventFloat:UnityEvent<float> {

	}

	public UIEventFloat OnHoverFloat;
	public UIEventFloat OnUnhoverFloat;
	public UIEventFloat OnPressFloat;
	public UIEventFloat OnUnpressFloat;

	Slider slider;

	public virtual float floatElement { get { return slider.value; } set { slider.value = value; } }

	protected override void Awake () {
		base.Awake ();
		for (int i = 0; i < transform.childCount; i++) {
			Transform child = transform.GetChild (i);
			Slider slider = child.GetComponent<Slider> ();
			if (slider) {
				if (this.slider) {
					Debug.LogWarning ("Don't put multiple Sliders on a single UISlider");
				}
				this.slider = slider;
			}
		}
	}

	public void SetValue (float f) {
		floatElement = f;
	}

	public override void OnPointerHover (GameObject source) {
		base.OnPointerHover (source);
		if (!hidden) {
			OnHoverFloat.Invoke (floatElement);
		}
	}


	public override void OnPointerUnhover (GameObject source) {
		base.OnPointerUnhover (source);
		if (!hidden) {
			OnUnhoverFloat.Invoke (floatElement);

		}
	}

	public override void OnPointerPress (GameObject source) {
		base.OnPointerPress (source);
		if (!hidden) {
			OnPressFloat.Invoke (floatElement);
		}
	}

	public override void OnPointerUnpress (GameObject source) {
		base.OnPointerUnpress (source);
		if (!hidden) {
			OnUnpressFloat.Invoke (floatElement);
		}
	}
}
