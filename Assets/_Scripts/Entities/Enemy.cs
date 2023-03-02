using UnityEngine;

public enum EnemyStates
{
	Sleep, Idle, Attack
}

public class Enemy : Entity
{
	private EnemyStates state;

	protected override void OnEnable()
	{
		base.OnEnable();
		
		state = EnemyStates.Sleep;
		
		GameManager.StartStage += OnStartStage;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		GameManager.StartStage -= OnStartStage;
	}

	protected override void OnAttacked()
	{
		base.OnAttacked();
		state = EnemyStates.Attack;
	}

	private void OnStartStage()
	{
		if (Player.Instance.currentStage == currentStage)
		{
			state = EnemyStates.Idle;
		}
	}

	private void Update()
	{
		switch (state)
		{
			case EnemyStates.Sleep:
				break;
			
			case EnemyStates.Idle:
				if (Player.Instance.isDead || Player.Instance.currentStage != currentStage) return;

				// ищем игрока в радиусе атаки
				if (Vector3.Distance(transform.position, Player.Instance.transform.position) <= startData.attackRadius)
				{
					state = EnemyStates.Attack;
				}
				break;
			
			case EnemyStates.Attack:
				// если игрок атаковал этого врага сам, или попал в радиус атаки, пытаемся атаковать его, или приближаемся
				
				if (Vector3.Distance(transform.position, Player.Instance.transform.position) > startData.attackRadius)
				{
					var t = transform;
					
					t.LookAt(Player.Instance.transform);
					t.position = Vector3.MoveTowards(t.position, Player.Instance.transform.position,
						startData.moveSpeed * Time.deltaTime);
				}
				else
				{
					// перезарядка
					if (Time.time > NextAttackTime)
					{
						weapon.Attack(this,Player.Instance);
						NextAttackTime = Time.time + RuntimeData.AttackSpeed;
					}
				}
				break;
			
			default:
				Debug.LogError("Unknown Enemy State");
				break;
		}
	}
}