using UnityEngine;
using System.Collections;




public class dtNodePool
{
	private dtNode[] m_nodes;
	private ushort[] m_first;
	private ushort[] m_next;
	private int m_maxNodes;
	private int m_hashSize;
	private int m_nodeCount=0;
	
	
	public dtNodePool(int maxNodes, int hashSize)
	{
		m_maxNodes = maxNodes;
		m_hashSize = hashSize;
		
		m_nodes = new dtNode[m_maxNodes];
		m_next = new ushort[m_maxNodes];		
		m_first = new ushort[m_hashSize];
		
		for( int i=0; i<m_maxNodes; ++i )
		{
			m_next[i] = 0xff;
			m_nodes[i] = new dtNode();
			m_nodes[i].index = (uint)i;
		}
		
		for( int i=0; i<m_hashSize; ++i )
		{
			m_first[i] = 0xff;
		}		
	}	
	
	public void clear()
	{
		for( int i=0; i<m_hashSize; ++i )
		{
			m_first[i] = 0xff;
		}	
		m_nodeCount = 0;
	}
	
	
	public dtNode findNode(uint id)
	{
		uint bucket = (uint)(hashint(id) & (m_hashSize-1));
		ushort i = m_first[bucket];
		while (i != 0xff)
		{
			if (m_nodes[i].id == id)
				return m_nodes[i];
			i = m_next[i];
		}
		return null;
	}
	
	
	public dtNode getNode(uint id)
	{
		uint bucket = (uint)(hashint(id) & (m_hashSize-1));
		ushort i = m_first[bucket];
		dtNode node = null;
		while (i != 0xff)
		{
			if (m_nodes[i].id == id)
				return m_nodes[i];
			i = m_next[i];
		}
		
		if (m_nodeCount >= m_maxNodes)
			return null;
		
		i = (ushort)m_nodeCount;
		m_nodeCount++;
		
		// Init node
		node = m_nodes[i];
		node.pidx = 0;
		node.cost = 0;
		node.total = 0;
		node.id = id;
		node.flags = 0;
		
		m_next[i] = m_first[bucket];
		m_first[bucket] = i;
		
		return node;
	}	
	

	public uint getNodeIdx(dtNode node)
	{
		if (null == node) 
			return 0;
		
		return node.index+1;
	}

	public dtNode getNodeAtIdx(uint idx)
	{
		if (0 == idx) 
			return null;
		
		return m_nodes[idx-1];
	}	

	private uint hashint(uint a)
	{
		a += ~(a<<15);
		a ^=  (a>>10);
		a +=  (a<<3);
		a ^=  (a>>6);
		a += ~(a<<11);
		a ^=  (a>>16);
		return a;
	}
	
};