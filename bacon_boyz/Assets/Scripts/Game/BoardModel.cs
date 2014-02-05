using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardModel 
{
	public List<List<GemProxy>> gemProxys;

	public BoardModel()
	{
		gemProxys= new List<List<GemProxy>>();
	} 

	public void SortModel()
	{
		List<GemProxy> bottoms =  GetBottomGems();
		List<GemProxy> sort;

		gemProxys.Clear();

		foreach(GemProxy g in bottoms)
		{
			sort = new List<GemProxy>();
			sort.Add(g);

			Debug.Log("NEW COLUMN "+g.color);

			GemProxy p = g;

			while(p.next != null)
			{
				sort.Add(p.next);
				Debug.Log("ADD "+p.next.color);
				p = p.next;
			}

			gemProxys.Add(sort);
		}
	}

	public GemProxy GetGemAtPosition(int x, int y)
	{
		if(x >= 0 && x < Constants.GEM_AMOUNT_WIDTH)
		{
			List<GemProxy> bottomGems = gemProxys[x];
			if(y >= 0 && y < Constants.GEM_AMOUNT_HEIGHT)
			{
				return bottomGems[y];
			}
		}
		   
		return null;
	}	

	public List<GemProxy> GetBottomGems()
	{
		List<GemProxy> bottomGems = new List<GemProxy>();

		foreach(List<GemProxy> column in gemProxys)
		{
			bottomGems.Add(column[0].GetBottomGem());

		}

		return bottomGems;
	}
}
