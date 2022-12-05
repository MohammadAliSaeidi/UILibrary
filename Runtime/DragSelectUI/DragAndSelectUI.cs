using UnityEngine;
using UnityEngine.EventSystems;

namespace Chromium.UILibrary
{
    public class DragAndSelectUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private RectTransform _boxRect;

		private Vector2 _beginDragPosition;

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

			_boxRect.sizeDelta = new Vector2 (
				x: Mathf.Abs(deltaX), 
				y: Mathf.Abs(deltaY));

			_boxRect.localScale = new Vector2(signX, signY);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			_boxRect.gameObject.SetActive(false);
		}
	}
}
