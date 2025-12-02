using MapGenTool.Generic;

namespace MapGenTool.Generators;

public static partial class Erosion {
    public static Tiles[,] DrunkardsWalk(int width, int height, int seed, Tiles[,] baseGrid, int agents, int iterations, int steps) {
        Random rng = new(seed);
        IntVector2 agent;

        HashSet<IntVector2> validSpaces = [];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (baseGrid[x, y] == Tiles.Space)
                    validSpaces.Add(new IntVector2(x, y));
            }
        }

        for (int a = 0; a < agents; a++) {
            agent = validSpaces.ElementAt(rng.Next(validSpaces.Count));

            for (int i = 0; i < iterations; i++)
                StepAgent();
        }
        void StepAgent() {
            bool ishorizontalStep = rng.Next(0, 2) == 1;
            if (ishorizontalStep) {
                int nextX = agent.x + rng.Next(-steps, steps + 1);
                nextX = Math.Clamp(nextX, -1, width);
                int startX = agent.x;
                int sign = (startX < nextX) ? 1 : -1;
                for (int x = startX; x != nextX; x += sign) {
                    agent.x = x;
                    validSpaces.Add(agent);
                    baseGrid[agent.x, agent.y] = Tiles.Space;
                }
            }
            else {
                int nextY = agent.y + rng.Next(-steps, steps + 1);
                nextY = Math.Clamp(nextY, -1, height);
                int startY = agent.y;
                int sign = (startY < nextY) ? 1 : -1;
                for (int y = startY; y != nextY; y += sign) {
                    agent.y = y;
                    validSpaces.Add(agent);
                    baseGrid[agent.x, agent.y] = Tiles.Space;
                }
            }
        }

        return baseGrid;
    }
}
