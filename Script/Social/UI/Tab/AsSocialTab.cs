using UnityEngine;
using System.Collections;


public abstract class AsSocialTab : MonoBehaviour
{
	protected bool m_isClone = false;

	public bool  IsClone
	{
		get { return m_isClone; }
		set { m_isClone = value; }
	}

	abstract public void Init();
	public void SetVisible( bool flag)
	{
		gameObject.SetActiveRecursively( flag);
	}

	abstract public UIScrollList getList();
}

