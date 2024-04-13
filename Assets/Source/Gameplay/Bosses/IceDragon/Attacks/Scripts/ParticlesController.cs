using UnityEngine;

public class ParticlesController : MonoBehaviour
{
	[SerializeField] private ParticleSystem _particleSystem;

	public void Play()
	{
		_particleSystem.Play();
		Invoke(nameof(Stop), 1);
	}

	private void Stop()
	{
		_particleSystem.Stop();
	}
}
