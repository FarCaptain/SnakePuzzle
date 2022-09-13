public class ExpandAction : ActionBase
{
    private readonly ItemSpawner _spawner;


    public ExpandAction(Snake snake, ItemSpawner spawner):base(snake)
    {
        _spawner = spawner;
    }


    public override void Execute()
    {
        // expand -> move
        _snake.Expand(_snake.GetPreTail());
    }

    public override void Undo()
    {
        // move -> shrink
        _spawner.SpawnFruit(1, _snake.GetHead());
        _snake.Shrink();
    }
}
