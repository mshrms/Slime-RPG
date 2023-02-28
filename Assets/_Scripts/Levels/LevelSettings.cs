using System;
using UnityEngine;

[CreateAssetMenu]
public class LevelSettings : ScriptableObject
{
	public Vector3 tileSpawnStartPoint;

	/// <summary>
	/// Используется для сдвига точки спавна нового тайлика
	/// </summary>
	public float tileWidth;
	
	public Level[] levels;
}

[Serializable]
public struct Level
{
	public LevelBiome biome;
	public Stage[] stages;

	// TODO: базовые префабы врагов, которые автолевелятся согласно текущему уровню
	public Enemy[] enemyPrefabs;
	public Enemy[] bossPrefabs;
}

[Serializable]
public struct Stage
{
	public int enemiesCount;
	public bool isBossStage;
}