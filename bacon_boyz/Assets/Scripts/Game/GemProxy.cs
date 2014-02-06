using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public enum GemColor
{
	RED,
	GREEN,
	BLUE,
	PURPLE,
	YELLOW,
}

public enum GemType
{
	GEM,
	CANON,
	BLOCKER,
}

public class GemProxy : MonoBehaviour 
{
	public GemColor color;
	public GemType type = GemType.GEM;

	public GemProxy prev;
	public GemProxy next;

	public Sprite blueSprite;
	public Sprite greenSprite;
	public Sprite orangeSprite;
	public Sprite purpleSprite;
	public Sprite redSprite;

	void Awake()
	{
		SetNewColor();
	}


	public void UpdateColorTo(GemColor color)
	{
		this.color = color;
		UpdateColor();
	}

	public GemProxy GetTopGem()
	{
		if(next == null)
		{
			return this;
		}
		else
		{
			return next.GetTopGem();
		}
	}

	public GemProxy GetBottomGem()
	{
		if(prev == null)
		{
			return this;
		}
		else
		{
			return prev.GetBottomGem();
		}
	}

	public void MoveDown()
	{
		if(prev == null)
		{
			StartMove();
		}
		else
		{
			prev.MoveDown();
		}
	}

	public void SetOffscreen()
	{
		transform.position = new Vector3(transform.position.x, Constants.OFFSCREEN_POSITION_Y, transform.position.z);
		SetNewColor();
	}

	private void SetNewColor()
	{
		color = Helper.RandomColor();
		UpdateColor();
	}

	private void UpdateColor()
	{
		SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
		renderer.sprite = GetSpriteForGem(color);
	}

	private Sprite GetSpriteForGem(GemColor gem)
	{
		Sprite sprite = redSprite;
		
		switch(gem)
		{
		case GemColor.RED:
			sprite = redSprite;
			break;
		case GemColor.GREEN:
			sprite = greenSprite;
			break;
		case GemColor.BLUE:
			sprite = blueSprite;
			break;
		case GemColor.PURPLE:
			sprite = purpleSprite;
			break;
		case GemColor.YELLOW:
			sprite = orangeSprite;
			break;
		}
		
		return sprite;
	}
	
	private void StartMove()
	{
		Vector3 position = GetTweenToPosition();

		if(position != transform.position)
		{			
			TweenParms parms = new TweenParms().Prop("position", GetTweenToPosition()).Ease(EaseType.Linear).OnComplete(MoveNext);
			HOTween.To(transform, Constants.DROP_GEMS_DURATION, parms);
		}
		else
		{
			MoveNext();
		}
	}

	private void MoveNext()
	{
		if(next != null)
		{
			next.StartMove();
		}
	}

	private Vector3 GetTweenToPosition()
	{
		Vector3 position = new Vector3(transform.position.x, transform.parent.position.y, 0);

		if(prev != null)
		{
			position  = new Vector3(prev.transform.position.x, prev.transform.position.y + Constants.GEM_UNIT_DIMENSION, prev.transform.position.z);
		}

		return position;
	}
}