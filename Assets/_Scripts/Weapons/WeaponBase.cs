using System;
using UnityEngine;

public interface IWeapon
{
	void Attack(Entity _entity);
}

[Serializable]
public abstract class WeaponBase : MonoBehaviour, IWeapon
{
	public virtual void Attack(Entity _entity)
	{
		
	}
}