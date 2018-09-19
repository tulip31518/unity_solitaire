using System;

/// <summary>
/// Class runs delayed actions
/// </summary>
public class Timer
{

    public Timer(float time, Action finishAction)
    {
		TweenAnimator.instance.RunTimer(time, finishAction);
    }


}
