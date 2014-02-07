using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class InvaderProxy : MonoBehaviour 
{

	static float INVADER_SCALE = 0.85f;

	public enum MovementType
	{
		INVADE,
		DROP
	}


	public int GridX = Constants.GEM_AMOUNT_WIDTH;
	public int GridY = Constants.GEM_AMOUNT_HEIGHT;
	public float delayBetweenMoves = 3f;
	public int healthPoints;
	
	public void StartMoving()
	{
		healthPoints = Constants.INVADER_HEALTH;
		TweenToHealthScale();
		StartCoroutine("Move");
	}

	private MovementType _movement = MovementType.INVADE;
	public MovementType movementType {
				get { return _movement; }
				set { _movement = value; }
	}

	void OnDisable () 
	{
		StopCoroutine("Move");
	}

	IEnumerator Move ()
	{
				yield return new WaitForSeconds (delayBetweenMoves);
				int direction = -1;
				while (true) {
						if (this.movementType == MovementType.INVADE) {
								GridX += direction;
								if (GridX < 1) {
										GridX = 1;
										direction = 1;
										GridY --;
								} else if (GridX > Constants.GEM_AMOUNT_WIDTH) {
										GridX = Constants.GEM_AMOUNT_WIDTH;
										direction = -1;
										GridY --;
								}
								
						} else if (this.movementType == MovementType.DROP) {
								GridY--;
								
						}
						TweenToNewPosition ();
						yield return new WaitForSeconds (delayBetweenMoves);
				}
	}

	public void TweenToHealthScale ()
	{
		float scale = (0.6f + 0.2f * healthPoints) * INVADER_SCALE;
		TweenParms parms = new TweenParms().Prop("localScale", new Vector3(scale,scale,scale)).Ease(EaseType.EaseInOutElastic);
		HOTween.To (transform,0.4f, parms);
	}

	public void OnHit()
	{
		healthPoints --;
		TweenToHealthScale();
	}

	public void DieDieDie()
	{
		StopCoroutine("Move");
		SetNewGridPosition(Constants.GEM_AMOUNT_WIDTH, Constants.GEM_AMOUNT_HEIGHT);
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
		return new Vector3(GridX * Constants.GEM_UNIT_DIMENSION - Constants.GEM_UNIT_DIMENSION*0.5f, GridY * Constants.GEM_UNIT_DIMENSION, -1);
	}
}
