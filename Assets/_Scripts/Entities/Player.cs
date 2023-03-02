using System.Collections;
using UnityEngine;

public enum PlayerStates
{
	Idle, Attack, Move, Win
}

public class Player : Entity
{
	public static Player Instance;

	private PlayerStates state;

	public AnimationCurve moveCurve;

	// целевая позиция на следующем тайле, передается событием
	private Vector3 nextMovePos;
	private bool isMoving;
	
	protected override void OnEnable()
	{
		base.OnEnable();
		
		state = PlayerStates.Idle;

		GameManager.StartStage += OnStartStage;
	}

	public override void Death()
	{
		base.Death();
		GameManager.DeadScreen?.Invoke();
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		GameManager.StartStage -= OnStartStage;
	}
	
	private void OnStartStage()
	{
		state = PlayerStates.Attack;
	}

	//TODO: перенести рассчеты смены состояний в фиксед апдейт и сделать тик пореже
	private void Update()
	{
		switch (state)
		{
			// слайм ждет тапа игрока, чтобы начать новый этап (событие)
			case PlayerStates.Idle:
				break;

			// начать перемещение на следующий тайлик (событие)
			case PlayerStates.Move:
				if (!isMoving)
				{
					StartCoroutine(MoveToNextTile());
				}

				break;

			case PlayerStates.Attack:
				// находим ближайшего врага и атакуем его (если не получается, подходим ближе)
				
				var minDistance = Mathf.Infinity;
				Entity closestEnemy = null;
				
				foreach (var entity in LevelBuilder.Instance.currentLevelEntityList)
				{
					if (entity == Instance) continue;

					// не проверяем мертвых и спящих врагов на следующих этапах
					if (entity.currentStage != currentStage || entity.isDead) continue;

					var distance = Vector3.Distance(transform.position, entity.transform.position);
					if (distance < minDistance)
					{
						minDistance = distance;
						closestEnemy = entity;
					}
				}

				if (closestEnemy == null)
				{
					GameManager.WinScreen?.Invoke();
					state = PlayerStates.Move;
					break;
				}
				
				if (Vector3.Distance(transform.position, closestEnemy.transform.position) > RuntimeData.AttackRadius)
				{
					var t = transform;
					t.LookAt(closestEnemy.transform);
					t.position = Vector3.MoveTowards(t.position, closestEnemy.transform.position,
						RuntimeData.MoveSpeed * Time.deltaTime);
				}
				else
				{
					// перезарядка
					if (Time.time > NextAttackTime)
					{
						weapon.Attack(this, closestEnemy);
						NextAttackTime = Time.time + RuntimeData.AttackSpeed;
					}
				}

				break;

			default:
				Debug.LogError("Unknown Enemy State");
				break;
		}
	}

	private IEnumerator MoveToNextTile()
	{
		isMoving = true;
		var startPos = transform.position;
		
		for (float t = 0f; t < 1f; t += Time.deltaTime/RuntimeData.MoveSpeed)
		{
			Vector3.LerpUnclamped(startPos, nextMovePos, moveCurve.Evaluate(t));
			yield return null;
		}
		
		isMoving = false;
		state = PlayerStates.Idle;
	}
}