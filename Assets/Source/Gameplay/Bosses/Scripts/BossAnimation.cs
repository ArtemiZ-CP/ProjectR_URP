using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossAnimation : MonoBehaviour
{
	private readonly int _baseAttack = Animator.StringToHash("Attack1");
	private readonly int _bigAttack = Animator.StringToHash("Attack2");

	private Animator _animator;

	private void Awake()
	{
		_animator = GetComponent<Animator>();
	}

	public void BaseAttack()
	{
		_animator.SetTrigger(_baseAttack);
	}

	public void BigAttack()
	{
		_animator.SetTrigger(_bigAttack);
	}
}
