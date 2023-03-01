using UnityEngine;

public class Tile : MonoBehaviour
{
	public LevelBiome biome;
	
	// для первой клетки это точка спавна игрока, а для остальных - точка его остановки
	public Transform playerPosition;

	public Transform enemiesPosition;
	public float enemiesSpawnRadius;

}