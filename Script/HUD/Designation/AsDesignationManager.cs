using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public enum eDesignationType
{
	Invalid = 0,
	
	Normal,
	Unique,
	
	Max
};

public enum eDesignationCategory
{
	Invalid = 0,
	
	Character,
	Monster,
	Area,
	Item,
	Quest,
	Friends,
	Unique,
	Etc,
	Blind,
	
	Max
};

public enum eDesignationSortState
{
	Invalid = 0,
	
	Name_Ascending,
	Name_Descending,
	Rank_Ascending,
	Rank_Descending,
	
	Max
};

public class DesignationData : AsTableRecord
{
	public int id = -1;
	public eDesignationType eType = eDesignationType.Invalid;
	public eDesignationCategory eCategory = eDesignationCategory.Invalid;
	public int name = -1;
	public string nameColor = string.Empty;
	public int desc = -1;
	public int effectDesc = -1;
	public int notice = -1;
	public int rankPoint = 0;

	public int DivineKnight_Item_ID = -1;
	public int DivineKnight_Item_Count = 0;
	public int Magician_Item_ID = -1;
	public int Magician_Item_Count = 0;
	public int Cleric_Item_ID = -1;
	public int Cleric_Item_Count = 0;
	public int Hunter_Item_ID = -1;
	public int Hunter_Item_Count = 0;

	public bool isRewardReceived = false;

	public DesignationData( XmlNode node)
	{
		try
		{
			SetValue( ref id, node, "SubTitle_ID");
			SetValue( ref eType, node, "SubTitle_Type");
			SetValue( ref eCategory, node, "SubTitle_Category");
			SetValue( ref name, node, "SubTitle_Name");
			SetValue( ref nameColor, node, "SubTitle_Color");
			SetValue( ref desc, node, "SubTitle_Description1");
			SetValue( ref effectDesc, node, "SubTitle_Description2");
			SetValue( ref notice, node, "SubTitle_Notice");
			SetValue( ref rankPoint, node, "SubTitle_RankPoint");

			SetValue( ref DivineKnight_Item_ID, node, "DivineKnight_Item_ID");
			SetValue( ref DivineKnight_Item_Count, node, "DivineKnight_Item_Count");
			SetValue( ref Magician_Item_ID, node, "Magician_Item_ID");
			SetValue( ref Magician_Item_Count, node, "Magician_Item_Count");
			SetValue( ref Cleric_Item_ID, node, "Cleric_Item_ID");
			SetValue( ref Cleric_Item_Count, node, "Cleric_Item_Count");
			SetValue( ref Hunter_Item_ID, node, "Hunter_Item_ID");
			SetValue( ref Hunter_Item_Count, node, "Hunter_Item_Count");
		}
		catch( System.Exception e)
		{
			Debug.LogError(e);
		}
	}
}

public class AsDesignationManager
{
	private Dictionary<int,DesignationData> allDesignations = new Dictionary<int,DesignationData>();

	private bool isSendRequestReward = false;
	public bool SendRequestReward
	{
		get{return isSendRequestReward;}
		set{isSendRequestReward = value;}
	}

	private List<int> characterDesignations = new List<int>();
	private List<int> monsterDesignations = new List<int>();
	private List<int> areaDesignations = new List<int>();
	private List<int> itemDesignations = new List<int>();
	private List<int> questDesignations = new List<int>();
	private List<int> friendsDesignations = new List<int>();
	private List<int> uniqueDesignations = new List<int>();
	private List<int> etcDesignations = new List<int>();
	private List<int> blindDesignations = new List<int>();
	private List<int> obtainedDesignation = new List<int>();
	private int currentID = -1;
	private List<int> alarmList = new List<int>();
	private int obtainedCharacterDesignationCount = 0;
	private int obtainedMonsterDesignationCount = 0;
	private int obtainedAreaDesignationCount = 0;
	private int obtainedItemDesignationCount = 0;
	private int obtainedQuestDesignationCount = 0;
	private int obtainedFriendsDesignationCount = 0;
	private int obtainedUniqueDesignationCount = 0;
	private int obtainedEtcDesignationCount = 0;
	private int totalObtainedDesignationRankPoint = 0;
	public int TotalObtainedDesignationRankPoint
	{
		get	{ return totalObtainedDesignationRankPoint; }
	}
	private eDesignationSortState[] eSortState = new eDesignationSortState[ (int)eDesignationCategory.Max]
	{
		eDesignationSortState.Invalid,
		eDesignationSortState.Invalid,
		eDesignationSortState.Invalid,
		eDesignationSortState.Invalid,
		eDesignationSortState.Invalid,
		eDesignationSortState.Invalid,
		eDesignationSortState.Invalid,
		eDesignationSortState.Invalid,
		eDesignationSortState.Invalid,
		eDesignationSortState.Invalid,
	};
	
	private static AsDesignationManager instance = null;
	public static AsDesignationManager Instance
	{
		get
		{
			if( null == instance)
				instance = new AsDesignationManager();
			
			return instance;
		}
	}
	
	private AsDesignationManager()
	{
	}
	
	public eDesignationSortState GetSortState( eDesignationCategory category)
	{
		return eSortState[ (int)category];
	}
	
	public void SetSortState( eDesignationCategory category, eDesignationSortState state)
	{
		eSortState[ (int)category] = state;
	}
	
	private static int _ascendingSortByName( int id1, int id2)
	{
		DesignationData data1 = AsDesignationManager.Instance.GetDesignation( id1);
		DesignationData data2 = AsDesignationManager.Instance.GetDesignation( id2);
		string str1 = AsTableManager.Instance.GetTbl_String( data1.name);
		string str2 = AsTableManager.Instance.GetTbl_String( data2.name);
		
		return str1.CompareTo( str2);
	}
	
	private static int _descendingSortByName( int id1, int id2)
	{
		DesignationData data1 = AsDesignationManager.Instance.GetDesignation( id1);
		DesignationData data2 = AsDesignationManager.Instance.GetDesignation( id2);
		string str1 = AsTableManager.Instance.GetTbl_String( data1.name);
		string str2 = AsTableManager.Instance.GetTbl_String( data2.name);
		
		return str2.CompareTo( str1);
	}
	
	private static int _ascendingSortByRank( int id1, int id2)
	{
		DesignationData data1 = AsDesignationManager.Instance.GetDesignation( id1);
		DesignationData data2 = AsDesignationManager.Instance.GetDesignation( id2);
		
		return data1.rankPoint.CompareTo( data2.rankPoint);
	}
	
	private static int _descendingSortByRank( int id1, int id2)
	{
		DesignationData data1 = AsDesignationManager.Instance.GetDesignation( id1);
		DesignationData data2 = AsDesignationManager.Instance.GetDesignation( id2);
		
		return data2.rankPoint.CompareTo( data1.rankPoint);
	}
	
	public void AscendingSortByName( eDesignationCategory category)
	{
		eSortState[ (int)category] = eDesignationSortState.Name_Ascending;
		
		switch( category)
		{
		case eDesignationCategory.Invalid:	obtainedDesignation.Sort( _ascendingSortByName);	break;
		case eDesignationCategory.Character:	characterDesignations.Sort( _ascendingSortByName);	break;
		case eDesignationCategory.Monster:	monsterDesignations.Sort( _ascendingSortByName);	break;
		case eDesignationCategory.Area:	areaDesignations.Sort( _ascendingSortByName);	break;
		case eDesignationCategory.Item:	itemDesignations.Sort( _ascendingSortByName);	break;
		case eDesignationCategory.Quest:	questDesignations.Sort( _ascendingSortByName);	break;
		case eDesignationCategory.Friends:	friendsDesignations.Sort( _ascendingSortByName);	break;
		case eDesignationCategory.Unique:	uniqueDesignations.Sort( _ascendingSortByName);	break;
		case eDesignationCategory.Etc:	etcDesignations.Sort( _ascendingSortByName);	break;
		case eDesignationCategory.Blind:	break;
		default:
			Debug.LogError( "AsDesignationManager:AscendingSortByName -> Invalid category");
			break;
		}
	}
	
	public void DescendingSortByName( eDesignationCategory category)
	{
		eSortState[ (int)category] = eDesignationSortState.Name_Descending;
		
		switch( category)
		{
		case eDesignationCategory.Invalid:	obtainedDesignation.Sort( _descendingSortByName);	break;
		case eDesignationCategory.Character:	characterDesignations.Sort( _descendingSortByName);	break;
		case eDesignationCategory.Monster:	monsterDesignations.Sort( _descendingSortByName);	break;
		case eDesignationCategory.Area:	areaDesignations.Sort( _descendingSortByName);	break;
		case eDesignationCategory.Item:	itemDesignations.Sort( _descendingSortByName);	break;
		case eDesignationCategory.Quest:	questDesignations.Sort( _descendingSortByName);	break;
		case eDesignationCategory.Friends:	friendsDesignations.Sort( _descendingSortByName);	break;
		case eDesignationCategory.Unique:	uniqueDesignations.Sort( _descendingSortByName);	break;
		case eDesignationCategory.Etc:	etcDesignations.Sort( _descendingSortByName);	break;
		case eDesignationCategory.Blind:	break;
		default:
			Debug.LogError( "AsDesignationManager:DescendingSortByName -> Invalid category");
			break;
		}
	}
	
	public void AscendingSortByRank( eDesignationCategory category)
	{
		eSortState[ (int)category] = eDesignationSortState.Rank_Ascending;
		
		switch( category)
		{
		case eDesignationCategory.Invalid:	obtainedDesignation.Sort( _ascendingSortByRank);	break;
		case eDesignationCategory.Character:	characterDesignations.Sort( _ascendingSortByRank);	break;
		case eDesignationCategory.Monster:	monsterDesignations.Sort( _ascendingSortByRank);	break;
		case eDesignationCategory.Area:	areaDesignations.Sort( _ascendingSortByRank);	break;
		case eDesignationCategory.Item:	itemDesignations.Sort( _ascendingSortByRank);	break;
		case eDesignationCategory.Quest:	questDesignations.Sort( _ascendingSortByRank);	break;
		case eDesignationCategory.Friends:	friendsDesignations.Sort( _ascendingSortByRank);	break;
		case eDesignationCategory.Unique:	uniqueDesignations.Sort( _ascendingSortByRank);	break;
		case eDesignationCategory.Etc:	etcDesignations.Sort( _ascendingSortByRank);	break;
		case eDesignationCategory.Blind:	break;
		default:
			Debug.LogError( "AsDesignationManager:AscendingSortByRank -> Invalid category");
			break;
		}
	}
	
	public void DescendingSortByRank( eDesignationCategory category)
	{
		eSortState[ (int)category] = eDesignationSortState.Rank_Descending;
		
		switch( category)
		{
		case eDesignationCategory.Invalid:	obtainedDesignation.Sort( _descendingSortByRank);	break;
		case eDesignationCategory.Character:	characterDesignations.Sort( _descendingSortByRank);	break;
		case eDesignationCategory.Monster:	monsterDesignations.Sort( _descendingSortByRank);	break;
		case eDesignationCategory.Area:	areaDesignations.Sort( _descendingSortByRank);	break;
		case eDesignationCategory.Item:	itemDesignations.Sort( _descendingSortByRank);	break;
		case eDesignationCategory.Quest:	questDesignations.Sort( _descendingSortByRank);	break;
		case eDesignationCategory.Friends:	friendsDesignations.Sort( _descendingSortByRank);	break;
		case eDesignationCategory.Unique:	uniqueDesignations.Sort( _descendingSortByRank);	break;
		case eDesignationCategory.Etc:	etcDesignations.Sort( _descendingSortByRank);	break;
		case eDesignationCategory.Blind:	break;
		default:
			Debug.LogError( "AsDesignationManager:DescendingSortByRank -> Invalid category");
			break;
		}
	}
	
	public int CurrentID
	{
		get	{ return currentID; }
		set	{ currentID = value; }
	}
	
	public int TotalCount
	{
		get	{ return allDesignations.Count; }
	}
	
	public int ObtainedCount
	{
		get	{ return obtainedDesignation.Count; }
	}
	
	public int ObtainedCharacterDesignationCount
	{
		get	{ return obtainedCharacterDesignationCount; }
	}
	
	public int ObtainedMonsterDesignationCount
	{
		get	{ return obtainedMonsterDesignationCount; }
	}
	
	public int ObtainedAreaDesignationCount
	{
		get	{ return obtainedAreaDesignationCount; }
	}
	
	public int ObtainedItemDesignationCount
	{
		get	{ return obtainedItemDesignationCount; }
	}
	
	public int ObtainedQuestDesignationCount
	{
		get	{ return obtainedQuestDesignationCount; }
	}
	
	public int ObtainedFriendsDesignationCount
	{
		get	{ return obtainedFriendsDesignationCount; }
	}
	
	public int ObtainedUniqueDesignationCount
	{
		get	{ return obtainedUniqueDesignationCount; }
	}
	
	public int ObtainedEtcDesignationCount
	{
		get	{ return obtainedEtcDesignationCount; }
	}
	
	public int CountByCharacter
	{
		get	{ return characterDesignations.Count; }
	}
	
	public int CountByMonster
	{
		get	{ return monsterDesignations.Count; }
	}

	public int CountByArea
	{
		get	{ return areaDesignations.Count; }
	}
	
	public int CountByItem
	{
		get	{ return itemDesignations.Count; }
	}
	
	public int CountByQuest
	{
		get	{ return questDesignations.Count; }
	}
	
	public int CountByFriends
	{
		get	{ return friendsDesignations.Count; }
	}
	
	public int CountByUnique
	{
		get	{ return uniqueDesignations.Count; }
	}
	
	public int CountByEtc
	{
		get	{ return etcDesignations.Count; }
	}
	
	public int CountByBlind
	{
		get	{ return blindDesignations.Count; }
	}
	
	public int AlarmCount
	{
		get	{ return alarmList.Count; }
	}
	
	public void Clear()
	{
		obtainedCharacterDesignationCount = 0;
		obtainedMonsterDesignationCount = 0;
		obtainedAreaDesignationCount = 0;
		obtainedItemDesignationCount = 0;
		obtainedQuestDesignationCount = 0;
		obtainedFriendsDesignationCount = 0;
		obtainedUniqueDesignationCount = 0;
		obtainedEtcDesignationCount = 0;
		totalObtainedDesignationRankPoint = 0;
		obtainedDesignation.Clear();
	}
	
	public int GetObtainedDesignation( int index)
	{
		if( obtainedDesignation.Count <= index)
			return -1;
		
		return obtainedDesignation[ index];
	}

    public int GetDesignationIdx( int id)
    {
        return obtainedDesignation.IndexOf( id);
    }
	
	public bool IsObtainedDesignation( int id)
	{
		return obtainedDesignation.Contains( id);
	}

    public int GetObtainedDesignationCount()
    {
        return obtainedDesignation.Count;
    }
	
	public int GetCharacterDesignation( int index)
	{
		if( characterDesignations.Count <= index)
			return -1;
		
		return characterDesignations[ index];
	}
	
	public int GetMonsterDesignation( int index)
	{
		if( monsterDesignations.Count <= index)
			return -1;
		
		return monsterDesignations[ index];
	}
	
	public int GetAreaDesignation( int index)
	{
		if( areaDesignations.Count <= index)
			return -1;
		
		return areaDesignations[ index];
	}
	
	public int GetItemDesignation( int index)
	{
		if( itemDesignations.Count <= index)
			return -1;
		
		return itemDesignations[ index];
	}
	
	public int GetQuestDesignation( int index)
	{
		if( questDesignations.Count <= index)
			return -1;
		
		return questDesignations[ index];
	}
	
	public int GetFriendsDesignation( int index)
	{
		if( friendsDesignations.Count <= index)
			return -1;
		
		return friendsDesignations[ index];
	}
	
	public int GetUniqueDesignation( int index)
	{
		if( uniqueDesignations.Count <= index)
			return -1;
		
		return uniqueDesignations[ index];
	}
	
	public int GetEtcDesignation( int index)
	{
		if( etcDesignations.Count <= index)
			return -1;
		
		return etcDesignations[ index];
	}
	
	public int GetBlindDesignation( int index)
	{
		if( blindDesignations.Count <= index)
			return -1;
		
		return blindDesignations[ index];
	}
	
	public DesignationData GetCurrentDesignation()
	{
		if( true == allDesignations.ContainsKey( currentID))
			return allDesignations[ currentID];
		
		return null;
	}
	
	public DesignationData GetDesignation( int id)
	{
		if( true == allDesignations.ContainsKey( id))
			return allDesignations[ id];
		
		return null;
	}
	
	public void ObtainDesignation( int id)
	{
		if( true == obtainedDesignation.Contains( id))
			return;
		
		DesignationData data = GetDesignation( id);
		Debug.Assert( null != data);
		
		switch( data.eCategory)
		{
		case eDesignationCategory.Invalid:	break;
		case eDesignationCategory.Character:	obtainedCharacterDesignationCount++;	break;
		case eDesignationCategory.Monster:	obtainedMonsterDesignationCount++;	break;
		case eDesignationCategory.Area:	obtainedAreaDesignationCount++;	break;
		case eDesignationCategory.Item:	obtainedItemDesignationCount++;	break;
		case eDesignationCategory.Quest:	obtainedQuestDesignationCount++;	break;
		case eDesignationCategory.Friends:	obtainedFriendsDesignationCount++;	break;
		case eDesignationCategory.Unique:	obtainedUniqueDesignationCount++;	break;
		case eDesignationCategory.Etc:	obtainedEtcDesignationCount++;	break;
		case eDesignationCategory.Blind:	break;
		default:
			Debug.LogError( "AsDesignationManager:ObtainDesignation -> Invalid category");
			break;
		}
		
		obtainedDesignation.Add( id);
		
		totalObtainedDesignationRankPoint += data.rankPoint;
	}

	public void SetDesignationRewardReceive( int id , bool _isReward )
	{
		DesignationData data = GetDesignation( id);
		if (data != null) 
		{
			data.isRewardReceived = _isReward;
		}
	}

	public void ResetDesignationRewardReceiveFlag()
	{
		foreach (KeyValuePair<int,DesignationData> pair in allDesignations) 
		{
			pair.Value.isRewardReceived = false;
		}
	}
	
	public void InsertAlarm( int id)
	{
		if( true == alarmList.Contains( id))
			return;
		
		alarmList.Add( id);
	}
	
	public void RemoveAlarm( int index)
	{
		if( index >= alarmList.Count)
			return;
		
		alarmList.RemoveAt( index);
	}
	
	public int GetAlarm( int index)
	{
		if( index >= alarmList.Count)
			return -1;
		
		return alarmList[ index];
	}
	
	public void LoadTable()
	{
		try
		{
			string strTableName = "Table/SubTitleTable";
			
			GameObject main = GameObject.Find( "GameMain");
			if( main != null )
			{
				AsGameMain asMain = main.GetComponent<AsGameMain>();
			
				GAME_LANGUAGE gameLanguage = asMain.GetGameLanguage();
				switch( gameLanguage)
				{
				case GAME_LANGUAGE.LANGUAGE_KOR:
					strTableName = "Table/SubTitleTable";
					break;
					
				case GAME_LANGUAGE.LANGUAGE_JAP:
					strTableName = "Table/SubTitleTable_jap";
					break;
				}
			}
			
		    XmlElement root = AsTableBase.GetXmlRootElement( strTableName );
		    XmlNodeList nodes = root.ChildNodes;

			foreach( XmlNode node in nodes)
		    {
				DesignationData designation = new DesignationData( node);
				allDesignations.Add( designation.id, designation);
				
				switch( designation.eCategory)
				{
				case eDesignationCategory.Character:	characterDesignations.Add( designation.id);	break;
				case eDesignationCategory.Monster:	monsterDesignations.Add( designation.id);	break;
				case eDesignationCategory.Area:	areaDesignations.Add( designation.id);	break;
				case eDesignationCategory.Item:	itemDesignations.Add( designation.id);	break;
				case eDesignationCategory.Quest:	questDesignations.Add( designation.id);	break;
				case eDesignationCategory.Friends:	friendsDesignations.Add( designation.id);	break;
				case eDesignationCategory.Unique:	uniqueDesignations.Add( designation.id);	break;
				case eDesignationCategory.Etc:	etcDesignations.Add( designation.id);	break;
				case eDesignationCategory.Blind:	blindDesignations.Add( designation.id);	break;
				default:
					Debug.LogError( "Invalid designation category : " + designation.eCategory);
					break;
				}
		    }
		}
		catch( System.Exception e)
		{
			Debug.LogError(e);
			AsUtil.ShutDown( "AsDesignationManager:LoadTable");
		}
	}
}
