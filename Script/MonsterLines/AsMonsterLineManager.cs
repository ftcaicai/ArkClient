using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;

public enum eMonsterLineType : int
{
	Invalid = -1,
	
	Add,
	HpProb,
	Death,
	Skill,
	
	Max
};

public enum eConditionValueCheck : int
{
	Invalid = -1,
	
	None,
	Above,
	Equal,
	Below,
	Range,
	
	Max
};

public class MonsterLineData : AsTableRecord
{
	public int groupIndex = -1;
	public eMonsterLineType eType = eMonsterLineType.Invalid;
	public int conditionValue;
	public int probability;
	public int lineIndex1;
	public int lineIndex2;
	public int lineIndex3;
	
	public MonsterLineData( XmlElement _element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;
			
			SetValue( ref groupIndex, node, "Index");
			SetValue( ref eType, node, "BallonCondition");
			SetValue( ref conditionValue, node, "BallonValue");
			SetValue( ref probability, node, "BallonProb");
			SetValue( ref lineIndex1, node, "BallonString1");
			SetValue( ref lineIndex2, node, "BallonString2");
			SetValue( ref lineIndex3, node, "BallonString3");
			
			if( int.MaxValue == lineIndex2)
				lineIndex2 = lineIndex1;
			
			if( int.MaxValue == lineIndex3)
				lineIndex3 = lineIndex1;
		}
		catch( System.Exception e)
		{
			Debug.LogError(e);
		}
	}
}

public class AsMonsterLineManager
{
	private Dictionary<int,List<MonsterLineData>> dicLineData = new Dictionary<int,List<MonsterLineData>>();
	
	static private AsMonsterLineManager instance = null;
	static public AsMonsterLineManager Instance
	{
		get
		{
			if( null == instance)
				instance = new AsMonsterLineManager();
			
			return instance;
		}
	}
	
	public string GetLine( int monsterTableIdx, eMonsterLineType type, eConditionValueCheck check=eConditionValueCheck.None, int conditionValue1=-1, int conditionValue2=-1)
	{
		Tbl_Npc_Record npcRecord = AsTableManager.Instance.GetTbl_Npc_Record( monsterTableIdx);
		if( null == npcRecord)
			return null;
		
		int groupIndex = npcRecord.LineIndex;
		
		if( false == dicLineData.ContainsKey( groupIndex))
			return null;
		
		List<MonsterLineData> lstLine = dicLineData[ groupIndex];
		foreach( MonsterLineData line in lstLine)
		{
			if( type == line.eType)
			{
				int probability = Random.Range( 0, 100);
				if( probability > line.probability)
					return null;
				
				switch( check)
				{
				case eConditionValueCheck.None:
					break;
				case eConditionValueCheck.Above:
					if( line.conditionValue > conditionValue1)
						return null;
					break;
				case eConditionValueCheck.Equal:
					if( line.conditionValue != conditionValue1)
						return null;
					break;
				case eConditionValueCheck.Below:
					if( line.conditionValue < conditionValue1)
						return null;
					break;
				case eConditionValueCheck.Range:
					{
						if( 0 == conditionValue1)
							return null;
					
						float ratio = ( (float)conditionValue1 / (float)conditionValue2) * 100.0f;
						if( ( ratio < ( line.conditionValue - 10)) || ( ratio > ( line.conditionValue + 10)))
							return null;
					}
					break;
				}
				
				int strVar = Random.Range( 0, 3);
				string ret = string.Empty;
				switch( strVar)
				{
				case 0:	ret = AsTableManager.Instance.GetTbl_String( line.lineIndex1);	break;
				case 1:	ret = AsTableManager.Instance.GetTbl_String( line.lineIndex2);	break;
				case 2:	ret = AsTableManager.Instance.GetTbl_String( line.lineIndex3);	break;
				}
				
				StringBuilder sb = new StringBuilder( ret);
				sb.Replace( "'USER'", AsUserInfo.Instance.SavedCharStat.charName_);
				
				return sb.ToString();
			}
		}
		
		return null;
	}
	
	public void LoadTable()
	{
		try
		{
			XmlElement root = AsTableBase.GetXmlRootElement( "Table/BalloonTable");
			XmlNodeList nodes = root.ChildNodes;

			foreach( XmlNode node in nodes)
			{
				MonsterLineData lineData = new MonsterLineData( (XmlElement)node);

				if( true == dicLineData.ContainsKey( lineData.groupIndex))
				{
					List<MonsterLineData> lstLine = dicLineData[ lineData.groupIndex];
					lstLine.Add( lineData);
				}
				else
				{
					List<MonsterLineData> lstLine = new List<MonsterLineData>();
					lstLine.Add( lineData);
					dicLineData.Add( lineData.groupIndex, lstLine);
				}
			}
		}
		catch( System.Exception e)
		{
			Debug.LogError(e);
			AsUtil.ShutDown( "AsMonsterLineManager:LoadTable");
		}
	}
}
