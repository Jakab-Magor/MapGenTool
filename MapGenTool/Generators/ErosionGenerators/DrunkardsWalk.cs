using MapGenTool.Generic;

namespace MapGenTool.Generators.ErosionGenerators;

public class DrunkardsWalk(int agentsPerSpace, int iterations, int stepLength = 1) : IGenerator<Tiles,Tiles>
{
    public int Iterations { get; set; } = iterations;
    public int StepLength { get; set; } = stepLength;

    public Tiles[,] Generate(Tiles[,] baseGrid, int width, int height, int seed)
    {/*
        IntVector2[] agents;
        if (_generator is null)
        {
            baseGrid = new Tiles[width, height];
            IntVector2 center = new IntVector2(width, height) / 2;
            baseGrid[center.x, center.y] = Tiles.Space;
            agents = new IntVector2[AgentsCount];
            for (int i = 0; i < agents.Length; i++)
                agents[i] = center;

        }
        else
        {
            baseGrid = _generator.Invoke(width, height, seed);
            List<IntVector2> tmpAgents = [];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (baseGrid[x, y] == Tiles.Wall)
                    {
                        var chord = new IntVector2(x, y);
                        for (int i = 0; i < AgentsCount; i++)
                        {
                            tmpAgents.Add(chord);
                        }
                    }
                }
            }
            agents = [.. tmpAgents];
        }
        foreach (var agent in agents)
            DrawAgent(ref baseGrid, agent);
        Random rng = new(seed);


        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (baseGrid[x, y] == Tiles.Space)
                    throw new NotImplementedException("JA!");

            }
        }

        for (int i = 0; i < agents.Length; i++)
        {
            DrawAgent(ref baseGrid, agents[i]);
        }

        for (int i = 0; i < Iterations; i++)
            for (int j = 0; j < agents.Length; j++)
                StepAgent(ref baseGrid, ref rng, ref agents[j], width, height);*/

        return baseGrid;
    }

    private void StepAgent(ref Tiles[,] grid, ref Random rng, ref IntVector2 agent, int width, int height)
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
                DrawAgent(ref grid, agent);
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
                DrawAgent(ref grid, agent);
            }
        }
    }

    private void DrawAgent(ref Tiles[,] grid, IntVector2 agent)
    {
        grid[agent.x, agent.y] = Tiles.Space;
    }
}
