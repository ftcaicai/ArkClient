using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsAnimClipController : MonoBehaviour
{
	List<TronTrail_CS> trailList = new List<TronTrail_CS>();
	float defaultTime = 0f;

	//AsAnimation m_Animation;

	// Use this for initialization
	void Start()
	{
//		m_Animation = transform.parent.GetComponent<AsAnimation>();
//		if(m_Animation == null)
//			Debug.LogError("AsAnimClipController::Start: no AsAnimation class is attach");

		UpdateTrailList();
	}

	public void UpdateTrailList()
	{
		trailList.Clear();
		trailList.AddRange( transform.gameObject.GetComponentsInChildren<TronTrail_CS>() as TronTrail_CS[]);
		if( trailList.Count > 0)
			defaultTime = trailList[0].time;
	}

	void tronOn( float _time)
	{
		if( trailList.Count == 0)
			UpdateTrailList();

		for( int i = 0; i < trailList.Count; ++i)
		{
			trailList[i].SetUpdate( true);
			if( _time > 0f)
			{
				trailList[i].SetTime( _time);
			}
			else
			{
				trailList[i].SetTime( defaultTime);
			}
		}
	}

	void tronOff()
	{
		for( int i = 0; i < trailList.Count; ++i)
		{
			trailList[i].SetUpdate( false);
		}
	}

	void EffectOn( int _index)
	{
	}

	public void OnAniHit( int _index)
	{
//		if( !Application.isEditor)
//			EntityManager.Instance.DispatchMessage( Convert.ToUInt32( gameObject.transform.parent.name), new Hit( _index));
	}

//	public void OnBulletOn()
//	{
////		return;
////		Entity myEntity = EntityManager.Instance.GetEntity( uint.Parse( gameObject.transform.parent.name));
////		Entity enemyEntity = EntityManager.Instance.GetEntity( myEntity.GetValue<uint>( "targetID"));
////		if( null == myEntity || null == enemyEntity)
////			return;
////
////		Vector3 dir = enemyEntity.GetValue<Vector3>( "vPosition") - myEntity.GetValue<Vector3>( "vPosition");
////
////		GameObject go = GameObject.Instantiate( Resources.Load( "Effect/Common/arrow01/arrow01")) as GameObject;
////		go.transform.rotation = Quaternion.LookRotation(dir);
////		go.transform.position = new Vector3( gameObject.transform.position.x, 1, gameObject.transform.position.z);
////
////		float time = Vector3.Distance( enemyEntity.GetValue<Vector3>( "vPosition"), myEntity.GetValue<Vector3>( "vPosition")) / 20.0f;
////
////		var transforms = enemyEntity.go_.transform.parent.GetComponentsInChildren<Transform>();
////		foreach( var trans in transforms)
////		{
////			if( trans.name == "Bip01 Spine1")
////			{
////        		iTween.MoveTo( go, iTween.Hash( "position", trans, "time", time));
////				break;
////			}
////		}
////
////		Destroy( go, time);
//	}
}