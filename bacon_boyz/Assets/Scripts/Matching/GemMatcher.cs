using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GemMatcher  
{
	private BoardModel boardModel;

	public GemMatcher(BoardModel boardModel)
	{
		this.boardModel = boardModel;
	}

	public GemProxy GetRandomNeighbour(int x, int y)
	{
		List<GemProxy> neighbours = GetNeighbours(x, y);

		return neighbours[Random.Range(0, neighbours.Count -1)];
	}

	public bool HasMatchingNeighbours(int x, int y)
	{
		GemProxy origin = boardModel.GetGemAtPosition(x, y);
		
		List<GemProxy> neighbours = GetNeighbours(x, y);

		foreach(GemProxy n in neighbours)
		{
			if(n.color.Equals(origin.color))
			{
				return true;
			}
		}

		return false;
	}

	private List<GemProxy> GetNeighbours(int x, int y)
	{
		List<GemProxy> neighbours = new List<GemProxy>();
		
		GemProxy n = boardModel.GetGemAtPosition(x - 1, y);
		if(n != null ) neighbours.Add(n);
		
		n = boardModel.GetGemAtPosition(x + 1, y);
		if(n != null ) neighbours.Add(n);
		
		n = boardModel.GetGemAtPosition(x, y - 1);
		if(n != null ) neighbours.Add(n);
		
		n = boardModel.GetGemAtPosition(x, y + 1);
		if(n != null ) neighbours.Add(n);

		return neighbours;
	}
	
	public List<GemProxy> FindMatchingGemsForPosition(int x, int y)
	{
		Stack<Vector2> stack = new Stack<Vector2>();
		HashSet<Vector2> visited = new HashSet<Vector2>();
		stack.Push(new Vector2(x,y));
		
		Vector2 top; 
		List<GemProxy> matches = new List<GemProxy>();
		
		GemProxy origin = boardModel.GetGemAtPosition(x, y);
		if(origin != null)
		{
			matches.Add(origin);
		}
		GemProxy gem;
		
		while(stack.Count >0)
		{
			top = stack.Pop();
			
			if(visited.Contains(top) == false)
			{
				visited.Add(top);
				
				gem = boardModel.GetGemAtPosition((int)top.x, (int)top.y);
				
				if(gem != null && gem.color == origin.color)
				{
					if(matches.Contains(gem) == false)
					{
						matches.Add(gem);
					}
					
					stack.Push (new Vector2 (top.x - 1, top.y));
					stack.Push (new Vector2 (top.x + 1, top.y));
					stack.Push (new Vector2 (top.x, top.y + 1));
					stack.Push (new Vector2 (top.x, top.y - 1));
				}
				
			}
		}
		
		return matches;
	}
}
