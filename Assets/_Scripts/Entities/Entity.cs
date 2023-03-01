using UnityEngine;

public abstract class Entity : MonoBehaviour
{
	public EntityType type;
	
	/// <summary>
	/// Параметры сущности
	/// </summary>
	public EntityData data;
}