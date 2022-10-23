using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UIManager
{
	public static class UiLibraryMenus
	{
		private const int MenuPriority = -50;
		private static Canvas _lastSelectedCanvas;

		private const string PrefabManagerPath = "Assets/UI Library/Prefabs/UILibraryPrefabManager.asset";
		private static PrefabManagerSO LocatePrefabManager()
		{
			return AssetDatabase.LoadAssetAtPath<PrefabManagerSO>(PrefabManagerPath);
		}

		private static void SafeInstantiate(Func<PrefabManagerSO, GameObject> itemSelector, Transform transformParent)
		{
			var prefabManager = LocatePrefabManager();

			if (!prefabManager)
			{
				Debug.LogWarning($"PrefabManager not found at path {PrefabManagerPath}");
				return;
			}

			var item = itemSelector(prefabManager);
			var instance = PrefabUtility.InstantiatePrefab(item, transformParent);

			Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");
			Selection.activeObject = instance;
		}

		// Same method from the start of the blog post.
		[MenuItem("GameObject/UILibrary/UIScreen", priority = MenuPriority)]
		private static void CreateUIScreen()
		{
			var canvas = GetParentForUI().transform;
			SafeInstantiate(prefabManager => prefabManager.UIScreen, canvas);
		}

		private static Transform GetParentForUI()
		{
			Transform parent;
			Canvas canvas;
			var selectedGO = Selection.activeGameObject;

			if (IsSelectionCanvasChild())
			{
				parent = selectedGO.transform;
				canvas = selectedGO.GetComponentInParent<Canvas>();
			}

			else if (_lastSelectedCanvas && _lastSelectedCanvas.gameObject.activeInHierarchy)
			{
				canvas = _lastSelectedCanvas;
				parent = canvas.transform;
			}

			else if (IsSelectionCanvas() && selectedGO.activeInHierarchy)
			{
				canvas = selectedGO.GetComponent<Canvas>();
				parent = canvas.transform;
			}

			else
			{
				canvas = UnityEngine.Object.FindObjectOfType<Canvas>(includeInactive: false);

				if (canvas == null)
				{
					canvas = new GameObject("canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
					Undo.RegisterCreatedObjectUndo(canvas, $"Create {canvas.name}");
				}

				parent = canvas.transform;
			}

			_lastSelectedCanvas = canvas;

			return parent;
		}

		private static bool IsSelectionCanvas()
		{
			return Selection.activeGameObject && Selection.activeGameObject.GetComponent<Canvas>();
		}

		private static bool IsSelectionCanvasChild()
		{
			return Selection.activeGameObject && Selection.activeGameObject.GetComponentInParent<Canvas>();
		}
	}
}
