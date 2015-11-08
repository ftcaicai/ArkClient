using UnityEngine;
using System.Collections;

public enum NOTIFY_MSG_TYPE
{
	NM_INVALID = -1,
	BM_CLICKED,
	BM_DBL_CLICKED,
	SU_UNLOCKED,

	NOTIFY_MSG_MAX
}

public struct NOTIFY_MSG
{
	public int id;
	public NOTIFY_MSG_TYPE type;
}

public abstract class AsCtrl : MonoBehaviour
{
	public int id = -1;

	// Use this for initialization
	void Start()
	{
	}
	// Update is called once per frame
	void Update()
	{
	}
}
