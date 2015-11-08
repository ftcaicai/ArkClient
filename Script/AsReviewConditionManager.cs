


#define _USE_REVIEW_CONDITION



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eREVIEW_CONDITION
{
	PROMOTION,
	MARKETING_BANNER,
	PLATFORM_VIEW,
	FRIEND_REWARD,
	RECOMMEND_REWARD,
	REVIEW_REWARD,
	COUPON,
	SOCIAL_REWARD,
}

public class AsReviewConditionManager : MonoBehaviour 
{
	static AsReviewConditionManager m_instance;
	public static AsReviewConditionManager Instance{ get{  return m_instance;}}

	private Dictionary<eREVIEW_CONDITION, bool>			m_dicCondition		= new Dictionary<eREVIEW_CONDITION,bool>();

	void Awake()
	{
		m_instance = this;

		m_dicCondition.Add (eREVIEW_CONDITION.PROMOTION, true);
		m_dicCondition.Add (eREVIEW_CONDITION.MARKETING_BANNER, true);
		m_dicCondition.Add (eREVIEW_CONDITION.PLATFORM_VIEW, true);
		m_dicCondition.Add (eREVIEW_CONDITION.FRIEND_REWARD, true);
		m_dicCondition.Add (eREVIEW_CONDITION.RECOMMEND_REWARD, true);
		m_dicCondition.Add (eREVIEW_CONDITION.REVIEW_REWARD, true);
		m_dicCondition.Add (eREVIEW_CONDITION.COUPON, true);
		m_dicCondition.Add (eREVIEW_CONDITION.SOCIAL_REWARD, true);
	}

	// Use this for initialization
	void Start () 
	{
			
	}

	public void SetReviewCondition( eREVIEW_CONDITION condition , bool enable )
	{
		if (m_dicCondition.ContainsKey (condition) == false) 
		{
			m_dicCondition.Add( condition , enable );
			return;
		}

		m_dicCondition [condition] = enable;
	}

	public bool IsReviewCondition( eREVIEW_CONDITION condition )
	{
		#if _USE_REVIEW_CONDITION
		return	m_dicCondition[condition];
		#endif

		return true;
	}

	public void SetCondition_LoginResult(AS_LC_LOGIN_RESULT result)
	{
		SetReviewCondition (eREVIEW_CONDITION.PROMOTION, result.bIOS_Promotion);
		SetReviewCondition (eREVIEW_CONDITION.MARKETING_BANNER, result.bIOS_Marketing_Banner);
		SetReviewCondition (eREVIEW_CONDITION.PLATFORM_VIEW, result.bIOS_Platform);
		SetReviewCondition (eREVIEW_CONDITION.FRIEND_REWARD, result.bIOS_Friend_Reward);
		SetReviewCondition (eREVIEW_CONDITION.RECOMMEND_REWARD, result.bIOS_Recommend_Reward);
		SetReviewCondition (eREVIEW_CONDITION.REVIEW_REWARD, result.bIOS_Review_Reward);
		SetReviewCondition (eREVIEW_CONDITION.COUPON, result.bIOS_Coupon);
		SetReviewCondition (eREVIEW_CONDITION.SOCIAL_REWARD, result.bIOS_Social_Reward);
	}

	public void SetCondition_WemeLoginResult(body_LC_WEMELOGIN_RESULT result)
	{
		SetReviewCondition (eREVIEW_CONDITION.PROMOTION, result.bIOS_Promotion);
		SetReviewCondition (eREVIEW_CONDITION.MARKETING_BANNER, result.bIOS_Marketing_Banner);
		SetReviewCondition (eREVIEW_CONDITION.PLATFORM_VIEW, result.bIOS_Platform);
		SetReviewCondition (eREVIEW_CONDITION.FRIEND_REWARD, result.bIOS_Friend_Reward);
		SetReviewCondition (eREVIEW_CONDITION.RECOMMEND_REWARD, result.bIOS_Recommend_Reward);
		SetReviewCondition (eREVIEW_CONDITION.REVIEW_REWARD, result.bIOS_Review_Reward);
		SetReviewCondition (eREVIEW_CONDITION.COUPON, result.bIOS_Coupon);
		SetReviewCondition (eREVIEW_CONDITION.SOCIAL_REWARD, result.bIOS_Social_Reward);
	}

	void DisplayConditionStateLog()
	{
		Debug.LogError ("review_condition PROMOTION : " + IsReviewCondition (eREVIEW_CONDITION.PROMOTION));
		Debug.LogError ("review_condition MARKETING_BANNER : " + IsReviewCondition (eREVIEW_CONDITION.MARKETING_BANNER));
		Debug.LogError ("review_condition PLATFORM_VIEW : " + IsReviewCondition (eREVIEW_CONDITION.PLATFORM_VIEW));
		Debug.LogError ("review_condition FRIEND_REWARD : " + IsReviewCondition (eREVIEW_CONDITION.FRIEND_REWARD));
		Debug.LogError ("review_condition RECOMMEND_REWARD : " + IsReviewCondition (eREVIEW_CONDITION.RECOMMEND_REWARD));
		Debug.LogError ("review_condition REVIEW_REWARD : " + IsReviewCondition (eREVIEW_CONDITION.REVIEW_REWARD));
		Debug.LogError ("review_condition COUPON : " + IsReviewCondition (eREVIEW_CONDITION.COUPON));
		Debug.LogError ("review_condition SOCIAL_REWARD : " + IsReviewCondition (eREVIEW_CONDITION.SOCIAL_REWARD));
	}

	// Update is called once per frame
	void Update () {
	
	}
}
