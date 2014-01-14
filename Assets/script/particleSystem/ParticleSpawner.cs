﻿using UnityEngine;
using System.Collections;

public class ParticleSpawner : MonoBehaviour {

	[SerializeField]
	private GameObject[] particles;

	[SerializeField]
	private float spawnTime;

	private float timeCount;


	void Update () {
		if(Time.deltaTime>(spawnTime+timeCount)){
			timeCount += Time.deltaTime-timeCount;
			int loops = (int)Mathf.Floor( timeCount/spawnTime);
			Spawn();
			for (int i = 0; i<loops;i++){
				timeCount -=spawnTime;
				Spawn();
			}
		}else{
			timeCount -= Time.deltaTime;
		}
	}

	void Spawn(){
		Debug.Log("Spawn Spawn " );
	}
}
