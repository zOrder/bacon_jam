using UnityEngine;
using System.Collections;

public class Constants 
{
	public const int GEM_AMOUNT_WIDTH = 7;
	public const int GEM_AMOUNT_HEIGHT = 9;
	public const int GEM_DIMENSION = 75;
	public const int PIXEL_PER_UNIT = 100;
	public const float GEM_UNIT_DIMENSION = (float)GEM_DIMENSION / (float)PIXEL_PER_UNIT;
	public const float DROP_GEMS_DURATION = 0.05f;
	public const float SHOOT_CANON_DURATION = 0.075f;
	public const float OFFSCREEN_POSITION_Y = 10f;
	public const int NUMBER_OF_CANONS = 4;
	public const int MIN_MATCH_SIZE = 2;
	public const int TURNS_PER_GAME = 20;
	public const int DEFAULT_HEALTH = 50;

	public const int INVADER_HEALTH = 3;
}
