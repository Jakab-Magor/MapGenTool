using MapGenTool.Generic;

namespace MapGenTool.Generators.ErosionGenerators;

public class DrunkardsWalk() : IGenerator<Tiles>
{
    public int AgentsCount { get; set; }
    public int Iterations { get; set; }
    public int StepLength { get; set; }

    public bool UsesInput => true;

    private Tiles[,] _baseGrid = null!;

    public byte ArgsCount => 3;

    public Type InputType => typeof(Tiles);

    public Tiles[,] Generate(int width, int height, int seed)
    {
        Random rng = new(seed);
        IntVector2 agent;

        HashSet<IntVector2> validSpaces = [];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (_baseGrid[x, y] == Tiles.Space)
                    validSpaces.Add(new IntVector2(x, y));
            }
        }

        for (int a = 0; a < AgentsCount; a++)
        {
            agent = validSpaces.ElementAt(rng.Next(validSpaces.Count));

            for (int i = 0; i < Iterations; i++)
                StepAgent();
        }
        void StepAgent()
        {
            bool ishorizontalStep = rng.Next(0, 2) == 1;
            if (ishorizontalStep)
            {
                int nextX = agent.x + rng.Next(-StepLength, StepLength + 1);
                nextX = Math.Clamp(nextX, -1, width);
                int startX = agent.x;
                int sign = (startX < nextX) ? 1 : -1;
                for (int x = startX; x != nextX; x += sign)
                {
                    agent.x = x;
                    validSpaces.Add(agent);
                    _baseGrid[agent.x, agent.y] = Tiles.Space;
                }
            }
            else
            {
                int nextY = agent.y + rng.Next(-StepLength, StepLength + 1);
                nextY = Math.Clamp(nextY, -1, height);
                int startY = agent.y;
                int sign = (startY < nextY) ? 1 : -1;
                for (int y = startY; y != nextY; y += sign)
                {
                    agent.y = y;
                    validSpaces.Add(agent);
                    _baseGrid[agent.x, agent.y] = Tiles.Space;
                }
            }
        }

        return _baseGrid;
    }

    public void Parse(params string[] args)
    {
        AgentsCount = int.Parse(args[0]);
        Iterations = int.Parse(args[1]);
        StepLength = int.Parse(args[2]);
    }

    public void SetBaseGrid<T>(T[,] basegrid) where T : IConvertible
    {
        _baseGrid = IGenerator<Tiles>.CastGrid<T>(basegrid);
    }
}
