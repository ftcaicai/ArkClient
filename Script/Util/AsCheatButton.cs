using UnityEngine;
using System.Collections;

public class AsCheatButton : MonoBehaviour
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
		CT_ItemMake, // item
		CT_ItemUp,
		CT_Gold,
		CT_Miracle, // cash
		CT_ItemDel,
		CT_Inchent,
		CT_MobDel, // monster
		CT_MobSpawn,
		CT_MobStop,
		CT_QAccept, // quest
		CT_QDel,
		CT_QReset,
		CT_QClear,
		CT_QComplete,
		CT_CollectMake,
		CT_PrivateShop, // shop
		CT_SocialPoint, // social
		CT_SkillLearn,
		CT_WayPoint,
		CT_Test, // etc

		CT_Max
	};

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	void OnGUI()
	{
		GUILayout.BeginArea( new Rect( 1, 1, 120, 100));
		if( true == GUILayout.Button( "MobSpawn 260113 10"))
		{
			int nRadius = 10;
			body_CG_CHEAT packet = new body_CG_CHEAT( (int)eCheatType.CT_MobSpawn, "", 260133, nRadius, 10, 0, 0, 0);
			byte[] data = packet.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( data);
		}

		GUILayout.EndArea();
	}
}
