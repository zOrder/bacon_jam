using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;

public class Canon : MonoBehaviour 
{
	List<GameObject>pool = new List<GameObject>();
		
	public void ShootFromTo(Vector3 start, Vector3 destination)
	{
		Debug.Log("SHOOT FORM "+start +"  TO  "+destination);

		GameObject bullet = GetBullet();
		bullet.SetActive(true);
		bullet.transform.position = start;

		TweenParms parms = new TweenParms().Prop("position", destination).Ease(EaseType.Linear).OnComplete(ReturnToPool, bullet);
		HOTween.To(bullet.transform, Constants.SHOOT_CANON_DURATION, parms);
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
			GameObject resource = Resources.Load("Gem") as GameObject;
			bullet = Instantiate(resource) as GameObject;
			bullet.transform.localScale =new Vector3(0.25f, 0.25f, 1);
			bullet.transform.parent = transform;
		}

		return bullet;
	}


}
