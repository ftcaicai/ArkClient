
#define _COROUTINE_ROLLING

using UnityEngine;
using System.Collections;
using System.Text;

public class AsLoadingInfo : MonoBehaviour
{
	[SerializeField] SimpleSprite bg = null;
	[SerializeField] SpriteText tip = null;
	
	// Use this for initialization
	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( tip);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Init( int mapID)
	{
		Map map = TerrainMgr.Instance.GetMap( mapID);
		Debug.Assert( null != map);


		StringBuilder sb = new StringBuilder( "UIPatchResources/Loading/LoadingBG_");
		switch( map.MapData.getMapType)
		{
		case eMAP_TYPE.Pvp:
			sb.Append( "PvP_01");
			break;
		case eMAP_TYPE.Summon:
			sb.Append( "Summon_01");
			break;
		default:
			sb.AppendFormat( "{0:D2}", GetLoadingBGIndex() );
			break;
		}
		
		Texture tex = ResourceLoad.Loadtexture( sb.ToString());
		
		bg.renderer.material.mainTexture = tex;
		
#if _COROUTINE_ROLLING
		StartCoroutine( "TipRolling");
#else
		InvokeRepeating( "TipRolling", 2.0f, 2.0f);
#endif
	}

	int		GetLoadingBGIndex()
	{
		int nUserLevel = AsUserInfo.Instance.SavedCharStat.level_;
		Tbl_GlobalWeight_Record loadingType1 = AsTableManager.Instance.GetTbl_GlobalWeight_Record (205);
		Tbl_GlobalWeight_Record loadingType2 = AsTableManager.Instance.GetTbl_GlobalWeight_Record (206);
		
		int nLoadingIndex = 1;

		if( loadingType1 != null && nUserLevel <= loadingType1.Value )
			nLoadingIndex = 5;
		else if( loadingType2 != null && nUserLevel <= loadingType2.Value ) 
			nLoadingIndex = 6;
		else
			nLoadingIndex = Random.Range( 1, 5);

		return nLoadingIndex;
	}
	
	#if _COROUTINE_ROLLING
	IEnumerator TipRolling()
	{
		AsLoadingTipManager.Instance.SuitableTipCollect();
		
		while( true)
		{
			tip.Text = AsLoadingTipManager.Instance.GetLoadingTip();
			
			yield return new WaitForSeconds( 2.0f);
		}
	}
#else
	void TipRolling()
	{
		tip.Text = AsLoadingTipManager.Instance.GetLoadingTip();
	}
#endif
}
