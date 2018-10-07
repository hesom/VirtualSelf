using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VirtualSelf
{

public class Gamescore : MonoBehaviour
{
	public int Score;
	public int WinScore = 10;
	[Tooltip("Reaching this score is not necessary to win, but the player can continue and the slider max is set to" +
	         " this value. A second win event is fired for reaching the second score")]
	public int FullClearScore = 10;
	public Slider Slider;
	public UnityEvent OnWin;

	private readonly HashSet<ScoreObject> _scoreObjects = new HashSet<ScoreObject>();
	private bool _won;
	
	void Start()
	{
		Slider.wholeNumbers = true;
		Slider.maxValue = WinScore;
		Reset();
	}

	public void Increment(ScoreObject scoreObject, int amount = 1)
	{
		_scoreObjects.Add(scoreObject);
		Score += amount;
		Slider.value = Score;
		CheckWin();
	}

	public void Increment(int amount)
	{
		Score += amount;
		Slider.value = Score;
		CheckWin();
	}

	private void CheckWin()
	{
		if (!_won)
		{
			if (Score >= WinScore)
			{
				OnWin.Invoke();
				_won = true;
				if (FullClearScore > WinScore)
				{
					Slider.maxValue = FullClearScore;
				}
			}
		}
		else
		{
			// note that there is no check here for being over the fullclear score
			// so make sure this truly is the maximum possible score, or many win events will be invoked after that!
			if (FullClearScore > WinScore && Score >= FullClearScore)
			{
				OnWin.Invoke();
			}
		}
	}
	
	public void Reset()
	{
		Score = 0;
		Slider.value = Score;
		foreach (ScoreObject o in _scoreObjects)
		{
			o.Reset();
		}
		_won = false;
	}

	void OnValidate()
	{
		Slider.maxValue = WinScore;
	}
}
	
}