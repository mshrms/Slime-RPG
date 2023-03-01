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

	/// <summary>
	/// Пулы сущностей
	/// </summary>
	public EntityPools entityPools;

	#endregion

	#region Private Fields

	private int currentLevelNumber;
	private int currentStageNumber;
	private LevelBiome currentLevelBiome;

	private List<Tile> currentLevelTileList;
	private List<Entity> currentLevelEntityList;

	#endregion
	
	#region MonoBehaviour Methods

	private void Awake()
	{
		currentLevelTileList = new List<Tile>();
		currentLevelEntityList = new List<Entity>();
	}

	#endregion
	
	#region Methods

	public void ClearLevel()
	{
		for (int i = 0; i < currentLevelTileList.Count; i++)
		{
			tilePools.ReleaseTile(currentLevelTileList[i]);
		}

		for (int i = 0; i < currentLevelEntityList.Count; i++)
		{
			entityPools.ReleaseEntity(currentLevelEntityList[i]);
		}
		
		currentLevelNumber = 0;
		currentStageNumber = 0;
		
		currentLevelTileList.Clear();
		currentLevelEntityList.Clear();
	}
	
	public void BuildLevel(int _levelNumber)
	{
		// читаем настройки уровней, создаем нужное количество клеток нужного типа, заполняем ими список,
		// спавним игрока на первой клетке в его позиции, отправляем событие в момент по окончанию сборки уровня
		currentLevelNumber = _levelNumber;
		
		var level = settings.levels[currentLevelNumber];
		
		currentLevelBiome = level.biome;
		Vector3 currentTileSpawnPos = settings.tileSpawnStartPoint;

		if (_levelNumber < 0 || _levelNumber >= settings.levels.Length)
		{
			Debug.LogError("Wrong Level Number");
			return;
		}
		
		if (currentLevelTileList.Count > 0 || currentLevelEntityList.Count > 0)
		{
			ClearLevel();
		}

		// проходимся по этапам текущего уровня и создаем тайлик на каждый из них
		for (int i = 0; i < level.stages.Length; i++)
		{
			var tile = tilePools.GetTileOfType(currentLevelBiome);
			currentLevelTileList.Add(tile);

			tile.transform.position = currentTileSpawnPos;
			currentTileSpawnPos.x += settings.tileWidth;
			
			// создаем врагов на тайлике
			for (int j = 0; j < level.stages[i].enemiesCount; j++)
			{
				var curTile = currentLevelTileList[i];

				// TODO: логика спавна боссов и обычных врагов повторяется!!!!!!
				if (level.stages[i].isBossStage && level.bossPrefabs.Length > 0)
				{
					int randomBossNum = Random.Range(0, level.bossPrefabs.Length);
					var bossType = level.bossPrefabs[randomBossNum].type;
					
					var boss = entityPools.GetEntityOfType(bossType);
					currentLevelEntityList.Add(boss);

					var spawnPos = curTile.enemiesPosition.position;
					var posInCircle = Random.insideUnitCircle * curTile.enemiesSpawnRadius;

					var newSpawnPos = new 
						Vector3(spawnPos.x + posInCircle.x, spawnPos.y, spawnPos.z + posInCircle.y);

					boss.transform.position = newSpawnPos;
				}
				else if (level.enemyPrefabs.Length > 0)
				{
					int randomEnemyNum = Random.Range(0, level.enemyPrefabs.Length);
					var enemyType = level.enemyPrefabs[randomEnemyNum].type;
					
					var enemy = entityPools.GetEntityOfType(enemyType);
					currentLevelEntityList.Add(enemy);
					
					var spawnPos = curTile.enemiesPosition.position;
					var posInCircle = Random.insideUnitCircle * curTile.enemiesSpawnRadius;

					var newSpawnPos = new 
						Vector3(spawnPos.x + posInCircle.x, spawnPos.y, spawnPos.z + posInCircle.y);

					enemy.transform.position = newSpawnPos;
				}
			}
		}
		
		// спавним игрока на первой клетке
		var player = entityPools.GetEntityOfType(EntityType.Player);
		currentLevelEntityList.Add(player);
		
		player.transform.position = currentLevelTileList[0].playerPosition.position;
	}

	#endregion
}