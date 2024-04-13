using UnityEngine;

public class ObstacleCollider : MonoBehaviour
{
	[SerializeField] private Obstacle _obstacle;

	public Obstacle Obstacle => _obstacle;
}
