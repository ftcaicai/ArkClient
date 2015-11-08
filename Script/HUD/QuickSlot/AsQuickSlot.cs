using UnityEngine;
using System.Collections;



public class AsQuickSlot : MonoBehaviour
{
	public AsSlotPackChanger packChanger = null;
	public AsSlotPack packSlot;
	private int curPackIndex = 0;
	//private int prevPackIndex = 0;
	
	public GameObject 		m_goLockImg;
	
	public int getCurPackIndex
	{
		get { return curPackIndex; }
	}
	
	public void	SetLock( bool bSet )
	{
		if( gameObject.active == false )
			return;
		
		m_goLockImg.SetActiveRecursively( bSet );
	}

	public void ActiveToggle(bool bLock)
	{
		if( gameObject.active == true )
			Off();
		else
			On(bLock);
	}
	
	public void On( bool bLock )
	{
		gameObject.SetActive(true);
		SetLock( bLock );
		
		foreach( AsSlot slot in packSlot.slotArray )
		{
			slot.Disable = false;
		}
	}

	public void Off()
	{
		gameObject.SetActive(false);
		
		foreach( AsSlot slot in packSlot.slotArray )
		{
			slot.Disable = true;
		}
	}
	
	public bool IsOn()
	{
		return gameObject.activeSelf;
	}
	
	void Awake()
	{
		if( m_goLockImg == null )
		{
			Transform lockTransform = gameObject.transform.Find("Unlocked");
			if( lockTransform != null )
				m_goLockImg = lockTransform.gameObject;
		}
	}

	
	// Use this for initialization
	void Start()
	{
		if( null == packChanger)
			return;	
		
		packSlot.Show = true;
	}
	
	// Update is called once per frame
	void Update()
	{
		if( null == packChanger)
			return;
		
		if( curPackIndex == packChanger.curIndex)
			return;
		
		curPackIndex = packChanger.curIndex;
		
		UpdatePackState();
	}
	
	public void BeginCooltime( int skillID)
	{
		packSlot.BeginCooltime( skillID);
	}
	
	public void BeginCharge( int skillID)
	{		
		packSlot.BeginCharge( skillID);
	}
	
	private void UpdatePackState()
	{
		// begin 여기에서 퀵슬롯 변경
//		if( curPackIndex != prevPackIndex)
//			packArray[ prevPackIndex].Show = false;
//		packArray[ curPackIndex].Show = true;
		// end

		//prevPackIndex = curPackIndex;
	}
	
	public void DisableAllPack( bool flag)
	{		
		packSlot.DisableAllSlot( flag);
	}
	
	void OnEnable()
	{		
		packSlot.Show = true;
	}
	
	//$yde
	public void ChangeStance(StanceInfo _stance)
	{
		packSlot.ChangeStance(_stance);
	}
}

