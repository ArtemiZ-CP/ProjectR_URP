using UnityEngine;
using UnityEditor;

public static class CurrentChunkSettings
{
	public static ChunkSettings Settings { get; private set; }

	static CurrentChunkSettings()
	{
		InitializeSettings();
	}

#if UNITY_EDITOR
	[MenuItem("Tools/Initialize Chunk Settings")]
	private static void ManualInitializeSettings()
	{
		InitializeSettings();
	}
#endif

	private static void InitializeSettings()
	{
		Debug.Log("Initializing Chunk Settings");
		Settings = Resources.Load<ChunkSettings>("ChunkSettings");
	}
}