using UnityEngine;
using UnityEngine.Pool;

public enum LevelBiome
{
	Forest, Desert, Mountain
}

public class TilePools : MonoBehaviour
{
	public Tile forestTilePrefab;
	public Tile desertTilePrefab;
	public Tile mountainTilePrefab;
	
	private ObjectPool<Tile> forestPool;
	private ObjectPool<Tile> desertPool;
	private ObjectPool<Tile> mountainPool;

	private void Awake()
	{
		forestPool = CreatePoolOfType(LevelBiome.Forest);
		desertPool = CreatePoolOfType(LevelBiome.Desert);
		mountainPool = CreatePoolOfType(LevelBiome.Mountain);
	}

	private ObjectPool<Tile> CreatePoolOfType(LevelBiome _biome)
	{
		GameObject prefab;
		
		switch (_biome)
		{
			case LevelBiome.Forest:
				prefab = forestTilePrefab.gameObject;
				break;
			case LevelBiome.Desert:
				prefab = desertTilePrefab.gameObject;
				break;
			case LevelBiome.Mountain:
				prefab = mountainTilePrefab.gameObject;
				break;
			default:
				Debug.LogError("Unknown Biome");
				prefab = null;
				break;
		}
		
		var pool = new ObjectPool<Tile>(() =>
		{
			return Instantiate(prefab, transform).GetComponent<Tile>();
		}, _tile =>
		{
			_tile.gameObject.SetActive(true);
		}, _tile =>
		{
			_tile.gameObject.SetActive(false);
		}, _tile =>
		{
			Destroy(_tile.gameObject);
		}, true, 6, 20);

		return pool;
	}

	public Tile GetTileOfType(LevelBiome _biome)
	{
		Tile tile;
		
		switch (_biome)
		{
			case LevelBiome.Forest:
				forestPool.Get(out tile);
				break;
			case LevelBiome.Desert:
				desertPool.Get(out tile);
				break;
			case LevelBiome.Mountain:
				mountainPool.Get(out tile);
				break;
			default:
				Debug.Log("Unknown Biome");
				tile = null;
				break;
		}

		return tile;
	}

	public void ReleaseTile(Tile _tile)
	{
		switch (_tile.biome)
		{
			case LevelBiome.Forest:
				forestPool.Release(_tile);
				break;
			case LevelBiome.Desert:
				desertPool.Release(_tile);
				break;
			case LevelBiome.Mountain:
				mountainPool.Release(_tile);
				break;
			default:
				Debug.Log("Unknown Biome");
				break;
		}
	}
}