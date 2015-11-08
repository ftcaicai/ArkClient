using UnityEngine;
using System.Collections;
using System.Globalization;

public class AsGuildDetailInfo : MonoBehaviour
{
	static private int MAX_GUILD_LEVEL = 10;
	public SpriteText CUR_LEVEL = null;
	public SpriteText CUR_MAX_GUILDER = null;
	public SpriteText CUR_STORAGE = null;
	public SpriteText NEXT_LEVEL = null;
	public SpriteText NEXT_MAX_GUILDER = null;
	public SpriteText NEXT_STORAGE = null;
	public SpriteText LEVELUP_CONDITION = null;
	public SpriteText NEED_GOLD = null;
	public SpriteText title = null;
	public SpriteText curLevel = null;
	public SpriteText curMaxGuilder = null;
	public SpriteText curStorage = null;
	public SpriteText nextLevel = null;
	public SpriteText nextMaxGuilder = null;
	public SpriteText nextStorage = null;
	public SpriteText needGold = null;
	public UIButton levelUpBtn = null;
	private int savedLevel = -1;
	public int SavedLevel
	{
		set	{ savedLevel = value; }
	}
	
	// Use this for initialization
	void Start()
	{
		title.Text = AsTableManager.Instance.GetTbl_String(1265);
		CUR_LEVEL.Text = AsTableManager.Instance.GetTbl_String(1273);
		CUR_MAX_GUILDER.Text = AsTableManager.Instance.GetTbl_String(1275);
		CUR_STORAGE.Text = AsTableManager.Instance.GetTbl_String(1276);
		NEXT_LEVEL.Text = AsTableManager.Instance.GetTbl_String(1274);
		NEXT_MAX_GUILDER.Text = AsTableManager.Instance.GetTbl_String(1275);
		NEXT_STORAGE.Text = AsTableManager.Instance.GetTbl_String(1276);
		LEVELUP_CONDITION.Text = AsTableManager.Instance.GetTbl_String(1277);
		NEED_GOLD.Text = AsTableManager.Instance.GetTbl_String(1278);
		levelUpBtn.Text = AsTableManager.Instance.GetTbl_String(1279);

		AsUtil.SetRenderingState( gameObject, false);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Init( body_SC_GUILD_INFO_DETAIL_RESULT data)
	{
		AsUtil.SetRenderingState( gameObject, true);

		int expIndex = savedLevel + 4210 - 1;

		curLevel.Text = string.Format( "{0}{1}", AsTableManager.Instance.GetTbl_String( 1094), savedLevel);
		curMaxGuilder.Text = string.Format( "{0}{1}", data.nCurMaxMember, AsTableManager.Instance.GetTbl_String(337));
		curStorage.Text = string.Format( "{0}", AsTableManager.Instance.GetTbl_String( expIndex));
		
		if( MAX_GUILD_LEVEL > savedLevel)
		{
			nextLevel.Text = string.Format( "{0}{1}", AsTableManager.Instance.GetTbl_String( 1094), ( savedLevel + 1));
			nextMaxGuilder.Text = string.Format( "{0}{1}", data.nNextMaxMember, AsTableManager.Instance.GetTbl_String(337));
			nextStorage.Text = string.Format( "{0}", AsTableManager.Instance.GetTbl_String( expIndex + 1));
			needGold.Text = data.nPrice.ToString( "#,#0", CultureInfo.InvariantCulture);
		}
		else
		{
			nextLevel.gameObject.SetActiveRecursively( false);
			nextMaxGuilder.gameObject.SetActiveRecursively( false);
			nextStorage.gameObject.SetActiveRecursively( false);
			needGold.gameObject.SetActiveRecursively( false);
			levelUpBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			levelUpBtn.spriteText.Color = Color.gray;
		}
	}
	
	private void OnLevelupBtn()
	{
		Debug.Log( "OnLevelupBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		body_CS_GUILD_LEVEL_UP levelUp = new body_CS_GUILD_LEVEL_UP();
		byte[] packet = levelUp.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
		
		GameObject.DestroyImmediate( gameObject.transform.parent.gameObject);
	}
	
	private void OnCloseBtn()
	{
		Debug.Log( "OnCloseBtn");
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		GameObject.DestroyImmediate( gameObject.transform.parent.gameObject);
	}
}
