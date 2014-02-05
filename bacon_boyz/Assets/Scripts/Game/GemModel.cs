using UnityEngine;
using System.Collections;

public enum GemColors
{
	RED,
	GREEN,
	BLUE,
	PURPLE,
	YELLOW,
}

public enum GemType
{
	GEM,
	CANON,
	BLOCKER,
}

public class GemModel : MonoBehaviour 
{
	public GemColors color;
	public GemType type;
}