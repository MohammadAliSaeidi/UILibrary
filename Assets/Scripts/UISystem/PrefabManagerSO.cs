using System;
using UnityEditor;
using UnityEngine;

namespace UIManager
{
	// Remove [CreateAssetMenu] when you've created an instance, because you don't need more than one.
	public class PrefabManagerSO : ScriptableObject
	{
#if UNITY_EDITOR
		public GameObject UIScreen;
#endif
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(PrefabManagerSO))]
	public class PrefabManagerSOEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.HelpBox("If you move this file somewhere else, also change the path in UiLibraryMenus! ", MessageType.Info);
		}
	}
#endif
}
