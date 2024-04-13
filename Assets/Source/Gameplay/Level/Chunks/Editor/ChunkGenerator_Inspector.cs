using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(ChunkGenerator))]
public class ChunkGenerator_Inspector : Editor
{
	[SerializeField] private VisualTreeAsset _inspectorXML;

	public override VisualElement CreateInspectorGUI()
	{
		VisualElement myInspector = new();

		_inspectorXML.CloneTree(myInspector);

		ChunkGenerator script = (ChunkGenerator)target;

		var generateButton = myInspector.Q<Button>("Generate");
		generateButton.clicked += script.GenerateObstacles;

		var randomGenerateButton = myInspector.Q<Button>("RandomGenerate");
		randomGenerateButton.clicked += script.GenerateObstaclesWithRandomSeed;

		var destroyAllObstaclesButton = myInspector.Q<Button>("DestroyAllObstacles");
		destroyAllObstaclesButton.clicked += script.DestroyAllObstacles;

		#region Value1

		var value1 = myInspector.Q<Slider>("Value1");
		var min1 = myInspector.Q<Slider>("Min1");
		var max1 = myInspector.Q<Slider>("Max1");


		value1.RegisterValueChangedCallback(evt =>
		{
			if (evt.newValue < min1.value)
			{
				value1.value = min1.value;
			}

			if (evt.newValue > max1.value)
			{
				value1.value = max1.value;
			}
		});

		min1.RegisterValueChangedCallback(evt =>
		{
			if (evt.newValue > max1.value)
			{
				max1.value = evt.newValue;
			}

			if (value1.value < evt.newValue)
			{
				value1.value = evt.newValue;
			}
		});

		max1.RegisterValueChangedCallback(evt =>
		{
			if (evt.newValue < min1.value)
			{
				min1.value = evt.newValue;
			}

			if (value1.value > evt.newValue)
			{
				value1.value = evt.newValue;
			}
		});

		#endregion

		#region Value2

		var value2 = myInspector.Q<Slider>("Value2");
		var min2 = myInspector.Q<Slider>("Min2");
		var max2 = myInspector.Q<Slider>("Max2");


		value2.RegisterValueChangedCallback(evt =>
		{
			if (evt.newValue < min2.value)
			{
				value2.value = min2.value;
			}

			if (evt.newValue > max2.value)
			{
				value2.value = max2.value;
			}
		});

		min2.RegisterValueChangedCallback(evt =>
		{
			if (evt.newValue > max2.value)
			{
				max2.value = evt.newValue;
			}

			if (value2.value < evt.newValue)
			{
				value2.value = evt.newValue;
			}
		});

		max2.RegisterValueChangedCallback(evt =>
		{
			if (evt.newValue < min2.value)
			{
				min2.value = evt.newValue;
			}

			if (value2.value > evt.newValue)
			{
				value2.value = evt.newValue;
			}
		});

		#endregion

		#region Value3

		var value3 = myInspector.Q<Slider>("Value3");
		var min3 = myInspector.Q<Slider>("Min3");
		var max3 = myInspector.Q<Slider>("Max3");


		value3.RegisterValueChangedCallback(evt =>
		{
			if (evt.newValue < min3.value)
			{
				value3.value = min3.value;
			}

			if (evt.newValue > max3.value)
			{
				value3.value = max3.value;
			}
		});

		min3.RegisterValueChangedCallback(evt =>
		{
			if (evt.newValue > max3.value)
			{
				max3.value = evt.newValue;
			}

			if (value3.value < evt.newValue)
			{
				value3.value = evt.newValue;
			}
		});

		max3.RegisterValueChangedCallback(evt =>
		{
			if (evt.newValue < min3.value)
			{
				min3.value = evt.newValue;
			}

			if (value3.value > evt.newValue)
			{
				value3.value = evt.newValue;
			}
		});

		#endregion

		#region Value4

		var value4 = myInspector.Q<Slider>("Value4");
		var min4 = myInspector.Q<Slider>("Min4");
		var max4 = myInspector.Q<Slider>("Max4");


		value4.RegisterValueChangedCallback(evt =>
		{
			if (evt.newValue < min4.value)
			{
				value4.value = min4.value;
			}

			if (evt.newValue > max4.value)
			{
				value4.value = max4.value;
			}
		});

		min4.RegisterValueChangedCallback(evt =>
		{
			if (evt.newValue > max4.value)
			{
				max4.value = evt.newValue;
			}

			if (value4.value < evt.newValue)
			{
				value4.value = evt.newValue;
			}
		});

		max4.RegisterValueChangedCallback(evt =>
		{
			if (evt.newValue < min4.value)
			{
				min4.value = evt.newValue;
			}

			if (value4.value > evt.newValue)
			{
				value4.value = evt.newValue;
			}
		});

		#endregion

		#region Value5

		var value5 = myInspector.Q<Slider>("Value5");
		var min5 = myInspector.Q<Slider>("Min5");
		var max5 = myInspector.Q<Slider>("Max5");


		value5.RegisterValueChangedCallback(evt =>
		{
			if (evt.newValue < min5.value)
			{
				value5.value = min5.value;
			}

			if (evt.newValue > max5.value)
			{
				value5.value = max5.value;
			}
		});

		min5.RegisterValueChangedCallback(evt =>
		{
			if (evt.newValue > max5.value)
			{
				max5.value = evt.newValue;
			}

			if (value5.value < evt.newValue)
			{
				value5.value = evt.newValue;
			}
		});

		max5.RegisterValueChangedCallback(evt =>
		{
			if (evt.newValue < min5.value)
			{
				min5.value = evt.newValue;
			}

			if (value5.value > evt.newValue)
			{
				value5.value = evt.newValue;
			}
		});

		#endregion

		return myInspector;
	}
}
