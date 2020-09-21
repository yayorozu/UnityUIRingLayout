using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Yorozu.UI
{
	public class RingLayoutItem
	{
		public int Index;
		public readonly RectTransform RectTransform;
		public Canvas Canvas;
		public Image[] Images;
		public Button Button;

		public RingLayoutItem(RectTransform rect, int index)
		{
			RectTransform = rect;
			Index = index;

			Canvas = rect.GetComponent<Canvas>();
			if (Canvas == null)
			{
				Canvas = rect.gameObject.AddComponent<Canvas>();
			}

			Canvas.overrideSorting = true;

			var images = rect.GetComponentsInChildren<Image>();
			var image = rect.GetComponent<Image>();
			Images = new Image[image != null ? images.Length + 1 : images.Length];
			for (var i = 0; i < images.Length; i++)
			{
				Images[i] = images[i];
			}

			if (image != null)
			{
				Images[Images.Length - 1] = image;
			}

			// クリック判定用にRayCasterセット
			var graphicRayCaster = rect.GetComponent<GraphicRaycaster>();
			if (graphicRayCaster == null)
			{
				rect.gameObject.AddComponent<GraphicRaycaster>();
			}

			Button = rect.gameObject.GetComponent<Button>();
		}

		public void Dispose()
		{
			GameObject.Destroy(Canvas);

			if (Button != null)
				Button.onClick.RemoveAllListeners();

			if (Images != null)
			{
				for (var i = 0; i < Images.Length; ++i)
					Images[i] = null;

				Images = null;
			}
		}
	}
}
