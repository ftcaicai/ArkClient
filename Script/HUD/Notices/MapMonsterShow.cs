using UnityEngine;
using System.Collections;

public class MapMonsterShow : MonoBehaviour {
	
	[SerializeField] SimpleSprite center_;
	[SerializeField] SimpleSprite left_;
	[SerializeField] SimpleSprite right_;
	[SerializeField] SpriteText text_;
	
	[SerializeField] float time_;
	
	public void Init(string _str)
	{
		text_.Text = _str;
	}
	
	// Use this for initialization
	void Start () {
		CalcShape();
		
		Destroy(gameObject, time_);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void CalcShape()
	{
		center_.width = text_.TotalWidth;
		left_.transform.position = new Vector3( center_.transform.position.x - ( center_.width * 0.5f), center_.transform.position.y, center_.transform.position.z);
		right_.transform.position = new Vector3( center_.transform.position.x + ( center_.width * 0.5f), center_.transform.position.y, center_.transform.position.z);
	}
}
