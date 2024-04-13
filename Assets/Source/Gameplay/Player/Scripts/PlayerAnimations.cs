using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimations : MonoBehaviour
{
	private readonly int Tumble = Animator.StringToHash("Tumble");
	private readonly int Jump = Animator.StringToHash("Jump");
	private readonly int IsGrounded = Animator.StringToHash("IsGrounded");
	private readonly int RunSpeed = Animator.StringToHash("RunSpeed");

	[SerializeField] private PlayerMovement _playerMovement;
	[SerializeField] private PlayerHealth _playerHealth;

	private Animator _animator;

	private void Awake()
	{
		_animator = GetComponent<Animator>();
	}

	private void OnEnable()
	{
		_animator.SetBool(IsGrounded, _playerMovement.IsGrounded);
		_playerMovement.OnGroundedChange += OnGroundedChange;
		_playerMovement.OnJump += () => _animator.SetTrigger(Jump);
		_playerMovement.OnTumble += OnTumble;
	}

	private void OnDisable()
	{
		_playerMovement.OnGroundedChange -= OnGroundedChange;
		_playerMovement.OnJump -= () => _animator.SetTrigger(Jump);
		_playerMovement.OnTumble -= OnTumble;
	}

	private void Update()
	{
		_animator.SetFloat(RunSpeed, _playerMovement.CurrentSpeedMultiplyer);
	}

	private void OnGroundedChange(bool isGrounded)
	{
		_animator.SetBool(IsGrounded, isGrounded);
	}

	private void OnTumble(float duration)
	{
		_animator.SetTrigger(Tumble);
	}
}
