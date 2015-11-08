using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionEnemyList : MonoBehaviour {
	

	List<GameObject> listEnemy = new List<GameObject>();
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public GameObject GetEnemy(int index)
	{
		return listEnemy[index];
	}
	
	public void InitEnemyList()
	{
		GameObject goObj = GameObject.Find( "StartPosition");	
		listEnemy.Add(goObj);		
	}
	
	public void EnemyInsert(int count)
	{	
		GameObject goObj = GameObject.Find( "StartPosition");			
		GameObject goCopy;
		for(int i = 0; i < count; ++i)
		{
			Vector3 vecPos = goObj.transform.position;		
			vecPos.x += UnityEngine.Random.Range(-2f, 2F);
			vecPos.z += UnityEngine.Random.Range(-2f, 2F);	
			Quaternion rotate = Quaternion.identity;
			rotate.eulerAngles = new Vector3( 0.0f, 180, 0.0f );		
			goCopy = GameObject.Instantiate( goObj, vecPos, 	rotate ) as GameObject;			
			listEnemy.Add(goCopy);
		}
	}
	
	public void ChangeCount(int count)
	{
		if(listEnemy.Count < count)
		{
			EnemyInsert(count - listEnemy.Count );
		}
	}
}
