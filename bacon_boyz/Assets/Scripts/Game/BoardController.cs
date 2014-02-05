using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BoardController : MonoBehaviour 
{
	private BoardModel boardModel;

	void Start()
	{
		SetupModel();
		InitBoard();
	}
		
	public void OnTap( TapGesture gesture)
	{
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint (new Vector3 (gesture.Position.x, gesture.Position.y, 0f));
		int x = (int)(worldPoint.x / Constants.GEM_UNIT_DIMENSION);
		int y = (int)(worldPoint.y / Constants.GEM_UNIT_DIMENSION);

		Debug.Log("tap "+gesture.Position +" "+gesture.Selection+" "+worldPoint + " -> "+x +" "+ y );

		GemModel gem = boardModel.GetGemAtPosition(x, y);
		if(gem != null)
		{

		}

		List<GemModel> matches = FindMatchingGemsForPosition(x,y);

		foreach(GemModel found in matches)
		{
			SpriteRenderer renderer = found.gameObject.GetComponent<SpriteRenderer>();
			renderer.color = Color.white;
		}

	}

	private List<GemModel> FindMatchingGemsForPosition(int x, int y)
	{
		Stack<Vector2> stack = new Stack<Vector2>();
		HashSet<Vector2> visited = new HashSet<Vector2>();
		stack.Push(new Vector2(x,y));

		Vector2 top; 
		List<GemModel> matches = new List<GemModel>();

		GemModel origin = boardModel.GetGemAtPosition(x, y);
		if(origin != null)
		{
			matches.Add(origin);
		}
		GemModel gem;

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

		GameObject root = new GameObject();
		root.transform.parent = this.transform;

		for(int i = 0; i < Constants.GEM_AMOUNT_WIDTH; i++ )
		{
			for(int j = 0; j < Constants.GEM_AMOUNT_HEIGHT; j++ )
			{
				GameObject gem = Instantiate(resource) as GameObject;

				GemModel model = gem.AddComponent<GemModel>();
				model.color = RandomColor();

				SpriteRenderer renderer = gem.GetComponent<SpriteRenderer>();
				renderer.color = GetColorForGem(model.color);

				gem.transform.parent = root.transform;
				gem.transform.position = new Vector3(i * Constants.GEM_UNIT_DIMENSION, j * Constants.GEM_UNIT_DIMENSION, 0 );

				boardModel.gems.Add(model);
			}
		}

		//TODO center root
	}


	private Color GetColorForGem(GemColors gem)
	{
		Color gemColor = Color.white;

		switch(gem)
		{
		case GemColors.RED:
			gemColor = Color.red;
			break;
		case GemColors.GREEN:
			gemColor = Color.green;
			break;
		case GemColors.BLUE:
			gemColor = Color.blue;
			break;
		case GemColors.PURPLE:
			gemColor = Color.magenta;
			break;
		case GemColors.YELLOW:
			gemColor = Color.yellow;
			break;
		}

		return gemColor;
	}


	private System.Random random = new System.Random();
	private GemColors RandomColor()
	{
		var values = Enum.GetValues(typeof(GemColors));
		return (GemColors)values.GetValue(random.Next(values.Length));
	}
}
