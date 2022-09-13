using UnityEngine;

public class ShrinkAction : ActionBase
{
    private readonly ItemSpawner _spawner;

    private Vector2Int _preTail;

    public ShrinkAction(Snake snake, ItemSpawner spawner) : base(snake)
    {
        _spawner = spawner;
    }

    public override void Execute()
    {
        // shrink after move!
        _preTail = _snake.GetTail();
        _snake.Shrink();
    }

    public override void Undo()
    {
        // undo shrink first first before move
        _snake.Expand(_preTail);
        _spawner.SpawnFruit(0, _snake.GetHead());
        // do this when retreating
    }
}
