using UnityEngine;
using System.Collections;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum eMaintenanceType : int
{
	Invalid = -1,
	
	Temporary,
	Regular,
	
	Max
};

public class AsInspectionDlg : MonoBehaviour
{
	[SerializeField]private SimpleSprite[] types = new SimpleSprite[0];
	[SerializeField]private SpriteText period = null;
	[SerializeField]private SpriteText reason = null;
	[SerializeField]private UIButton confirmBtn = null;
	
	void Awake()
	{
		foreach( SimpleSprite type in types)
			type.gameObject.SetActiveRecursively( false);
	}
	
	// Use this for initialization
	void Start()
	{
		confirmBtn.Text = AssetbundleManager.Instance.GetPatcherString(5);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void SetType( string type)
	{
		if( "regular" == type)
			types[ (int)eMaintenanceType.Regular].gameObject.SetActiveRecursively( true);
		else if( "temporary" == type)
			types[ (int)eMaintenanceType.Temporary].gameObject.SetActiveRecursively( true);
		else
			Debug.Assert( false);
	}
	
	public void SetPeriod( string begin, string end)
	{
		string beginYear = begin.Substring( 0, 4);
		string beginMonth = begin.Substring( 4, 2);
		string beginDay = begin.Substring( 6, 2);
		string beginHour = begin.Substring( 8, 2);
		string beginMin = begin.Substring( 10, 2);
		string endYear = end.Substring( 0, 4);
		string endMonth = end.Substring( 4, 2);
		string endDay = end.Substring( 6, 2);
		string endHour = end.Substring( 8, 2);
		string endMin = end.Substring( 10, 2);
		
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat( "{0}.{1}.{2} {3}:{4} ~ {5}.{6}.{7} {8}:{9}", beginYear, beginMonth, beginDay, beginHour, beginMin, endYear, endMonth, endDay, endHour, endMin);
		period.Text = sb.ToString();
	}
	
	public void SetReason( string rsn)
	{
		reason.Text = rsn;
	}
	
	private void OnConfirm()
	{
		AsUtil.Quit();
	}
}
