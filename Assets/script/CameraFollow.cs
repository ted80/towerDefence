﻿using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
	public GameObject cameraTarget;
	
	public float smoothTime = 0.1f;
	public bool cameraFollowX = true;
	public bool cameraFollowY = true;
	
	private Vector2 velocity;
	private Vector3 newPos;
	private float newXPos;
	private float newYPos;
	private Vector3 targetPos;
	
	public float maxXpos;
	public float minXpos;
	public float maxYpos;
	public float minYpos;
	
	void LateUpdate()
	{
		newPos = transform.position;
		targetPos = cameraTarget.transform.position;
		
		if (cameraFollowX)
		{
			newXPos = Mathf.SmoothDamp(newPos.x, targetPos.x, ref velocity.x, smoothTime);
		}
		if (cameraFollowY)
		{
			newYPos = Mathf.SmoothDamp(newPos.y, targetPos.y, ref velocity.y, smoothTime);
		}
		if(newXPos>maxXpos){
			newXPos = maxXpos;
		}else if(newXPos<minXpos){
			newXPos = minXpos;
		}
		if(newYPos>maxYpos){
			newYPos = maxYpos;
		}else if(newYPos<minYpos){
			newYPos = minYpos;
		}
		//Update camera position
		newPos = new Vector3(newXPos,newYPos,newPos.z);
		transform.position = newPos;
	}
}
