using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeSim;

public class World
{
    private readonly Dictionary<Point2, Organism> _grid = new();
    private readonly List<Organism> _organisms = new();

    public World(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public int Width { get; }

    public int Height { get; }

    public int Tick { get; private set; }

    public IEnumerable<Organism> All => _organisms.Where(o => o.IsAlive);

    public void Add(Organism org)
    {
        if (_grid.ContainsKey(org.Position))
        {
            return;
        }

        _organisms.Add(org);
        _grid[org.Position] = org;
    }

    public void Remove(Organism org)
    {
        if (!org.IsAlive)
        {
            return;
        }

        org.IsAlive = false;
        _grid.Remove(org.Position);
    }

    public void MoveTo(Organism org, Point2 newPos)
    {
        if (!org.IsAlive)
        {
            return;
        }

        var wrappedPos = Wrap(newPos);
        if (_grid.ContainsKey(wrappedPos))
        {
            return;
        }

        _grid.Remove(org.Position);
        org.Position = wrappedPos;
        _grid[wrappedPos] = org;
    }

    public bool IsEmpty(Point2 p) => !_grid.ContainsKey(Wrap(p));

    public Point2 Wrap(Point2 p)
    {
        var x = ((p.X % Width) + Width) % Width;
        var y = ((p.Y % Height) + Height) % Height;
        return new Point2(x, y);
    }

    public void Step()
    {
        Tick++;
        var snapshot = All.OrderBy(_ => RandomHelper.Next(0, int.MaxValue)).ToList();
        foreach (var o in snapshot)
        {
            if (o.IsAlive)
            {
                o.Tick();
            }
        }

        _organisms.RemoveAll(o => !o.IsAlive);
    }

    public IEnumerable<Point2> Neighbors8(Point2 p)
    {
        for (var dy = -1; dy <= 1; dy++)
        {
            for (var dx = -1; dx <= 1; dx++)
            {
                if (dx != 0 || dy != 0)
                {
                    yield return Wrap(new Point2(p.X + dx, p.Y + dy));
                }
            }
        }
    }

    public IEnumerable<Point2> EmptyNeighbors8(Point2 p)
    {
        foreach (var n in Neighbors8(p))
        {
            if (IsEmpty(n))
            {
                yield return n;
            }
        }
    }

    public void Seed(int count , IOrganismFactory organismFactory)
    {
        for (var i = 0; i < count; i++)
        {
            var p = RandomEmptyCell();
            if (p == null)
            {
                break;
            }

            Add(organismFactory.Create(this, p.Value));
        }
    }

    public Point2? RandomEmptyCell()
    {
        for (var i = 0; i < 500; i++)
        {
            var p = new Point2(RandomHelper.Next(0, Width), RandomHelper.Next(0, Height));
            if (IsEmpty(p))
            {
                return p;
            }
        }

        var empties = new List<Point2>();
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var p = new Point2(x, y);
                if (IsEmpty(p))
                {
                    empties.Add(p);
                }
            }
        }

        return empties.Count == 0 ? null : empties.Pick();
    }

    public Organism? FindNearest<T>(Point2 from, int visionRange)
        where T : Organism
    {
        Organism? best = null;
        var bestDist = int.MaxValue;

        foreach (var o in All)
        {
            if (o is T)
            {
                var dx = ToroidalDistance(from.X, o.Position.X, Width);
                var dy = ToroidalDistance(from.Y, o.Position.Y, Height);
                var distance = dx + dy;
                if (distance <= visionRange && distance < bestDist)
                {
                    best = o;
                    bestDist = distance;
                }
            }
        }

        return best;
    }

    public string SerializeWorldSnapshot()
    {
        var items = All.Select(o => $"{o.GetType().Name}@{o.Position.X},{o.Position.Y}");
        return $"Tick={Tick} | {string.Join(";", items)}";
    }

    public IReadOnlyDictionary<Point2, Organism> GridSnapshot() => new Dictionary<Point2, Organism>(_grid);

    private static int ToroidalDistance(int a, int b, int size)
    {
        var diff = Math.Abs(a - b);
        return Math.Min(diff, size - diff);
    }
}
