using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIBase : MonoBehaviour {

	public bool startHidden;
	public Graphic[] graphicElements;
	public RectTransform[] transformableElements;
	

	// aniamtions
	protected const float AnimationDuration = 0.15f;
	protected const float AnimationDistance = 10f;
	static AnimationCurve animationCurve = AnimationCurve.EaseInOut (0, 0, 1, 1);

	public enum UICommonAnimation {
		None,
		LowerAlpha,
		RaiseAlpha,
		BounceUp,
		BounceDown,
		Enlarge,
		Unenlarge,
		EnlargeSideways,
		UnenlargeSideways
	}

	public UICommonAnimation hoverAnimation;
	public UICommonAnimation unhoverAnimation;
	public UICommonAnimation pressAnimation;
	public UICommonAnimation unpressAnimation;

	public enum UIShowHideAnimation {
		None,
		RiseInFallOut,
		ScaleInOutSideways
	}

	public UIShowHideAnimation showHideAnimation = UIShowHideAnimation.RiseInFallOut;

	Queue<IEnumerator> animationQueue = new Queue<IEnumerator> ();
	bool animationQueueWorkerAlive = false;

	bool[] graphicsElementsRaycastTargetable;
	Vector3[] transformableElementsOriginalLocations;


	// events
	[System.Serializable]
	public class UIEventVoid:UnityEvent {
		
	}

	public UIEventVoid OnHover;
	public UIEventVoid OnUnhover;
	public UIEventVoid OnPress;
	public UIEventVoid OnUnpress;

	public bool hidden { get; protected set; }

	protected virtual void Awake () {
		graphicsElementsRaycastTargetable = new bool[graphicElements.Length];
		for (int i = 0; i < graphicElements.Length; i++) {
			graphicsElementsRaycastTargetable [i] = graphicElements [i].raycastTarget;
		}
		transformableElementsOriginalLocations = new Vector3[transformableElements.Length];
		for (int i = 0; i < transformableElements.Length; i++) {
			transformableElementsOriginalLocations [i] = transformableElements [i].transform.localPosition;
		}
	}

	protected virtual void Start () {
		if (startHidden) {
			foreach (UIBase element in GetComponentsInChildren<UIBase>()) {
				element.HideImmediately ();
			}
		}
	}

	//---------------------//

	public virtual void OnPointerHover (GameObject source) {
		if (!hidden) {
			OnHover.Invoke ();
			QueueAnimation (_OnPointerHover ());
		}
	}

	IEnumerator _OnPointerHover () {
		yield return PlayCommonAnimation (hoverAnimation);
	}

	public virtual void OnPointerUnhover (GameObject source) {
		if (!hidden) {
			OnUnhover.Invoke ();
			QueueAnimation (_OnPointerUnhover ());
		}
	}

	IEnumerator _OnPointerUnhover () {
		yield return PlayCommonAnimation (unhoverAnimation);
	}

	public virtual void OnPointerPress (GameObject source) {
		if (!hidden) {
			OnPress.Invoke ();
			QueueAnimation (_OnPointerPress ());
		}
	}

	IEnumerator _OnPointerPress () {
		yield return PlayCommonAnimation (pressAnimation);
	}

	public virtual void OnPointerUnpress (GameObject source) {
		if (!hidden) {
			OnUnpress.Invoke ();
			QueueAnimation (_OnPointerUnpress ());
		}
	}

	IEnumerator _OnPointerUnpress () {
		yield return PlayCommonAnimation (unpressAnimation);
	}

	IEnumerator PlayCommonAnimation (UICommonAnimation commonAnimation) {
		switch (commonAnimation) {
		case UICommonAnimation.BounceUp:
			yield return A_BounceUp ();
			break;
		case UICommonAnimation.BounceDown:
			yield return A_BounceDown ();
			break;
		case UICommonAnimation.RaiseAlpha:
			yield return A_RaiseAlpha ();
			break;
		case UICommonAnimation.LowerAlpha:
			yield return A_LowerAlhpa ();
			break;
		case UICommonAnimation.Enlarge:
			yield return A_Enlarge ();
			break;
		case UICommonAnimation.Unenlarge:
			yield return A_Unenlarge ();
			break;
		case UICommonAnimation.EnlargeSideways:
			yield return A_EnlargeSideways ();
			break;
		case UICommonAnimation.UnenlargeSideways:
			yield return A_UnenlargeSideways ();
			break;
		}
	}

	//----------------------//

	public void Show () {
		StartCoroutine (_Show ());
		for(int i=0; i<transform.childCount;i++){
			UIBase childElement = transform.GetChild (i).GetComponent<UIBase> ();
			if (childElement && !childElement.startHidden) {
				childElement.Show ();
			}
		}
	}

	IEnumerator _Show () {
		SetElementsScale (1);
		switch (showHideAnimation) {
		case UIShowHideAnimation.RiseInFallOut:
			yield return A_RiseIn ();
			break;
		case UIShowHideAnimation.ScaleInOutSideways:
			yield return A_ScaleInSideways ();
			break;
		}
		ShowImmediately ();
	}

	void ShowImmediately () {
		SetElementsRaycastTargetable (true);
		SetElementsAlpha (1);
		SetElementsScale (1);
		SetElementsOffset (Vector2.zero);
		hidden = false;
	}

	public void Hide () {
		foreach (UIBase element in GetComponentsInChildren<UIBase>()) {
			element.StartCoroutine (element._Hide ());
		}
	}

	void HideImmediately () {
		hidden = true;
		SetElementsAlpha (0);
		SetElementsScale (1);
		SetElementsOffset (Vector2.zero);
		SetElementsRaycastTargetable (false);
		animationQueue.Clear ();
	}

	IEnumerator _Hide () {
		hidden = true;
		SetElementsRaycastTargetable (false);
		animationQueue.Clear ();
		switch (showHideAnimation) {
		case UIShowHideAnimation.RiseInFallOut:
			yield return A_FallOut ();
			break;
		case UIShowHideAnimation.ScaleInOutSideways:
			yield return A_ScaleOutSideways ();
			break;
		}
		HideImmediately ();
	}

	IEnumerator A_RiseIn () {
		float lifetime = 0;
		while (lifetime < AnimationDuration) {
			float frac = animationCurve.Evaluate (lifetime / AnimationDuration);
			SetElementsOffset (Vector3.down * AnimationDistance * (1 - frac));
			SetElementsAlpha (frac);
			yield return null;
			lifetime += Time.deltaTime;
		}
		SetElementsOffset (Vector3.zero);
		SetElementsAlpha (1);
	}

	IEnumerator A_FallOut () {
		float lifetime = 0;
		while (lifetime < AnimationDuration) {
			float frac = animationCurve.Evaluate (lifetime / AnimationDuration);
			SetElementsOffset (Vector3.down * AnimationDistance * frac);
			SetElementsAlpha (1 - frac);
			yield return null;
			lifetime += Time.deltaTime;
		}
		SetElementsOffset (Vector3.down * AnimationDistance);
		SetElementsAlpha (0);
	}

	IEnumerator A_ScaleInSideways () {
		SetElementsAlpha (1);
		float lifetime = 0;
		while (lifetime < AnimationDuration) {
			float frac = animationCurve.Evaluate (lifetime / AnimationDuration);
			SetElementsScale (new Vector2 (frac, 1));
			yield return null;
			lifetime += Time.deltaTime;
		}
		SetElementsScale (1);
	}

	IEnumerator A_ScaleOutSideways () {
		float lifetime = 0;
		while (lifetime < AnimationDuration) {
			float frac = animationCurve.Evaluate (lifetime / AnimationDuration);
			SetElementsScale (new Vector2 (1 - frac, 1));
			yield return null;
			lifetime += Time.deltaTime;
		}
		SetElementsScale (0);
	}

	//---------------------//

	
	IEnumerator A_BounceDown () {
		float lifetime = 0;
		while (lifetime < AnimationDuration) {
			float frac = animationCurve.Evaluate (lifetime / AnimationDuration);
			SetElementsOffset (Vector3.up * AnimationDistance * (1 - frac));
			yield return null;
			lifetime += Time.deltaTime * 2;
		}
		SetElementsOffset (Vector3.zero);
	}

	IEnumerator A_BounceUp () {
		float lifetime = 0;
		while (lifetime < AnimationDuration) {
			float frac = animationCurve.Evaluate (lifetime / AnimationDuration);
			SetElementsOffset (Vector3.up * AnimationDistance * frac);
			yield return null;
			lifetime += Time.deltaTime * 2;
		}
		SetElementsOffset (Vector3.up * AnimationDistance);
	}

	IEnumerator A_LowerAlhpa () {
		float lifetime = 0;
		while (lifetime < AnimationDuration) {
			float frac = animationCurve.Evaluate (lifetime / AnimationDuration);
			SetElementsAlpha (1 - 0.2f * frac);
			yield return null;
			lifetime += Time.deltaTime;
		}
		SetElementsAlpha (0.8f);
	}

	IEnumerator A_RaiseAlpha () {
		float lifetime = 0;
		while (lifetime < AnimationDuration) {
			float frac = animationCurve.Evaluate (lifetime / AnimationDuration);
			SetElementsAlpha (0.8f + 0.2f * frac);
			yield return null;
			lifetime += Time.deltaTime;
		}
		SetElementsAlpha (1);
	}

	IEnumerator A_Enlarge () {
		float lifetime = 0;
		while (lifetime < AnimationDuration) {
			float frac = animationCurve.Evaluate (lifetime / AnimationDuration);
			SetElementsScale (1 + 0.1f * frac);
			yield return null;
			lifetime += Time.deltaTime;
		}
		SetElementsScale (1.1f);
	}

	IEnumerator A_Unenlarge () {
		float lifetime = 0;
		while (lifetime < AnimationDuration) {
			float frac = animationCurve.Evaluate (lifetime / AnimationDuration);
			SetElementsScale (1 + 0.1f * (1 - frac));
			yield return null;
			lifetime += Time.deltaTime;
		}
		SetElementsScale (1);
	}

	IEnumerator A_EnlargeSideways () {
		float lifetime = 0;
		while (lifetime < AnimationDuration) {
			float frac = animationCurve.Evaluate (lifetime / AnimationDuration);
			SetElementsScale (new Vector2 (1 + 0.1f * frac, 1));
			yield return null;
			lifetime += Time.deltaTime;
		}
		SetElementsScale (new Vector2 (1.1f, 1));
	}

	IEnumerator A_UnenlargeSideways () {
		float lifetime = 0;
		while (lifetime < AnimationDuration) {
			float frac = animationCurve.Evaluate (lifetime / AnimationDuration);
			SetElementsScale (new Vector2 (1 + 0.1f * (1 - frac), 1));
			yield return null;
			lifetime += Time.deltaTime;
		}
		SetElementsScale (1);
	}

	//--------//

	protected void QueueAnimation (IEnumerator animation) {
		animationQueue.Enqueue (animation);
		if (!animationQueueWorkerAlive) {
			StartCoroutine (AnimationQueueWorker ());
		}
	}

	IEnumerator AnimationQueueWorker () {
		animationQueueWorkerAlive = true;
		while (animationQueue.Count > 0 && !hidden) {
			IEnumerator animation = animationQueue.Dequeue ();
			while (animation.MoveNext () && !hidden) {
				yield return animation.Current;
			}
		}
		animationQueueWorkerAlive = false;
	}

	protected void SetElementsAlpha (float alpha) {
		for (int i = 0; i < graphicElements.Length; i++) {
			graphicElements [i].CrossFadeAlpha (alpha, 0, true);
		}
	}

	protected void SetElementsRaycastTargetable (bool targetable) {
		for (int i = 0; i < graphicElements.Length; i++) {
			if (graphicsElementsRaycastTargetable [i]) {
				graphicElements [i].raycastTarget = targetable;
			}
		}
	}

	protected void SetElementsScale (float scale) {
		SetElementsScale (Vector2.one * scale);
	}

	protected void SetElementsScale (Vector2 scale) {
		for (int i = 0; i < transformableElements.Length; i++) {
			transformableElements [i].transform.localScale = new Vector3 (scale.x, scale.y, 1);
		}
	}

	protected void SetElementsOffset (Vector3 offset) {
		for (int i = 0; i < transformableElements.Length; i++) {
			transformableElements [i].transform.localPosition = transformableElementsOriginalLocations [i] + offset;
		}
	}

}
