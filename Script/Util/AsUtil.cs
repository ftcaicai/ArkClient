using UnityEngine;
using System;
using System.Collections;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Net;
using System.IO;
using System.Text;
using System.Net.NetworkInformation;

public class DepthComparer : IComparer
{
	public int Compare( object a, object b)
	{
		RaycastHit hit1 = ( RaycastHit)a;
		RaycastHit hit2 = ( RaycastHit)b;
		
		return hit1.distance.CompareTo( hit2.distance);
	}
}

public class AsUtil
{
	static public int DistanceCompare( AS_SC_NPC_APPEAR_2 appear1, AS_SC_NPC_APPEAR_2 appear2)
	{
		AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
		Debug.Assert( null != user);
		Vector3 pos = user.transform.position;
		
		float dist1 = Mathf.Abs( ( pos - appear1.sCurPosition).magnitude);
		float dist2 = Mathf.Abs( ( pos - appear2.sCurPosition).magnitude);
		
		return dist1.CompareTo( dist2);
	}
	
	static public string Base64Encode( string str)
	{
		return Convert.ToBase64String( System.Text.Encoding.GetEncoding( "euc-kr").GetBytes( str));
	}
	
	static public string Base64Decode( string str)
	{
		return System.Text.Encoding.GetEncoding( "euc-kr").GetString( System.Convert.FromBase64String( str));
	}
	
	static public string GetMacAddress()
	{
#if UNITY_IPHONE
		NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
		
		foreach( NetworkInterface adapter in nics)
		{
			PhysicalAddress address = adapter.GetPhysicalAddress();
			
			if( "" != address.ToString())
			{
				Debug.Log( address.ToString());
				return address.ToString();
			}
		}
		
		return "error lectura mac address";
#endif
		
#if UNITY_ANDROID
		string macAddr = null;
		AndroidJavaObject wifiMgr = null;
		
		using( AndroidJavaObject activity = new AndroidJavaClass( "com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>( "currentActivity"))
		{
			wifiMgr = activity.Call<AndroidJavaObject>( "getSystemService", "wifi");
		}
		
		macAddr = wifiMgr.Call<AndroidJavaObject>( "getConnectionInfo").Call<string>( "getMacAddress");
		macAddr = macAddr.Replace( ":", "");
		
		return macAddr;
#endif
		
		return null;//dopamin Complie error 
	}
		
	public static string ShowNetworkInterfaces()
	{
		StringBuilder sb = new StringBuilder();
		
		IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
		NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
		Console.WriteLine( "Interface information for {0}.{1}", computerProperties.HostName, computerProperties.DomainName);
		if( nics == null || nics.Length < 1)
			return "No network interfaces found.";
		
		Debug.Log( "Number of interfaces .................... : {0}" + nics.Length);
		foreach( NetworkInterface adapter in nics)
		{
//			IPInterfaceProperties properties = adapter.GetIPProperties();
			Debug.Log( "\n");
			Debug.Log( adapter.Description);
			Debug.Log( String.Empty.PadLeft( adapter.Description.Length, '='));
			Debug.Log( "Interface type .......................... : {0}" + adapter.NetworkInterfaceType);
			Debug.Log( "Physical address ........................ : ");
			
			PhysicalAddress address = adapter.GetPhysicalAddress();
			byte[] bytes = address.GetAddressBytes();
			for( int i = 0; i< bytes.Length; i++)
			{
				// Display the physical address in hexadecimal.
				sb.AppendFormat( "X2", bytes[i]);
				// Insert a hyphen after each byte, unless we are at the end of the 
				// address.
				if( i != bytes.Length -1)
					sb.Append( '-');
			}
			
			sb.AppendLine();
		}
		
		return sb.ToString();
	}
	
	static public bool PtInCollider( Camera cam, Collider col, Vector2 pt, bool all=false)
	{
		bool ret = false;
		
		if( true == all)
		{
			Ray ray = cam.ScreenPointToRay( new Vector3( pt.x, pt.y, 0.0f));
			RaycastHit hit = new RaycastHit();
			ret = col.Raycast( ray, out hit, 500.0f);
		}
		else
		{
			IComparer comp = new DepthComparer();
			
			Ray ray = cam.ScreenPointToRay( new Vector3( pt.x, pt.y, 0.0f));
			RaycastHit[] hits = Physics.RaycastAll( ray);
			if( 0 == hits.Length)
				return false;
			
			Array.Sort( hits, comp);

			ret = ( col == hits[0].collider) ? true : false;
		}
		
		return ret;
	}
	
	static public bool PtInCollider( Camera cam, Collider col, Collider excludeCol, Vector2 pt, bool all=false)
	{
		bool ret = false;
		
		if( true == all)
		{
			Ray ray = cam.ScreenPointToRay( new Vector3( pt.x, pt.y, 0.0f));
			RaycastHit hit = new RaycastHit();
			ret = col.Raycast( ray, out hit, 500.0f);
		}
		else
		{
			IComparer comp = new DepthComparer();
			
			Ray ray = cam.ScreenPointToRay( new Vector3( pt.x, pt.y, 0.0f));
			RaycastHit[] hits = Physics.RaycastAll( ray);
			if( 0 == hits.Length)
				return false;
			
			Array.Sort( hits, comp);
			
			int index = 0;
			if( excludeCol == hits[0].collider)
				index = 1;

			ret = ( col == hits[ index].collider) ? true : false;
		}
		
		return ret;
	}
	
	static public bool PtInCollider( Collider col, Ray ray, bool all=false)
	{
		bool ret = false;
		
		if( true == all)
		{
			RaycastHit hit = new RaycastHit();
			ret = col.Raycast( ray, out hit, 500.0f);
		}
		else
		{
			IComparer comp = new DepthComparer();
			
			RaycastHit[] hits = Physics.RaycastAll( ray);
			if( 0 == hits.Length)
				return false;
			
			Array.Sort( hits, comp);

			ret = ( col == hits[0].collider) ? true : false;
		}
		
		return ret;
	}
	
	static public bool PtInCollider( Collider col, Collider excludeCol, Ray ray, bool all=false)
	{
		bool ret = false;
		
		if( true == all)
		{
			RaycastHit hit = new RaycastHit();
			ret = col.Raycast( ray, out hit, 500.0f);
		}
		else
		{
			IComparer comp = new DepthComparer();
			
			RaycastHit[] hits = Physics.RaycastAll( ray);
			if( 0 == hits.Length)
				return false;
			
			Array.Sort( hits, comp);
			
			int index = 0;
			if( excludeCol == hits[0].collider)
				index = 1;
			
			ret = ( col == hits[ index].collider) ? true : false;
		}
		
		return ret;
	}
	
	static public float CharacterDecalSize( GameObject _EntityObject)
	{
		float fScale = 1.0f;
		CharacterController ctrl = _EntityObject.GetComponentInChildren<CharacterController>();
		if( null != ctrl)
		{
			if( ctrl.height > 8.0f)
				fScale = 3.0f;
			else if( ctrl.height > 6.0f)
				fScale = 2.0f;
		}
		
		return fScale;
	}
	
	public static bool IsEqual( float a, float b)
	{
		return ( 0.01f >= Mathf.Abs( a - b)) ? true : false;
	}
	
	public static string GetRealString( string str)
	{
		string[] strings = str.Split( '\0');
		return strings[0];
	}
	
	public static void Capture()
	{
		string captureCount =  System.DateTime.Now.ToString( "O").Replace( ":", "-");

		if( Application.isEditor == true)
			Application.CaptureScreenshot( "screenshot" + captureCount +".png");
		else
			Application.CaptureScreenshot( "..\\screenshot" + captureCount +".png");
	}
	
	public static void ShutDown( string msg)
	{
		Debug.LogError( msg);
		
#if UNITY_EDITOR
		EditorUtility.DisplayDialog( "Error", msg, "OK", null);
		EditorApplication.isPlaying = false;
		EditorApplication.isPaused = false;
#else
		Quit();
#endif
	}

	public static void Quit()
	{
#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
		EditorApplication.isPaused = false;
#else
	#if UNITY_ANDROID
		// dispose
		System.Diagnostics.ProcessThreadCollection ptc = System.Diagnostics.Process.GetCurrentProcess().Threads;
		foreach( System.Diagnostics.ProcessThread pt in ptc)
		{
			pt.Dispose();
		}
		System.Diagnostics.Process.GetCurrentProcess().Kill();
	#endif

		Application.Quit();
#endif
	}
	
	public static string ModifyMonsterDescriptionInTooltip( string desc, int id, int level, int curChargeStep)
	{
		Tbl_MonsterSkill_Record skillRecord = AsTableManager.Instance.GetTbl_MonsterSkill_Record( id);
		Tbl_MonsterSkillLevel_Record skillLevelRecord = AsTableManager.Instance.GetTbl_MonsterSkillLevel_Record( level, id);
//		Tbl_Action_Record actionRecord = AsTableManager.Instance.GetTbl_Action_Record( skillLevelRecord.SkillAction_Index);
		CharacterLoadData savedCharStat = AsUserInfo.Instance.SavedCharStat;

		StringBuilder sb = new StringBuilder( desc);
		StringBuilder sbParam = new StringBuilder();
		
		StringBuilder sbFormat = new StringBuilder();
		for( int i = 0; i < 5; i++)
		{
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "Potency{0}ActivityRatio", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
				sbParam.AppendFormat( "{0:F1}%", Mathf.Abs( skillRecord.listSkillPotency[i].Potency_Enable_ConditionValue) * 0.1f);
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "Potency{0}ActivityProb", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
				sbParam.AppendFormat( "{0:F1}%", Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Prob));
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "Potency{0}Ratio", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
				sbParam.AppendFormat( "{0:F1}%", Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Value) * 0.1f);
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "Potency{0}Value", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
				sbParam.AppendFormat( "{0:D}", (int)Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_IntValue));
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "Potency{0}Duration", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				float duration = skillLevelRecord.listSkillLevelPotency[i].Potency_Duration;
				int hour = (int)( duration / 3600.0f);
				int min = (int)( ( duration % 3600.0f) / 60.0f);
				float sec = ( duration % 3600.0f) % 60.0f;
				
				sbParam.Remove( 0, sbParam.Length);
				
				if( 0 == hour)
				{
					if( 0 == min)
						sbParam.AppendFormat( "{0:D}{1}", sec, AsTableManager.Instance.GetTbl_String(90));
					else
						sbParam.AppendFormat( "{0:D}{1} {2:D}{3}", min, AsTableManager.Instance.GetTbl_String(89),
							sec, AsTableManager.Instance.GetTbl_String(90));
				}
				else
				{
					sbParam.AppendFormat( "{0:D}{1} {2:D}{3} {4:D}{5}", hour, AsTableManager.Instance.GetTbl_String(88),
						min, AsTableManager.Instance.GetTbl_String(89), sec, AsTableManager.Instance.GetTbl_String(90));
				}
				
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}
			
			// Physical Attack
			float physicalAttack = ( savedCharStat.sFinalStatus.nPhysicDmg_Min + savedCharStat.sFinalStatus.nPhysicDmg_Max) * 0.5f;
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "P{0}Ratio", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
				sbParam.AppendFormat( "{0:F1}", physicalAttack * Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Value) * 0.001f);
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}
			
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "PAttack{0}Total", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
//				sbParam.AppendFormat( "{0:F1}", physicalAttack + ( physicalAttack * Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Value) * 0.001f)
//					+ Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_IntValue));
				AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
				if( null != user)
				{
					eRACE race = user.GetProperty<eRACE>( eComponentProperty.RACE);
					eCLASS __class = user.GetProperty<eCLASS>( eComponentProperty.CLASS);
					Tbl_Class_Record record = AsTableManager.Instance.GetTbl_Class_Record( race, __class);
					
					sbParam.AppendFormat( "{0:F1}", ( ( physicalAttack * skillLevelRecord.listSkillLevelPotency[i].Potency_Value * 0.001f)
						+ Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_IntValue)) * ( record.BaseAttackCycle * 0.001f));
					sb.Replace( sbFormat.ToString(), sbParam.ToString());
				}
			}
			
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "PAttack{0}Ratio", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
				sbParam.AppendFormat( "{0:F1}", physicalAttack + ( physicalAttack * Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Value) * 0.001f));
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}

			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "PAttack{0}Value", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
				sbParam.AppendFormat( "{0:F1}", physicalAttack + Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Value));
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}
			
			// Magical Attack
			float magicalAttack = ( savedCharStat.sFinalStatus.nMagicDmg_Min + savedCharStat.sFinalStatus.nMagicDmg_Max) * 0.5f;
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "MAttack{0}Total", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
//				sbParam.AppendFormat( "{0:F1}", magicalAttack + ( magicalAttack * Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Value) * 0.001f)
//					+ Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_IntValue));
				AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
				if( null != user)
				{
					eRACE race = user.GetProperty<eRACE>( eComponentProperty.RACE);
					eCLASS __class = user.GetProperty<eCLASS>( eComponentProperty.CLASS);
					Tbl_Class_Record record = AsTableManager.Instance.GetTbl_Class_Record( race, __class);
					
					sbParam.AppendFormat( "{0:F1}", ( ( magicalAttack * skillLevelRecord.listSkillLevelPotency[i].Potency_Value * 0.001f)
						+ Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_IntValue)) * ( record.BaseAttackCycle * 0.001f));
					sb.Replace( sbFormat.ToString(), sbParam.ToString());
				}
			}
			
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "MAttack{0}Ratio", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
				sbParam.AppendFormat( "{0:F1}", magicalAttack + ( magicalAttack * Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Value) * 0.001f));
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}
			
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "MAttack{0}Value", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
				sbParam.AppendFormat( "{0:F1}", magicalAttack + Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Value));
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}

			// Heal
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "Heal{0}Total", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
//				sbParam.AppendFormat( "{0:F1}", magicalAttack * Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Value) * 0.001f
//					+ Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_IntValue));
				sbParam.AppendFormat( "{0:F1}", magicalAttack * skillLevelRecord.listSkillLevelPotency[i].Potency_Value * 0.001f
					+ Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_IntValue));
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}
			
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "Heal{0}Ratio", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
				sbParam.AppendFormat( "{0:F1}", magicalAttack * Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Value) * 0.001f);
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}
		}
		
		return sb.ToString();
	}
	
	public static string ModifyDescriptionInTooltip( string desc, int id, int level, int curChargeStep)
	{
		Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record( id);
		Tbl_SkillLevel_Record skillLevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( level, id);
		if( int.MaxValue != skillLevelRecord.ChargeMaxStep)
			skillLevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( level, id, curChargeStep);
		Tbl_Action_Record actionRecord = AsTableManager.Instance.GetTbl_Action_Record( skillLevelRecord.SkillAction_Index);
		CharacterLoadData savedCharStat = AsUserInfo.Instance.SavedCharStat;
		
		StringBuilder sb = new StringBuilder( desc);
		StringBuilder sbParam = new StringBuilder();
		
		if( true == desc.Contains( "SkillActivityRatio"))
		{
			sbParam.Remove( 0, sbParam.Length);
			sbParam.AppendFormat( "{0:D1}%", skillRecord.SkillIcon_Enable_ConditionValue);
			sb.Replace( "SkillActivityRatio", sbParam.ToString());
		}
		
		if( true == desc.Contains( "AggroRatio"))
		{
			sbParam.Remove( 0, sbParam.Length);
			sbParam.AppendFormat( "{0:D1}%", skillLevelRecord.Aggro_Value);
			sb.Replace( "AggroRatio", sbParam.ToString());
		}
		
		if( true == desc.Contains( "AggroValue"))
			sb.Replace( "AggroValue", skillLevelRecord.Aggro_Value.ToString());
		
		if( true == desc.Contains( "Angle"))
		{
			sbParam.Remove( 0, sbParam.Length);
			sbParam.AppendFormat( "{0:F1}'", actionRecord.HitAnimation.hitInfo.HitAngle_);
			sb.Replace( "Angle", sbParam.ToString());
		}
		
		if( true == desc.Contains( "MinDistance"))
		{
			sbParam.Remove( 0, sbParam.Length);
			sbParam.AppendFormat( "{0:F1}m", actionRecord.HitAnimation.hitInfo.HitMinDistance_ * 0.01f);
			sb.Replace( "MinDistance", sbParam.ToString());
		}
		
		if( true == desc.Contains( "MaxDistance"))
		{
			sbParam.Remove( 0, sbParam.Length);
			sbParam.AppendFormat( "{0:F1}m", actionRecord.HitAnimation.hitInfo.HitMaxDistance_ * 0.01f);
			sb.Replace( "MaxDistance", sbParam.ToString());
		}
		
		if( true == desc.Contains( "OffsetX"))
		{
			sbParam.Remove( 0, sbParam.Length);
			sbParam.AppendFormat( "{0:F1}m", actionRecord.HitAnimation.hitInfo.HitOffsetX_);
			sb.Replace( "OffsetX", sbParam.ToString());
		}
		
		if( true == desc.Contains( "OffsetY"))
		{
			sbParam.Remove( 0, sbParam.Length);
			sbParam.AppendFormat( "{0:F1}m", actionRecord.HitAnimation.hitInfo.HitOffsetY_);
			sb.Replace( "OffsetY", sbParam.ToString());
		}
		
		if( true == desc.Contains( "SkillDistance"))
		{
			sbParam.Remove( 0, sbParam.Length);
			sbParam.AppendFormat( "{0:F1}m", skillLevelRecord.Usable_Distance * 0.01f);
			sb.Replace( "SkillDistance", sbParam.ToString());
		}
		
		StringBuilder sbFormat = new StringBuilder();
		for( int i = 0; i < 5; i++)
		{
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "Potency{0}ActivityRatio", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
				sbParam.AppendFormat( "{0:F1}%", Mathf.Abs( skillRecord.listSkillPotency[i].Potency_Enable_ConditionValue) * 0.1f);
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "Potency{0}ActivityProb", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
				sbParam.AppendFormat( "{0:F1}%", Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Prob));
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "Potency{0}Ratio", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
				sbParam.AppendFormat( "{0:F1}%", Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Value) * 0.1f);
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "Potency{0}Value", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
				sbParam.AppendFormat( "{0:D}", (int)Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_IntValue));
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "Potency{0}Duration", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				float duration = skillLevelRecord.listSkillLevelPotency[i].Potency_Duration;
				int hour = (int)( duration / 3600.0f);
				int min = (int)( ( duration % 3600.0f) / 60.0f);
				float sec = ( duration % 3600.0f) % 60.0f;
				
				sbParam.Remove( 0, sbParam.Length);
								
				if( 0 == hour && 0 == min )
				{
					//sec
					sbParam.AppendFormat( "{0:F1}{1}", sec, AsTableManager.Instance.GetTbl_String(90)); // sec
				}
				else if( 0 == hour )					
				{
					//min
					if( 0 == sec )
					{
						sbParam.AppendFormat( "{0:D}{1}", min, AsTableManager.Instance.GetTbl_String(89)); 	
					}
					else
					{
						sbParam.AppendFormat( "{0:D}{1} {2:F1}{3}", min, AsTableManager.Instance.GetTbl_String(89),
							sec, AsTableManager.Instance.GetTbl_String(90));
					}
				}
				else
				{
					// hour
					if( 0 == min && 0 == sec )
					{
						sbParam.AppendFormat( "{0:D}{1}", hour, AsTableManager.Instance.GetTbl_String(931)); 
					}
					else if( 0 == sec )
					{
						sbParam.AppendFormat( "{0:D}{1} {2:D}{3}", hour, AsTableManager.Instance.GetTbl_String(931),
							min, AsTableManager.Instance.GetTbl_String(89));
					}
					else if( 0 == min )
					{
						sbParam.AppendFormat( "{0:D}{1} {2:F1}{3}", hour, AsTableManager.Instance.GetTbl_String(931),
							sec, AsTableManager.Instance.GetTbl_String(90));
					}
					else
					{
						sbParam.AppendFormat( "{0:D}{1} {2:D}{3} {4:F1}{5}", hour, AsTableManager.Instance.GetTbl_String(931),
							min, AsTableManager.Instance.GetTbl_String(89), sec, AsTableManager.Instance.GetTbl_String(90));
					}
				}
				
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}
			
			// Physical Attack
			float physicalAttack = ( savedCharStat.sFinalStatus.nPhysicDmg_Min + savedCharStat.sFinalStatus.nPhysicDmg_Max) * 0.5f;
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "P{0}Ratio", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
				sbParam.AppendFormat( "{0:F1}", physicalAttack * Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Value) * 0.001f);
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}
			
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "PAttack{0}Total", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
//				sbParam.AppendFormat( "{0:F1}", physicalAttack + ( physicalAttack * Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Value) * 0.001f)
//					+ Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_IntValue));
				AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
				if( null != user)
				{
					eRACE race = user.GetProperty<eRACE>( eComponentProperty.RACE);
					eCLASS __class = user.GetProperty<eCLASS>( eComponentProperty.CLASS);
					Tbl_Class_Record record = AsTableManager.Instance.GetTbl_Class_Record( race, __class);
					
					sbParam.AppendFormat( "{0:F1}", ( ( physicalAttack * skillLevelRecord.listSkillLevelPotency[i].Potency_Value * 0.001f)
						+ Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_IntValue)) * ( record.BaseAttackCycle * 0.001f));
					sb.Replace( sbFormat.ToString(), sbParam.ToString());
				}
			}
			
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "PAttack{0}Ratio", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
				sbParam.AppendFormat( "{0:F1}", physicalAttack + ( physicalAttack * Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Value) * 0.001f));
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}

			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "PAttack{0}Value", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
				sbParam.AppendFormat( "{0:F1}", physicalAttack + Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Value));
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}
			
			// Magical Attack
			float magicalAttack = ( savedCharStat.sFinalStatus.nMagicDmg_Min + savedCharStat.sFinalStatus.nMagicDmg_Max) * 0.5f;
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "MAttack{0}Total", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
//				sbParam.AppendFormat( "{0:F1}", magicalAttack + ( magicalAttack * Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Value) * 0.001f)
//					+ Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_IntValue));
				AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
				if( null != user)
				{
					eRACE race = user.GetProperty<eRACE>( eComponentProperty.RACE);
					eCLASS __class = user.GetProperty<eCLASS>( eComponentProperty.CLASS);
					Tbl_Class_Record record = AsTableManager.Instance.GetTbl_Class_Record( race, __class);
					
					sbParam.AppendFormat( "{0:F1}", ( ( magicalAttack * skillLevelRecord.listSkillLevelPotency[i].Potency_Value * 0.001f)
						+ Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_IntValue)) * ( record.BaseAttackCycle * 0.001f));
					sb.Replace( sbFormat.ToString(), sbParam.ToString());
				}
			}
			
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "MAttack{0}Ratio", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
				sbParam.AppendFormat( "{0:F1}", magicalAttack + ( magicalAttack * Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Value) * 0.001f));
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}
			
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "MAttack{0}Value", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
				sbParam.AppendFormat( "{0:F1}", magicalAttack + Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Value));
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}

			// Heal
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "Heal{0}Total", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
//				sbParam.AppendFormat( "{0:F1}", magicalAttack * Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Value) * 0.001f
//					+ Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_IntValue));
				sbParam.AppendFormat( "{0:F1}", magicalAttack * skillLevelRecord.listSkillLevelPotency[i].Potency_Value * 0.001f
					+ Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_IntValue));
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}
			
			sbFormat.Remove( 0, sbFormat.Length);
			sbFormat.AppendFormat( "Heal{0}Ratio", ( i + 1));
			if( true == desc.Contains( sbFormat.ToString()))
			{
				sbParam.Remove( 0, sbParam.Length);
				sbParam.AppendFormat( "{0:F1}", magicalAttack * Mathf.Abs( skillLevelRecord.listSkillLevelPotency[i].Potency_Value) * 0.001f);
				sb.Replace( sbFormat.ToString(), sbParam.ToString());
			}
		}
		
		return sb.ToString();
	}
	
	static public bool CheckCharacterName( string strName)
	{
		if( string.Empty == strName || 0 == strName.Length)
			return false;

		char[] arrName = strName.ToCharArray();
		
		foreach( char c in arrName)
		{
			if( false == _isCheckCharacterName( c))
				return false;
		}
		
		//$yde - 20130108
		if( AsTableManager.Instance.TextFiltering_Name( strName) == false)
		{
			Debug.Log( "AsCharacterCreateFramework::_isCheckCharacterName: Invalid character name = " + strName);
			return false;
		}
		
		return true;
	}
	
	static private string m_strEngChar = "abcdefghijklmnopqrstuvwxyz";
	static private bool _isCheckCharacterName( char c)
	{
		/*
		if( c >= 0xAC00 && c <= 0xD7AF) // complete hangul
			return true;
		else if( c >= 0x3130 && c <= 0x318F) // composition hangul
			return false;
		else if( true == System.Text.RegularExpressions.Regex.IsMatch( c.ToString(), "^\\d+$")) // digit
			return true;
		else if( true == m_strEngChar.Contains( c.ToString().ToLower())) // english
			return true;

		return false;
		*/

		return CheckCharFromLanguageType( c);
	}
	
	static public bool CheckCharFromLanguageType( char c)
	{
		if( AsLanguageManager.LanguageType.LanguageType_Korea == AsLanguageManager.Instance.NowLanguageType)
		{
			if( c >= 0xAC00 && c <= 0xD7AF) // complete hangul
				return true;
			else if( true == System.Text.RegularExpressions.Regex.IsMatch( c.ToString(), "^\\d+$")) // digit
				return true;
			else if( true == m_strEngChar.Contains( c.ToString().ToLower())) // english
				return true;

			return false;
		}
		
		if( AsLanguageManager.LanguageType.LanguageType_Japan == AsLanguageManager.Instance.NowLanguageType)
		{
			if( true == m_strEngChar.Contains( c.ToString().ToLower())) // english
				return true;
			else if( true == System.Text.RegularExpressions.Regex.IsMatch( c.ToString(), "^\\d+$")) // digit
				return true;
			
			else if( c >= 0x3040 && c <= 0x309F) // Hiragana
				return true;
			else if( c >= 0x30A0 && c <= 0x30FF) // Katakana
				return true;
			
			else if( c >= 0x31F0 && c <= 0x31FF) // Katakana Phonetic Extensions
				return true;
			else if( c >= 0x2E80 && c <= 0x2EFF) // CJK Radicals Supplement
				return true;
			else if( c >= 0x4E00 && c <= 0x9FBF) // CJK Unified Ideographs
				return true;
			else if( c >= 0xF900 && c <= 0xFAFF) // CJK Compatibility Ideographs
				return true;
			else if( c >= 0x3400 && c <= 0x4DBF) // CJK Unified Ideographs Extension A
				return true;
			else if( c >= 0x20000 && c <= 0x2A6DF) // CJK Unified Ideographs Extension B
				return true;
			else if( c >= 0x2A700 && c <= 0x2B73F) // CJK Unified Ideographs Extension C
				return true;
			else if( c >= 0x2F800 && c <= 0x2FA1F) // CJK Compatibility Ideographs Supplement
				return true;

			else if( c >= 0xFF61 && c <= 0xFF9F )	// bankak
				return true;
			else if( c >= 0xFF10 && c <= 0xFF19 )	// junkak 0~9
				return true;
			else if( c >= 0xFF21 && c <= 0xFF3A )	// Junkak A~Z
				return true;
			else if( c >= 0xFF41 && c <= 0xFF5A )	// Junkak a~z
				return true;

			return false;
		}
		
		return false;
	}
	
	static public void SetRenderingState( GameObject obj, bool show)
	{
		Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
		foreach( Renderer ren in renderers)
			ren.enabled = show;
	}
	
	
	
	static public eMAP_TYPE GetMapType()
	{
		Map map = TerrainMgr.Instance.GetCurrentMap();
		if( null == map)
			return eMAP_TYPE.Invalid;
		
		return map.MapData.getMapType;
	}

	static public string GetClassName( eCLASS _type)
	{
		switch( _type)
		{
		case eCLASS.DIVINEKNIGHT:
			return AsTableManager.Instance.GetTbl_String(306);
		case eCLASS.MAGICIAN:
			return AsTableManager.Instance.GetTbl_String(307);
		case eCLASS.CLERIC:
			return AsTableManager.Instance.GetTbl_String(308);
		case eCLASS.HUNTER:
			return AsTableManager.Instance.GetTbl_String(309);
		default:
			return AsTableManager.Instance.GetTbl_String(0);
		}
	}
	
	static public void SetButtonState( UIButton btn , UIButton.CONTROL_STATE state )
	{
		switch( state )
		{
		case UIButton.CONTROL_STATE.NORMAL:
			btn.SetControlState( state );
			btn.controlIsEnabled = true;
			if( btn.spriteText != null )
				btn.spriteText.Color = Color.black;
			break;
			
		case UIButton.CONTROL_STATE.DISABLED:
			btn.SetControlState( state );
			btn.controlIsEnabled = false;
			if( btn.spriteText != null )
				btn.spriteText.Color = Color.gray;
			break;
		}
	}

	static public bool IsScreenPadRatio()
	{
		float width = Screen.width;
		float height = Screen.height;
		
		float fRatio = width / height;
		
		if (fRatio < 1.5f)
			return true;

		return false;
	}

	static public string strSaveDataPath
	{
		get
		{
			/*
#if UNITY_IPHONE
			return Application.temporaryCachePath;
#else
			return Application.persistentDataPath;
#endif
			*/
			//return Application.streamingAssetsPath;
			return Application.temporaryCachePath;
		}
	}
}




