using UnityEngine;
using System.Collections;
using Holoville.HOTween.Plugins;
using Holoville.HOTween;

public class ShipAnimation : MonoBehaviour {

	public float speed = 1;
	public float distance = 1;

	void Start () 
	{
		StartTween();
	}

	private void StartTween()
	{
		TweenParms parms = new TweenParms().Prop("rotation", new Vector3(0f,0f,distance)).Ease(EaseType.EaseInOutCubic).Loops(-1, LoopType.Yoyo);
		HOTween.To(this.transform, speed, parms);
	}
}
