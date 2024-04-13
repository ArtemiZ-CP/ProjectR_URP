using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaseObstacle))]
public class BaseObstacle_Inspector : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		BaseObstacle myScript = (BaseObstacle)target;

		if (myScript.CanDisplace)
		{
			GUIContent intFieldContent = new("Offset", "-1: left\n0: random\n1: right");
			myScript.OffsetAwake = EditorGUILayout.IntSlider(intFieldContent, myScript.OffsetAwake, -1, 1);
		}
	}
}
