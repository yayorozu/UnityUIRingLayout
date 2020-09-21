using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Yorozu.UI
{
	[DefaultExecutionOrder(1)]
	public class RingLayout : UIBehaviour, ILayoutGroup, IDragHandler, IEndDragHandler
	{
		[SerializeField]
		[Range(0f, 359f)]
		protected float _snapAngle;

		[SerializeField]
		protected Vector2 _radius = new Vector2(100f, 100f);

		[SerializeField]
		[Range(0f, 359f)]
		private float _space;

		protected int _index;

		protected float _offset;
		protected float _splitAngle;
		private float _spaceRatio;

		protected List<RingLayoutItem> _items;

		/// <summary>
		/// 初期化処理イベント
		/// </summary>
		/// <param name="obj"></param>
		public delegate void Init(GameObject obj);
		public event Init InitEvent;

		/// <summary>
		/// クリック時のイベント
		/// </summary>
		/// <param name="index"></param>
		public delegate void Click(int index);
		public event Click ClickEvent;

		public delegate void UpdateItem(RingLayoutItem item, float t);
		public event UpdateItem UpdateItemEvent;

		/// <summary>
		/// 要素を取得
		/// </summary>
		public IEnumerable<T> GetItems<T>() where T : Component
		{
			return _items.Select(i => i.RectTransform.GetComponent<T>());
		}

#if UNITY_EDITOR

		protected override void OnValidate()
		{
			base.OnValidate();
			Awake();
			Reposition();
		}

#endif

		protected override void OnDestroy()
		{
			base.OnDestroy();

			if (_items == null)
				return;

			for (var i = 0; i < _items.Count; ++i)
			{
				if (_items[i] == null)
					continue;

				_items[i].Dispose();
			}

			_items = null;
		}

		protected override void Awake()
		{
			base.Awake();

			_spaceRatio = (180f - _space / 2f) / 180f;
			InitList();
		}

		private void InitList()
		{
			if (_items != null)
			{
				for (var i = 0; i < _items.Count; ++i)
					_items[i] = null;
				_items = null;
			}

			_items = new List<RingLayoutItem>(transform.childCount);
			for (var i = 0; i < transform.childCount; ++i)
			{
				Add(transform.GetChild(i).gameObject);
			}
		}

		public void SetLayoutHorizontal()
		{
		}

		public void SetLayoutVertical()
		{
			Awake();
			Reposition();
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			_offset += (eventData.delta.x + eventData.delta.y) / 5f;
			Reposition();
		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
		}

		/// <summary>
		/// Set Offset Angle
		/// </summary>
		protected void SetParam(float angle, Vector2 radius)
		{
			_offset = angle;
			_radius = radius;
			Reposition();
		}

		protected void Reposition()
		{
			if (_items == null || _items.Count <= 0)
				return;

			_splitAngle = 360f / transform.childCount;

			if (_offset > 360f)
				_offset = _offset % 360f;
			if (_offset < 0f)
				_offset = _offset + 360f;

			_index = Mathf.RoundToInt(_offset / _splitAngle);
			for (var i = 0; i < _items.Count; ++i)
			{
				var currentAngle = (_splitAngle * i + _offset + _snapAngle) % 360f;
				if (currentAngle >= 180f)
					currentAngle = currentAngle * _spaceRatio + _space;
				else
					currentAngle *= _spaceRatio;

				var t = Mathf.Abs((currentAngle + 180 - _snapAngle) % 360 - 180f) / 180f;
				UpdateItemEvent?.Invoke(_items[i], t);

				var pos = _items[i].RectTransform.anchoredPosition;
				pos.x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * _radius.x;
				pos.y = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * _radius.y;
				_items[i].RectTransform.anchoredPosition = pos;
			}
		}

		/// <summary>
		/// 要素を追加
		/// </summary>
		public void Add(GameObject gameObject, int count = 1)
		{
			for (var i = 0; i < count; i++)
			{
				var index = _items.Count;
				// 初期化処理
				InitEvent?.Invoke(gameObject);

				gameObject.transform.parent = transform;
				var pos = gameObject.transform.position;
				pos.z = transform.position.z;
				gameObject.transform.position = pos;

				var item = new RingLayoutItem(gameObject.transform as RectTransform, index);
				item.Button?.onClick.AddListener(() => ClickEvent?.Invoke(index));
				_items.Add(item);
			}
			Reposition();
		}

		/// <summary>
		/// 要素を削除
		/// </summary>
		public void Remove(int index)
		{
			if (_items.Count <= index)
				return;

			_items[index].Dispose();
			_items.RemoveAt(index);
			Reposition();
		}
	}
}
