using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Yorozu.UI
{
	public class SampleRingLayout : MonoBehaviour
	{
		[SerializeField]
		private Button _button;

		[SerializeField]
		private RingLayout _layout;

		[SerializeField]
		private RingLayoutMenu _menu;

		[SerializeField]
		private GameObject _resource;

		private void Awake()
		{
			_button.onClick.AddListener(Click);
			if (_layout != null)
				_layout.UpdateItemEvent += LayoutUpdateItem;

			if (_menu != null)
				_menu.UpdateItemEvent += LayoutMenuUpdateItem;
		}

		private void LayoutUpdateItem(RingLayoutItem item, float t)
		{
			// 開始場所によって変える必要がありそう
			// 0.1単位でOrderセット
			item.Canvas.sortingOrder = Mathf.CeilToInt(t * 10);
			item.RectTransform.localScale = Vector3.one * Mathf.Lerp(0.6f, 1f, t);
		}

		private void LayoutMenuUpdateItem(RingLayoutItem item, float t)
		{
			item.Canvas.sortingOrder = Mathf.CeilToInt(t * 10);
			foreach (var image in item.Images)
			{
				image.color = new Color(t, t, t);
			}

			if (item.Button == null)
				return;

			// 複数アクティブになる可能性あり
			if (t > 0.9f)
				item.Button.interactable = true;
			else if (item.Button.interactable)
				item.Button.interactable = false;
		}

		private void Click()
		{
			if (_layout != null)
				_layout.Add(Instantiate(_resource));

			if (_menu != null)
				_menu.Add(Instantiate(_resource));
		}
	}
}
