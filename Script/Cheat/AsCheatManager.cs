
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;


public delegate void CheatResultTextErrDelegate( AsCheatManager.eCheatErrType cheatErrType);
public delegate void CheatResultTextDelegate( AsCheatManager.eCheatType cheatType, string strFront="", string strTail="");

public class AsCheatManager
{
	public enum eCheatType
	{
		CT_UIDel = 0,
		CT_Level, // character
		CT_Die,
		CT_Tel,
		CT_Recall,
		CT_Go,
		CT_Strong,
		CT_Hide,
		CT_Cool,
		CT_Speed,
		CT_HP,
		CT_MP,
		CT_AttSpeed,
		CT_Accuracy,
		CT_Condition,
		CT_InventoryExtendMax, // item
		CT_ItemMakeDropGroup,
		CT_ItemMake,
		CT_ItemUp,
		CT_Gold,
		CT_Miracle, // cash
		CT_ItemDel,
		CT_Inchent,
		CT_DropRate,
		CT_MobDel, // monster
		CT_MobSpawn,
		CT_MobStop,
		CT_QAccept, // quest
		CT_QDel,
		CT_QReset,
		CT_QClear,
		CT_QComplete,
		CT_QSet,
		CT_CollectMake,
		CT_PrivateShop, // shop
		CT_SocialPoint, // social
		CT_SkillLearn,
		CT_WayPoint,
		CT_Rank,
		CT_EquipItemUpgrade,
		CT_PvPPointModify,
		CT_UIOpen,
		CT_PetLevel,
		CT_PetHungry,
		CT_AttendanceBonus,
        CT_ApPoint,
        CT_ApSort,
		CT_Test, // etc
		
		CT_Max
	};
	
	public enum eCheatErrType
	{
		CE_EmptyCommand = 0,
		CE_NotFoundCommand,
		CE_LevelMinErr,
		CE_LevelMaxErr,
		CE_HPErr,
		CE_MPErr,
		CE_AttSpeedErr,
		
		CE_Max
	};
	
	public enum eCheatHelpType
	{
		CH_Character = 0,
		CH_Item,
		CH_Monster,
		CH_Quest,
		
		CH_Max
	}
	
	static AsCheatManager m_instance;
	public static AsCheatManager Instance
	{
		get
		{
			if( null == m_instance)
				m_instance = new AsCheatManager();
			
			return m_instance;
		}
	}
	
	private CheatResultTextErrDelegate cheatResultTextErrDelegate = null;
	public CheatResultTextErrDelegate SetCheatResultTextErrDelegate	{ set { cheatResultTextErrDelegate = value; } }
	private CheatResultTextDelegate cheatResultTextDelegate = null;
	public CheatResultTextDelegate SetCheatResultTextDelegate	{ set { cheatResultTextDelegate = value; } }
	
	#region cheat table
	public Dictionary<int, string> m_CheatTableData = new Dictionary<int, string>();

	public void LoadCheatTable()
	{
		if( m_CheatTableData.Count > 0)
			return;
		
		XmlElement root = AsTableBase.GetXmlRootElement( "Table/CheatTable");
		XmlNodeList nodes = root.ChildNodes;
		
		foreach( XmlNode node in nodes)
		{
			int nIndex = int.Parse( node["Index"].InnerText);
			string str = node["String"].InnerText;
			
			m_CheatTableData.Add( nIndex, str);
		}
	}
	
	public bool GetCheatString(int nKey, out string str)
	{
		return m_CheatTableData.TryGetValue( nKey, out str);
	}
	
	public int GetCheatTableDataCount()
	{
		return m_CheatTableData.Count;
	}
	#endregion cheat table

	#region cheat command
	public void Cheat_UIDel()
	{
		if( true == AsHUDController.Instance.gameObject.active)
		{
			AsHUDController.SetActiveRecursively( false);
			cheatResultTextDelegate( eCheatType.CT_UIDel, "", " : On");
		}
		else
		{
			AsHUDController.Instance.Init();
			AsHudDlgMgr.Instance.Init();
			PlayerBuffMgr.Instance.SetShowUI();

			cheatResultTextDelegate( eCheatType.CT_UIDel, "", " : Off");
		}
	}

	public void Cheat_Level(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nLevel = Convert.ToInt32( strCommandEdit[1]);
				
				if( nLevel <= 0)
					cheatResultTextErrDelegate( eCheatErrType.CE_LevelMinErr);
				else
				{
					_Send_CharacterLevel( nLevel);
					cheatResultTextDelegate( eCheatType.CT_Level, "", " : Lv." + strCommandEdit[1]);
				}
				
				return;
			}
		}

		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}
	
	public void Cheat_Die()
	{
		_Send_CharacterDie();
		cheatResultTextDelegate( eCheatType.CT_Die);
	}

	public void Cheat_Tel(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nWarpID = Convert.ToInt32( strCommandEdit[1]);
				
				_Send_Teleport( nWarpID);
				cheatResultTextDelegate( eCheatType.CT_Tel, "", " : WarpID: " + strCommandEdit[1]);
				
				return;
			}
		}

		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_Recall(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			_Send_Recall( strCommandEdit[1]);
			cheatResultTextDelegate( eCheatType.CT_Recall, strCommandEdit[1], "");
			return;
		}

		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_Go(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			_Send_Go( strCommandEdit[1]);
			cheatResultTextDelegate( eCheatType.CT_Go, strCommandEdit[1], "");
			return;
		}

		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_Strong(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nStrong = Convert.ToInt32( strCommandEdit[1]);
				
				_Send_Strong( nStrong);
				
				if( 0 == nStrong)
					cheatResultTextDelegate( eCheatType.CT_Strong, "", " : Off");
				else
					cheatResultTextDelegate( eCheatType.CT_Strong, "", " : On");
				
				return;
			}
		}

		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_Hide(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nHide = Convert.ToInt32( strCommandEdit[1]);
				
				_Send_Hide( nHide);
				
				if( 0 == nHide)
					cheatResultTextDelegate( eCheatType.CT_Hide, "", " : Off");
				else
					cheatResultTextDelegate( eCheatType.CT_Hide, "", " : On");
				
				return;
			}
		}

		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_Cool(string[] strCommandEdit)
	{
		AsPlayerFsm playerFsm = AsEntityManager.Instance.GetPlayerCharFsm();
		if( null == playerFsm)
		{
			cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
			return;
		}
		
		playerFsm.bUseCoolTime = !playerFsm.bUseCoolTime;

		if( true == playerFsm.bUseCoolTime)
			cheatResultTextDelegate( eCheatType.CT_Cool, "", " : On");
		else
			cheatResultTextDelegate( eCheatType.CT_Cool, "", " : Off");
		
		_Send_Cool( playerFsm.bUseCoolTime);
	}

	public void Cheat_Speed(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nSpeed = Convert.ToInt32( strCommandEdit[1]);
				
				_Send_MoveSpeed( nSpeed);
				cheatResultTextDelegate( eCheatType.CT_Speed, "", " : " + strCommandEdit[1] + " cm/s");
				
				return;
			}
		}

		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_HP(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nHPPercent = Convert.ToInt32( strCommandEdit[1]);
				
				if( nHPPercent > 0 && nHPPercent <= 100)
				{
					_Send_HP( nHPPercent);
					cheatResultTextDelegate( eCheatType.CT_HP, "", " : " + strCommandEdit[1] + " %");
				}
				else
					cheatResultTextErrDelegate( eCheatErrType.CE_HPErr);
				
				return;
			}
		}

		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_MP(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nMPPercent = Convert.ToInt32( strCommandEdit[1]);
				
				if( nMPPercent > 0 && nMPPercent <= 100)
				{
					_Send_MP( nMPPercent);
					cheatResultTextDelegate( eCheatType.CT_MP, "", " : " + strCommandEdit[1] + " %");
				}
				else
					cheatResultTextErrDelegate( eCheatErrType.CE_MPErr);
				
				return;
			}
		}

		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}
	
	public void Cheat_AttSpeed(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDouble( strCommandEdit[1]))
			{
				double fAttSpeed = Convert.ToDouble( strCommandEdit[1]);
				
				if( fAttSpeed >= 0.1 && fAttSpeed <= 10)
				{
					int nRes = (int)( fAttSpeed * 1000.0f);
					_Send_AttSpeed( nRes);
					cheatResultTextDelegate( eCheatType.CT_AttSpeed, "", " : " + strCommandEdit[1] + " x");
				}
				else
					cheatResultTextErrDelegate( eCheatErrType.CE_AttSpeedErr);
				
				return;
			}
		}

		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_Accuracy(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nValue = Convert.ToInt32( strCommandEdit[1]);
				
				_Send_Accuracy( nValue);
				cheatResultTextDelegate( eCheatType.CT_Accuracy, "", " : " + strCommandEdit[1] + " %");
				
				return;
			}
		}

		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_Condition(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nValue = Convert.ToInt32( strCommandEdit[1]);
				
				_Send_Condition( nValue);
				cheatResultTextDelegate( eCheatType.CT_Condition, "", " : " + strCommandEdit[1]);
				
				return;
			}
		}

		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}
	
	public void Cheat_InventoryExtendMax()
	{
		_Send_InventoryExtendMax();
		cheatResultTextDelegate( eCheatType.CT_InventoryExtendMax);
	}

	public void Cheat_ItemMakeDropGroup(string[] strCommandEdit)
	{
		if( 3 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]) && true == _isDigit( strCommandEdit[2]))
			{
				int nValue = Convert.ToInt32( strCommandEdit[1]);
				int nValue2 = Convert.ToInt32( strCommandEdit[2]);
				
				_Send_ItemMakeDropGroup( nValue, nValue2);
				cheatResultTextDelegate( eCheatType.CT_ItemMakeDropGroup, "", " : " + strCommandEdit[1] + " , Upgrade: " + strCommandEdit[2]);
				
				return;
			}
		}

		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_ItemMake(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nID = Convert.ToInt32( strCommandEdit[1]);
				_Send_ItemMake( nID);
				cheatResultTextDelegate( eCheatType.CT_ItemMake, "", " : ID:" + strCommandEdit[1]);
				return;
			}
		}
		else if( 3 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]) && true == _isDigit( strCommandEdit[2]))
			{
				int nID = Convert.ToInt32( strCommandEdit[1]);
				int nCount = Convert.ToInt32( strCommandEdit[2]);
				_Send_ItemMake( nID, nCount);
				cheatResultTextDelegate( eCheatType.CT_ItemMake, "", " : ID:" + strCommandEdit[1] + ", Count: " + strCommandEdit[2]);
				return;
			}
		}
		else if( 4 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]) && true == _isDigit( strCommandEdit[2]) && true == _isDigit( strCommandEdit[3]))
			{
				int nID = Convert.ToInt32( strCommandEdit[1]);
				int nCount = Convert.ToInt32( strCommandEdit[2]);
				int nInchent = Convert.ToInt32( strCommandEdit[3]);
				
				_Send_ItemMake( nID, nCount, nInchent);
				cheatResultTextDelegate( eCheatType.CT_ItemMake, "", " : ID:" + strCommandEdit[1] + ", Count: " + strCommandEdit[2] + ", Inchent: " + strCommandEdit[2]);
				
				return;
			}
		}
		
		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_ItemUp(string[] strCommandEdit)
	{
		if( 3 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]) && true == _isDigit( strCommandEdit[2]))
			{
				int nID = Convert.ToInt32( strCommandEdit[1]);
				int nInchent = Convert.ToInt32( strCommandEdit[2]);
				_Send_ItemUp( nID, nInchent);
				cheatResultTextDelegate( eCheatType.CT_ItemUp, "", " : ID:" + strCommandEdit[1] + ", Inchent: " + strCommandEdit[2]);
				return;
			}
		}
		
		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_Gold(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nGold = Convert.ToInt32( strCommandEdit[1]);
				_Send_Gold( nGold);
				cheatResultTextDelegate( eCheatType.CT_Gold, "", " : Gold:" + strCommandEdit[1]);
				return;
			}
		}
		
		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_Miracle(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nMiracle = Convert.ToInt32( strCommandEdit[1]);
				_Send_Miracle( nMiracle);
				cheatResultTextDelegate( eCheatType.CT_Miracle, "", " : Miracle:" + strCommandEdit[1]);
				return;
			}
		}
		
		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_ItemDel()
	{
		_Send_ItemDel();
		cheatResultTextDelegate( eCheatType.CT_ItemDel);
	}

	public void Cheat_Inchent()
	{
		cheatResultTextDelegate( eCheatType.CT_Inchent);
	}

	public void Cheat_DropRate(string[] strCommandEdit)
	{
		if( 3 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]) && true == _isDigit( strCommandEdit[2]))
			{
				int nValue = Convert.ToInt32( strCommandEdit[1]);
				int nValue2 = Convert.ToInt32( strCommandEdit[2]);
				
				_Send_DropRate( nValue, nValue2, 0);
				cheatResultTextDelegate( eCheatType.CT_DropRate, "", " : MonsterID: " + nValue.ToString() + ", Count: " + nValue2.ToString());
				return;
			}
		}
		else if( 4 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]) && true == _isDigit( strCommandEdit[2]) && true == _isDigit( strCommandEdit[3]))
			{
				int nValue = Convert.ToInt32( strCommandEdit[1]);
				int nValue2 = Convert.ToInt32( strCommandEdit[2]);
				int nValue3 = ( Convert.ToInt32( strCommandEdit[3]) > 0) ? 1 : 0;
				
				_Send_DropRate( nValue, nValue2, nValue3);
				cheatResultTextDelegate( eCheatType.CT_DropRate, "", " : MonsterID: " + nValue.ToString() + ", Count: " + nValue2.ToString() + ", Champion: " + nValue3.ToString());
				return;
			}
		}

		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_MobDel()
	{
		_Send_MobDel();
		cheatResultTextDelegate( eCheatType.CT_MobDel);
	}

	public void Cheat_MobSpawn(string[] strCommandEdit)
	{
		if( 1 == strCommandEdit.Length)
		{
			_Send_MobSpawn();
			cheatResultTextDelegate( eCheatType.CT_MobSpawn, "", " : All");
			return;
		}
		else if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nID = Convert.ToInt32( strCommandEdit[1]);
				_Send_MobSpawn( nID);
				cheatResultTextDelegate( eCheatType.CT_MobSpawn, "", " : ID:" + nID);
				return;
			}
		}
		else if( 3 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]) && true == _isDigit( strCommandEdit[2]))
			{
				int nID = Convert.ToInt32( strCommandEdit[1]);
				int nCount = Convert.ToInt32( strCommandEdit[2]);
				_Send_MobSpawn( nID, nCount);
				cheatResultTextDelegate( eCheatType.CT_MobSpawn, "", " : ID:" + nID + ", Count:" + nCount);
				return;
			}
		}
		else if( 4 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]) && true == _isDigit( strCommandEdit[2]) && true == _isDigit( strCommandEdit[3]))
			{
				int nID = Convert.ToInt32( strCommandEdit[1]);
				int nCount = Convert.ToInt32( strCommandEdit[2]);
				int nChannel_1 = Convert.ToInt32( strCommandEdit[3]);
				_Send_MobSpawn( nID, nCount, nChannel_1, 0);
				cheatResultTextDelegate( eCheatType.CT_MobSpawn, "", " : ID:" + nID + ", Count:" + nCount + ", Channel:" + nChannel_1);
				return;
			}
		}
		else if( 5 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]) && true == _isDigit( strCommandEdit[2]) && true == _isDigit( strCommandEdit[3]) && true == _isDigit( strCommandEdit[4]))
			{
				int nID = Convert.ToInt32( strCommandEdit[1]);
				int nCount = Convert.ToInt32( strCommandEdit[2]);
				int nChannel_1 = Convert.ToInt32( strCommandEdit[3]);
				int nChannel_2 = Convert.ToInt32( strCommandEdit[4]);
				_Send_MobSpawn( nID, nCount, nChannel_1, nChannel_2);
				cheatResultTextDelegate( eCheatType.CT_MobSpawn, "", " : ID:" + nID + ", Count:" + nCount + ", Channel:" + nChannel_1 + "~" + nChannel_2);
				return;
			}
		}

		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_MobStop(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				bool isStop = false;
				int nStop = Convert.ToInt32( strCommandEdit[1]);
				if( 0 == nStop)
					isStop = true;
				else
					isStop = false;
				_Send_MobStop( isStop);
				cheatResultTextDelegate( eCheatType.CT_MobStop, "", " : " + isStop.ToString());
				return;
			}
		}

		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_QAccept(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nID = Convert.ToInt32( strCommandEdit[1]);
				_Send_QAccept( nID);
				cheatResultTextDelegate( eCheatType.CT_QAccept, "", " : ID: " + strCommandEdit[1]);
				return;
			}
		}

		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_QDel(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nID = Convert.ToInt32( strCommandEdit[1]);
				_Send_QDel( nID);
				cheatResultTextDelegate( eCheatType.CT_QDel, "", " : ID: " + strCommandEdit[1]);
				return;
			}
		}

		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_QReset(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nID = Convert.ToInt32( strCommandEdit[1]);
				_Send_QReset( nID);
                ArkQuestmanager.instance.ResetQuest(nID);
				cheatResultTextDelegate( eCheatType.CT_QReset, "", " : ID: " + strCommandEdit[1]);
				return;
			}
			else if( true == strCommandEdit[1].Equals( "all"))
			{
				_Send_QReset( 0);
                ArkQuestmanager.instance.ResetQuestAll();
				cheatResultTextDelegate( eCheatType.CT_QReset, "", " : " + strCommandEdit[1]);
				return;
			}
		}

		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_QClear(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nID = Convert.ToInt32( strCommandEdit[1]);
				_Send_QClear( nID);
				cheatResultTextDelegate( eCheatType.CT_QReset, "", " : ID: " + strCommandEdit[1]);
				return;
			}
		}

		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_QComplete(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nID = Convert.ToInt32( strCommandEdit[1]);
				_Send_QComplete( nID);
				cheatResultTextDelegate( eCheatType.CT_QComplete, "", " : ID: " + strCommandEdit[1]);
				return;
			}
		}

		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}
	
	public void Cheat_QSet(string[] strCommandEdit)
	{
		if( 4 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]) && true == _isDigit( strCommandEdit[2]) && true == _isDigit( strCommandEdit[3]))
			{
				int nID = Convert.ToInt32( strCommandEdit[1]);
				int nIndex = Convert.ToInt32( strCommandEdit[2]);
				int nCount = Convert.ToInt32( strCommandEdit[3]);
				_Send_QSet( nID, nIndex, nCount);
				cheatResultTextDelegate( eCheatType.CT_QSet, "", " : ID: " + strCommandEdit[1] + ", Index: " + strCommandEdit[2] + ", Count: " + strCommandEdit[3]);
				return;
			}
		}
		
		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_CollectMake()
	{
		cheatResultTextDelegate( eCheatType.CT_CollectMake);
	}
	
	public void Cheat_PrivateShop(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nType = Convert.ToInt32( strCommandEdit[1]);
				_Send_PrivateShop( nType);
				cheatResultTextDelegate( eCheatType.CT_PrivateShop, "", " : Type:" + strCommandEdit[1]);
				return;
			}
		}
		else if( 3 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]) && true == _isDigit( strCommandEdit[2]))
			{
				int nType = Convert.ToInt32( strCommandEdit[1]);
				int nType2 = Convert.ToInt32( strCommandEdit[2]);
				_Send_PrivateShop( nType, nType2);
				cheatResultTextDelegate( eCheatType.CT_PrivateShop, "", " : Type:" + strCommandEdit[1] + ", bTest:" + strCommandEdit[1]);
				return;
			}
		}
		
		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}
	
	public void Cheat_SocialPoint(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nSocialPoint = Convert.ToInt32( strCommandEdit[1]);
				_Send_SocialPoint( nSocialPoint);
				cheatResultTextDelegate( eCheatType.CT_SocialPoint, "", " : SocialPoint:" + strCommandEdit[1]);
				return;
			}
		}
		
		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}
	
	public void Cheat_SkillLearn()
	{
		SkillBook.Instance.InitSkillBook();
		_Send_SkillLearn();
		cheatResultTextDelegate( eCheatType.CT_SkillLearn);
	}
	
	public void Cheat_WayPoint()
	{
		_Send_WayPoint();
		cheatResultTextDelegate( eCheatType.CT_WayPoint);
	}
	
	public void Cheat_Rank(string[] strCommandEdit)
	{
		if( 5 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]) && true == _isDigit( strCommandEdit[2]) && true == _isDigit( strCommandEdit[3]) && true == _isDigit( strCommandEdit[4]))
			{
				int nHour = Convert.ToInt32( strCommandEdit[1]);
				int nMin = Convert.ToInt32( strCommandEdit[2]);
				int nResetHour = Convert.ToInt32( strCommandEdit[3]);
				int nResetMin = Convert.ToInt32( strCommandEdit[4]);
				_Send_Rank( nHour, nMin, nResetHour, nResetMin);
				cheatResultTextDelegate( eCheatType.CT_Rank, "", " : Hour: " + strCommandEdit[1] + ", Min: " + strCommandEdit[2] + ", ResetHour: " + strCommandEdit[3] + ", ResetMin: " + strCommandEdit[4]);
				return;
			}
		}
		
		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_EquipItemUpgrade(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nValue = Convert.ToInt32( strCommandEdit[1]);
				_Send_EquipItemUpgrade( nValue);
				cheatResultTextDelegate( eCheatType.CT_EquipItemUpgrade, "", " : Upgrade: " + strCommandEdit[1]);
				return;
			}
		}
		
		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_PvPPointModify(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nValue = Convert.ToInt32( strCommandEdit[1]);
				
				if( nValue >= 0 && nValue <= 65000)
				{
					_Send_PvPPointModify( nValue);
					cheatResultTextDelegate( eCheatType.CT_PvPPointModify, "", " : PvPPoint: " + strCommandEdit[1]);
					return;
				}
			}
		}
		
		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

	public void Cheat_UIOpen()
	{
		eCLASS nowClass = AsEntityManager.Instance.UserEntity.GetProperty<eCLASS>(eComponentProperty.CLASS);

		Dictionary<int, PrepareOpenUIType> dic = AsTableManager.Instance.GetTbl_PrepareOpenUI(nowClass);

		foreach( KeyValuePair<int, PrepareOpenUIType> pair in dic)
		{
			_Send_QComplete( pair.Key);
			cheatResultTextDelegate( eCheatType.CT_UIOpen);
		}
	}

	public void Cheat_PetLevel(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nValue = Convert.ToInt32( strCommandEdit[1]);
				
				_Send_PetLevel( nValue);
				cheatResultTextDelegate( eCheatType.CT_PetLevel, "", " : PetLevel: " + strCommandEdit[1]);
				return;
			}
		}
		
		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}
	
	public void Cheat_PetHungry(string[] strCommandEdit)
	{
		if( 2 == strCommandEdit.Length)
		{
			if( true == _isDigit( strCommandEdit[1]))
			{
				int nValue = Convert.ToInt32( strCommandEdit[1]);
				
				_Send_PetHungry( nValue);
				cheatResultTextDelegate( eCheatType.CT_PetHungry, "", " : PetHungry: " + strCommandEdit[1]);
				return;
			}
		}
		
		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}

    public void Cheat_ApPointUp(string[] strCommandEdit)
    {
        if (2 == strCommandEdit.Length)
        {
            if (true == _isDigit(strCommandEdit[1]))
            {
                int nValue = Convert.ToInt32(strCommandEdit[1]);
                _Send_ApPointUp(nValue);
                cheatResultTextDelegate(eCheatType.CT_ApPoint, "", " : Ap Point Up: " + strCommandEdit[1]);
                return;
            }
        }

        cheatResultTextErrDelegate(eCheatErrType.CE_NotFoundCommand);
    }

    public void Cheat_ApRankSort()
    {
        _Send_ApRankSort();
        cheatResultTextDelegate(eCheatType.CT_ApSort);
    }

	public void Cheat_AttendanceBonus()
	{
		_Send_AttendanceBonus();
		cheatResultTextDelegate( eCheatType.CT_AttendanceBonus);
	}

 
		

	public void Cheat_Test(string[] strCommandEdit)
	{
		if( 1 < strCommandEdit.Length && strCommandEdit.Length < 8)
		{
			int n1 = 0;
			int n2 = 0;
			int n3 = 0;
			float f1 = 0.0f;
			float f2 = 0.0f;
			float f3 = 0.0f;
			
			if( true == _isDigit( strCommandEdit[1]))
				n1 = Convert.ToInt32( strCommandEdit[1]);
			
			if( 2 < strCommandEdit.Length && true == _isDigit( strCommandEdit[2]))
				n2 = Convert.ToInt32( strCommandEdit[2]);
			
			if( 3 < strCommandEdit.Length && true == _isDigit( strCommandEdit[3]))
				n3 = Convert.ToInt32( strCommandEdit[3]);
			
			if( 4 < strCommandEdit.Length && true == _isDouble( strCommandEdit[4]))
				f1 = (float)Convert.ToDouble( strCommandEdit[4]);

			if( 5 < strCommandEdit.Length && true == _isDouble( strCommandEdit[5]))
				f2 = (float)Convert.ToDouble( strCommandEdit[5]);

			if( 6 < strCommandEdit.Length && true == _isDouble( strCommandEdit[6]))
				f3 = (float)Convert.ToDouble( strCommandEdit[6]);
			
			_Send_Test( n1, n2, n3, f1, f2, f3);
			cheatResultTextDelegate( eCheatType.CT_Test, "",
				" : int " + n1.ToString() + ", " + n2.ToString() + ", " + n3.ToString() + ", float " + f1.ToString() + ", " + f2.ToString() + ", " + f3.ToString());
			return;
		}
		
		cheatResultTextErrDelegate( eCheatErrType.CE_NotFoundCommand);
	}
	#endregion cheat command
	
	// < private
	private bool _isDigit(string str)
	{
		if( str == null)
			return false;
		
		return System.Text.RegularExpressions.Regex.IsMatch( str, "^\\d+$");
	}
	
	private bool _isDouble(string str)
	{
		if( str == null)
			return false;
		
		return System.Text.RegularExpressions.Regex.IsMatch( str, @"^[+-]?\d*(\.?\d*)$");
	}

	#region send cheat command
	private void _Send_CharacterLevel(int nLevel)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_Level, "", nLevel, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void _Send_CharacterDie()
	{
		AsEntityManager.Instance.BroadcastMessageToAllEntities( new Msg_Cheat_Death());
		
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_Die, "", 0, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void _Send_Teleport(int nWarpID)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_Tel, "", nWarpID, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _Send_Recall(string strName)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_Recall, strName, 0, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _Send_Go(string strName)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_Go, strName, 0, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _Send_Strong(int nStrong)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_Strong, "", nStrong, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void _Send_Hide(int nHide)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_Hide, "", nHide, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _Send_Cool(bool bCool)
	{
		int nCool = 0;
		if( true == bCool)
			nCool = 1;
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_Cool, "", nCool, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _Send_MoveSpeed(int nMoveSpeed)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_Speed, "", nMoveSpeed, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void _Send_HP(int nHPPercent)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_HP, "", nHPPercent, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void _Send_MP(int nMPPercent)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_MP, "", nMPPercent, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void _Send_AttSpeed(int nAttSpeed)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_AttSpeed, "", nAttSpeed, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _Send_Accuracy(int nAccuracy)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_Accuracy, "", nAccuracy, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _Send_Condition(int nCondition)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_Condition, "", nCondition, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void _Send_InventoryExtendMax()
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_InventoryExtendMax, "", 0, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _Send_ItemMakeDropGroup(int nItemMakeDropGroup, int nItemUpgrade)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_ItemMakeDropGroup, "", nItemMakeDropGroup, nItemUpgrade, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _Send_ItemMake(int nID)
	{
		_Send_ItemMake( nID, 0, 0);
	}
	
	private void _Send_ItemMake(int nID, int nCount)
	{
		_Send_ItemMake( nID, nCount, 0);
	}
	
	private void _Send_ItemMake(int nID, int nCount, int nInchent)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_ItemMake, "", nID, nCount, nInchent, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void _Send_ItemUp(int nID, int nInchent)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_ItemUp, "", nID, nInchent, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void _Send_Gold(int nGold)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_Gold, "", nGold, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void _Send_Miracle(int nMiracle)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_Miracle, "", nMiracle, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _Send_ItemDel()
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_ItemDel, "", 0, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void _Send_DropRate(int nValue1, int nValue2, int nValue3)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_DropRate, "", nValue1, nValue2, nValue3, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _Send_MobDel()
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_MobDel, "", 0, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void _Send_MobSpawn()
	{
		_Send_MobSpawn( 0, 0);
	}
	
	private void _Send_MobSpawn(int nID)
	{
		_Send_MobSpawn( nID, 1);
	}
	
	private void _Send_MobSpawn(int nID, int nCount)
	{
		int nRadius = 10;
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_MobSpawn, "", nID, nRadius, nCount, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _Send_MobSpawn(int nID, int nCount, int nChannel_1, int nChannel_2)
	{
		int nRadius = 10;
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_MobSpawn, "", nID, nRadius, nCount, nChannel_1, nChannel_2, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void _Send_MobStop(bool isStop)
	{
		int stop = 0;
		if( true == isStop)
			stop = 1;
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_MobStop, "", stop, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void _Send_QAccept(int nID)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_QAccept, "", nID, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _Send_QDel(int nID)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_QDel, "", nID, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _Send_QReset(int nID)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_QReset, "", nID, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void _Send_QClear(int nID)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_QClear, "", nID, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _Send_QComplete(int nID)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_QComplete, "", nID, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void _Send_QSet(int nID, int nIndex, int nCount)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_QSet, "", nID, nIndex, nCount, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void _Send_PrivateShop(int nType)
	{
		_Send_PrivateShop( nType, 0);
	}
	
	private void _Send_PrivateShop(int nType, int nType2)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_PrivateShop, "", nType, nType2, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _Send_SocialPoint(int nSocialPoint)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_SocialPoint, "", nSocialPoint, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void _Send_SkillLearn()
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_SkillLearn, "", 0, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _Send_WayPoint()
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_WayPoint, "", 0, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void _Send_Rank(int nHour, int nMin, int nResetHour, int nResetMin)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_Rank, "", nHour, nMin, nResetHour, (float)nResetMin, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _Send_EquipItemUpgrade(int nValue)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_EquipItemUpgrade, "", nValue, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _Send_PvPPointModify(int nValue)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_PvPPointModify, "", nValue, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _Send_PetLevel(int nValue)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_PetLevel, "", nValue, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void _Send_PetHungry(int nValue)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_PetHungry, "", nValue, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void _Send_AttendanceBonus()
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_AttendanceBonus, "", 0, 0, 0, 0, 0, 0);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

    private void _Send_ApPointUp(int nValue)
    {
        body_CG_CHEAT packet = new body_CG_CHEAT((Int32)eCheatType.CT_ApPoint, "", nValue, 0, 0, 0, 0, 0);
        byte[] data = packet.ClassToPacketBytes();
        AsNetworkMessageHandler.Instance.Send(data);
    }

    private void _Send_ApRankSort()
    {
        body_CG_CHEAT packet = new body_CG_CHEAT((Int32)eCheatType.CT_ApSort, "", 0, 0, 0, 0, 0, 0);
        byte[] data = packet.ClassToPacketBytes();
        AsNetworkMessageHandler.Instance.Send(data);
    }

	private void _Send_Test(int n1, int n2, int n3, float f1, float f2, float f3)
	{
		body_CG_CHEAT packet = new body_CG_CHEAT( (Int32)eCheatType.CT_Test, "", n1, n2, n3, f1, f2, f3);
		byte[] data = packet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	#endregion send cheat command
	// private >
}
