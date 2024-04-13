using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChunkSettings", menuName = "ScriptableObjects/ChunkSettings", order = 1)]
public class ChunkSettings : ScriptableObject
{
	[Header("Road")]
	[SerializeField] private float _roadOffset;
	[SerializeField] private GameObject _roadPrefab;
	[Header("Base")]
	[SerializeField] private BaseObstacle _baseObstaclePrefab;
	[SerializeField] private BasePowerup _basePowerupPrefab;
	[SerializeField] private BossProjectile _bossProjectilePrefab;
	[Header("Curve World")]
	[SerializeField] private List<Material> _materialsLit;
	[SerializeField] private List<Material> _materialsUnlit;
	[SerializeField] private float _curveAmountX;
	[SerializeField] private float _curveAmountY;
	[Header("Powerups")]
	[SerializeField] private List<PowerupInfo> _powerupInfos = new();

	public float RoadOffset => _roadOffset;
	public List<PowerupInfo> PowerupInfos => _powerupInfos;
	public BaseObstacle BaseObstaclePrefab => _baseObstaclePrefab;
	public BasePowerup BasePowerupPrefab => _basePowerupPrefab;
	public BossProjectile BossProjectilePrefab => _bossProjectilePrefab;
	public GameObject RoadPrefab => _roadPrefab;

	private void OnValidate()
	{
		SetShader();
	}

	private void SetShader()
	{
		if (_materialsLit.Count != 0)
		{
			foreach (var material in _materialsLit)
			{
				if (material == null)
				{
					continue;
				}

				material.shader = Shader.Find("Shader Graphs/Curved World Lit");

				material.SetFloat("_Curve_X", _curveAmountX);
				material.SetFloat("_Curve_Y", _curveAmountY);
			}
		}

		if (_materialsUnlit.Count != 0)
		{
			foreach (var material in _materialsUnlit)
			{
				if (material == null)
				{
					continue;
				}

				material.shader = Shader.Find("Shader Graphs/Curved World Unlit");

				material.SetFloat("_Curve_X", _curveAmountX);
				material.SetFloat("_Curve_Y", _curveAmountY);
			}
		}
	}
}
