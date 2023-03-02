using UnityEngine;

[CreateAssetMenu]
public class EntityData : ScriptableObject
{
	public float health;
	/// <summary>
	/// Скорость приближения к врагу в состоянии атаки
	/// </summary>
	public float moveSpeed;

	public float nextTileMoveSpeed;
	public float attackRadius;
	public float attackPower;
	public float attackSpeed;
}