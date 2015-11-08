using UnityEngine;
using System.Collections;
using System.Xml;


public class AgreementsData : AsTableRecord
{
	public string agreements = null;
	public string agreementsEx = null;
	public string policy = null;
	
	public AgreementsData( XmlElement _element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;
			
			SetValue( ref agreements, node, "Agreements");
			SetValue( ref agreementsEx, node, "AgreementsEx");
			SetValue( ref policy, node, "Policy");
		}
		catch( System.Exception e)
		{
			Debug.LogError(e);
		}
	}
}


public class AsUsePersonalInfoAgreement : MonoBehaviour
{
	public UIScrollList userAgreements = null;
	public UIScrollList handlingPolicy = null;
	public UIRadioBtn rdoUserAgreements = null;
	public UIRadioBtn rdoHandlingPolicy = null;
	public GameObject listItem = null;
	[HideInInspector] public bool agreementsFlag = false;
	[HideInInspector] public bool policyFlag = false;
	private AgreementsData docuData = null;
	
	// Use this for initialization
	void Start()
	{
		LoadDocument();
		
		userAgreements.CreateItem( listItem, docuData.agreements);
		userAgreements.CreateItem( listItem, docuData.agreementsEx);
		handlingPolicy.CreateItem( listItem, docuData.policy);
		
		rdoUserAgreements.Value = false;
		rdoHandlingPolicy.Value = false;
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	private void OnAgreementsBtn()
	{
		agreementsFlag = !agreementsFlag;
		rdoUserAgreements.Value = agreementsFlag;
	}
	
	private void OnPolicyBtn()
	{
		policyFlag = !policyFlag;
		rdoHandlingPolicy.Value = policyFlag;
	}
	
	private void LoadDocument()
	{
		try
		{
		    XmlElement root = AsTableBase.GetXmlRootElement( "Table/AgreementsPolicy");
		    XmlNodeList nodes = root.ChildNodes;

			foreach( XmlNode node in nodes)
		    {
				docuData = new AgreementsData( (XmlElement)node);
		    }
		}
		catch( System.Exception e)
		{
			Debug.LogError(e);//$yde
			AsUtil.ShutDown( "AsUsePersonalInfoAgreement:LoadDocument");
		}
	}
}
