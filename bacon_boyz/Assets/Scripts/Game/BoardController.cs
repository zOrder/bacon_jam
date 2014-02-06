using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BoardController : MonoBehaviour 
{
	private BoardModel boardModel;

	public string turns = "20";
	public string health = "50";
	public Canon canonBehaviour;

	private int remainingTurns = Constants.TURNS_PER_GAME;
	private int remainingHealth = Constants.TURNS_PER_GAME;
	private GameObject root;

	void Start()
	{
		SetupModel();
		InitBoard();
		InitCanons();
		DropGems();
		UpdateTurns();
		UpdateHealth();
		SpawnBlocker();
	}
		
	public void OnTap( TapGesture gesture)
	{
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint (new Vector3 (gesture.Position.x, gesture.Position.y, 0f));
		worldPoint.x -= root.transform.position.x;
		worldPoint.y -= root.transform.position.y;

		int x = (int)(worldPoint.x / Constants.GEM_UNIT_DIMENSION );
		int y = (int)(worldPoint.y / Constants.GEM_UNIT_DIMENSION );

		Debug.Log("tap "+gesture.Position +" "+gesture.Selection+" "+worldPoint + " -> "+x +" "+ y );

		List<GemProxy> matches = FindMatchingGemsForPosition(x,y);

		if(matches.Count >= Constants.MIN_MATCH_SIZE)
		{
			int canonsFired = EvaluateCanons(matches);
			MoveMatchesOffscreen(matches);
			boardModel.SortModel();
			
			DropGems();
			
			remainingTurns --;
			UpdateTurns();
			UpdateHealth();
			MoveBlocker();
			SpawnBlocker();
			SpawnNewCanons(canonsFired);
		}
	}

	private void SpawnBlocker()
	{
		if(remainingTurns % 2 == 0 )
		{
			GemProxy blocker = boardModel.GetRandomTopGem();
			blocker.MakeBlocker();
		}
	}

	private void MoveBlocker()
	{
		List<GemProxy> topGems = boardModel.GetTopGems();
		foreach(GemProxy gem in topGems)
		{
			gem.MoveBlockerDown();
		}
	}

	private int EvaluateCanons(List<GemProxy> matches)
	{
		List<GemProxy> canons = new List<GemProxy>();
		foreach(GemProxy found in matches)
		{
			if(found.type.Equals(GemType.CANON))
			{
				canons.Add(found);
			}
		}

		int shots = 0;
		foreach(GemProxy canon in canons)
		{
			GemProxy blocker = canon.GetCanonBlocker();
			if(blocker != null)
			{
				blocker.ResetType();
				canonBehaviour.ShootFromTo(canon.transform.position, blocker.transform.position);
			}
			else
			{
				canonBehaviour.ShootFromTo(canon.transform.position, new Vector3(canon.transform.position.x, 10f, canon.transform.position.z));
				shots ++;
			}
		}

		remainingHealth -= shots * matches.Count;

		return canons.Count;
	}

	private void MoveMatchesOffscreen(List<GemProxy> matches)
	{
		foreach(GemProxy found in matches)
		{
			GemProxy top = found.GetTopGem();
			
			if(found.prev != null)
			{
				found.prev.next = found.next;
				top  = found.prev.GetTopGem();
			}
			if(found.next != null)
			{
				found.next.prev = found.prev;
			}
			
			top.next = found;
			found.prev = top;
			found.next = null;
			
			found.SetOffscreen();
			found.ResetType();
		}
	}

	private void UpdateTurns()
	{
		turns = "Turns left " + remainingTurns;
	}

	private void UpdateHealth()
	{
		health = "health " + remainingHealth;
	}
	
	private List<GemProxy> FindMatchingGemsForPosition(int x, int y)
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

	private void SetupModel()
	{
		boardModel = new BoardModel();
	}

	private void InitBoard()
	{
		GameObject resource = Resources.Load("Gem") as GameObject;

		root = new GameObject();
		root.transform.parent = this.transform;

		for(int i = 0; i < Constants.GEM_AMOUNT_WIDTH; i++ )
		{
			GemProxy prevGem = null;

			List<GemProxy> column = new List<GemProxy>();

			for(int j = 0; j < Constants.GEM_AMOUNT_HEIGHT; j++ )
			{
				GameObject gem = Instantiate(resource) as GameObject;

				GemProxy gemProxy = gem.AddComponent<GemProxy>();

				if(prevGem != null)
				{
					prevGem.next = gemProxy;
				}
				gemProxy.prev = prevGem;
				prevGem = gemProxy;

				gem.transform.parent = root.transform;
				gem.transform.position = new Vector3(i * Constants.GEM_UNIT_DIMENSION, Constants.OFFSCREEN_POSITION_Y , 0 );

				column.Add(gemProxy);
			}

			boardModel.gemProxys.Add(column);
		}

		root.transform.position = new Vector3(Constants.GEM_UNIT_DIMENSION / 2, Constants.GEM_UNIT_DIMENSION, 10);
	}

	private void InitCanons()
	{
		SpawnNewCanons(Constants.NUMBER_OF_CANONS);
	}

	private void SpawnNewCanons(int amount)
	{
		List<GemProxy> canons = boardModel.GetRandomCanons(amount);
		foreach(GemProxy canon in canons)
		{
			canon.MakeCanon();
		}
	}

	private void DropGems()
	{
		List<GemProxy> bottomGems = boardModel.GetBottomGems();
		foreach(GemProxy p in bottomGems)
		{
			p.MoveDown();
		}
	}
}
