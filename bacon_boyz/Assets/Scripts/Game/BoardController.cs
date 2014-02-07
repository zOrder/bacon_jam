using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BoardController : MonoBehaviour 
{
	private GameObject root;
	private BoardModel boardModel;

	public string turns = "20";
	public string health = "50";
	private Canon canonBehaviour;

	private int remainingTurns = Constants.TURNS_PER_GAME;
	private int remainingHealth = Constants.TURNS_PER_GAME;

	private List<InvaderProxy> invaders = new List<InvaderProxy>();
	private List<InvaderProxy> invaderPool = new List<InvaderProxy>();

	private float spawnDelay = 15f;
	private GemMatcher matcher;

	void Start()
	{
		SetupModel();
		InitBoard();
		SetupCanon();
		DropGems();
		UpdateTurns();
		UpdateHealth();

		matcher = new GemMatcher(boardModel);

		StartCoroutine(SpawnInvader());
	}

	IEnumerator SpawnInvader()
	{
		while(true)
		{
			yield return new WaitForSeconds(spawnDelay);

			UpdateSpawnDelay();
			SetInvader();
			UpdateSpawnDelay();
			SetInvader();
			UpdateSpawnDelay();
			SetInvader();
		}
	}

	private void UpdateSpawnDelay()
	{
		spawnDelay = Math.Max(7.5f, spawnDelay - 0.05f);
	}

	private void SetInvader()
	{
		InvaderProxy invader = GetInvader();
		int randomGridX = UnityEngine.Random.Range(1, Constants.GEM_AMOUNT_WIDTH);
		invader.SetNewGridPosition(randomGridX, Constants.GEM_AMOUNT_HEIGHT);
		invader.delayBetweenMoves = 4f;
		invader.movementType = InvaderProxy.MovementType.DROP;
		invader.StartMoving();

		invaders.Add(invader);
	}
		
	public void OnTap( TapGesture gesture)
	{
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint (new Vector3 (gesture.Position.x, gesture.Position.y, 0f));
		worldPoint.x -= root.transform.position.x;
		worldPoint.y -= root.transform.position.y;

		int x = (int)(worldPoint.x / Constants.GEM_UNIT_DIMENSION );
		int y = (int)(worldPoint.y / Constants.GEM_UNIT_DIMENSION );

		Debug.Log("tap "+gesture.Position +" "+gesture.Selection+" "+worldPoint + " -> "+x +" "+ y );

		List<GemProxy> matches = matcher.FindMatchingGemsForPosition(x,y);

		if(matches.Count >= Constants.MIN_MATCH_SIZE)
		{
			EvaluateShot(matches, x , y);
			MoveMatchesOffscreen(matches);
			boardModel.SortModel();
			
			DropGems();
			
			remainingTurns --;
			UpdateTurns();
			UpdateHealth();
		}
		if(ShouldFindOrphans())
		{
			FindOrphanedGems();
		}
	}

	private bool ShouldFindOrphans()
	{
		return UnityEngine.Random.Range(1, 3) % 2 == 0;
	}

	private void EvaluateShot(List<GemProxy> matches, int x, int y)
	{
		foreach(InvaderProxy invader in invaders)
		{
			if(invader.GridX == x+1)
			{
				invader.OnHit();
				canonBehaviour.ShootFromTo(ConvertGridToBoard(new Vector2(x, y)), invader.GridY * Constants.GEM_UNIT_DIMENSION + root.transform.position.y );

				if(invader.healthPoints <= 0)
				{
					invader.DieDieDie();
					invader.gameObject.SetActive(false);
					
					invaders.Remove(invader);
					invaderPool.Add(invader);
				}

				return; 
			}
		}

		canonBehaviour.ShootFromTo(ConvertGridToBoard(new Vector2(x, y)), 10f);

		remainingHealth -= matches.Count;
	}

	private Vector2 ConvertGridToBoard(Vector2 gridPos)
	{
		return new Vector2(gridPos.x * Constants.GEM_UNIT_DIMENSION + root.transform.position.x, gridPos.y * Constants.GEM_UNIT_DIMENSION + root.transform.position.y);
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

	private void SetupCanon()
	{
		canonBehaviour = root.AddComponent<Canon>();
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

				GemProxy gemProxy = gem.GetComponent<GemProxy>();

				if(prevGem != null)
				{
					prevGem.next = gemProxy;
				}
				gemProxy.prev = prevGem;
				prevGem = gemProxy;

				gem.transform.parent = root.transform;
				gem.transform.position = new Vector3(i * Constants.GEM_UNIT_DIMENSION, Constants.OFFSCREEN_POSITION_Y , 1 );

				column.Add(gemProxy);
			}

			boardModel.gemProxys.Add(column);
		}

		root.transform.position = new Vector3(Constants.GEM_UNIT_DIMENSION / 2, Constants.GEM_UNIT_DIMENSION, 0);
	}

	private void DropGems()
	{
		List<GemProxy> bottomGems = boardModel.GetBottomGems();
		foreach(GemProxy p in bottomGems)
		{
			p.MoveDown();
		}
	}

	private void FindOrphanedGems()
	{
		for(int i = 0; i< Constants.GEM_AMOUNT_WIDTH; i++)
		{
			for(int j = 0; j< Constants.GEM_AMOUNT_HEIGHT / 2; j++)
			{
				if(matcher.HasMatchingNeighbours(i, j) == false)
				{
					GemProxy origin  = boardModel.GetGemAtPosition(i, j);
					GemProxy neighbour = matcher.GetRandomNeighbour(i, j);

					origin.UpdateColorTo(neighbour.color);

					return;
				}
			}
		}
	}

	private InvaderProxy GetInvader()
	{
		InvaderProxy invader = null;
		
		if(invaderPool.Count > 0)
		{
			invader = invaderPool[0];
			invaderPool.RemoveAt(0);
		}
		else
		{
			GameObject resource = Resources.Load("Gem") as GameObject;
			GameObject invaderGO = Instantiate(resource) as GameObject;
			
			invaderGO.transform.localScale =new Vector3(0.5f, 0.5f, 1);
			SpriteRenderer renderer = invaderGO.GetComponent<SpriteRenderer>();
			renderer.color = Color.grey;
			invaderGO.transform.parent = root.transform;
			invaderGO.transform.position = new Vector3(transform.position.x, transform.position.y, -1);
			invader = invaderGO.AddComponent<InvaderProxy>();
		}

		invader.gameObject.SetActive(true);

		return invader;
	}
}
