using UnityEngine;

namespace Chromium.UILibrary
{
	[RequireComponent(typeof(RectTransform))]
	public class UIMouseFollower : MonoBehaviour
	{
		private RectTransform _rectTransform;

		private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
		}

		private void Update()
		{
			_rectTransform.position = Input.mousePosition;
		}
	}
}
