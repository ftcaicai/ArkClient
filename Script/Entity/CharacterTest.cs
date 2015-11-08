using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterTest : MonoBehaviour {
	
//	GameObject obj;
	CharacterController control;
	
	void Start () {
//		obj = GameObject.Find("DivineKnightMale");
//		obj.animation.Play("Idle");
//		obj.animation["Idle"].wrapMode = WrapMode.Loop;
//		obj.animation["SkillMagneticField"].wrapMode = WrapMode.PingPong;
		
//		MorphTargets[] morph = GetComponentsInChildren<MorphTargets>();
//		foreach(MorphTargets node in morph)
//		{
//			Debug.Log(node);
//		}
	}
	
	void Update () {
	
	}
	
//	#region - ongui -
//	void OnGUI()
//	{
//		if(GUI.Button(new Rect(10, 10, 200, 50), "begin") == true)
//		{
//			obj.animation.CrossFade("SkillMagneticField", 0.1f);
//			StartCoroutine("AnimEnded", obj.animation["SkillMagneticField"].length);
//		}
//	}
//	#endregion
//	
//	IEnumerator AnimEnded(float _time)
//	{
//		yield return new WaitForSeconds(_time + 1);
//		obj.animation.CrossFadeQueued("Idle", 1);
//	}
}
