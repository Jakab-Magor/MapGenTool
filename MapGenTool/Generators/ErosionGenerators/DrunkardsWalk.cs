using MapGenTool.Generic;

namespace MapGenTool.Generators;

public static partial class Erosion {
    public static Tiles[,] DrunkardsWalk(int width, int height, int seed, Tiles[,] baseGrid, int agents, int iterations, int steps) {
        Random rng = new(seed);
        IntVector2 agent;
        steps++;

        HashSet<IntVector2> validSpaces = [];
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (baseGrid[x, y] == Tiles.Space) {
                    // Only on edges not on tiles surrounded by spaces
                    for (int i = 0; i < dx.Length; i++) {
                        int nx = x + dx[i];
                        int ny = y + dy[i];

                        if (nx < 0 || nx >= width || ny < 0 || ny >= height) {
                            continue;
                        }

                        if (baseGrid[nx, ny] == Tiles.Wall) {
                            validSpaces.Add(new IntVector2(x, y));
                            break;
                        }
                    }
                }
            }
        }

        for (int a = 0; a < agents; a++) {
            int r = rng.Next(validSpaces.Count);
            agent = validSpaces.ElementAt(r);
            validSpaces.Remove(agent);

            for (int i = 0; i < iterations; i++) {
                // step agent
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
        }

        return baseGrid;
    }
}
