using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AutoCombatPotionSpecifier : ScriptableObject
{
	[SerializeField]
	int[] m_PotionIndices; public int[] PotionIndices{get{return m_PotionIndices;}}
	Dictionary<int, int> m_dicIndicies;
	
	void OnEnable()
	{
		m_dicIndicies = new Dictionary<int, int>();
		foreach(int node in m_PotionIndices)
		{
			m_dicIndicies.Add(node, node);
		}
	}
	
	public bool CheckValidPotion(int _idx)
	{
		return m_dicIndicies.ContainsKey(_idx);
	}
}

