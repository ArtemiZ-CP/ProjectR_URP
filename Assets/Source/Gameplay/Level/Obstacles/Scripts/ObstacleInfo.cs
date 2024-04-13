using UnityEngine;

public class ObstacleInfo
{
	public BaseObstacle BaseObstacle;
	public Chunk Chunk;
	public CellType CellType = CellType.Empty;
	public StatusEffect StatusEffect = StatusEffect.None;
	public float StatusEffectDuration;
	public int Damage = 1;
	public int Line;
	public int Offset;
	public int Length;
	public bool IsDestroyable;
	public bool CanDisplace;

	public ObstacleInfo(Chunk chunk)
	{
		Chunk = chunk;
	}

	public Vector3 GetCellPosition()
	{
		return Chunk.ChunkGenerator.GetCellPosition(this);
	}

	public void ReloadCell()
	{
		BaseObstacle.Init(this);
		BaseObstacle.ChangeType();
	}
}
