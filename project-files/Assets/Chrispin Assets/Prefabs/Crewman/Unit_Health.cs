﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Unit_Health : NetworkBehaviour 
{
	const int INIT_HEALTH = 100;

	[SyncVar (hook = "OnHealthChange")] 
	int health = INIT_HEALTH;

	Text healthText;

	bool shouldDie = false;
	public bool isDead = false;

	public delegate void DieDelegate();
	public event DieDelegate EventDie;

	public delegate void RespawnDelegate();
	public event RespawnDelegate EventRespawn;

	public override void OnStartLocalPlayer ()
	{
		healthText = GameObject.Find ("HealthText").GetComponent<Text>();
		SetHealthText();
	}
	
	// Update is called once per frame
	void Update () 
	{
		CheckCondition();
	}

	void CheckCondition()
	{
		if (health <= 0 && !shouldDie && !isDead)
		{
			shouldDie = true;
		}

		if (health <= 0 && shouldDie)
		{
			if (EventDie != null)
			{
				EventDie();
			}

			shouldDie = false;
		}

		if (health > 0 && isDead)
		{
			if (EventRespawn != null)
			{
				EventRespawn();
			}

			isDead = false;
		}
	}

	void SetHealthText()
	{
		if (isLocalPlayer)
		{
			healthText.text = "Health: " + health.ToString();
		}

	}

	public void takeDamage(int dmg)
	{
		health -= dmg;
	}

	void OnHealthChange(int newHealth)
	{
		health = newHealth;

		SetHealthText();
		CheckCondition();
	}

	public void ResetHealth()
	{
		health = INIT_HEALTH;
	}
}
