using UnityEngine;
using System;
using System.Collections;
using System.Text;

public class AsDelegateImageDlgItem : MonoBehaviour
{
	[SerializeField]UIButton[] rowBtn;
	[SerializeField]SimpleSprite[] rowImg;
	[SerializeField]SimpleSprite assignedMark = null;
	[SerializeField]SimpleSprite selectMark = null;
	[SerializeField]GameObject unlockEffect = null;
	public DelegateImageData[] data = new DelegateImageData[3];
	[HideInInspector]public AsDelegateImageDlg parentDlg = null;
	bool isStarted = false;
	int selectIndex = -1;
	static public GameObject confirmDlg = null;
	static public GameObject statDlg = null;
	
	AsCheckTap[]			checkTap;
	
	void Awake()
	{
		if( rowBtn.Length > 0 )
		{
			checkTap = new AsCheckTap[rowBtn.Length];
			
			for(int i=0;i<rowBtn.Length;i++)
			{
				GameObject	goCheckTap = new GameObject("CheckTap");
				goCheckTap.transform.parent = rowBtn[i].gameObject.transform;
				checkTap[i] = goCheckTap.AddComponent<AsCheckTap>();	
				checkTap[i].SetTapDelegate(ProcessTap);
				checkTap[i].TapDelay = 0.3f;
			}
		}
	}
	
	// Use this for initialization
	void Start()
	{
		isStarted = true;
		Init();
	}
	
	public void Init()
	{
		selectMark.gameObject.SetActiveRecursively( false);
		assignedMark.gameObject.SetActiveRecursively( false);
		unlockEffect.gameObject.SetActiveRecursively( false);
		
		if( rowBtn.Length != data.Length || rowImg.Length != data.Length )
		{
			Debug.LogError("DelegateImageDlgItem data rowBtn rowImg length error");
			return;
		}
		
		for(int i=0;i<data.Length;i++)
		{
			if( null != data[i])
			{
				rowBtn[i].gameObject.SetActiveRecursively( true);
				rowImg[i].gameObject.SetActiveRecursively( true);
				
				if( true == data[i].locked)
				{
					rowBtn[i].Text = AsTableManager.Instance.GetTbl_String(4082);
					rowImg[i].renderer.material.color = Color.gray;
				}
				else
				{
					rowBtn[i].Text = string.Empty;
					rowImg[i].renderer.material.color = Color.white;
				}
				
				StringBuilder sb = new StringBuilder( "UIPatchResources/DelegateImage/");
				sb.Append( data[i].iconName);
				Texture2D tex = ResourceLoad.Loadtexture( sb.ToString()) as Texture2D;
				rowImg[i].SetTexture( tex);
				rowImg[i].SetUVsFromPixelCoords( new Rect( 0.0f, 0.0f, tex.width, tex.height));
				
				if( AsDelegateImageManager.Instance.AssignedImageID == data[i].id)
				{
					assignedMark.gameObject.SetActiveRecursively( true);
					assignedMark.transform.localPosition = rowBtn[i].gameObject.transform.localPosition;
				}
				
				if( AsDelegateImageManager.Instance.SelectedImageID == data[i].id)
				{
					selectMark.gameObject.SetActiveRecursively( true);
					selectMark.transform.localPosition = rowBtn[i].gameObject.transform.localPosition;
					selectIndex = 0;
				}
			}
			else
			{
				rowBtn[i].gameObject.SetActiveRecursively( false);
				rowBtn[i].Text = string.Empty;
				rowImg[i].gameObject.SetActiveRecursively( false);
			}		
		}
	}
	
	void OnEnable()
	{
		if( false == isStarted)
			return;

		Init();
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	void OnBtn0()
	{
		BtnClicked(0);
	}
	
	void OnBtn1()
	{
		BtnClicked(1);
	}
	
	void OnBtn2()
	{
		BtnClicked(2);
	}
	
	void BtnClicked(int nIndex)
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		parentDlg.SendMessage( "ClearSelectMark");
		
		selectIndex = nIndex;
		
		if( selectIndex >= rowBtn.Length )
		{
			Debug.LogError("DelegateImageDlgItem selectIndex overflow rowBtn.Length : " + selectIndex.ToString() );
			return;
		}
		
		if( data[selectIndex] == null )
		{
			Debug.LogError("DelegateImageDlgItem data[" + selectIndex.ToString() + "] is null");
			return;
		}
		
		selectMark.gameObject.SetActiveRecursively( true);
		selectMark.transform.localPosition = rowBtn[selectIndex].gameObject.transform.localPosition;
		AsDelegateImageManager.Instance.SelectedImageID = data[selectIndex].id;
		
		for(int i=0;i<checkTap.Length;i++)
		{
			if( i == selectIndex )
				checkTap[i].Tap();
			else
				checkTap[i].Cancel();
		}
	}
	
	void ProcessTap(int nTapCount)
	{
		if( selectIndex < 0 || selectIndex >= data.Length )
			return;

		bool isPopupComfirmDlg = true;
		
		if (nTapCount >= 2)
			isPopupComfirmDlg = false;
		else
			isPopupComfirmDlg = true;

		if( data[selectIndex] != null && (data[selectIndex].locked == false || data[selectIndex].eType == eDelegateImageType.Rare ) )
		{
			isPopupComfirmDlg = false;
		}

		if( isPopupComfirmDlg == true )
			PromptConfirmDlg(data[selectIndex]);
		else
			DelegateImageStatDlg(data[selectIndex]);
	}
	
	public void ClearSelectMark()
	{
		selectIndex = -1;
		selectMark.gameObject.SetActiveRecursively( false);
		AsDelegateImageManager.Instance.SelectedImageID = -1;
	}
	
	public void UnlockDelegateImage( Int32 id)
	{
		for(int i=0;i<data.Length;i++)
		{
			if( ( null != data[i]) && ( data[i].id == id))
			{
				PlayUnlockEffect();
				break;
			}
		}
	}
	
	private void PromptConfirmDlg( DelegateImageData data)
	{
		if( false == data.locked)
			return;
		
		if( statDlg != null )
			Destroy( statDlg.gameObject);
		
		if( null != confirmDlg)
			Destroy( confirmDlg.gameObject);
		
		confirmDlg = GameObject.Instantiate( ResourceLoad.LoadGameObject( "UI/Optimization/Prefab/DelegateImageConfirm")) as GameObject;
		Debug.Assert( null != confirmDlg);
		AsDelegateImageConfirmDlg dlg = confirmDlg.GetComponent<AsDelegateImageConfirmDlg>();
		Debug.Assert( null != dlg);
		dlg.Init( this, data);
	}
			
	private void DelegateImageStatDlg( DelegateImageData data )
	{
		if( confirmDlg != null )
			Destroy( confirmDlg.gameObject);
		
		if( null != statDlg)
			Destroy( statDlg.gameObject);
		
		statDlg = GameObject.Instantiate( ResourceLoad.LoadGameObject( "UI/Optimization/Prefab/DelegateImageInfo")) as GameObject;
		Debug.Assert( null != statDlg);
		AsDelegateImageStatDlg dlg = statDlg.GetComponent<AsDelegateImageStatDlg>();
		Debug.Assert( null != dlg);
		dlg.Init( data);
	}
			
	
	public void PlayUnlockEffect()
	{
		if( selectIndex < 0 || selectIndex >= rowBtn.Length )
		{
			Debug.LogError("PlayUnlockEffect selectIndex error : " + selectIndex.ToString() );
			return;
		}
		
		unlockEffect.gameObject.SetActiveRecursively( true);
		
		rowBtn[selectIndex].Text = string.Empty;
		rowImg[selectIndex].renderer.material.color = Color.white;
		unlockEffect.transform.localPosition = new Vector3( rowBtn[selectIndex].gameObject.transform.localPosition.x,
			rowBtn[selectIndex].gameObject.transform.localPosition.y, rowBtn[selectIndex].gameObject.transform.localPosition.z - 1.0f);

		StartCoroutine( "PlayEffect");
	}
	
	IEnumerator PlayEffect()
	{
		yield return new WaitForSeconds( 2.0f);
		
		unlockEffect.gameObject.SetActiveRecursively( false);
	}
}
