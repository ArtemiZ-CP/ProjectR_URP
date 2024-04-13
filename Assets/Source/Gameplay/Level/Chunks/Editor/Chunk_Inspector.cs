using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Chunk))]
public class Chunk_Inspector : Editor
{
	[SerializeField] private VisualTreeAsset _inspectorXML;

	public override VisualElement CreateInspectorGUI()
	{
		VisualElement myInspector = new();

		_inspectorXML.CloneTree(myInspector);

		Chunk script = (Chunk)target;

		var spawnObjectButton = myInspector.Q<Button>("SpawnObject");
		spawnObjectButton.clicked += script.SpawnObject;

		var spawnPowerUpButton = myInspector.Q<Button>("SpawnPowerUp");
		spawnPowerUpButton.clicked += script.SpawnPowerUp;

		var reloadButton = myInspector.Q<Button>("Reload");
		reloadButton.clicked += script.Reload;

		var roadLength = myInspector.Q<IntegerField>("RoadLength"); 
		roadLength.RegisterValueChangedCallback(evt =>
		{
			if (evt.newValue < 1)
			{
				roadLength.value = 1;
			}
		});

		return myInspector;
	}
}
