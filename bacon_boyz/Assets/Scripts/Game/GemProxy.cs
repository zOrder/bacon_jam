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

	private GameObject canon;
	private GameObject blocker;

	void Awake()
	{
		SetNewColor();
		CreateCanonAndBlocker();
	}

	private void CreateCanonAndBlocker()
	{
		GameObject resource = Resources.Load("Gem") as GameObject;
		canon = Instantiate(resource) as GameObject;
		
		canon.transform.localScale =new Vector3(0.5f, 0.5f, 1);
		SpriteRenderer renderer = canon.GetComponent<SpriteRenderer>();
		renderer.color = Color.cyan;
		canon.transform.parent = transform;
		canon.transform.position = new Vector3(transform.position.x, transform.position.y, -1);
		canon.SetActive(false);

		blocker = Instantiate(resource) as GameObject;
		
		blocker.transform.localScale =new Vector3(0.5f, 0.5f, 1);
		renderer = blocker.GetComponent<SpriteRenderer>();
		renderer.color = Color.black;
		blocker.transform.parent = transform;
		blocker.transform.position = new Vector3(transform.position.x, transform.position.y, -1);
		blocker.SetActive(false);
	}

	public void MakeCanon()
	{
		SetType(GemType.CANON);
	}

	private void SetType(GemType type)
	{
		this.type = type;

		switch(type)
		{
		case GemType.BLOCKER:
			blocker.SetActive(true);
			canon.SetActive(false);
			break;
		case GemType.CANON:
			blocker.SetActive(false);
			canon.SetActive(true);
			break;
		case GemType.GEM:
			blocker.SetActive(false);
			canon.SetActive(false);
			break;
		}
	}

	public void ResetType()
	{
		SetType(GemType.GEM);
	}

	public void MakeBlocker()
	{
		SetType(GemType.BLOCKER);
	}

	public void UpdateColorTo(GemColor color)
	{
		this.color = color;
		UpdateColor();
	}

	public void MoveBlockerDown()
	{
		if(prev != null)
		{
			prev.MoveBlockerDown();

			if(type.Equals(GemType.BLOCKER))
			{
				SetType(prev.type);
				prev.MakeBlocker();

				GemColor tmpColor = prev.color;
				prev.UpdateColorTo(color);
				UpdateColorTo(tmpColor);
			}
		}
		else
		{
			if(type.Equals(GemType.BLOCKER))
			{
				Debug.Log("BLOCKER REACHED BOTTOM");
			}
		}
	}

	public GemProxy GetCanonBlocker()
	{
		if(next != null)
		{
			if(next.type.Equals(GemType.BLOCKER))
			{
				return next;
			}
			else
			{
				return next.GetCanonBlocker();
			}
		}
		return null;
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
		Vector3 position = new Vector3(transform.position.x, transform.parent.position.y, 0);

		if(prev != null)
		{
			position  = new Vector3(prev.transform.position.x, prev.transform.position.y + Constants.GEM_UNIT_DIMENSION, prev.transform.position.z);
		}

		return position;
	}
}