using UnityEngine;
using UnityEngine.EventSystems;

namespace Chromium.UILibrary
{
	public class DragAndSelectUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		[SerializeField]
		private RectTransform _boxRect;

		[SerializeField]
		private Canvas _canvas;

		private Vector2 _beginDragPosition;
		private Rect _rect;

		private void OnValidate()
		{
			if (!_boxRect)
			{
				Debug.LogError($"<color=yellow>Box Rect</color> field of {gameObject.name} should not be Empty!");
			}

			if (!_canvas)
			{
				Debug.LogError($"<color=yellow>Canvas</color> field of {gameObject.name} should not be Empty!");
			}
		}

		private void Start()
		{
			_boxRect.gameObject.SetActive(false);
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			_boxRect.gameObject.SetActive(true);
			_boxRect.transform.position = eventData.position;
			_beginDragPosition = eventData.position;
		}

		public void OnDrag(PointerEventData eventData)
		{
			var deltaX = eventData.position.x - _beginDragPosition.x;
			var deltaY = eventData.position.y - _beginDragPosition.y;
			var signX = Mathf.Sign(deltaX);
			var signY = Mathf.Sign(deltaY);

			_boxRect.sizeDelta = new Vector2(
				x: Mathf.Abs(deltaX) / _canvas.transform.localScale.x,
				y: Mathf.Abs(deltaY) / _canvas.transform.localScale.y);

			_boxRect.localScale = new Vector2(signX, signY);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			_boxRect.gameObject.SetActive(false);
		}

		public bool IsInsideRect(Vector3 position, Camera camera)
		{
			_rect.Set(x: _boxRect.position.x, 
				y: _boxRect.position.y, 
				width: _boxRect.rect.width, 
				height: _boxRect.rect.height);

			Debug.Log("rect: " + _rect.ToString());

			if (_boxRect.rect.Contains(camera.WorldToScreenPoint(position)))
				return true;

			return false;
		}
	}
}
