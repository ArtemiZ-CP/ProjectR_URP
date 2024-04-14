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
	[SerializeField] private List<Material> _materialsTextureLit;
	[SerializeField] private List<Material> _materialsSpriteUnlit;
	[SerializeField] private List<Material> _materialsUnlit;
	[SerializeField] private float _curveMultiplier;
	[SerializeField] private Vector2 _curveDirection;
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
		SetShader(_materialsLit, "Shader Graphs/Curved World Lit");
		SetShader(_materialsTextureLit, "Shader Graphs/Curved World Texture Lit");
		SetShader(_materialsSpriteUnlit, "Shader Graphs/Curved World Sprite Unlit");
		SetShader(_materialsUnlit, "Shader Graphs/Curved World Unlit");
	}

	private void SetShader(List<Material> materials, string path)
	{
		if (materials == null || materials.Count == 0)
		{
			return;
		}

		foreach (var material in materials)
		{
			if (material == null)
			{
				continue;
			}

			material.shader = Shader.Find(path);

			material.SetFloat("_Curve_X", _curveDirection.normalized.x);
			material.SetFloat("_Curve_Y", _curveDirection.normalized.y);
			material.SetFloat("_Curve", _curveMultiplier);
		}
	}
}
