using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PointerEventRedirect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {
	public UIBase redirectTo;

	public void OnPointerEnter (PointerEventData eventData) {
		redirectTo.OnPointerHover (gameObject);
	}

	public void OnPointerExit (PointerEventData eventData) {
		redirectTo.OnPointerUnhover (gameObject);

	}

	public void OnPointerDown (PointerEventData eventData) {
		redirectTo.OnPointerPress (gameObject);
	}

	public void OnPointerUp (PointerEventData eventData) {
		redirectTo.OnPointerUnpress (gameObject);
	}
}
