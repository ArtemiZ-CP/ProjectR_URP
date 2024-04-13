using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(MapVisualizer))]
public class MapVisualizer_Inspector : Editor
{
	[SerializeField] private VisualTreeAsset _inspectorXML;

	public override VisualElement CreateInspectorGUI()
	{
		VisualElement myInspector = new();

		_inspectorXML.CloneTree(myInspector);

		var customInspector = new IMGUIContainer(OnInspectorGUI);
		myInspector.Add(customInspector);

		return myInspector;
	}

	public override void OnInspectorGUI()
	{
		MapVisualizer script = (MapVisualizer)target;

		EditorGUILayout.Space();

		if (script.NoiseTexture != null)
		{
			float previewWidth = EditorGUIUtility.currentViewWidth * script.ViewScale;
			float aspectRatio = (float)script.NoiseTexture.width / script.NoiseTexture.height;
			float previewHeight = previewWidth / aspectRatio;

			float previewOffset = EditorGUIUtility.currentViewWidth * (1- script.ViewScale) / 2 - 20;

			Rect previewRect = new(previewOffset, GUILayoutUtility.GetLastRect().yMax + 10, previewWidth, previewHeight);
			EditorGUI.DrawPreviewTexture(previewRect, script.NoiseTexture);

			EditorGUILayout.Space(previewHeight + 20f);
		}
		else
		{
			EditorGUILayout.HelpBox("Noise texture is not assigned.", MessageType.Warning);
		}
	}
}