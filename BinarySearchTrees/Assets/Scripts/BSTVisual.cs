using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSTVisual {

	private int _key = 0;
	private List<BSTVisualItem> _items = new List<BSTVisualItem>();

	public BSTVisual()
	{
	}

	public int Key
	{
		get
		{
			return _key;
		}

		set
		{
			_key = value;
		}
	}

	public List<BSTVisualItem> Items
	{
		get
		{
			return _items;
		}

		set
		{
			_items = value;
		}
	}

	public void AddItem(BSTVisualItem item)
	{
		_items.Add(item);
	}

	public void ClearItems()
	{
		_items.Clear();
	}
}
