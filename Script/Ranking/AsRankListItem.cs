using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class AsRankListItem : MonoBehaviour
{
	[SerializeField] SpriteText rankText = null;
	[SerializeField] SimpleSprite portrait = null;
	[SerializeField] SimpleSprite[] classes = new SimpleSprite[0];
	[SerializeField] SpriteText levelText = null;
	[SerializeField] SpriteText nameText = null;
	[SerializeField] SpriteText rankPointText = null;
	[SerializeField] SpriteText fluctuationPointText = null;
	[SerializeField] SimpleSprite[] fluctuations = new SimpleSprite[0];
    [SerializeField] GameObject[] objRewardSlots = null;
    [SerializeField] GameObject objSlotParents = null;
    [SerializeField] GameObject objFluctuationParents = null;

	private sRANKINFO rankInfo = null;
    private bool isSetRewardInfo = false;

    public List<ApRewardInfo> listApRewardInfo = new List<ApRewardInfo>();
	
	// Use this for initialization
	void Start()
	{

	}

    void OnEnable()
    {
        if (objSlotParents != null)
            objSlotParents.SetActive(isSetRewardInfo);
    }
	
	// Update is called once per frame
	void Update()
	{

	}

    public void DisableFluctuation()
    {
        if (objFluctuationParents != null)
            objFluctuationParents.SetActiveRecursively(false);
    }
	
	public void Init( sRANKINFO data)
	{
		rankInfo = data;

		StringBuilder sb = new StringBuilder();
		sb.AppendFormat( AsTableManager.Instance.GetTbl_String(1766), data.nRank);
		rankText.Text = sb.ToString();
		
		foreach( SimpleSprite cls in classes)
			cls.gameObject.SetActiveRecursively( false);

        if (eCLASS.NONE != data.eClass)
        {
            Debug.LogWarning(data.eClass.ToString());

            classes[(int)data.eClass - 1].gameObject.SetActiveRecursively(true);
        }

		levelText.Text = data.nLevel.ToString();
		nameText.Text = data.szCharName;
		rankPointText.Text = data.nRankPoint.ToString();
		fluctuationPointText.Text = Mathf.Abs( data.nDiffRank).ToString();
		
		foreach( SimpleSprite fluc in fluctuations)
			fluc.gameObject.SetActiveRecursively( false);
		if( 0 == data.nDiffRank)
		{
			fluctuations[1].gameObject.SetActiveRecursively( true);
			fluctuationPointText.Color = new Color( 0.3f, 1.0f, 0.1f, 1.0f);//Color.green;
		}
		else if( 0 < data.nDiffRank)
		{
			fluctuations[2].gameObject.SetActiveRecursively( true);
			fluctuationPointText.Color = new Color( 0.9f, 0.3f, 0.1f, 1.0f);//Color.red;
		}
		else
		{
			fluctuations[0].gameObject.SetActiveRecursively( true);
			fluctuationPointText.Color = new Color( 0.1f, 0.4f, 1.0f, 1.0f);//Color.blue;
		}

		SetDelegateImage();
	}
	
	void SetDelegateImage()
	{
		DelegateImageData delegateImage = null;
		
		if( rankInfo.nCharUniqKey == AsUserInfo.Instance.SavedCharStat.uniqKey_)
			delegateImage = AsDelegateImageManager.Instance.GetAssignedDelegateImage();
		else
			delegateImage = AsDelegateImageManager.Instance.GetDelegateImage( rankInfo.nImageTableIdx);
		
		if( null == delegateImage)
			return;
		
		StringBuilder sb = new StringBuilder( "UIPatchResources/DelegateImage/");
		sb.Append( delegateImage.iconName);
		Texture2D tex = ResourceLoad.Loadtexture( sb.ToString()) as Texture2D;
		portrait.SetTexture( tex);
		portrait.SetUVsFromPixelCoords( new Rect( 0.0f, 0.0f, tex.width, tex.height));
	}
	
	private void OnClick()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( AsUserInfo.Instance.SavedCharStat.uniqKey_ == rankInfo.nCharUniqKey)
		{
			Debug.Log( "My character!!!");
			return;
		}

		GameObject go = ResourceLoad.CreateGameObject( "UI/Optimization/Prefab/RankPopup");
		Debug.Assert( null != go);
//		AsRankPopup popup = go.GetComponent<AsRankPopup>();
//		Debug.Assert( null != popup);
		AsHudDlgMgr.Instance.RankPopup = go.GetComponent<AsRankPopup>();
		Debug.Assert( null != AsHudDlgMgr.Instance.RankPopup);
		AsHudDlgMgr.Instance.RankPopup.RankInfo = rankInfo;
	}

    public void SetApRewardInfo( List<ApRewardInfo> _listApRewardInfo)
    {
        listApRewardInfo = _listApRewardInfo;

        if (listApRewardInfo.Count >= 1)
            objSlotParents.SetActive(true);

        for (int i = 0; i < listApRewardInfo.Count; i++)
        {
            if (i >= 3)
                continue;

            GameObject objIcon = AsNpcStore.GetItemIcon(listApRewardInfo[i].itemID.ToString(), listApRewardInfo[i].itemCount);

            if (objRewardSlots[i] != null)
            {
                objIcon.transform.parent = objRewardSlots[i].transform;
                objIcon.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f);
                objIcon.transform.localScale = new Vector3(0.7f, 0.7f, 1.0f);
            }
        }
    }

    public void ShowTooltip(Ray _ray)
    {
        int count = 0;
        foreach (GameObject obj in objRewardSlots)
        {
            BoxCollider collider = obj.GetComponent<BoxCollider>();

            if (collider == null)
                continue;

            if (true == AsUtil.PtInCollider(collider, _ray))
            {
                if (listApRewardInfo.Count > count)
                {
                    TooltipMgr.Instance.OpenTooltip(TooltipMgr.eOPEN_DLG.normal, listApRewardInfo[count].itemID);
                    break;
                }
            }
            count++;
        }
    }
}
