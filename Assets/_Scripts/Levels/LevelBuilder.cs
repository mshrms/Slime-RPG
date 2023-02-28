using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Составляет уровни согласно настройкам из LevelSettings
/// 
/// </summary>
public class LevelBuilder : MonoBehaviour
{
	#region Public/Serialized Fields

	/// <summary>
	/// Сценарий с описанием уровней
	/// </summary>
	public LevelSettings settings;

	/// <summary>
	/// Пулы тайлов. Используются при построении уровней
	/// </summary>
	public TilePools tilePools;

	public EntityPools entityPools;

	#endregion

	#region Private Fields

	private int currentLevelNumber;
	private int currentStageNumber;
	private List<Tile> currentLevelTileList;
	private LevelBiome currentLevelBiome;

	#endregion
	
	#region MonoBehaviour Methods

	private void Awake()
	{
		currentLevelTileList = new List<Tile>();
	}

	#endregion
	
	#region Methods

	public void ClearLevel()
	{
		for (int i = 0; i < currentLevelTileList.Count; i++)
		{
			tilePools.ReleaseTile(currentLevelTileList[i], currentLevelBiome);
		}
		
		currentLevelNumber = 0;
		currentStageNumber = 0;
		currentLevelTileList.Clear();
		currentLevelBiome = LevelBiome.None;
	}
	
	public void BuildLevel(int _levelNumber)
	{
		if (_levelNumber < 0 || _levelNumber >= settings.levels.Length)
		{
			Debug.LogError("Wrong Level Number");
			return;
		}
		
		if (currentLevelTileList.Count > 0)
		{
			ClearLevel();
		}
		
		// читаем настройки уровней, создаем нужное количество клеток нужного типа, заполняем ими список,
		// спавним игрока на первой клетке в его позиции, отправляем событие в момент по окончанию сборки уровня
		currentLevelNumber = _levelNumber;
		
		currentLevelBiome = settings.levels[currentLevelNumber].biome;
		Vector3 currentTileSpawnPos = settings.tileSpawnStartPoint;

		// проходимся по этапам текущего уровня и создаем тайлик на каждый из них
		for (int i = 0; i < settings.levels[currentLevelNumber].stages.Length; i++)
		{
			var tile = tilePools.GetTileOfType(currentLevelBiome);
			currentLevelTileList.Add(tile);

			tile.transform.position = currentTileSpawnPos;
			currentTileSpawnPos.x += settings.tileWidth;
		}
	}

	#endregion
}