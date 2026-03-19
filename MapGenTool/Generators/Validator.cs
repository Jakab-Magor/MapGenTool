
using MapGenTool.Generic;

namespace MapGenTool.Generators;

public static partial class Misc {
    record class AStarNode(int x, int y) {
        public int g, h;
        public AStarNode? parent = null;
    }

    public static Tiles[,] Validate(int width, int height, Tiles[,] input, int cullingTreshold) {
        /// Dr. Orosz Ákos suggestion
        /// Floodfill all "islands" of Tiles.space
        ///     Cull any below a treshold to remove stray tiles
        int[,] islandMap = new int[width, height];
        // The current islands value in fillmap
        // remember to -1 when indexing!!!!! starts with 1
        List<(int, int)> islands = [];

        // Iterate for movement
        // results in left, right, up, down priority
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        Console.WriteLine("Finding islands:");
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (!(input[x, y] == Tiles.Space && islandMap[x, y] == 0)) {
                    continue;
                }

                islands.Add((x, y));

                FloodFill(x, y, islands.Count, out int volume);

                if (volume < cullingTreshold) {
                    FloodFill(x, y, 0, out _);
                    islands.RemoveAt(islands.Count - 1);
                    Console.WriteLine($"culled - volume: {volume}");
                    continue;
                }
                Console.WriteLine($"#{islands.Count} - volume: {volume}");
            }
        }

        void FloodFill(int originX, int originY, int value, out int volume) {
            Queue<(int, int)> floodfillQ = new();
            floodfillQ.Enqueue((originX, originY));
            volume = 0;
            islandMap[originX, originY] = value;

            while (floodfillQ.Count != 0) {
                (int qx, int qy) = floodfillQ.Dequeue();
                volume++;
                for (int i = 0; i < dx.Length; i++) {
                    int nx = qx + dx[i];
                    int ny = qy + dy[i];

                    if (nx < 0 || nx >= width || ny < 0 || ny >= height) {
                        continue;
                    }

                    if (input[nx, ny] == Tiles.Space && islandMap[nx, ny] != value) {
                        islandMap[nx, ny] = value;
                        floodfillQ.Enqueue((nx, ny));
                    }
                }
            }
        }

        /// On a seperate layer expand bounds of all "islands"
        ///     Where two bounds meet connect with corridor
        ///     Backfill with either color to handle as same room
        int[,] boundsMap = (int[,])islandMap.Clone();
        Queue<(int x, int y, int parentx, int parenty)> boundsQ = new();
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                int color = boundsMap[x, y];
                if (color != 0) {
                    boundsQ.Enqueue((x, y, x, y));
                }
            }
        }

        Console.WriteLine("\nConnecting rooms and overriding:");
        while (boundsQ.Count != 0) {
            (int x, int y, int parentx, int parenty) = boundsQ.Dequeue();
            int colorCurrent = boundsMap[parentx, parenty];
            boundsMap[x, y] = colorCurrent;
            for (int i = 0; i < dx.Length; i++) {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (nx < 0 || nx >= width || ny < 0 || ny >= height) {
                    continue;
                }

                int colorNext = boundsMap[nx, ny];
                if (colorNext == 0) {
                    boundsMap[nx, ny] = colorCurrent;
                    boundsQ.Enqueue((nx, ny, x, y));
                    continue;
                }
                // two bounds intersect
                if (colorCurrent != colorNext) {
                    (int ax, int ay) = islands[colorCurrent - 1];
                    (int bx, int by) = islands[colorNext - 1];
                    Console.WriteLine($"#{colorCurrent} -> #{colorNext}");

                    //HashSet<(int, int)> path = [.. findPathAStar(ax, ay, nx, ny), .. findPathAStar(bx, by, nx, ny)];
                    HashSet<(int, int)> path = [.. findPathAStar(ax, ay, bx, by)];
                    foreach (var pos in path) {
                        (int px, int py) = pos;
                        islandMap[px, py] = colorCurrent;
                        boundsMap[px, py] = colorCurrent;
                    }
                    changeBoundsColors(colorNext, colorCurrent);
                }
            }
        }
        List<(int, int)> findPathAStar(int startX, int startY, int goalX, int goalY) {
            List<AStarNode> open = [];
            HashSet<(int, int)> closed = [];
            int goalColor = islandMap[goalX, goalY];

            AStarNode startNode = new(startX, startY) {
                g = 0,
                h = heuristics(startX, startY, goalX, goalY)
            };
            open.Add(startNode);

            while (open.Count > 0) {
                AStarNode current = open[0];
                open.RemoveAt(0);

                int currentColor = islandMap[current.x, current.y];
                if (currentColor == goalColor) {
                    // return path
                    List<(int, int)> path = [];
                    AStarNode? pathNode = current;

                    while (pathNode is not null) {
                        path.Add((pathNode.x, pathNode.y));
                        pathNode = pathNode.parent;
                    }
                    //path.Reverse();
                    return path;
                }

                closed.Add((current.x, current.y));

                for (int i = 0; i < dx.Length; i++) {
                    int nx = current.x + dx[i];
                    int ny = current.y + dy[i];

                    if (nx < 0 || nx >= width || ny < 0 || ny >= height) {
                        continue;
                    }
                    if (closed.Contains((nx, ny))) {
                        continue;
                    }

                    int tileValue = islandMap[nx, ny];
                    const int zeroPenalty = 100;
                    // if wall
                    int moveCost = tileValue == 0 ? zeroPenalty : 1;

                    AStarNode neighbour = new(nx, ny) {
                        g = current.g + moveCost,
                        h = heuristics(nx, ny, goalX, goalY),
                        parent = current
                    };

                    AStarNode? existing = open.Find(n => n.x == nx && n.y == ny);

                    if (existing is not null) {
                        if (existing.g > neighbour.g) {
                            existing.g = neighbour.g;
                            existing.parent = neighbour.parent;
                        }
                        continue;
                    }

                    if (existing is null) {
                        int o = 0;
                        while (o < open.Count && final(open[o]) <= final(neighbour)) {
                            o++;
                        }
                        open.Insert(o, neighbour);
                    }
                }
            }

            return [];
        }

        void changeBoundsColors(int origionalColor, int newColor) {
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (boundsMap[x, y] == origionalColor) {
                        boundsMap[x, y] = newColor;
                    }
                }
            }
        }

        int heuristics(int x1, int y1, int x2, int y2) {
            int difX = Math.Abs(x1 - x2);
            int difY = Math.Abs(y1 - y2);
            int dist = difX * difX + difY * difY;
            return dist;
        }
        int final(AStarNode node) {
            // squering g because h is distance squared as well
            return node.g * node.g + node.h;
        }

        Tiles[,] result = new Tiles[width, height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                result[x, y] = islandMap[x, y] == 0 ? Tiles.Wall : Tiles.Space;
            }
        }

        return result;
    }
}