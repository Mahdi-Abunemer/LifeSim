using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeSim;

public sealed class WorldGrid
{
    private readonly Dictionary<Point2, Organism> _grid = new();
    private readonly List<Organism> _organisms = new();

    public WorldGrid(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public int Width { get; }
    public int Height { get; }

    public IEnumerable<Organism> All => _organisms.Where(o => o.IsAlive);

    public void Add(Organism organism)
    {
        if (_grid.ContainsKey(organism.Position))
        {
            return;
        }

        _organisms.Add(organism);
        _grid[organism.Position] = organism;
    }

    public void Remove(Organism organism)
    {
        if (!organism.IsAlive)
        {
            return;
        }

        organism.IsAlive = false;
        _grid.Remove(organism.Position);
    }

    public void MoveTo(Organism organism, Point2 newPosition)
    {
        if (!organism.IsAlive)
        {
            return;
        }

        var wrappedPosition = Wrap(newPosition);
        if (_grid.ContainsKey(wrappedPosition))
        {
            return;
        }

        _grid.Remove(organism.Position);
        organism.Position = wrappedPosition;
        _grid[wrappedPosition] = organism;
    }

    public bool IsEmpty(Point2 position) => !_grid.ContainsKey(Wrap(position));

    public Point2 Wrap(Point2 position)
    {
        var x = ((position.X % Width) + Width) % Width;
        var y = ((position.Y % Height) + Height) % Height;
        return new Point2(x, y);
    }

    public IEnumerable<Point2> Neighbors8(Point2 position)
    {
        for (var yDirectionStep = -1; yDirectionStep <= 1; yDirectionStep++)
        {
            for (var xDirectionStep = -1; xDirectionStep <= 1; xDirectionStep++)
            {
                if (xDirectionStep != 0 || yDirectionStep != 0)
                {
                    yield return Wrap(
                        new Point2(
                            position.X + xDirectionStep, 
                            position.Y + yDirectionStep));
                }
            }
        }
    }

    public IEnumerable<Point2> EmptyNeighbors8(Point2 position)
    {
        foreach (var n in Neighbors8(position))
        {
            if (IsEmpty(n))
            {
                yield return n;
            }
        }
    }

    public Point2? RandomEmptyCell()
    {
        for (var i = 0; i < 500; i++)
        {
            var position = new Point2(RandomHelper.Next(0, Width), RandomHelper.Next(0, Height));
            if (IsEmpty(position))
            {
                return position;
            }
        }

        var emptiesPosition = new List<Point2>();
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var position = new Point2(x, y);
                if (IsEmpty(position))
                {
                    emptiesPosition.Add(position);
                }
            }
        }

        return emptiesPosition.Count == 0 ? null : emptiesPosition.Pick();
    }

    public Organism? FindNearest<T>(Point2 fromPosition, int visionRange)
    where T : Organism
    {
        Organism? bestOrganism = null;
        var bestDistance = int.MaxValue;

        foreach (var organism in All)
        {
            if (organism is T)
            {
                var xDirectionStep = ToroidalDistance(fromPosition.X, organism.Position.X, Width);
                var yDirectionStep = ToroidalDistance(fromPosition.Y, organism.Position.Y, Height);
                var distance = xDirectionStep + yDirectionStep;
                if (distance <= visionRange && distance < bestDistance)
                {
                    bestOrganism = organism;
                    bestDistance = distance;
                }
            }
        }

        return bestOrganism;
    }

    public IReadOnlyDictionary<Point2, Organism> GridSnapshot()
    => new Dictionary<Point2, Organism>(_grid);

    public void RemoveAllDeadOrganisms()
    {
        _organisms.RemoveAll(organism => !organism.IsAlive);
    }

    private static int ToroidalDistance(int firstCoordinate, int secondCoordinate, int worldSize)
    {
        var directDistance = Math.Abs(firstCoordinate - secondCoordinate);
        return Math.Min(directDistance, worldSize - directDistance);
    }
}
