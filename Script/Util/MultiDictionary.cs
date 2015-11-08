﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// A MultiMap generic collection class that can store more than one value for a Key.
/// </summary>
/// <typeparam name="Key"></typeparam>
/// <typeparam name="Value"></typeparam>
public class Multimap<Key, Value> : IDisposable where Key:IComparable
{
    private Dictionary<Key, List<Value>> dictMultiMap;
    private bool dispose;
    private object lockThisToDispose = new object();
    private int CurrentItemIndex;
    private Key CurrentKey;

    /// <summary>
    /// Construction of Multi map
    /// </summary>
    public Multimap()
    {
        try
        {
            CurrentKey = default(Key);
            CurrentItemIndex = 0;
            dispose = false;
            dictMultiMap = new Dictionary<Key, List<Value>>();
        }
        catch(Exception ex)
        {
            throw new MultiDictionaryException(ex, ex.Message);
        }
    }

    /// <summary>
    /// Construction copying from another Multi map
    /// </summary>
    /// <param name="DictToCopy"></param>
    public Multimap(ref Multimap<Key, Value> DictToCopy)
    {
        try
        {
            CurrentKey = default(Key);
            CurrentItemIndex = 0;
            dispose = false;
            dictMultiMap = new Dictionary<Key, List<Value>>();

            if (DictToCopy != null)
            {
                Dictionary<Key, List<Value>>.Enumerator enumCopy = DictToCopy.dictMultiMap.GetEnumerator();

                while (enumCopy.MoveNext())
                {
                    List<Value> listValue = new List<Value>();
                    List<Value>.Enumerator enumList = enumCopy.Current.Value.GetEnumerator();

                    while (enumList.MoveNext())
                    {
                        listValue.Add(enumList.Current);
                    }

                    dictMultiMap.Add(enumCopy.Current.Key, listValue);
                }
              }
        }
        catch(Exception ex)
        {
            throw new MultiDictionaryException(ex, ex.Message);   
        }
    }

    /// <summary>
    /// Adds an element to the Multi map.
    /// </summary>
    /// <param name="KeyElement"></param>
    /// <param name="ValueElement"></param>
    public void Add(Key KeyElement, Value ValueElement)
    {
        try
        {
            List<Value> listToAdd = null;
            if (dictMultiMap.TryGetValue(KeyElement, out listToAdd))
            {
                listToAdd.Add(ValueElement);
            }
            else
            {
                listToAdd = new List<Value>();
                listToAdd.Add(ValueElement);
                dictMultiMap.Add(KeyElement, listToAdd);
            }
        }
        catch(Exception ex)
        {
            throw new MultiDictionaryException(ex, ex.Message);
        }
    }

    /// <summary>
    /// Gets the first Item in the Multi map.
    /// </summary>
    /// <param name="ItemToFind"></param>
    /// <returns></returns>
    public Value GetFirstItem(Key ItemToFind)
    {
        Value retVal = default(Value);
        try
        {
            List<Value> listItems = null;

            if (dictMultiMap.TryGetValue(ItemToFind, out listItems))
            {
                if (listItems.Count > 0)
                {
                    retVal = listItems[0];
                    CurrentKey = ItemToFind;
                    CurrentItemIndex = 1;
                }
            }
        }
        catch (Exception ex)
        {
            throw new MultiDictionaryException(ex, ex.Message);
        }

        return retVal;

    }

    /// <summary>
    /// Gets the Next Item in Multi map.  If this method is called first, it returns first item.
    /// </summary>
    /// <param name="ItemToFind"></param>
    /// <returns></returns>
    public Value GetNextItem(Key ItemToFind)
    {
        Value retVal = default(Value);

        try
        {
            List<Value> listItems = null;
            if (dictMultiMap.TryGetValue(ItemToFind, out listItems))
            {
                if (ItemToFind.CompareTo(CurrentKey) != 0)
                {
                    CurrentItemIndex = 0;
                }

                if (CurrentItemIndex < listItems.Count)
                {
                    retVal = listItems[CurrentItemIndex];
                    CurrentItemIndex++;
                    CurrentKey = ItemToFind;
                }
            }
        }
        catch (Exception ex)
        {
            throw new MultiDictionaryException(ex, ex.Message);
        }

        return retVal;
    }

    /// <summary>
    /// Iterates through all the values for the Key one by one.
    /// </summary>
    /// <param name="ItemToFind"></param>
    /// <returns></returns>
    public Value Iterate(Key ItemToFind)
    {
        return GetNextItem(ItemToFind);
    }

    /// <summary>
    /// Removes the Key and all the values for an item.
    /// </summary>
    /// <param name="KeyElement"></param>
    public bool DeleteAll(Key KeyElement)
    {
        bool retVal = false;
        try
        {
            List<Value> listToRemove = null;

            if (dictMultiMap.TryGetValue(KeyElement, out listToRemove))
            {
                listToRemove.Clear();
                dictMultiMap.Remove(KeyElement);
                retVal = true;
            }
        }
        catch (Exception ex)
        {
            throw new MultiDictionaryException(ex, ex.Message);
        }

        return retVal;
    }

    /// <summary>
    /// Deletes one Key and one Value from the Multi map.
    /// </summary>
    /// <param name="KeyElement"></param>
    /// <param name="ValueElement"></param>
    public bool Delete(Key KeyElement, Value ValueElement)
    {
        bool retVal = false;
        try
        {
            List<Value> listToRemove = null;

            if (dictMultiMap.TryGetValue(KeyElement, out listToRemove))
            {
                listToRemove.Remove(ValueElement);

                if (listToRemove.Count == 0)
                {
                    listToRemove = null;
                    dictMultiMap.Remove(KeyElement);
                    retVal = true;
                }
            }

        }
        catch(Exception ex)
        {
            throw new MultiDictionaryException(ex, ex.Message);
        }

        return retVal;
    }


    /// <summary>
    /// Disposes the Keys and Values.  Useful in case if unmanaged resources are stored here.
    /// </summary>

    public void Dispose()
    {
        lock (lockThisToDispose)
        {
            if (dispose == false)
            {
                dispose = true;
                Dictionary<Key, List<Value>>.Enumerator enumDictElements = dictMultiMap.GetEnumerator();

                while (enumDictElements.MoveNext())
                {
                    try
                    {
                        IDisposable disposeObj = (IDisposable)enumDictElements.Current.Key;

                        if (null != disposeObj)
                        {
                            disposeObj.Dispose();
                        }
                    }
                    catch (Exception)
                    { // Object not disposable
                    }

                    List<Value>.Enumerator enuValue = enumDictElements.Current.Value.GetEnumerator();
                    while (enuValue.MoveNext())
                    {
                        try
                        {
                            IDisposable disposeObj = (IDisposable)enuValue.Current;

                            if (null != disposeObj)
                            {
                                disposeObj.Dispose();
                            }
                        }
                        catch (Exception)
                        {
                            // object not disposable
                        }
                    }

                    enumDictElements.Current.Value.Clear();
                }

                dictMultiMap.Clear();

            }
        }
    }

    /// <summary>
    /// Finalizer
    /// </summary>
    ~Multimap()
    {
        Dispose();
    }
}

public class MultiDictionary<TKey, TValue>// : IEnumerable
{
    public MultiDictionary()
    {
        dic = new SortedDictionary<TKey, List<TValue>>();
    }

    public MultiDictionary(IComparer<TKey> comparer)
    {
        dic = new SortedDictionary<TKey, List<TValue>>(comparer);
    }

    public void Add(TKey key, TValue value)
    {
        List<TValue> list;

        if (dic.TryGetValue(key, out list))
        {
            list.Add(value);
        }
        else
        {
            list = new List<TValue>();
            list.Add(value);

            dic.Add(key, list);
        }
    }
	
	public void Remove(TKey key, TValue value)
    {
        List<TValue> list;

        if (dic.TryGetValue(key, out list))
        {
            list.Remove(value);
        }
//        else
//        {
//            list = new List<TValue>();
//            list.Add(value);
//
//            dic.Add(key, list);
//        }
		
		if(list.Count == 0)
			dic.Remove(key);
    }
	
	public void Clear()
	{
		foreach( KeyValuePair<TKey, List<TValue>> it in dic )
			it.Value.Clear();
		
		dic.Clear();
	}

    public bool ContainsKey(TKey key)
    {
        return dic.ContainsKey(key);
    } 

    public IEnumerable<TKey> Keys
    {
        get
        {
            return this.dic.Keys;
        }
    }

    public List<TValue> this[TKey key]
    {
        get
        {
            List<TValue> list;

            if (this.dic.TryGetValue(key, out list))
            {
                return list;
            }
            else
            {
                return new List<TValue>();
            }
        }
    }
	
//	public IEnumerator GetEnumerator()
//	{
//		return new MyEnumerator(this);
//	}
//	
//	private class MyEnumerator : IEnumerator
//	{
//		TKey m_CurKey;
//		TValue m_CurValue;
//		MultiDictionary<TKey, TValue> m_Mdic;
//		
//		public MyEnumerator(MultiDictionary<TKey, TValue> _mdic)
//		{
//			m_Mdic = _mdic;
//		}
//		
//		public bool MoveNext()
//		{
////			m_Mdic.Keys.GetEnumerator m_CurKey
//			
//			return true;
//		}
//		
//		public void Reset()
//		{
//		}
//		
//		public object Current{get{return m_Mdic;}}
//	}

    private SortedDictionary<TKey, List<TValue>> dic = null;
}  