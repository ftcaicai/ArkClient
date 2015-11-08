using UnityEngine;
using System.Collections;

public class ProductionProgressItem : MonoBehaviour 
{
	
	
	private byte m_nSlot =0;
	
	protected void SetSlot( byte slot )
	{
		m_nSlot = slot;
	}
	
	
	public byte getSlot
	{
		get
		{
			return m_nSlot;
		}
	}
	
	
	//input	
	public virtual void GuiInputDown( Ray inputRay )
	{	
							
	}	
		  
	public virtual void GuiInputUp(Ray inputRay)
	{ 
				
	}
}
