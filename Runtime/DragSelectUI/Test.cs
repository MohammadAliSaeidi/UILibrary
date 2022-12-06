using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromium.UILibrary
{
    public class Test : MonoBehaviour
    {
        private Vector2 _startMousePos;

		[SerializeField]
		private List<GameObject> goList = new List<GameObject>();

		public DragAndSelectUI DragAndSelectUI;

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				_startMousePos = Input.mousePosition;
				Debug.Log("Start pos of mouse: " + _startMousePos);
			}
			if (Input.GetKeyUp(KeyCode.Mouse0))
			{
				Debug.Log("End pos of mouse: " + Input.mousePosition);
				foreach (var go in goList)
				{
					if (DragAndSelectUI.IsInsideRect(go.transform.position, Camera.main))
					{
						Debug.Log(go.name);
					}
				}
			}
		}
	}
}
