using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Составляет уровни согласно настройкам из LevelSettings
/// </summary>
public class LevelBuilder : MonoBehaviour
{
	#region Public/Serialized Fields

	public static LevelBuilder Instance;

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

	[HideInInspector]
	public List<Tile> currentLevelTileList;
	[HideInInspector]
	public List<Entity> currentLevelEntityList;

	#endregion
	
	#region MonoBehaviour Methods

	private void Awake()
	{
		Instance = this;
		
		currentLevelTileList = new List<Tile>();
		currentLevelEntityList = new List<Entity>();
		
	}

	#endregion

	private void Start()
	{
		BuildLevel(0);
		currentLevelNumber = 0;
		currentStageNumber = 0;
	}

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

				Enemy[] prefabArray;

				if (level.stages[i].isBossStage && level.bossPrefabs.Length > 0)
				{
					prefabArray = level.bossPrefabs;
				}
				else
				{
					prefabArray = level.enemyPrefabs;
				}
				
				int randomNum = Random.Range(0, prefabArray.Length);
				var enemyType = prefabArray[randomNum].type;
					
				var entity = entityPools.GetEntityOfType(enemyType);
				currentLevelEntityList.Add(entity);

				entity.currentLevel = currentLevelNumber;
				entity.currentStage = i;

				var spawnPos = curTile.enemiesPosition.position;
				var posInCircle = Random.insideUnitCircle * curTile.enemiesSpawnRadius;

				var newSpawnPos = new 
					Vector3(spawnPos.x + posInCircle.x, spawnPos.y, spawnPos.z + posInCircle.y);

				entity.transform.position = newSpawnPos;
			}
		}
		
		// спавним игрока на первой клетке
		var player = entityPools.GetEntityOfType(EntityType.Player);
		currentLevelEntityList.Add(player);

		Player.Instance = (Player)player;
		
		player.transform.position = currentLevelTileList[0].playerPosition.position;
		
		player.currentLevel = currentLevelNumber;
		player.currentStage = 0;
	}

	#endregion
}