using UnityEngine;

[CreateAssetMenu]
public class EntityData : ScriptableObject
{
	/// <summary>
	/// Способ атаки сущности. Может лупить вблизи/кидать прожектайлы и тд.
	/// </summary>
	public WeaponBase weapon;
	public float health;
	/// <summary>
	/// У игрока - скорость прокрутки до нового тайлика
	/// У врагов - скорость движения в сторону игрока при обнаружении
	/// </summary>
	public float moveSpeed;
	public float attackRadius;
	public float attackPower;
	public float attackSpeed;
}