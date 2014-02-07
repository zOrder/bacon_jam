using UnityEngine;
using System.Collections;
using Holoville.HOTween.Plugins;
using Holoville.HOTween;

public class Waves : MonoBehaviour {

	public float speed = 1;
	public float distance = 1;

	void Start () 
	{
		StartTween();
	}

	private void StartTween()
	{
		TweenParms parms = new TweenParms().Prop("position", new PlugVector3X(distance)).Ease(EaseType.Linear).Loops(-1, LoopType.Yoyo);
		HOTween.To(this.transform, speed, parms);
	}
}
