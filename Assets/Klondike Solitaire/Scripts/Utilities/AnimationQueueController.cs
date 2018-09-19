using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum AnimationStatus { none, inProgress }

public class AnimationQueueController : MonoBehaviour
{

    private Queue<Action> actionQueue;
    [SerializeField]
    private string lastRunAnimation;
    [SerializeField]
    private AnimationStatus animationStatus = AnimationStatus.none;
    private int cardsInMove = 0;

    void Awake()
    {
        ResetState();
    }

    public void ResetState()
    {
        animationStatus = AnimationStatus.none;
        lastRunAnimation = "";
        cardsInMove = 0;
        actionQueue = new Queue<Action>();
    }

    public void AddActionToQueue(Action action)
    {
        if (animationStatus == AnimationStatus.none && cardsInMove  <= 0)
        {
            cardsInMove = 0;
            lastRunAnimation = action.Method.Name;
            action();
        }
        else
        {
            actionQueue.Enqueue(action);
        }
    }

    public void SetAnimationStatus(AnimationStatus status)
    {
        animationStatus = status;
    }

    public void CastNextAnimation()
    {
        animationStatus = AnimationStatus.none;
        lastRunAnimation = "";
        if (actionQueue.Count > 0)
        {
            lastRunAnimation = actionQueue.Peek().Method.Name;
            actionQueue.Dequeue()();
        }
    }
    

    public void AddMovingCard()
    {
        cardsInMove++;
    }

    public void FinishCardMoving()
    {
        cardsInMove--;
        if(cardsInMove <= 0)
        {
            cardsInMove = 0;
            CastNextAnimation();
        }
    }


    public bool IsCurrentActionUndoOrGetCard()
    {
        if (lastRunAnimation == "GetCard" || lastRunAnimation == "Undo")
            return true;
        return false;
    }
    public bool IsBusy()
    {
        return animationStatus == AnimationStatus.inProgress;
    }

    public string GetFirstActionName()
    {
        return lastRunAnimation;
    }
}
