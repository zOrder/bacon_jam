using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class DropperProxy : MonoBehaviour 
{
	public int GridX = Constants.GEM_AMOUNT_WIDTH;
	public int GridY = Constants.GEM_AMOUNT_HEIGHT;

	public float delayBetweenMoves = 10f;

	void Start () 
	{
		StartCoroutine(Move());
	}

	IEnumerator Move()
	{
		while(true)
		{
			GridY --;

			TweenToNewPosition();

			yield return new WaitForSeconds(delayBetweenMoves);
		}
	}

	public void SetNewGridPosition(int x, int y)
	{
		GridX = x;
		GridY = y;
		TweenToNewPosition();
	}

	private void TweenToNewPosition()
	{		
		TweenParms parms = new TweenParms().Prop("position", GetTweenToPosition()).Ease(EaseType.Linear);
		HOTween.To(transform, 0.4f, parms);
	}

	private Vector3 GetTweenToPosition()
	{
		return new Vector3(GridX * Constants.GEM_UNIT_DIMENSION, GridY * Constants.GEM_UNIT_DIMENSION, -1);
	}
}
