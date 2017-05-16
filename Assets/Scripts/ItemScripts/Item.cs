using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
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
}
