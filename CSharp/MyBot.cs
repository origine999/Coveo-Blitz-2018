using System;
using System.Collections.Generic;
using System.Linq;

public class MyBot
{
    public const string MyBotName = "Keep Summer Safe";
    public const ushort NeutralID = 0;
    public ushort myID;
    public Map map;

    public List<Location> warriors = new List<Location>();
    public List<Location> helpers = new List<Location>();
    public List<Location> miners = new List<Location>();

    public List<Location> neutrals = new List<Location>();
    public List<Location> enemies = new List<Location>();

    public ushort treshold = 25;
    
    public static void Main(string[] args) {
        //Log.Setup(@"C:\Users\phili\Desktop\Coveo-Blitz-2018\CSharp\log.txt");

        Console.SetIn(Console.In);
        Console.SetOut(Console.Out);
        ushort myID;
        Map map = Networking.getInit(out myID);
        MyBot bot = new MyBot(myID);
        bot.map = map;

        /* ------
            Do more prep work, see rules for time limit
        ------ */
        bot.Analyze();

        Networking.SendInit(MyBotName); // Acknoweldge the init and begin the game

        while (true)
        {
            Networking.getFrame(ref map); // Update the map to reflect the moves before this turn
            bot.map = map;

            List<Move> moves;
            bot.ComputeMove(out moves);

            Networking.SendMoves(moves); // Send moves
        }
        
    }

    public MyBot(ushort myID)
    {
        this.myID = myID;
    }

    public void Analyze()
    {

    }

    public List<Move> ComputeMove(out List<Move> moves)
    {
        moves = new List<Move>();

        AssignLists();

        foreach (var warrior in warriors)
        {
            Neighbour target = GetImmediateNeighbours(warrior).Where(n => n.Tile.Owner != myID && n.Tile.Strength < map[warrior].Strength).OrderByDescending(n => n.Tile.Production).FirstOrDefault();

            moves.Add(new Move
            {
                Location = warrior,
                Direction = target?.WhereIsThatNeighbour ?? Direction.Still
            });
        }

        foreach (var helper in helpers)
        {
            Neighbour warrior = GetImmediateNeighbours(helper).Where(n => warriors.Contains(n.Location)).First();
            if (map[helper].Strength >= map[helper].Production * 2)
            {
                moves.Add(new Move
                {
                    Location = helper,
                    Direction = GetDirectionToTargetLongestAxis(helper, warrior.Location)
                });
            }
            else
            {
                moves.Add(new Move
                {
                    Location = helper,
                    Direction = Direction.Still
                });
            }
        }

        foreach (var miner in miners)
        {
            if (map[miner].Strength >= map[miner].Production * 3)
            {
                Location target = warriors.OrderBy(n => DistanceManhattan(miner, n)).First();

                moves.Add(new Move
                {
                    Location = miner,
                    Direction = GetDirectionToTargetLongestAxis(miner, target)
                });
            }
            else
            {
                moves.Add(new Move
                {
                    Location = miner,
                    Direction = Direction.Still
                });
            }
        }

        return moves;
    }

    public void AssignLists()
    {
        warriors.Clear();
        helpers.Clear();
        miners.Clear();
        neutrals.Clear();
        enemies.Clear();

        for (ushort x = 0; x < map.Width; x++)
        {
            for (ushort y = 0; y < map.Height; y++)
            {
                if (map[x, y].Owner == myID)
                {
                    WarriorOrMiner(x, y);
                }
                else if (map[x, y].Owner != NeutralID)
                {
                    enemies.Add(new Location { X = x, Y = y });
                }
                else if (map[x, y].Production != 0)
                {
                    neutrals.Add(new Location { X = x, Y = y });
                }
            }
        }

        List<Location> minersToDelete = new List<Location>();
        foreach (var miner in miners)
        {
            if (GetImmediateNeighbours(miner.X, miner.Y).Any(n => warriors.Contains(n.Location)))
            {
                helpers.Add(miner);
                minersToDelete.Add(miner);
            }
        }

        foreach (var miner in minersToDelete)
        {
            miners.Remove(miner);
        }
    }

    public void WarriorOrMiner(ushort x, ushort y)
    {
        List<Neighbour> neighbours = GetImmediateNeighbours(x, y);

        if (neighbours.Any(s => s.Tile.Owner != myID))
        {
            warriors.Add(new Location { X = x, Y = y });
        }
        else
        {
            miners.Add(new Location { X = x, Y = y });
        }

        
    }

    public List<Neighbour> GetImmediateNeighbours(Location location)
    {
        return GetImmediateNeighbours(location.X, location.Y);
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
