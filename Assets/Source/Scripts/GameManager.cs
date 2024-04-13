using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private Boss _currentBoss;

	public static GameManager Instance;

	public Boss CurrentBoss => _currentBoss;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
