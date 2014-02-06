using UnityEngine;
using System.Collections;
using System;

public class Helper 
{
	private static System.Random random = new System.Random();
	public static GemColor RandomColor()
	{
		var values = Enum.GetValues(typeof(GemColor));
		return (GemColor)values.GetValue(random.Next(values.Length));
	}
}
