using Day23;

int SolvePart(IReadOnlyList<string> input, State goal)
{
    var begin = new State(input);

    var seen = new HashSet<State>();

    var pq = new PriorityQueue<SearchNode, int>();
    pq.Enqueue(new SearchNode(begin, 0, null), 0);

    var minCost = 0;
    while (pq.TryDequeue(out var node, out var cost))
    {
        var (state, _, prevNode) = node;

        if (seen.Contains(state)) continue;
        seen.Add(state);

        if (cost > minCost) minCost = cost;

        if (state == goal)
            break;

        foreach (var move in state.PossibleMoves())
        {
            var newCost = cost + move.Cost;
            pq.Enqueue(new SearchNode(move.Next, newCost, node), newCost);
        }
    }

    return minCost;
}

var input = File.ReadAllLines("input");
var part2Input = input[..3].Concat(new[] { "  #D#C#B#A#  ", "  #D#B#A#C#  " }).Concat(input[3..]).ToList();

Console.WriteLine($"Part 1: {SolvePart(input, State.Part1Goal)}");
Console.WriteLine($"Part 2: {SolvePart(part2Input, State.Part2Goal)}");


internal record SearchNode(State State, int Cost, SearchNode? Prev);