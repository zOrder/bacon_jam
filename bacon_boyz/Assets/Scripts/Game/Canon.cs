using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

public class Canon : MonoBehaviour 
{
	List<GameObject>pool = new List<GameObject>();
		
	public void ShootFromTo(Vector2 start, float destination)
	{
		Debug.Log("SHOOT FORM "+start +"  TO  "+destination);

		GameObject bullet = GetBullet();
		bullet.SetActive(true);
		bullet.transform.position = new Vector3(start.x, start.y, bullet.transform.position.z);

		float distance = destination - start .y;

		TweenParms parms = new TweenParms().Prop("position", new PlugVector3Y(destination)).Ease(EaseType.Linear).OnComplete(ReturnToPool, bullet);
		HOTween.To(bullet.transform, distance * Constants.SHOOT_CANON_DURATION, parms);
	}

	private void ReturnToPool( TweenEvent data)
	{
		GameObject bullet = data.parms[0] as GameObject;
		bullet.SetActive(false);
		pool.Add(bullet);
	}

	private GameObject GetBullet()
	{
		GameObject bullet = null;

		if(pool.Count > 0)
		{
			bullet = pool[0];
			pool.RemoveAt(0);
		}
		else
		{
			GameObject resource = Resources.Load("cannonBall") as GameObject;
			bullet = Instantiate(resource) as GameObject;
			bullet.transform.parent = transform;
		}

		return bullet;
	}


}
