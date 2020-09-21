using UnityEngine;
using UnityEngine.EventSystems;

namespace Yorozu.UI
{
	public class RingLayoutSelector : RingLayout
	{
		[SerializeField]
		private Canvas _canvas;

		[SerializeField]
		private CanvasGroup _canvasGroup;

		[SerializeField]
		[Range(0.1f, 3f)]
		private float _transitionTime = 0.5f;

		[SerializeField]
		[Range(0f, 720f)]
		private float _rotateAngle = 180f;

		[SerializeField]
		private Vector2 _openRadius = new Vector2(100f, 100f);

		private const float SCROLL_TIME = 0.2f;

		private float _targetOffsetAngle;
		private float _beginOffsetAngle;
		private bool _isAutoScroll;
		private float _scrollTime;

		// State
		private bool _isOpen;
		private bool _nextState;
		private float _time;
		private System.Action _stateChangedAction;

		protected override void Awake()
		{
			base.Awake();
			_scrollTime = 0f;
			Open();
		}

		public void Open()
		{
			if (_isOpen || _isOpen != _nextState)
				return;

			_canvas.SetEnable();
			_canvasGroup.alpha = 1f;
			_nextState = true;
			_time = _transitionTime;
		}

		public void Close()
		{
			if (_isOpen != _nextState || !_isOpen)
				return;

			_nextState = false;
			_time = _transitionTime;
			_stateChangedAction = () =>
			{
				_canvas.SetDisable();
				_canvasGroup.alpha = 0f;
			};
		}

		public override void OnDrag(PointerEventData eventData)
		{
			_isAutoScroll = false;
			base.OnDrag(eventData);
		}

		public override void OnEndDrag(PointerEventData eventData)
		{
			_beginOffsetAngle = _offset;
			_targetOffsetAngle = _index * _splitAngle;
			_scrollTime = 0f;
			_isAutoScroll = true;
		}

		private void Update()
		{
			AutoFit();
			RingMove();
		}

		private void AutoFit()
		{
			if (!_isAutoScroll)
				return;

			if (Mathf.Approximately(_offset, _targetOffsetAngle))
			{
				_isAutoScroll = false;
				return;
			}

			_scrollTime += Time.deltaTime;
			_offset = Mathf.Lerp(_beginOffsetAngle, _targetOffsetAngle, _scrollTime / SCROLL_TIME);
			Reposition();
		}

		private void RingMove()
		{
			if (_isOpen == _nextState)
				return;

			if (_time <= 0f)
			{
				_isOpen = _nextState;
				if (_stateChangedAction != null)
				{
					_stateChangedAction();
					_stateChangedAction = null;
				}

				return;
			}

			var t = _time / _transitionTime;
			// 閉じる場合はt が逆
			if (!_nextState)
				t = 1 - t;

			SetParam(Mathf.Lerp(0f, _rotateAngle, t), Vector2.Lerp(_openRadius, Vector2.zero, t));

			_time -= Time.deltaTime;
		}

		/// <summary>
		/// 指定のIndexをメインに
		/// </summary>
		public void ScrollTo(int index)
		{
			if (_items == null || _items.Count <= index)
				return;

			_isAutoScroll = true;
			_beginOffsetAngle = _offset;
			_targetOffsetAngle = index * _splitAngle;
			_scrollTime = 0f;
		}
	}
}
