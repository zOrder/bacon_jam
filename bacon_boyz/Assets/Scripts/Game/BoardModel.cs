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
			GemProxy p = g;

			while(p.next != null)
			{
				sort.Add(p.next);
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

	public GemProxy GetRandomTopGem()
	{
		List<GemProxy> topGems = GetTopGems();
		return topGems[Random.Range(0, topGems.Count-1)];
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

	public List<GemProxy> GetTopGems()
	{
		List<GemProxy> topGems = new List<GemProxy>();
		
		foreach(List<GemProxy> column in gemProxys)
		{
			topGems.Add(column[0].GetTopGem());			
		}
		
		return topGems;
	}

	public List<GemProxy> GetRandomCanons()
	{
		List<GemProxy> canons = new List<GemProxy>();
		for(var i = 0; i< Constants.NUMBER_OF_CANONS; i++)
		{
			canons.Add(GetRandomGem());
		}
		return canons;
	}

	private GemProxy GetRandomGem()
	{
		int colum = Random.Range(0, Constants.GEM_AMOUNT_WIDTH);
		int row = Random.Range(0, Constants.GEM_AMOUNT_HEIGHT);
		return gemProxys[colum][row];
	}
}
