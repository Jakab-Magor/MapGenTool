
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
        int[,] boundsMap = new int[width, height];
        // The current islands value in fillmap
        List<(int, int)> islands = [];

        // Iterate for movement
        // results in left, right, up, down priority
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (!(input[x, y] == Tiles.Space && boundsMap[x, y] == 0)) {
                    continue;
                }

                islands.Add((x, y));

                FloodFill(x, y, islands.Count, out int volume);

                if (volume < cullingTreshold) {
                    FloodFill(x, y, 0, out _);
                }


            }
        }

        void FloodFill(int fromX, int fromY, int with, out int volume) {
            /// runs forever
            Queue<(int, int)> floodfillQ = new();
            floodfillQ.Enqueue((fromX, fromY));
            volume = 0;

            while (floodfillQ.Count != 0) {
                (int qx, int qy) = floodfillQ.Dequeue();
                volume++;
                boundsMap[qx, qy] = with;
                for (int i = 0; i < dx.Length; i++) {
                    int nx = qx + dx[i];
                    int ny = qy + dy[i];

                    if (nx < 0 || nx >= width || ny < 0 || ny >= height) {
                        continue;
                    }

                    if (input[nx, ny] == Tiles.Space) {
                        floodfillQ.Enqueue((nx, ny));
                    }
                }
            }
        }

        /// On a seperate layer expand bounds of all "islands"
        ///     Where two bounds meet connect with corridor
        ///     Backfill with either color to handle as same room
        Queue<(int, int)> boundsQ = new();
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (boundsMap[x, y] != 0) {
                    boundsQ.Enqueue((x, y));
                }
            }
        }

        while (boundsQ.Count != 0) {
            (int x, int y) = boundsQ.Dequeue();
            for (int i = 0; i < dx.Length; i++) {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (nx < 0 || nx >= width || ny < 0 || ny >= height) {
                    continue;
                }

                int boundsCurrent = boundsMap[x, y];
                int boundsNext = boundsMap[nx, ny];
                if (boundsNext == 0) {
                    boundsQ.Enqueue((nx, ny));
                    continue;
                }
                if (boundsCurrent != boundsNext) {
                    (int ax, int ay) = islands[boundsCurrent];
                    (int bx, int by) = islands[boundsNext];

                    List<(int, int)> path = [.. FindPathAStar(ax, ay, nx, ny), .. FindPathAStar(bx, by, nx, ny)];
                    for (int p = 0; p < path.Count; p++) {
                        (int px, int py) = path[p];
                        boundsMap[px, py] = boundsCurrent;
                    }
                    FloodFill(bx, by, boundsCurrent, out _);
                    islands.RemoveAt(boundsNext);
                }
            }
        }
        List<(int, int)> FindPathAStar(int startX, int startY, int goalX, int goalY) {
            List<AStarNode> open = [];
            HashSet<(int, int)> closed = [];

            AStarNode startNode = new(startX, startY) {
                g = 0,
                h = heuristics(startX, startY, goalX, goalY)
            };
            open.Add(startNode);

            while (open.Count > 0) {
                AStarNode current = open[0];
                open.RemoveAt(0);

                if (current.x == goalX && current.y == goalY) {
                    // return path
                    List<(int, int)> path = [];
                    AStarNode? pathNode = current;

                    while (pathNode is not null) {
                        if (boundsMap[pathNode.x, pathNode.y] == 0) {
                            path.Add((pathNode.x, pathNode.y));
                        }
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

                    int boundsValue = boundsMap[nx, ny];
                    const int zeroPenalty = 10;
                    int moveCost = boundsValue == 0 ? zeroPenalty : 1;

                    AStarNode neighbour = new(nx, ny) {
                        g = current.g + moveCost,
                        h = heuristics(nx, ny, goalX, goalY),
                        parent = current
                    };

                    AStarNode? existing = open.Find(n => n.x == nx && n.y == ny);

                    if (existing is not null && existing.g <= neighbour.g) {
                        continue;
                    }

                    open.Add(neighbour);
                }
            }

            return [];
        }

        int heuristics(int x1, int y1, int x2, int y2) {
            int dist = Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
            return dist;
        }
        int final(AStarNode node) {
            return node.g + node.h;
        }

        Tiles[,] result = new Tiles[width, height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                result[x, y] = boundsMap[x,y]==0?Tiles.Wall:Tiles.Space;
            }
        }

        return result;
    }
}