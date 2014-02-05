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
	public GemType type;

	public GemProxy prev;
	public GemProxy next;

	void Awake()
	{
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
		UpdateColor();
	}

	private void UpdateColor()
	{
		color = Helper.RandomColor();
		SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
		renderer.color = Helper.GetColorForGem(color);
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
		Vector3 position = new Vector3(transform.position.x, 0, 0);

		if(prev != null)
		{
			position  = new Vector3(prev.transform.position.x, prev.transform.position.y + Constants.GEM_UNIT_DIMENSION, prev.transform.position.z);
		}

		return position;
	}

}