using System;
using UnityEngine;

public class MoveAction : ActionBase
{
    private readonly Vector2Int _direction;
    private readonly ItemSpawner _spawner;

    private Vector2Int _preTailCoord;
    private Vector2 _preDirection;

    public MoveAction(Snake snake, ItemSpawner spawner, Vector2Int direction) : base(snake)
    {
        _direction = direction;
        _spawner = spawner;
    }

    public override void Execute()
    {
        _preTailCoord = _snake.GetTail();
        _preDirection = _snake.GetDirection();
        _snake.Move(_direction);
    }

    public override void Undo()
    {
        _snake.SetDirection(_preDirection);
        _snake.Retreat(_preTailCoord);
    }
}
