using System;
using UnityEngine;

public abstract class ActionBase
{
    protected readonly Snake _snake;

    protected ActionBase(Snake snake)
    {
        _snake = snake;
    }

    public abstract void Execute();
    public abstract void Undo();
}
