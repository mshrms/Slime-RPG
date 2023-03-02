using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class RangedWeapon : WeaponBase
{
	public GameObject projectilePrefab;
	public float projectileFlightDuration;
	public AnimationCurve projectileFlightCurve;
	public AnimationCurve projectileVertFlightCurve;

	public ObjectPool<GameObject> projectilePool;

	private void Awake()
	{
		projectilePool = new ObjectPool<GameObject>(() =>
		{
			return Instantiate(projectilePrefab, transform);
		}, _projectile =>
		{
			_projectile.gameObject.SetActive(true);
		}, _projectile =>
		{
			_projectile.gameObject.SetActive(false);
		}, _projectile =>
		{
			Destroy(_projectile);
		}, true, 5, 30);
	}

	public override void Attack(Entity _attacker, Entity _target)
	{
		// создаем прожектайл и запускаем корутину полета до цели
		Debug.Log(_attacker + " Is Attacking " + _target);

		StartCoroutine(ShootProjectile(_attacker, _target));
	}

	private IEnumerator ShootProjectile(Entity _attacker, Entity _target)
	{
		Vector3 projectileOffset = new Vector3(0f, 0.5f, 0f);
		
		var projectile = projectilePool.Get();
		var projPos = _attacker.transform.position;
		projPos += projectileOffset;
		projectile.transform.position = projPos;

		var startPos = projPos;

		var targetPos = _target.transform.position + projectileOffset;

		for (float t = 0f; t < 1f; t += Time.deltaTime / projectileFlightDuration)
		{
			Vector3 newPosition = new Vector3(
				Mathf.LerpUnclamped(startPos.x, targetPos.x, projectileFlightCurve.Evaluate(t)),
				Mathf.LerpUnclamped(startPos.y, startPos.y + 1f, projectileVertFlightCurve.Evaluate(t)),
				Mathf.LerpUnclamped(startPos.z, targetPos.z, projectileFlightCurve.Evaluate(t)));

			projectile.transform.position = newPosition;
			
			yield return null;
		}

		projectilePool.Release(projectile);

		if (!_target.isDead)
		{
			_target.Attacked?.Invoke();

			_target.RuntimeData.CurrentHealth -= _attacker.RuntimeData.AttackPower;
			_target.UpdateHealthbar();
			
			if (_target.RuntimeData.CurrentHealth <= 0f)
			{
				_target.Death();
			}
		}
	}
}