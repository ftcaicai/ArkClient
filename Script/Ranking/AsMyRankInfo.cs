using UnityEngine;
using System.Collections;
using System.Text;

public class AsMyRankInfo : MonoBehaviour
{
	[SerializeField] SimpleSprite portrait = null;
	[SerializeField] SimpleSprite[] classes = new SimpleSprite[0];
	[SerializeField] SpriteText levelText = null;
	[SerializeField] SpriteText nameField = null;
	[SerializeField] SpriteText rankLabel = null;
	[SerializeField] SpriteText rankPoint = null;

	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Init( body_SC_RANK_SUMMARY_MYRANK_LOAD_RESULT data)
	{
		foreach( SimpleSprite cls in classes)
			cls.gameObject.SetActiveRecursively( false);
		classes[ AsUserInfo.Instance.SavedCharStat.class_ - 1].gameObject.SetActiveRecursively( true);

		levelText.Text = AsUserInfo.Instance.SavedCharStat.level_.ToString();
		nameField.Text = AsUserInfo.Instance.SavedCharStat.charName_;
		rankLabel.Text = AsTableManager.Instance.GetTbl_String(1668);
		rankPoint.Text = data.nCurItemRankPoint.ToString();
		
		SetDelegateImage();
	}
	
	void SetDelegateImage()
	{
		StringBuilder sb = new StringBuilder( "UIPatchResources/DelegateImage/");
		
		DelegateImageData imageData = AsDelegateImageManager.Instance.GetAssignedDelegateImage();
		if( null == imageData)
			sb.Append( "Default");
		else
			sb.Append( imageData.iconName);
		
		Texture2D tex = ResourceLoad.Loadtexture( sb.ToString()) as Texture2D;
		portrait.SetTexture( tex);
		portrait.SetUVsFromPixelCoords( new Rect( 0.0f, 0.0f, tex.width, tex.height));
	}
	
	public void SetMyInfo(eRankViewType type, int nRankPoint)
	{
		if( eRankViewType.PvpWorld == type || eRankViewType.PvpFriend == type)
			rankLabel.Text = AsTableManager.Instance.GetTbl_String(2107);
		else
			rankLabel.Text = AsTableManager.Instance.GetTbl_String(1668);
		
		rankPoint.Text = nRankPoint.ToString();
	}
}
