using MapGenTool.Generic;

namespace MapGenTool.Generators.ErosionGenerators;

public class DrunkardsWalk : ILevelGenerator
{
    public Func<int, int, int, Tiles[,]> _generator;
    public int AgentsCount { get; set; }
    public int Iterations { get; set; }
    public int StepLength { get; set; }

    public DrunkardsWalk(Func<int, int, int, Tiles[,]> generator, int agents, int iterations, int stepLength = 1)
    {
        _generator = generator;
        AgentsCount = agents;
        Iterations = iterations;
        StepLength = stepLength;
    }

    public Tiles[,] Generate(int width, int height, int seed)
    {
        Tiles[,] grid = _generator.Invoke(width, height, seed);
        Random rng = new(seed);
        IntVector2 agent;

        HashSet<IntVector2> validSpaces = [];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] == Tiles.Space)
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
                    grid[agent.x, agent.y] = Tiles.Space;
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
                    grid[agent.x, agent.y] = Tiles.Space;
                }
            }
        }

        return grid;
    }
}
