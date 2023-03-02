using System;
using UnityEngine;

public interface IWeapon
{
	void Attack(Entity _attacker,Entity _target);
}

[Serializable]
public abstract class WeaponBase : MonoBehaviour, IWeapon
{
	public virtual void Attack(Entity _attacker, Entity _target)
	{
		
	}
}