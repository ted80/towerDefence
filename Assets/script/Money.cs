﻿using UnityEngine;
using System.Collections;

public class Money : MonoBehaviour 
{
	private int amount = 10;
	private GameObject p;

	void Start () {
		p = GameObject.Find("player");
	}

	void Update () {
		float dis = Vector3.Distance(p.transform.position, transform.position); 
		if(dis < 0.5F) {
			pickUp();
		} else if(dis < 5) {
			Vector3 newpos = transform.position;
			Vector2 change = movement.ForceAndAngleToDirection(0.1F / (dis * 2), movement.angleToPoint(transform, p.transform.position)); // Vector3.Angle(transform.position, p.transform.position));

			newpos.x += change.x;
			newpos.y += change.y;

			transform.position = newpos;
		}
	}

	void pickUp() {
		//todo: add amount to money
		GameObject.Destroy(gameObject);
	}
}
