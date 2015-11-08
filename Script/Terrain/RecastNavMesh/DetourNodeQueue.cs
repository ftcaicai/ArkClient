using UnityEngine;
using System.Collections;

class dtNodeQueue
{	
	private dtNode[] m_heap;
	private int m_capacity;
	private int m_size;	
	

	public dtNodeQueue(int n)
	{
		m_capacity = n;			
		m_heap = new dtNode[m_capacity+1];
	}	
	
	public void clear()
	{
		m_size = 0;
	}
	
	public dtNode top()
	{
		return m_heap[0];
	}
	
	public dtNode pop()
	{
		dtNode result = m_heap[0];
		m_size--;
		trickleDown(0, m_heap[m_size]);
		return result;
	}
	
	public void push(dtNode node)
	{
		m_size++;
		bubbleUp(m_size-1, node);
	}
	
	public void modify(dtNode node)
	{
		for (int i = 0; i < m_size; ++i)
		{
			if (m_heap[i] == node)
			{
				bubbleUp(i, node);
				return;
			}
		}
	}
	
	public bool empty()
	{ 
		return m_size == 0; 
	}
	
	/*int getMemUsed()
	{
		return sizeof(*this) +sizeof(dtNode*)*(m_capacity+1);
	}*/	

	private void bubbleUp(int i, dtNode node)
	{
		int parent = (i-1)/2;
		// note: (index > 0) means there is a parent
		while ((i > 0) && (m_heap[parent].total > node.total))
		{
			m_heap[i] = m_heap[parent];
			i = parent;
			parent = (i-1)/2;
		}
		m_heap[i] = node;
	}
	
	private void trickleDown(int i, dtNode node)
	{
		int child = (i*2)+1;
		while (child < m_size)
		{
			if (((child+1) < m_size) && 
				(m_heap[child].total > m_heap[child+1].total))
			{
				child++;
			}
			m_heap[i] = m_heap[child];
			i = child;
			child = (i*2)+1;
		}
		bubbleUp(i, node);
	}
	
}	

