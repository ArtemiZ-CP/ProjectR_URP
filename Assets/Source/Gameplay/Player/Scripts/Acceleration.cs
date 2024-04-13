using UnityEngine;

public class Acceleration : MonoBehaviour
{
	[SerializeField] private float _accelerationPerSec;
	[Header("Status effects")]
	[SerializeField] private PlayerStatusEffects _playerStatusEffects;
	[SerializeField] private float _speedRatioOnFreeze;

	private float _baseMultiplier = 1;
	private float _multiplier = 1;

	public float Multiplyer => _multiplier;

	private void Update()
	{
		IncreaseMultiplyer();

		_multiplier = _baseMultiplier;

		if (_playerStatusEffects.HasStatusEffect(StatusEffect.Freeze))
		{
			_multiplier *= _speedRatioOnFreeze;
		}
	}

	private void IncreaseMultiplyer()
	{
		_baseMultiplier += _accelerationPerSec * Time.deltaTime;
	}
}
