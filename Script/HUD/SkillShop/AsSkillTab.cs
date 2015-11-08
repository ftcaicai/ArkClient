using UnityEngine;
using System.Collections;

public abstract class AsSkillTab : MonoBehaviour
{
	abstract public void Init( int npcID, bool flag=false);
//	abstract public void InsertSkillInfo();
	  
	abstract public UIScrollList getList();
	virtual public void PromptTooltipBySkillID( int id, int level)		{}
}
