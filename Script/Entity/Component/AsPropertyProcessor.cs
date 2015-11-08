using UnityEngine;
using System.Collections;

public class AsPropertyProcessor : AsBaseComponent
{
	#region - init -
	void Awake()
	{
		m_ComponentType = eComponentType.PROPERTY_PROCESSOR;
	}
	
	void Start()
	{
	}
	#endregion
	
	#region - update & message -
	void Update()
	{
	}
	
//	public override void HandleMessage(AsIMessage _msg)
//	{
//	}
	#endregion
}
