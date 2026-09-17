using System;
using UnityEngine;

public class GamePause
{
    public bool isPaused;

    public event Action OnPausedChange;

    public void ChangePause(bool isPause)
    { 
        isPaused = isPause;
        OnPausedChange.Invoke();
    }
}
