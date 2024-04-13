using System.Collections;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
	private int _damage = 1;
	private float _duration = 1;
	private AnimationCurve _moveCurve;
	private Vector3 _startPosition;
	private Vector3 _endPosition;

	public int Damage => _damage;

	public void Init(AnimationCurve moveCurve, Vector3 startPosition, Vector3 endPosition, float duration, int damage)
	{
		_moveCurve = moveCurve;
		_startPosition = startPosition;
		_endPosition = endPosition;
		_duration = duration;
		_damage = damage;
	}

	private void Start()
	{
		StartCoroutine(Move());
	}

	private IEnumerator Move()
	{
		float t = 0;

		while (t < 1)
		{
			t += Time.deltaTime / _duration;
			Vector3 newPosition = Boss.GetObstaclePosition(_moveCurve, _startPosition, _endPosition, t);
			transform.rotation = Quaternion.LookRotation(newPosition - transform.position);
			transform.position = newPosition;

			yield return null;
		}
	}
}
