using System;
using System.Collections.Generic;
using System.Linq;

public class MyBot
{
    public const string MyBotName = "Keep Summer Safe";
    public ushort myID;
    public Map map;
    
    public static void Main(string[] args) {
        Console.SetIn(Console.In);
        Console.SetOut(Console.Out);
        ushort myID;
        Map map = Networking.getInit(out myID);
        MyBot bot = new MyBot(myID);
        bot.map = map;

        /* ------
            Do more prep work, see rules for time limit
        ------ */
        bot.Analyze(ref map);

        Networking.SendInit(MyBotName); // Acknoweldge the init and begin the game

        while (true)
        {
            Networking.getFrame(ref map); // Update the map to reflect the moves before this turn
            bot.map = map;

            List<Move> moves;
            bot.ComputeMove(ref map, out moves);

            Networking.SendMoves(moves); // Send moves
        }
        
    }

    public MyBot(ushort myID)
    {
        this.myID = myID;
    }

    public void Analyze(ref Map map)
    {

    }

    public List<Move> ComputeMove(ref Map map, out List<Move> moves)
    {
        moves = new List<Move>();
        moves.Clear();

        for (ushort x = 0; x < map.Width; x++)
        {
            for (ushort y = 0; y < map.Height; y++)
            {
                if (map[x, y].Owner == myID)
                {
                    List<Neighbour> neighbours = GetImmediateNeighbours(x, y);
                    ushort bestProduction = 0;
                    Direction bestDirection = Direction.Still;

                    foreach (Neighbour neighbour in neighbours)
                    {
                        if (neighbour.Tile.Owner != myID && neighbour.Tile.Strength < map[x,y].Strength && neighbour.Tile.Production > bestProduction)
                        {
                            bestProduction = neighbour.Tile.Production;
                            bestDirection = neighbour.WhereIsThatNeighbour;
                        }
                    }

                    moves.Add(new Move
                    {
                        Location = new Location { X = x, Y = y },
                        Direction = bestDirection
                    });
                }
            }
        }

        return moves;
    }

    public List<Neighbour> GetImmediateNeighbours(ushort x, ushort y)
    {
        return new List<Neighbour>
        {
            new Neighbour
            {
                Location = new Location
                {
                    X = (ushort) (((x + map.Width) - 1) % map.Width),
                    Y = y
                },
                Tile = map[(ushort) (((x + map.Width) - 1) % map.Width), y],
                WhereIsThatNeighbour = Direction.West
            },
            new Neighbour
            {
                Location = new Location
                {
                    X = (ushort) (((x) + 1) % map.Width),
                    Y = y
                },
                Tile = map[(ushort) (((x) + 1) % map.Width), y],
                WhereIsThatNeighbour = Direction.East
            },
            new Neighbour
            {
                Location = new Location
                {
                    X = x,
                    Y = (ushort) (((y + map.Height) - 1) % map.Height)
                },
                Tile = map[x, (ushort) (((y + map.Height) - 1) % map.Height)],
                WhereIsThatNeighbour = Direction.North
            },
            new Neighbour
            {
                Location = new Location
                {
                    X = x,
                    Y = (ushort) (((y) + 1) % map.Height)
                },
                Tile = map[x, (ushort) (((y) + 1) % map.Height)],
                WhereIsThatNeighbour = Direction.South
            }
        };
    }

    public Direction GetDirectionToTargetLongestAxis(Location source, Location destination)
    {
        int[] distances = GetShortestDistances(source, destination);
        if(distances[0] > distances[1])
        {
            if(getWithinBounds((int)destination.X - (int)source.X, map.Width) == distances[0])
            {
                return Direction.East;
            }
            else
            {
                return Direction.West;
            }
        }
        else
        {
            if (getWithinBounds((int)destination.Y - (int)source.Y, map.Height) == distances[1])
            {
                return Direction.South;
            }
            else
            {
                return Direction.North;
            }
        }
    }

    public int[] GetShortestDistances(Location source, Location destination)
    {
        return new int[]
        {
            GetShortestDistance(getWithinBounds((int)destination.X - (int)source.X, map.Width), map.Width),
            GetShortestDistance(getWithinBounds((int)destination.Y - (int)source.Y, map.Height), map.Height)
        };
    }

    public int DistanceManhattan(Location source, Location destination)
    {
        return GetShortestDistances(source, destination).Sum();
    }

    public int GetShortestDistance(int distance, int max)
    {
        return Math.Min(distance, max - distance);
    }

    public int getWithinBounds(int num, int max)
    {
        var tmp = num;
        while(tmp < 0)
        {
            tmp += max;
        }
        while(tmp >= max)
        {
            tmp -= max;
        }
        return tmp;
    }

    public class Neighbour
    {
        public Site Tile { get; set; }
        public Location Location { get; set; }
        public Direction WhereIsThatNeighbour { get; set; }
    }
}
