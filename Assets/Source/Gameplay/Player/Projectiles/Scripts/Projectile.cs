using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
	private Vector3 _direction;
	private float _speed;

	public event Action<Projectile, Collider> OnHit;

	public void Init(float speed, Vector3 direction)
	{
		_speed = speed;
		_direction = direction.normalized;
	}

	private void Update()
	{
		transform.position += _direction * _speed * Time.deltaTime;
	}

	private void OnTriggerEnter(Collider other)
	{
		OnHit?.Invoke(this, other);
	}
}
