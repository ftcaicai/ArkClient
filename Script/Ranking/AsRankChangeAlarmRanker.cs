using UnityEngine;
using System.Collections;
using System.Text;

public class AsRankChangeAlarmRanker : MonoBehaviour
{
	[SerializeField]SpriteText nameText = null;
	[SerializeField]SpriteText fluctuationText = null;
	[SerializeField]SpriteText rankPointText = null;
	[SerializeField]SpriteText rank = null;
	[SerializeField]SimpleSprite portrait = null;
	[SerializeField]SpriteText nameField = null;
	[SerializeField]SimpleSprite[] fluctuationIcon = new SimpleSprite[0];
	[SerializeField]SpriteText fluctuationPoint = null;
	[SerializeField]SpriteText myRankPoint = null;
	
	// Use this for initialization
	void Start()
	{
		nameText.Text = AsTableManager.Instance.GetTbl_String(1763);
		fluctuationText.Text = AsTableManager.Instance.GetTbl_String(1764);
		rankPointText.Text = AsTableManager.Instance.GetTbl_String(1765);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	private void SetDelegateImage()
	{
		StringBuilder sb = new StringBuilder( "UIPatchResources/DelegateImage/");
		
		DelegateImageData delegateImage = AsDelegateImageManager.Instance.GetAssignedDelegateImage();
		if( null == delegateImage)
			sb.Append( "Default");
		else
			sb.Append( delegateImage.iconName);
			
		Texture2D tex = ResourceLoad.Loadtexture( sb.ToString()) as Texture2D;
		portrait.SetTexture( tex);
		portrait.SetUVsFromPixelCoords( new Rect( 0.0f, 0.0f, tex.width, tex.height));
	}
	
	public void Init( body_SC_RANK_CHANGE_MYRANK scData)
	{
		if( eRANKTYPE.eRANKTYPE_ARENA == scData.eRankType)
		{
			nameText.Text = AsTableManager.Instance.GetTbl_String(1763);
			fluctuationText.Text = AsTableManager.Instance.GetTbl_String(2104);
			rankPointText.Text = AsTableManager.Instance.GetTbl_String(2049);
		}
		else
		{
			nameText.Text = AsTableManager.Instance.GetTbl_String(1763);
			fluctuationText.Text = AsTableManager.Instance.GetTbl_String(1764);
			rankPointText.Text = AsTableManager.Instance.GetTbl_String(1765);
		}
		
		sMYRANKINFO data = scData.sMyRankInfo;
		
		SetDelegateImage();

		StringBuilder sb = new StringBuilder();
		sb.AppendFormat( AsTableManager.Instance.GetTbl_String(1766), data.nRank);
		rank.Text = sb.ToString();
		nameField.Text = AsUserInfo.Instance.SavedCharStat.charName_;
		
		foreach( SimpleSprite icon in fluctuationIcon)
			icon.gameObject.SetActiveRecursively( false);
		
		if( 0 > data.nDiffRank)
		{
			fluctuationIcon[0].gameObject.SetActiveRecursively( true);
			fluctuationPoint.Color = Color.blue;
		}
		else if( 0 < data.nDiffRank)
		{
			fluctuationIcon[2].gameObject.SetActiveRecursively( true);
			fluctuationPoint.Color = Color.red;
		}
		else
		{
			fluctuationIcon[1].gameObject.SetActiveRecursively( true);
			fluctuationPoint.Color = Color.green;
		}
		
		fluctuationPoint.Text = data.nDiffRank.ToString();
		myRankPoint.Text = data.nRankPoint.ToString();
	}

	/*
	public void Init( sMYRANKINFO data)
	{
		SetDelegateImage();

		StringBuilder sb = new StringBuilder();
		sb.AppendFormat( AsTableManager.Instance.GetTbl_String(1766), data.nRank);
		rank.Text = sb.ToString();
		nameField.Text = AsUserInfo.Instance.SavedCharStat.charName_;
		
		foreach( SimpleSprite icon in fluctuationIcon)
			icon.gameObject.SetActiveRecursively( false);
		
		if( 0 > data.nDiffRank)
		{
			fluctuationIcon[0].gameObject.SetActiveRecursively( true);
			fluctuationPoint.Color = Color.blue;
		}
		else if( 0 < data.nDiffRank)
		{
			fluctuationIcon[2].gameObject.SetActiveRecursively( true);
			fluctuationPoint.Color = Color.red;
		}
		else
		{
			fluctuationIcon[1].gameObject.SetActiveRecursively( true);
			fluctuationPoint.Color = Color.green;
		}
		
		fluctuationPoint.Text = data.nDiffRank.ToString();
		myRankPoint.Text = data.nRankPoint.ToString();
	}
	
	public void Init( sRANKINFO data)
	{
		SetDelegateImage();
		
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat( AsTableManager.Instance.GetTbl_String(1766), data.nRank);
		rank.Text = sb.ToString();
		nameField.Text = AsUtil.GetRealString( data.szCharName);
		
		foreach( SimpleSprite icon in fluctuationIcon)
			icon.gameObject.SetActiveRecursively( false);
		
		if( 0 > data.nDiffRank)
		{
			fluctuationIcon[0].gameObject.SetActiveRecursively( true);
			fluctuationPoint.Color = Color.blue;
		}
		else if( 0 < data.nDiffRank)
		{
			fluctuationIcon[2].gameObject.SetActiveRecursively( true);
			fluctuationPoint.Color = Color.red;
		}
		else
		{
			fluctuationIcon[1].gameObject.SetActiveRecursively( true);
			fluctuationPoint.Color = Color.green;
		}
		
		fluctuationPoint.Text = data.nDiffRank.ToString();
	}
	*/
}
