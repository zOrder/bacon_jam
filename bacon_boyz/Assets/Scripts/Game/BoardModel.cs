using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardModel 
{
	public List<GemModel> gems;

	public BoardModel()
	{
		gems = new List<GemModel>();
	} 

	public GemModel GetGemAtPosition(int x, int y)
	{
		if(x >= 0 && x < Constants.GEM_AMOUNT_WIDTH &&
		   y >= 0 && y < Constants.GEM_AMOUNT_HEIGHT)
		{
			int index = x * Constants.GEM_AMOUNT_HEIGHT+ y;
			
			if(index >= 0 && index < gems.Count)
			{
				return gems[index];
			}
		}
		return null;
	}	
}
