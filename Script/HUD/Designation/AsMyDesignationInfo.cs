using UnityEngine;
using System.Collections;

public class AsMyDesignationInfo : MonoBehaviour
{
	[SerializeField] SpriteText name = null;
	[SerializeField] SpriteText require = null;
	[SerializeField] SpriteText effect = null;
	[SerializeField] SpriteText rankPoint = null;
	
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Init( DesignationData data)
	{
		if( null == data)
		{
			name.Text = string.Empty;
			require.Text = string.Empty;
			effect.Text = string.Empty;
			rankPoint.Text = string.Empty;
		}
		else
		{
			name.Text = AsTableManager.Instance.GetTbl_String( data.name);
			require.Text = AsTableManager.Instance.GetTbl_String( data.desc);
			effect.Text = AsTableManager.Instance.GetTbl_String( data.effectDesc);
			rankPoint.Text = data.rankPoint.ToString();
		}
	}
}
