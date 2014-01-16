﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour {
	
	[SerializeField]
	private GameObject itemHolder;
	
	private float screenHeight;
	private float screenWidth;
	private Vector3 topRight;
	private Vector3 itemDisplasment;
	
	internal List<GameObject> buttonList;
	public List<GameObject> items;
	
	internal int itemLenght;
	
	[SerializeField]
	private Camera uiCam;
	private int shootTimer = 0;
	public int shootTime = 10;
	private GameObject bulletHolder;
	
	//line
	private LineRenderer lineRenderer;
	private int lengthOfLineRenderer = 2;
	[SerializeField]
	private Color lineColor1 = Color.yellow;
	[SerializeField]
	private Color lineColor2 = Color.red;
	[SerializeField]
	private Color lineColorTower1 = Color.yellow;
	[SerializeField]
	private Color lineColorTower2 = Color.red;
	[SerializeField]
	private float linewidth1 = 0.1f;
	[SerializeField]
	private float linewidth2 = 0.1f;
	
	//tower
	private float towerSpawnDistace = 0.3f;
	private TowerManager towerMngr;

	//gun
	[SerializeField]
	private GameObject gun;
	[SerializeField]
	private Transform gunBulletSpawn;
	
	private int currentItem = 0;
	
	void Awake () {
		buttonList = new List<GameObject>();
		towerMngr = GameObject.Find("gameManager").GetComponent<TowerManager>() as TowerManager;
		bulletHolder = GameObject.Find("bullets");
		itemLenght = items.Count;

		screenHeight = 2f * Camera.main.orthographicSize;
		screenWidth = screenHeight * Camera.main.aspect;
		topRight = new Vector3( screenWidth/2, screenHeight/2,10);
		itemDisplasment = new Vector3( -0.2f, -0.2f,0);
		
		for ( int i = 0; i < itemLenght; i++){
			CreateButton(i);
		}
		setItem(itemLenght-1);
		currentItem = itemLenght-1;
		
		//create line
		lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.SetColors(lineColor1, lineColor2);
		lineRenderer.SetWidth(linewidth1, linewidth2);
		lineRenderer.SetVertexCount(lengthOfLineRenderer);
		lineRenderer.material = new Material (Shader.Find("Particles/Additive"));
		lineRenderer.enabled = false;
		lineRenderer.sortingLayerName = "player";
	}
	
	public void CreateButton( int i){
		//create empry gameobject
		GameObject button = new GameObject("itemIcon");
		
		// add renderer
		SpriteRenderer img = button.AddComponent<SpriteRenderer>();
		img.sprite = items[i].GetComponent<Item>().iconS;
		img.sortingLayerName = "ui";
		
		//add collider
		BoxCollider2D collider = button.AddComponent<BoxCollider2D>();
		collider.size = new Vector2(0.64f,0.64f);
		collider.isTrigger = true;
		
		//add data script 
		ItemButton buttonData = button.AddComponent<ItemButton>();
		buttonData.on = false;
		buttonData.colorScale = 0;
		
		//position button
		button.transform.parent = itemHolder.transform;
		button.transform.position = itemHolder.transform.position+topRight+itemDisplasment+new Vector3(-0.32f+i*-0.64f,-0.32f,0);
		button.layer = LayerMask.NameToLayer("UI");
		
		buttonList.Add (button);
	}
	
	public void setItem(int item){
		for ( int i = 0; i < itemLenght; i++){
			if(item ==i){
				buttonList[i].GetComponent<ItemButton>().on = true;
			}else{
				buttonList[i].GetComponent<ItemButton>().on = false;
			}
		}
	}

	void FixedUpdate(){
		for ( int i = 0; i < itemLenght; i++){
			if(buttonList[i].GetComponent<ItemButton>().on){
				if(buttonList[i].GetComponent<ItemButton>().colorScale<1)
					buttonList[i].GetComponent<ItemButton>().colorScale+=0.1f;
			}else{
				if(buttonList[i].GetComponent<ItemButton>().colorScale>0)
					buttonList[i].GetComponent<ItemButton>().colorScale-=0.1f;
			}
		}

		for ( int i = 0; i < itemLenght; i++){
			buttonList[i].GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white,Color.grey,buttonList[i].GetComponent<ItemButton>().colorScale);
		}
	}
	
	void Update(){
		float scrol = -Input.GetAxis("Mouse ScrollWheel");
		if(scrol!=0){
			if(scrol>0){
				if(currentItem<itemLenght-1){
					currentItem++;
				}
			}else{
				if(currentItem>0){
					currentItem--;
				}
			}
			setItem(currentItem);
		}
		if(shootTimer>0){
			shootTimer--;
		}
		
		//get input and mouse position
		bool click = Input.GetMouseButtonDown(0);
		bool firing = Input.GetMouseButton(0);
		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		Ray uiRay = uiCam.ScreenPointToRay(Input.mousePosition);
		Vector3 mousePosition = new Vector3(mouseRay.origin.x,mouseRay.origin.y,0);
		Vector2 mousePos2D = new Vector2(mouseRay.origin.x,mouseRay.origin.y);
		Vector2 thisPos2D = new Vector2(transform.position.x,transform.position.y);
		Vector2 gunPos2D = new Vector2(gun.transform.position.x,gun.transform.position.y);

		//rotate gun
		Quaternion gunRotation = movement.RotateToPoint(gun.transform,mousePosition);
		if(transform.localScale.x>0){
			gunRotation.eulerAngles = new Vector3(gunRotation.eulerAngles.x,
			                                      gunRotation.eulerAngles.y,
			                                      gunRotation.eulerAngles.z - 90-transform.localEulerAngles.z);
		}else{
			gunRotation.eulerAngles = new Vector3(gunRotation.eulerAngles.x,
			                                      gunRotation.eulerAngles.y,
			                                      -(gunRotation.eulerAngles.z + 90)+transform.localEulerAngles.z);
		}
		gun.transform.localRotation = gunRotation;
		
		if(itemLenght>currentItem){
			Collider2D uiHit = Physics2D.OverlapPoint(uiRay.origin
			                                          ,1 << LayerMask.NameToLayer("UI"));
			// ui click
			if(uiHit&&click){   
				for (int i = 0;i<itemLenght;i++){
					if(buttonList[i].GetComponent<Collider2D>() == uiHit){
						currentItem = i;
						setItem(currentItem);
						break;
					}
				}
			// game mouse click
			}else{
				itemType currentItemType = items[currentItem].GetComponent<Item>().type;
				switch(currentItemType){
				case itemType.Tower:
					
					//cast aim ray
					Vector2 mousedirection = mousePos2D-gunPos2D;
					RaycastHit2D aimRay = Physics2D.Raycast(gunPos2D
															,mousedirection
															,5
					                                        ,1 << LayerMask.NameToLayer("Level") | 1 << LayerMask.NameToLayer("Towers")
					);
					if(aimRay.collider==null){
						//draw aim line if no wall found
						lineRenderer.enabled = true;
						lineRenderer.SetPosition(0, gun.transform.position);
						Vector2 aim = mousedirection.normalized*5 + gunPos2D;
						lineRenderer.SetPosition(1, new Vector3(aim.x,aim.y,0));
						lineRenderer.SetColors(lineColor1, lineColor2);
					}else{
						if (aimRay.collider.transform.gameObject.layer == LayerMask.NameToLayer("Towers")) {
							//draw aim line if tower found
							lineRenderer.enabled = true;
							lineRenderer.SetPosition(0, gun.transform.position);
							lineRenderer.SetPosition(1, aimRay.point);
							lineRenderer.SetColors(lineColorTower1, lineColorTower2);
							//sell tower
							if (click){   
								towerMngr.SellTower(aimRay.collider.transform.gameObject);
							}
						}else{
							//draw aim line if wall found
							lineRenderer.enabled = true;
							lineRenderer.SetPosition(0, gun.transform.position);
							lineRenderer.SetPosition(1, aimRay.point);
							lineRenderer.SetColors(lineColor1, lineColor2);
						   //spawn tower
						   if (click){                      
								Collider2D mouseCircle = Physics2D.OverlapCircle(aimRay.point
								                                                 ,towerSpawnDistace
								                                                 ,1 << LayerMask.NameToLayer("Towers"));
								if(mouseCircle!=null){
								}else{
									towerMngr.LoadTower(aimRay.point,items[currentItem],aimRay.normal);
								}
							}
						}
					}
					break;
				case itemType.Bullet:
					//fire bullet
					lineRenderer.enabled = false;
					if (firing&&shootTimer==0){
						shootTimer = shootTime;
						GameObject bul = GameObject.Instantiate( items[currentItem]
						                                        ,gunBulletSpawn.position 
						                                        ,movement.RotateToPoint(gun.transform,mousePosition)) as GameObject;
						bul.transform.parent = bulletHolder.transform;
					}
					break;
				}
			}
		}
	}
}


