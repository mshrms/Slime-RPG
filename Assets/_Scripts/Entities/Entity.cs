using System;
using System.Text;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public abstract class Entity : MonoBehaviour
{
	public TMP_Text hpText;
	public Image barFillingTexture;
	
	public int currentLevel;
	public int currentStage;

	public bool isDead;
	
	public Action Attacked;
	
	/// <summary>
	/// Способ атаки сущности. Может лупить вблизи/кидать прожектайлы и тд.
	/// </summary>
	public WeaponBase weapon;
	
	public EntityType type;
	
	/// <summary>
	/// Начальные параметры сущности
	/// </summary>
	public EntityData startData;

	/// <summary>
	/// Изменяемые игровые параметры сущности
	/// </summary>
	public RuntimeEntityData RuntimeData;
	
	protected float NextAttackTime;

	private StringBuilder sb;

	private void Awake()
	{
		sb = new StringBuilder();
	}

	// TODO: модификатор уровня
	protected virtual void OnEnable()
	{
		isDead = false;
		
		Attacked += OnAttacked;
		
		RuntimeData.MaxHealth = startData.health;
		RuntimeData.CurrentHealth = startData.health;
		UpdateHealthbar();
		
		RuntimeData.MoveSpeed = startData.moveSpeed;
		RuntimeData.AttackRadius = startData.attackRadius;
		RuntimeData.AttackPower = startData.attackPower;
		RuntimeData.AttackSpeed = startData.attackSpeed;
	}

	protected virtual void OnDisable()
	{
		Attacked -= OnAttacked;
	}

	protected virtual void OnAttacked()
	{
		
	}

	public virtual void Death()
	{
		isDead = true;
		LevelBuilder.Instance.entityPools.ReleaseEntity(this);
	}

	public virtual void UpdateHealthbar()
	{
		float fillPercentage = Mathf.InverseLerp(0f, RuntimeData.MaxHealth, RuntimeData.CurrentHealth);
		barFillingTexture.fillAmount = fillPercentage;

		sb.Clear();
		sb.Append(RuntimeData.CurrentHealth);
		sb.Append("/");
		sb.Append(RuntimeData.MaxHealth);
		hpText.text = sb.ToString();
	}
}


[Serializable]
public struct RuntimeEntityData
{
	public float CurrentHealth;
	public float MaxHealth;
	/// <summary>
	/// У игрока - скорость прокрутки до нового тайлика
	/// У врагов - скорость движения в сторону игрока при обнаружении
	/// </summary>
	public float MoveSpeed;
	public float AttackRadius;
	public float AttackPower;
	public float AttackSpeed;
}