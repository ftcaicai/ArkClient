using UnityEngine;
using System.Collections;

public class HadItemManagement
{
    private Inventory m_Inventory = new Inventory();
    private QuickSlot m_QuickSlot = new QuickSlot();
	private Storage m_Storage = new Storage();
	
	
	
	public Inventory Inven
	{
		get
		{
			return m_Inventory;
		}
	}

    public QuickSlot QuickSlot
	{
		get
		{
            return m_QuickSlot;
		}
	}
	
	public Storage Storage
	{
		get
		{
			return m_Storage;
		}
	}
}
