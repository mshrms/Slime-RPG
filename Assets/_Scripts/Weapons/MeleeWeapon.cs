using UnityEngine;

public class MeleeWeapon : WeaponBase
{
	public override void Attack(Entity _attacker, Entity _target)
	{
		// сразу наносим урон
		Debug.Log(_attacker + " Is Attacking " + _target);
		
		_target.Attacked?.Invoke();
		
		_target.RuntimeData.CurrentHealth -= _attacker.RuntimeData.AttackPower;
		_target.UpdateHealthbar();

		if (_target.RuntimeData.CurrentHealth <= 0f)
		{
			_target.Death();
		}
	}
}