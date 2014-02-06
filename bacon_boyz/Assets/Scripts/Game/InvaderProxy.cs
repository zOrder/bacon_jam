using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class InvaderProxy : MonoBehaviour 
{
	public int GridX = Constants.GEM_AMOUNT_WIDTH;
	public int GridY = Constants.GEM_AMOUNT_HEIGHT;

	public float delayBetweenMoves = 2f;

	void Start () 
	{
		StartCoroutine(Move());
	}

	IEnumerator Move()
	{
		int direction = -1;

		while(true)
		{
			GridX += direction;
			if(GridX < 1)
			{
				GridX = 1;
				direction = 1;
				GridY --;
			} else if (GridX > Constants.GEM_AMOUNT_WIDTH) {
				GridX = Constants.GEM_AMOUNT_WIDTH;
				direction = -1;
				GridY --;
			}

			TweenToNewPosition();

			yield return new WaitForSeconds(delayBetweenMoves);
		}
	}

	public void DieDieDie()
	{

	}

	public void SetNewGridPosition(int x, int y)
	{
		GridX = x;
		GridY = y;

		transform.position = GetTweenToPosition();
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
