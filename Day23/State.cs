using System.Runtime.CompilerServices;
using System.Text;

namespace Day23;

public struct State : IEquatable<State>
{
    private readonly char[] _hallway = { '.', '.', '.', '.', '.', '.', '.' };
    private readonly char[][] _rooms = new char[4][];

    private State(char[] hallway, char[][] rooms)
    {
        _hallway = hallway;
        _rooms = rooms;
    }

    public bool Equals(State other)
    {
        if (!_hallway.SequenceEqual(other._hallway)) return false;
        for (var i = 0; i < 4; i++)
        {
            if (!_rooms[i].SequenceEqual(other._rooms[i]))
                return false;
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        return obj is State other && Equals(other);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var c in _hallway) hash.Add(c);
        foreach (var room in _rooms)
        foreach (var c in room)
            hash.Add(c);

        return hash.ToHashCode();
    }

    public static bool operator ==(State left, State right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(State left, State right)
    {
        return !left.Equals(right);
    }


    private static readonly char[] CorrectOccupant = { 'A', 'B', 'C', 'D' };

    private static readonly Dictionary<char, int> CorrectRoom = new()
    {
        ['A'] = 0,
        ['B'] = 1,
        ['C'] = 2,
        ['D'] = 3,
    };

    private static readonly Dictionary<char, int> CostMap = new()
    {
        ['A'] = 1,
        ['B'] = 10,
        ['C'] = 100,
        ['D'] = 1000,
    };

    // Moves from the top-most square of the room (not the one inside the hallway)
    private static readonly int[][] RoomIntoHallwayDistance =
    {
        new[] { 3, 2, 2, 4, 6, 8, 9 },
        new[] { 5, 4, 2, 2, 4, 6, 7 },
        new[] { 7, 6, 4, 2, 2, 4, 5 },
        new[] { 9, 8, 6, 4, 2, 2, 3 },
    };

    private static readonly HashSet<int>[] ImmediatelyAccessibleRooms =
    {
        new(),
        new() { 0 },
        new() { 0, 1 },
        new() { 1, 2 },
        new() { 2, 3 },
        new() { 3 },
        new(),
    };

    private bool RoomAvailable(int room)
    {
        var correct = CorrectOccupant[room];
        foreach (var occupant in _rooms[room])
        {
            if (occupant is not '.' && occupant != correct)
                return false;
        }

        return true;
    }

    private bool RoomAccessible(int room, int hallway)
    {
        var leftRoom = hallway - 3;
        if (leftRoom == room) return true;

        var rightRoom = hallway - 2;
        if (rightRoom == room) return true;

        var leftHallway = room + 1;
        var rightHallway = room + 2;

        if (room < leftRoom)
        {
            // Look left
            for (var x = hallway - 1; x >= 0; x--)
            {
                if (_hallway[x] is not '.') break;
                var lookLeft = x - 3;
                if (lookLeft == room) return true;
            }
        }

        if (rightRoom < room)
        {
            // Look right
            for (var x = hallway + 1; x < _hallway.Length; x++)
            {
                if (_hallway[x] is not '.') break;
                var lookRight = x - 2;
                if (lookRight == room) return true;
            }
        }

        return false;
    }

    private Move? HallwayToRoom(int hallway)
    {
        var toMove = _hallway[hallway];
        var room = CorrectRoom[toMove];

        if (!RoomAvailable(room)) return null;

        if (!RoomAccessible(room, hallway)) return null;

        // Everything checks out. Move inside
        var hallwayCopy = _hallway.ToArray();
        hallwayCopy[hallway] = '.';

        var distanceMoved = RoomIntoHallwayDistance[room][hallway];

        var roomsCopy = _rooms.ToArray();
        var roomCopy = roomsCopy[room].ToArray();
        roomsCopy[room] = roomCopy;

        for (var i = roomCopy.Length - 1; i >= 0; i--)
        {
            if (roomCopy[i] is not '.') continue;
            roomCopy[i] = toMove;
            distanceMoved = distanceMoved + i;
            break;
        }

        return new Move(new State(hallwayCopy, roomsCopy), distanceMoved * CostMap[toMove]);
    }

    private bool RoomDone(int i)
    {
        var room = _rooms[i];
        var correct = CorrectOccupant.First();
        return room.All(c => c == correct);
    }

    public IEnumerable<Move> PossibleMoves()
    {
        for (var i = 0; i < _rooms.Length; i++)
        {
            if (RoomDone(i)) continue;
            for (var j = 0; j < _rooms[i].Length; j++)
            {
                var c = _rooms[i][j];
                if (c is '.') continue;

                foreach (var step in RoomToHallways(i, j)) yield return step;
                break;
            }
        }

        for (var i = 0; i < _hallway.Length; i++)
        {
            var c = _hallway[i];
            if (c is '.') continue;
            var intoRoom = HallwayToRoom(i);
            if (intoRoom is not null) yield return intoRoom;
        }
    }

    private IEnumerable<Move> RoomToHallways(int room, int index)
    {
        var leftHallway = room + 1;
        var rightHallway = leftHallway + 1;

        var availableHallway = new List<int>();

        for (var x = leftHallway; x >= 0; x--)
        {
            var c = _hallway[x];
            if (c != '.') break;

            availableHallway.Add(x);
        }

        for (var x = rightHallway; x < _hallway.Length; x++)
        {
            var c = _hallway[x];
            if (c != '.') break;

            availableHallway.Add(x);
        }

        var roomsCopy = _rooms.ToArray();
        var roomCopy = roomsCopy[room].ToArray();
        roomsCopy[room] = roomCopy;
        var toMove = roomCopy[index];
        roomCopy[index] = '.';

        foreach (var hallway in availableHallway)
        {
            var hallwayCopy = _hallway.ToArray();
            hallwayCopy[hallway] = toMove;

            var distance = RoomIntoHallwayDistance[room][hallway];
            distance += index;

            yield return new Move(new State(hallwayCopy, roomsCopy), CostMap[toMove] * distance);
        }
    }

    public static State Part1Goal = new(new[]
    {
        "#############",
        "#...........#",
        "###A#B#C#D###",
        "  #A#B#C#D#   ",
        "  #########   ",
    });

    public static State Part2Goal = new(new[]
    {
        "#############",
        "#...........#",
        "###A#B#C#D###",
        "  #A#B#C#D#   ",
        "  #A#B#C#D#   ",
        "  #A#B#C#D#   ",
        "  #########   ",
    });

    public State(IReadOnlyList<string> input)
    {
        for (var i = 0; i < 4; i++) _rooms[i] = new char[input.Count - 3];

        for (var i = 0; i < _rooms.Length; i++)
        for (var j = 0; j < _rooms[0].Length; j++)
            _rooms[i][j] = input[j + 2][3 + i * 2];
    }

    public override string ToString()
    {
        var s = new StringBuilder();
        s.AppendLine();
        var extraHallway = 0;
        for (var i = 0; i < _hallway.Length + 4; i++)
        {
            switch (i)
            {
                case 2 or 4 or 6 or 8:
                    s.Append(' ');
                    extraHallway += 1;
                    break;
                default:
                    s.Append(_hallway[i - extraHallway]);
                    break;
            }
        }

        s.AppendLine();
        for (var j = 0; j < _rooms[0].Length; j++)
        {
            s.Append("  ");
            for (var i = 0; i < 4; i++)
            {
                s.Append(_rooms[i][j]);
                s.Append(' ');
            }

            s.AppendLine();
        }

        return s.ToString();
    }
}

public record Move(State Next, int Cost);