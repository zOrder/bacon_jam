using UnityEngine;
using System.Collections;
using System;

public class Helper 
{
	
	public static Color GetColorForGem(GemColor gem)
	{
		Color gemColor = Color.white;
		
		switch(gem)
		{
		case GemColor.RED:
			gemColor = Color.red;
			break;
		case GemColor.GREEN:
			gemColor = Color.green;
			break;
		case GemColor.BLUE:
			gemColor = Color.blue;
			break;
		case GemColor.PURPLE:
			gemColor = Color.magenta;
			break;
		case GemColor.YELLOW:
			gemColor = Color.yellow;
			break;
		}
		
		return gemColor;
	}
	
	
	private static System.Random random = new System.Random();
	public static GemColor RandomColor()
	{
		var values = Enum.GetValues(typeof(GemColor));
		return (GemColor)values.GetValue(random.Next(values.Length));
	}
}
