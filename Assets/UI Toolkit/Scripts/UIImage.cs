using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImage : UIBase {

	Image image;

	public virtual Sprite spriteElement { get { return image.sprite; } set { image.sprite = value; } }
	public virtual Color colorElement { get { return image.color; } set { image.color = value; } }
	public virtual float fillAmount { get { return image.fillAmount; } set { image.fillAmount = value; } }

	protected override void Awake () {
		base.Awake();
		foreach (Graphic graphic in graphicElements) {
			if (graphic is Image) {
				if (image) {
					Debug.LogWarning ("Don't put multiple Images on a single UIImage");
				}
				image = (Image)graphic;
			}
		}
	}

	public void SetSprite (Sprite s) {
		spriteElement = s;
	}
}
