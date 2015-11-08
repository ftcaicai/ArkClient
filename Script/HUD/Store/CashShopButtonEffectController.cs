using UnityEngine;
using System.Collections;

public class CashShopButtonEffectController : MonoBehaviour {
	
	public GameObject[] effectObjects;
	
	public float deltaTime = 0.0f;
	public float coolTime = 0.0f;
	// Use this for initialization
	void Start () 
	{
		 coolTime = AsTableManager.Instance.GetTbl_GlobalWeight_Record(169).Value * 0.001f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (deltaTime >= coolTime)
		{
			OnEffect();
			deltaTime = 0.0f;
		}
		else
		{
			deltaTime += Time.deltaTime;
		}
		
	}
	
	void OnEffect()
	{
		foreach(GameObject obj in effectObjects)
		{
			obj.SetActive(false);
			obj.SetActive(true);

			Animation ani = obj.GetComponent<Animation>();
			if (ani != null)
				ani.Play();
		}
	}
}
