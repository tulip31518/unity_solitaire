using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
public enum ScoreType { none, normal, vegas};
public class MatchStatistics : MonoBehaviour {

	public static MatchStatistics instance;
	public bool isDraw3 { get; private set; }
	public Text timeText;
	public Text movesText;
	public Text scoreText;

	private int _score;
	private int _moves;
	private float startTime;
	private int _vegasScore = 0;
	private float totalTime;
	private bool timeRun = true;
	private ScoreType scoreType = ScoreType.normal;

	public void StopTime()
	{
		timeRun = false;
	}

	public void StartTime()
	{
		timeRun = true;
	}

	public bool IsVegasGame()
	{
		return scoreType == ScoreType.vegas;
	}

	public string GetStringScoreType()
	{
		return scoreType.ToString();
	}

	public int score
	{
		get
		{ return _score; }
		set
		{
			_score = value;
			RefreshPointsText();
		}
	}

	public void AddScore(int amount)
	{
		score += amount;
	}


	public int vegasScore
	{
		get
		{
			return _vegasScore;
		}
		set
		{
			_vegasScore = value;
			RefreshPointsText();
		}
	}

	public void OnNormalScoreToggleSetting(bool state) {
		if (state)
			ChangeScoringType(ScoreType.normal);
	}
	public void OnVegasScoreToggleSetting(bool state) {
		if (state)
			ChangeScoringType(ScoreType.vegas);
	}

	public void OnDraw3ToggleSetting(bool state) {
		isDraw3 = state;
	}



	private void ChangeScoringType(ScoreType type)
	{
		scoreType = type;
		RefreshPointsText();
	}

	public int moves
	{
		get { return _moves; }
		set
		{
			_moves = value;
			RefreshMovesText();
		}
	}

	void Awake()
	{
		instance = this;
		ResetState();
	}

	public float GetGameTime()
	{
		return totalTime - startTime;
	}

	public void ResetState()
	{
		startTime = Time.time;
		totalTime = startTime;
		vegasScore = -52;
		score = 0;
		moves = 0;
		StartTime();
	}

	void Update()
	{
		ShowTime();
	}

	private void ShowTime() {
		if (timeRun)
		{
			totalTime += Time.deltaTime;
			float t = totalTime - startTime;
			string stringTime = t > 99 * 60 ? "∞" : TimeToString.GetTimeStringMSFormat(totalTime - startTime);

			timeText.text = "time: " + stringTime;
		}
	}


	private void RefreshPointsText()
	{
		switch (scoreType)
		{
			case ScoreType.none:
				scoreText.text = "";
				break;
			case ScoreType.normal:
				scoreText.text = "score: " + score.ToString();
				break;
			case ScoreType.vegas:
				scoreText.text = "score: " + vegasScore.ToString() + "$";
				break;
			default:
				break;
		}
	}

	private void RefreshMovesText()
	{
		movesText.text = "moves: " +  moves.ToString();
	}   
}
