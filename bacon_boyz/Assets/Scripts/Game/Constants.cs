﻿using UnityEngine;
using System.Collections;

public class Constants 
{
	public const int GEM_AMOUNT_WIDTH = 6;
	public const int GEM_AMOUNT_HEIGHT = 8;
	public const int GEM_DIMENSION = 80;
	public const int PIXEL_PER_UNIT = 100;
	public const float GEM_UNIT_DIMENSION = (float)GEM_DIMENSION / (float)PIXEL_PER_UNIT;
	public const float DROP_GEMS_DURATION = 0.1f;
	public const float OFFSCREEN_POSITION_Y = 10f;
	public const int NUMBER_OF_CANONS = 4;
	public const int MIN_MATCH_SIZE = 3;
	public const int TURNS_PER_GAME = 20;
	public const int DEFAULT_HEALTH = 50;
}
