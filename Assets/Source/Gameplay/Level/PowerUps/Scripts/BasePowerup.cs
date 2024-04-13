using UnityEditor;
using UnityEngine;

public class BasePowerup : BaseObject
{
	[SerializeField] private int _additionalEnergy;
	[SerializeField] private PowerupType _powerupType;
	[Header("Options")]
	[SerializeField] private bool _isDestroyable = false;

	private PowerupType _lastType;

	public bool IsDestroyable => _isDestroyable;
	public int AdditionalEnergy => _additionalEnergy;

	public override int Init()
	{
		_isDestroyable = true;
		return 1;
	}

	protected override void SpawnObstacle()
	{
		_lastType = _powerupType;

		foreach (PowerupInfo powerupInfo in CurrentChunkSettings.Settings.PowerupInfos)
		{
			if (powerupInfo.Type == _powerupType)
			{
				if (transform.childCount > 0)
				{
					foreach (Transform child in transform)
					{
						DestroyImmediate(child.gameObject);
					}
				}

#if UNITY_EDITOR
				PrefabUtility.InstantiatePrefab(powerupInfo.Prefab, transform);
#endif
			}
		}
	}

	private void Update()
	{
		if (Application.isPlaying == false)
		{
			if (_lastType != _powerupType && FindObjectOfTypeInParents<Chunk>() != null)
			{
				SpawnObstacle();
			}
		}
	}
}
