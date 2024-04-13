using UnityEngine;
using UnityEngine.SceneManagement;

public class Reload : MonoBehaviour
{
	public void RaloadLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
