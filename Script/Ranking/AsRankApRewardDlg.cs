using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsRankApRewardDlg : MonoBehaviour {

    [SerializeField] SpriteText txtTitle;
    [SerializeField] SpriteText txtMessage;
    [SerializeField] SpriteText txtRanking;
    [SerializeField] GameObject[] objRewardSlots;
    [SerializeField] UIButton   btnGetReward;
    [SerializeField] UIButton   btnClose;

    private List<ApRewardInfo> listApRewardInfo = new List<ApRewardInfo>();

	// Use this for initialization
	void Start () 
    {
        AsLanguageManager.Instance.SetFontFromSystemLanguage(txtTitle);
        AsLanguageManager.Instance.SetFontFromSystemLanguage(txtMessage);
        AsLanguageManager.Instance.SetFontFromSystemLanguage(txtRanking);

        btnGetReward.AddInputDelegate(GetRewardButtonDelegate);
        btnClose.AddInputDelegate(CloseButtonDelegate);
	}
	
	void Update () 
    {
	
	}

    public void Show(uint _rank, int _rewardGroup)
    {
        sCHARVIEWDATA usrInfo = AsUserInfo.Instance.GetCurrentUserCharacterInfo();

        listApRewardInfo = AsTableManager.Instance.GetTbl_ApRewardInfoList(usrInfo.eClass, _rank, _rewardGroup);

        for(int i=0 ; i < listApRewardInfo.Count ; i++)
        {
            if (i >= objRewardSlots.Length)
                continue;

            GameObject objIcon = AsNpcStore.GetItemIcon(listApRewardInfo[i].itemID.ToString(), listApRewardInfo[i].itemCount);

            objIcon.transform.parent = objRewardSlots[i].transform;
            objIcon.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f);
            objIcon.transform.localScale = new Vector3(0.7f, 0.7f, 0.0f);
        }
    }

    void GetRewardButtonDelegate(ref POINTER_INFO ptr)
    {
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
            AsCommonSender.SendGetApRankReward();
    }

    void CloseButtonDelegate(ref POINTER_INFO ptr)
    {
        Destroy(this);
    }
}
