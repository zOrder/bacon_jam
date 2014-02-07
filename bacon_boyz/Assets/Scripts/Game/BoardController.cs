using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BoardController : MonoBehaviour 
{
	public string pointsString = "";
	public int points = 0;

	public dfButton startButton;
	private GameObject root;
	private BoardModel boardModel;
	private Canon canonBehaviour;
	private List<InvaderProxy> invaders = new List<InvaderProxy>();
	private List<InvaderProxy> invaderPool = new List<InvaderProxy>();

	private int wave = 0;
	private float spawnDelay = 7.5f;
	private float minSpawnDelay = 1f;
	private float invadeSpeed = 2.5f;

	private float lastTap;

	private GemMatcher matcher;

	private bool gameIsRunning = false;

	void Start()
	{
		startButton.Click += OnClick;
		NotificationManager.Instance.AddObserver(OnInvaderReachedBottom, NotificationConstants.INVADER_REACHED_BOTTOM);
	}

	public void OnInvaderReachedBottom( Notification notification)
	{
		EndGame();
	}

	public void OnClick( dfControl control, dfMouseEventArgs mouseEvent )
	{
		StartGame();
		startButton.gameObject.SetActive(false);
		AudioController.PlayMusicPlaylist();
	}

	private void StartGame()
	{
		gameIsRunning = true;
	
		SetupModel();
		InitBoard();
		SetupCanon();
		DropGems();

		wave = 0;
		points = 0;
		UpdatePoints();
		
		matcher = new GemMatcher(boardModel);
		
		StartCoroutine("SpawnInvader", 0.1f);
	}
	
	IEnumerator SpawnInvader(float initialDelay)
	{
		yield return new WaitForSeconds(initialDelay);

		while(gameIsRunning)
		{

			UpdateSpawnDelay();

			wave++;

			int columnA = UnityEngine.Random.Range(0, Constants.GEM_AMOUNT_WIDTH - 1);
			int columnB = (columnA + UnityEngine.Random.Range(1, 3)) % Constants.GEM_AMOUNT_WIDTH;
			int columnC = (columnB + UnityEngine.Random.Range(1, 4)) % Constants.GEM_AMOUNT_WIDTH;

			float speed = Mathf.Max (invadeSpeed*0.5f,invadeSpeed - wave*0.01f);

			InvaderProxy.MovementType movement = InvaderProxy.MovementType.DROP;
			int h = 3;

			if (wave > 12 && UnityEngine.Random.Range(0,100) < 50) { // big health
				h = 4;
			}
			else if (wave > 8 && UnityEngine.Random.Range(0,100) < 50) { // new movement
				movement = InvaderProxy.MovementType.INVADE;
				speed *= 0.45f;
			}

			SetInvader(columnA+1, speed, movement, h);
			SetInvader(columnB+1, speed, movement, h);
			if (wave > 1) {
				SetInvader(columnC+1, speed, movement, h);
			}
			if (wave > 15) {
				SetInvader(columnC+1, speed, movement, h);
			}
			if (wave > 20) {
				SetInvader(columnC+1, speed, movement, h);
			}

			yield return new WaitForSeconds(spawnDelay);
		}
	}

	private void UpdateSpawnDelay()
	{
		spawnDelay = Math.Max(minSpawnDelay, spawnDelay - 0.075f);
	}

	private void EndGame()
	{
		StopCoroutine("SpawnInvader");

		pointsString = ""+points;

		for(int i= invaders.Count-1; i>=0 ; i--)
		{
			InvaderProxy invader = invaders[i];
			KillInvader(invader);
		}

		invaderPool.Clear();

		gameIsRunning = false;
		startButton.gameObject.SetActive(true);
	}

	private void SetInvader(int column, float speed, InvaderProxy.MovementType movement, int h)
	{
		InvaderProxy invader = GetInvader();
		int randomGridX = column;
		invader.SetNewGridPosition(randomGridX, Constants.GEM_AMOUNT_HEIGHT);
		invader.delayBetweenMoves = invadeSpeed;
		invader.movementType = movement;
		invader.healthPoints = h;
		invader.StartMoving();

		invaders.Add(invader);
	}
		
	public void OnTap (TapGesture gesture)
	{
		if (Time.time - lastTap < 0.25f) {
			return;
		}

		lastTap = Time.time;

		if(gameIsRunning == false)
		{
			return;
		}

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

			
			if(invaders.Count == 0)
			{
				StopCoroutine("SpawnInvader");
				StartCoroutine("SpawnInvader", 0.5f);
			}

			MoveMatchesOffscreen(matches);
			boardModel.SortModel();
			
			DropGems();

			if(ShouldFindOrphans())
			{
				FindOrphanedGems();
			}


			AudioController.Play ("time");
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
			if(invader.GridX == x+1 && invader.GridY > y)
			{
				invader.OnHit(matches.Count);
				canonBehaviour.ShootFromTo(ConvertGridToBoard(new Vector2(x, y)), invader.GridY * Constants.GEM_UNIT_DIMENSION + root.transform.position.y );

				if(invader.healthPoints <= 0)
				{
					KillInvader(invader);
					points += 30;
				}

				return; 
			}
		}

		canonBehaviour.ShootFromTo(ConvertGridToBoard(new Vector2(x, y)), x+6f);

		points += matches.Count;
		UpdatePoints();
	}

	private void KillInvader(InvaderProxy invader)
	{
		invader.DieDieDie();
		invader.gameObject.SetActive(false);
		if(invaders.Contains(invader))
		{
			invaders.Remove(invader);
		}
		invaderPool.Add(invader);
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

	private void UpdatePoints()
	{
		pointsString = "" + points;
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

		if(root != null)
		{
			Destroy(root);
		}

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

		float borderWidth  = (Screen.width - (Constants.GEM_AMOUNT_WIDTH * Constants.GEM_DIMENSION)) / 2;

		root.transform.position = new Vector3(borderWidth / Constants.PIXEL_PER_UNIT , 0.6f, 0);
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
		int numberToFlip = 5;

		for(int i = 0; i< Constants.GEM_AMOUNT_WIDTH; i++)
		{
			for(int j = 0; j< Constants.GEM_AMOUNT_HEIGHT / 2; j++)
			{
				if(matcher.HasMatchingNeighbours(i, j) == false)
				{
					GemProxy origin  = boardModel.GetGemAtPosition(i, j);
					GemProxy neighbour = matcher.GetRandomNeighbour(i, j);

					origin.UpdateColorTo(neighbour.color);

					numberToFlip--;
					if (numberToFlip > 0) {
						return;
					}
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
			GameObject resource = Resources.Load("invader") as GameObject;
			GameObject invaderGO = Instantiate(resource) as GameObject;
			
			//invaderGO.transform.localScale =new Vector3(0.5f, 0.5f, 1);
			SpriteRenderer renderer = invaderGO.GetComponent<SpriteRenderer>();
			invaderGO.transform.parent = root.transform;

			invader = invaderGO.AddComponent<InvaderProxy>();
		}

		invader.gameObject.SetActive(true);

		return invader;
	}
}
