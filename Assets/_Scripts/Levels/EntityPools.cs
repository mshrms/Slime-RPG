using UnityEngine;
using UnityEngine.Pool;

public enum EntityType
{
	Player, MeleeEnemy, RangedEnemy, MeleeEnemyBoss, RangedEnemyBoss
}

public class EntityPools : MonoBehaviour
{
	public Entity playerPrefab;
	
	public Entity meleeEnemyPrefab;
	public Entity rangedEnemyPrefab;
	
	public Entity meleeEnemyBossPrefab;
	public Entity rangedEnemyBossPrefab;
	
	private ObjectPool<Entity> playerPool;
	
	private ObjectPool<Entity> meleeEnemyPool;
	private ObjectPool<Entity> rangedEnemyPool;
	
	private ObjectPool<Entity> meleeEnemyBossPool;
	private ObjectPool<Entity> rangedEnemyBossPool;

	private void Awake()
	{
		playerPool = CreatePoolOfType(EntityType.Player);
		
		meleeEnemyPool = CreatePoolOfType(EntityType.MeleeEnemy);
		rangedEnemyPool = CreatePoolOfType(EntityType.RangedEnemy);
		
		meleeEnemyBossPool = CreatePoolOfType(EntityType.MeleeEnemyBoss);
		rangedEnemyBossPool = CreatePoolOfType(EntityType.RangedEnemyBoss);
	}

	private ObjectPool<Entity> CreatePoolOfType(EntityType _type)
	{
		GameObject prefab;
		
		switch (_type)
		{
			case EntityType.Player:
				prefab = playerPrefab.gameObject;
				break;
			case EntityType.MeleeEnemy:
				prefab = meleeEnemyPrefab.gameObject;
				break;
			case EntityType.RangedEnemy:
				prefab = rangedEnemyPrefab.gameObject;
				break;
			case EntityType.MeleeEnemyBoss:
				prefab = meleeEnemyBossPrefab.gameObject;
				break;
			case EntityType.RangedEnemyBoss:
				prefab = rangedEnemyBossPrefab.gameObject;
				break;
			default:
				Debug.LogError("Unknown Entity");
				prefab = null;
				break;
		}

		var pool = new ObjectPool<Entity>(() =>
		{
			return Instantiate(prefab, transform).GetComponent<Entity>();
		}, _entity =>
		{
			_entity.gameObject.SetActive(true);
		}, _entity =>
		{
			_entity.gameObject.SetActive(false);
		}, _entity =>
		{
			Destroy(_entity.gameObject);
		}, true, 1, 50);

		return pool;
	}

	public Entity GetEntityOfType(EntityType _type)
	{
		Entity entity;
		
		switch (_type)
		{
			case EntityType.Player:
				playerPool.Get(out entity);
				break;
			case EntityType.MeleeEnemy:
				meleeEnemyPool.Get(out entity);
				break;
			case EntityType.RangedEnemy:
				rangedEnemyPool.Get(out entity);
				break;
			case EntityType.MeleeEnemyBoss:
				meleeEnemyBossPool.Get(out entity);
				break;
			case EntityType.RangedEnemyBoss:
				rangedEnemyBossPool.Get(out entity);
				break;
			default:
				Debug.LogError("Unknown Entity");
				entity = null;
				break;
		}

		return entity;
	}

	public void ReleaseEntity(Entity _entity)
	{
		switch (_entity.type)
		{
			case EntityType.Player:
				playerPool.Release(_entity);
				break;
			case EntityType.MeleeEnemy:
				meleeEnemyPool.Release(_entity);
				break;
			case EntityType.RangedEnemy:
				rangedEnemyPool.Release(_entity);
				break;
			case EntityType.MeleeEnemyBoss:
				meleeEnemyBossPool.Release(_entity);
				break;
			case EntityType.RangedEnemyBoss:
				rangedEnemyBossPool.Release(_entity);
				break;
			default:
				Debug.LogError("Unknown Entity");
				break;
		}
	}
}