using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class InvaderProxy : MonoBehaviour 
{
	public int GridX = Constants.GEM_AMOUNT_WIDTH;
	public int GridY = Constants.GEM_AMOUNT_HEIGHT;

	void Start () 
	{
		StartCoroutine(Move());
	}

	IEnumerator Move()
	{
		while(true)
		{
			GridX --;
			if(GridX <= 0)
			{
				GridX = Constants.GEM_AMOUNT_WIDTH;
				GridY --;
			}

			TweenToNewPosition();

			yield return new WaitForSeconds(1f);
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
