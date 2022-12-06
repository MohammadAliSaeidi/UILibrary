using UnityEngine;

namespace Chromium.UILibrary
{
	[RequireComponent(typeof(RectTransform))]
	public class UITransformFollower : MonoBehaviour
	{
		[SerializeField]
		private Transform _target;

		[SerializeField]
		private Vector3 _offset;

		[SerializeField]
		private bool _autoFindMainCameraAtStart;

		[SerializeField]
		private Camera _camera;

		private RectTransform _rectTransform;

		private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
		}

		private void Start()
		{
			if (_autoFindMainCameraAtStart)
			{
				_camera = Camera.main;
			}
		}

		private void Update()
		{
			if (_target && _camera)
			{
				_rectTransform.position = _camera.WorldToScreenPoint(_target.position) + _offset;
			}
		}
	}
}
