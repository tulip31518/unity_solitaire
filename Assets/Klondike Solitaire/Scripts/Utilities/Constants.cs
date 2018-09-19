using UnityEngine;
using System;

public class Constants {
	public const int TURN_OVER_CARD_POINTS = 5;
	public const int WASTE2TABLEAU_POINTS = 5;
	public const int WASTE2FOUNDATIONS_POINTS = 15;
	public const int TABLEAU2FOUNDATIONS_POINTS = 10;
	public const int FOUNDATION2TABLEAU_POINTS = -15;
	public const int NUMBER_OF_FULL_COLOR_CARDS = 13;
	public const int VEGAS_SCORE_PER_CARD = 5;
	public const int NUMBER_OF_TABLEAUS = 7;
	public const float AUTOCOMPLETE_TIME_SCALE = 1.5f;
	public const int STOCK_START_INDEX = 28;

	public const int CARDS_IN_DECK = 52;
	public const int CARDS_IN_COLOR = 13;
	public const int CARDS_COLORS = 4;

	public const float TABLEAU_SPACING = -127.7778f;
	public const float LANDSCAPE_TABLEAU_SPACING = -139f;
	public const float ANIM_TIME = 0.208f;
	public const float STOCK_ANIM_TIME = 0.125f;
	public const float MOVE_ANIM_TIME = 0.33f;
	public const float HINT_ANIM_TIME = 0.5f;
	public const float SOUND_LOCK_TIME = 0.075f;
	public const float CARD_SIZE_ANIM_SPEED = 0.33f;
	public const float TOP_BAR_SIZE = 80f;

	public static readonly Vector2 vectorZero = new Vector2(0, 0);
	public static readonly Vector2 vectorHalf = new Vector2(0.5f, 0.5f);
	public static readonly Vector2 baseCardSize = new Vector2(127.7778f, 180f);

	public static readonly Vector3 vectorRight = new Vector3(1, 0, 0);
	public static readonly Vector3 vectorOne = new Vector3(1.2f, 1.2f, 1);
	public static readonly Vector3 vectorZoom = new Vector3(1.3f, 1.3f, 0);
	public static readonly Vector3 rotationZ180 = new Vector3(0, 0, 180);
	public static readonly Vector3 rotationZ0 = new Vector3(0, 0, 0);

	public static readonly int[] availableCardsIndexes = new int[] { 0, 2, 5, 9, 14, 20, 27 };
}