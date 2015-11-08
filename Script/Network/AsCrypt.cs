using UnityEngine;
using System;
using System.Collections;

public class AsCrypt
{
	const int PACKETHEADERSIZE = 4;
	const int m_nC1 = 52845;
	const int m_nC2 = 22719;
	const int m_nKey = 72957;

	byte[] m_EncryptData = new byte[ (int)AsBaseClass.ePackingLength.PL_MAX ];
	byte[] m_DecryptData = new byte[ (int)AsBaseClass.ePackingLength.PL_MAX ];

	public  byte[] Encrypt( byte[] pSour)
	{
		int nKey = m_nKey;
		Array.Clear( m_EncryptData, 0, (int)AsBaseClass.ePackingLength.PL_MAX);

		for( int i = 0; i < pSour.Length ; i++)
		{
			if( i < PACKETHEADERSIZE)
			{
				m_EncryptData[i] =  pSour[i];
			}
			else
			{
				m_EncryptData[i] = (byte)( pSour[i] ^ ( nKey >> 8));
				nKey = ( m_EncryptData[i] + nKey) * m_nC1 + m_nC2;
			}
		}

		return m_EncryptData;
	}

	public byte[] Decrypt( byte[] pSour)
	{
		byte prev = 0;
		int nKey = m_nKey;

		Array.Clear( m_DecryptData, 0, (int)AsBaseClass.ePackingLength.PL_MAX);

		for( int i = 0; i < pSour.Length; i++)
		{
			if( i < PACKETHEADERSIZE)
			{
				m_DecryptData[i] = pSour[i];
			}
			else
			{
				prev = pSour[i];
				m_DecryptData[i] = (byte)( pSour[i] ^ ( nKey >> 8));
				nKey = ( prev + nKey) * m_nC1 + m_nC2;
			}
		}

		return m_DecryptData;
	}
}
