using UnityEngine;
using System.Collections;

public class Notification  
{
	private readonly object payload;

	public Notification(object payload)
	{
		this.payload = payload;
	}

	public object Payload
	{
		get{ return payload; }
	}

	public static Notification Empty
	{
		get
		{
			return new Notification(null);
		}
	}
}
