using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Vector2))]
public class ChunkSettings_Inspector : PropertyDrawer
{
	private const float DiagramSize = 100f;
	private const float PointSize = 10f;
	private const float PointDistance = 40f;
	private const float SnapThreshold = 0.2f;

	private bool _isDragging = false;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		Vector2 vector = property.vector2Value;

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		Rect diagramRect = new(position.x, position.y, DiagramSize, DiagramSize);
		DrawDiagram(diagramRect);

		Vector2 centerPosition = GetCenterPosition(diagramRect);
		DrawCenterPoint(centerPosition);

		Vector2 pointPosition = GetPointPosition(centerPosition, vector);
		DrawPoint(pointPosition);

		HandleInput(diagramRect, centerPosition, property);

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return base.GetPropertyHeight(property, label) + DiagramSize;
	}

	private void DrawDiagram(Rect diagramRect)
	{
		EditorGUI.DrawRect(diagramRect, Color.black);
	}

	private Vector2 GetCenterPosition(Rect diagramRect)
	{
		return new Vector2(diagramRect.x + DiagramSize / 2f, diagramRect.y + DiagramSize / 2f);
	}

	private void DrawCenterPoint(Vector2 centerPosition)
	{
		Rect centerPointRect = new Rect(centerPosition.x - 2f, centerPosition.y - 2f, 4f, 4f);
		EditorGUI.DrawRect(centerPointRect, Color.yellow);
	}

	private Vector2 GetPointPosition(Vector2 centerPosition, Vector2 vector)
	{
		Vector2 pointOffset = new Vector2(vector.x, -vector.y).normalized * PointDistance;
		return centerPosition + pointOffset;
	}

	private void DrawPoint(Vector2 pointPosition)
	{
		Rect pointRect = new(pointPosition.x - PointSize / 2f, pointPosition.y - PointSize / 2f, PointSize, PointSize);
		EditorGUI.DrawRect(pointRect, Color.white);
	}

	private void HandleInput(Rect diagramRect, Vector2 centerPosition, SerializedProperty property)
	{
		Event currentEvent = Event.current;

		if (currentEvent.type == EventType.MouseDown && diagramRect.Contains(currentEvent.mousePosition))
		{
			_isDragging = true;
			currentEvent.Use();
		}
		else if (currentEvent.type == EventType.MouseUp)
		{
			_isDragging = false;
		}
		else if (currentEvent.type == EventType.MouseDrag && _isDragging)
		{
			Vector2 newPosition = GetSnappedPosition(currentEvent.mousePosition, centerPosition);
			property.vector2Value = newPosition;
			currentEvent.Use();
		}
	}

	private Vector2 GetSnappedPosition(Vector2 mousePosition, Vector2 centerPosition)
	{
		Vector2 mouseOffset = mousePosition - centerPosition;
		Vector2 newPosition = new(mouseOffset.normalized.x, -mouseOffset.normalized.y);

		newPosition.x = SnapToKeyPoint(newPosition.x);
		newPosition.y = SnapToKeyPoint(newPosition.y);

		return newPosition;
	}

	private float SnapToKeyPoint(float value)
	{
		if (Mathf.Abs(value) < SnapThreshold)
			return 0f;
		else if (Mathf.Abs(value - 1f) < SnapThreshold)
			return 1f;
		else if (Mathf.Abs(value + 1f) < SnapThreshold)
			return -1f;

		return value;
	}
}