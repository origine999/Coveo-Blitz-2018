using System;
using System.Collections.Generic;
using System.Linq;

public class MyBot
{
    public const string MyBotName = "Keep Summer Safe";
    public ushort myID;
    public List<Move> moves;

    public static void Main(string[] args) {
        Console.SetIn(Console.In);
        Console.SetOut(Console.Out);
        ushort myID;
        Map map = Networking.getInit(out myID);
        MyBot bot = new MyBot(myID);

        /* ------
            Do more prep work, see rules for time limit
        ------ */
        bot.Analyze(ref map);

        Networking.SendInit(MyBotName); // Acknoweldge the init and begin the game

        while (true)
        {
            Networking.getFrame(ref map); // Update the map to reflect the moves before this turn

            var moves = bot.ComputeMove(ref map);

            Networking.SendMoves(moves); // Send moves
        }
        
    }

    public MyBot(ushort myID)
    {
        this.myID = myID;
        moves = new List<Move>();
    }

    public void Analyze(ref Map map)
    {

    }

    public List<Move> ComputeMove(ref Map map)
    {
        moves.Clear();
        var random = new Random();
        
        for (ushort x = 0; x < map.Width; x++)
        {
            for (ushort y = 0; y < map.Height; y++)
            {
                if (map[x, y].Owner == myID)
                {
                    moves.Add(new Move
                    {
                        Location = new Location { X = x, Y = y },
                        Direction = (Direction)random.Next(5)
                    });
                }
            }
        }

        return moves;
    }

    public List<Neighbour> GetImmediateNeighbours(ushort x, ushort y)
    {
        return null;
    }

    public class Neighbour
    {
        public Site tile { get; set; }
        public Direction whereIsThatNeighbour { get; set; }
    }
}
