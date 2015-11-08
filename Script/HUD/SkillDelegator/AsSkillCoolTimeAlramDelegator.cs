using UnityEngine;
using System.Collections;

public class AsSkillCoolTimeAlramDelegator : MonoBehaviour
{
	private GameObject m_Icon = null;
	public SpriteText textCoolTime = null;
	
	public GameObject Icon
	{
		get { return m_Icon;}
		set
		{
			m_Icon = value;
			m_Icon.transform.parent = transform;
			m_Icon.transform.localPosition = new Vector3( 0.0f, 0.55f, -0.1f);
			m_Icon.transform.localRotation = Quaternion.identity;
			m_Icon.transform.localScale = Vector3.one;
		}
	}

	void Start()
	{
	}
	
	void Update()
	{
	}
}
