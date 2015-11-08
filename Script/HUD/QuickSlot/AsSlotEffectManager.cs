using UnityEngine;
using System.Collections;

public class AsSlotEffectManager : MonoBehaviour
{
	public AsChargingEffect chargeEffect = null;
	public AsSlotUseEffect slotUseEffect = null;
	public AsUsableEffect usableEffect = null;
	public AsSkillChargeGuage chargeGuage = null;
	
	//begin kij 
	public AsChargingEffect strengthenIngEffect;
	public AsSlotUseEffect strengthenSuccEffect;
	public AsSlotUseEffect strengthenFailedEffect;
	//public AsSlotUseEffect strengthenFailedBrokenEffect;
	
	public void SetStrengthenIngEffect( bool isActive, Vector3 pos )		
	{
		if( null == strengthenIngEffect )
			return;
		
		if( false == isActive )
		{
			strengthenIngEffect.Enable = isActive;
			return;
		}
		
		strengthenIngEffect.Enable = true;		
		strengthenIngEffect.transform.position = new Vector3( pos.x, pos.y, pos.z - 2.0f);
	}
	
	public void SetStrengthenSuccEffect( bool isActive, Vector3 pos, bool isLoop = false)
	{
		if( null == strengthenSuccEffect )
			return;
		
		if( false == isActive )
		{
			strengthenSuccEffect.Enable = false;
			return;
		}
		
		strengthenSuccEffect.SetLoop( isLoop );
		strengthenSuccEffect.Draw( pos );
	}
	
	public void SetStrengthenFailedEffect( bool isActive, Vector3 pos, bool isLoop = false)
	{
		if( null == strengthenFailedEffect )
			return;
		
		if( false == isActive )
		{
			strengthenFailedEffect.Enable = false;
			return;
		}
		
		strengthenFailedEffect.SetLoop( isLoop );
		strengthenFailedEffect.Draw( pos );
	}
	
	
	//end kij
	
	private static AsSlotEffectManager instance = null;
	public static AsSlotEffectManager Instance
	{
		get	{ return instance; }
	}
	
	void Awake()
	{
		instance = this;
	}
	
	// Use this for initialization
	void Start()
	{
		chargeEffect.Enable = false;
		slotUseEffect.Enable = false;
		usableEffect.Enable = false;
		chargeGuage.Enable = false;
		
		strengthenIngEffect.Enable = false;
		strengthenSuccEffect.Enable = false;
		strengthenFailedEffect.Enable = false;
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void ShowChargeEffect( Vector3 pos)
	{
		chargeEffect.Enable = true;
		chargeEffect.transform.position = new Vector3( pos.x, pos.y, pos.z - 2.0f);
	}
	
	public void HideChargeEffect()
	{
		chargeEffect.Enable = false;
	}
	
	public void ShowUseEffect( Vector3 pos, bool isLoop = false)
	{		
		slotUseEffect.Draw( pos );
	}
	

	public void ShowUsableEffect( Vector3 pos)
	{
		usableEffect.Draw( pos);
	}
	
	void OnEnable()
	{
		chargeEffect.Enable = false;
		chargeGuage.Enable = false;
		strengthenIngEffect.Enable = false;
	}
}
