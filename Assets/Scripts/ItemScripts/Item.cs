using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
	public string itemName;
	public string itemDescription;

	public float movementSpeed;
	public float bulletSpeed;
	public float bulletKnockBack;
	public float bulletSize;
	public float bulletDelay;
	public float bulletDelayMultiplier;
	public float bulletDamage;
	public float bulletDamageMultiplier;
	public float maxHealth;

	void Update() {
		transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);
	}

	public virtual void apply(MainCharacterController p){
		if (p.movementSpeed + movementSpeed < 5)
			p.movementSpeed = 5;
		else if (p.movementSpeed + movementSpeed > 12)
			p.movementSpeed = 12;
		else
			p.movementSpeed += movementSpeed;

		if (p.bulletSpeed + bulletSpeed < 250)
			p.bulletSpeed = 250;
		else if (p.bulletSpeed + bulletSpeed > 1000)
			p.bulletSpeed = 1000;
		else
			p.bulletSpeed += bulletSpeed;

		p.bulletKnockBack += bulletKnockBack;

		if (p.bulletSize + bulletSize < .5f)
			p.bulletSize = .5f;
		else if (p.bulletSize + bulletSize > 3)
			p.bulletSize = 3;
		else
			p.bulletSize += bulletSize;

		if(p.bulletDelay-bulletDelay < 0.1f)
			p.bulletDelay = 0.1f;
		else
			p.bulletDelay -= bulletDelay;
		
		if (bulletDelayMultiplier > 0) {
			if (p.bulletDelay / bulletDelayMultiplier < 0.1f)
				p.bulletDelay = 0.1f;
			else
				p.bulletDelay /= bulletDelayMultiplier;
		} else if (bulletDelayMultiplier < 0) {
			p.bulletDelay *= -bulletDelayMultiplier;
		}

		p.bulletDamage += bulletDamage;

		if (bulletDamageMultiplier > 0)
			p.bulletDamage *= bulletDamageMultiplier;
		else if (bulletDamageMultiplier < 0)
			p.bulletDamage /= -bulletDamageMultiplier;

		p.maxHealth += maxHealth;
		p.currentHealth += maxHealth;
	}


	private const string PLUS = " + ";
	private const string MINUS = " - ";
	private const string TIMES = " * ";
	private const string DIVIDED = " / ";
	private const string NO_EFFECT = " 0 ";
	public void GetModifierStrings (out string mSpeed, out string bDamage, out string bDelay, out string bSpeed, out string bSize, out string bKnockback, out string mHealth) {
		mSpeed = GetAdditiveModifier (movementSpeed);
		bDamage = GetAdditiveModifier (bulletDamage);
		bDelay = GetAdditiveModifier (bulletDelay);
		bSpeed = GetAdditiveModifier (bulletSpeed);
		bSize = GetAdditiveModifier (bulletSize);
		bKnockback = GetAdditiveModifier (bulletKnockBack);
		mHealth = GetAdditiveModifier (maxHealth);
		if (bulletDamage == 0) {
			if (bulletDamageMultiplier < 0) {
				bDamage = DIVIDED + (-bulletDamageMultiplier);
			} else if (bulletDamageMultiplier != 1 && bulletDamageMultiplier != 0) {
				bDamage = TIMES + bulletDamageMultiplier;
			} else {
				bDamage = NO_EFFECT;
			}
		}

		if (bulletDelay == 0) {
			if (bulletDelayMultiplier < 0) {
				bDelay = TIMES + (-bulletDamageMultiplier);
			} else if (bulletDamageMultiplier != 1 && bulletDamageMultiplier != 0) {
				bDelay = DIVIDED + bulletDamageMultiplier;
			} else {
				bDelay = NO_EFFECT;
			}
		}
	}

	private string GetAdditiveModifier (float modifier) {
		if (modifier > 0) {
			return PLUS + modifier;
		} else if (modifier == 0) {
			return NO_EFFECT;
		} else {
			return MINUS + (-modifier);
		}
	}
}
