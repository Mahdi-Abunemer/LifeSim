using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeSim;

public class World
{
    private readonly WorldGrid _worldGrid;

    public World(WorldGrid worldGrid)
    {
        _worldGrid = worldGrid;
    }

    public int Tick { get; private set; }

    public int Width => _worldGrid.Width;

    public int Height => _worldGrid.Height;

    public IEnumerable<Organism> AllOrganisms => _worldGrid.All;

    public void Add(Organism organism)
    {
        _worldGrid.Add(organism);
    }

    public void Remove(Organism organism)
    {
        _worldGrid.Remove(organism);
    }

    public void MoveTo(Organism organism, Point2 newPosition)
    {
        _worldGrid.MoveTo(organism, newPosition);
    }

    public bool IsEmpty(Point2 position) => _worldGrid.IsEmpty(position);

    public Point2 Wrap(Point2 p)
    {
        return _worldGrid.Wrap(p);
    }

    public void Step()
    {
        Tick++;

        var snapshotOrganismsList = AllOrganisms
            .OrderBy(_ => RandomHelper.Next(0, int.MaxValue)).ToList();

        foreach (var organism in snapshotOrganismsList)
        {
            if (organism.IsAlive)
            {
                organism.Tick();
            }
        }

        _worldGrid.RemoveAllDeadOrganisms();
    }

    public IEnumerable<Point2> Neighbors8(Point2 position)
    {
        return _worldGrid.Neighbors8(position);
    }

    public IEnumerable<Point2> EmptyNeighbors8(Point2 position)
    {
        return _worldGrid.EmptyNeighbors8(position);
    }

    public void Seed(int count , IOrganismFactory organismFactory)
    {
        for (var i = 0; i < count; i++)
        {
            var position = RandomEmptyCell();
            if (position == null)
            {
                break;
            }

            Add(organismFactory.Create(this, position.Value));
        }
    }

    public Point2? RandomEmptyCell()
    {
        return _worldGrid.RandomEmptyCell();
    }

    public Organism? FindNearest<T>(Point2 fromPosition, int visionRange)
        where T : Organism
    {
        return _worldGrid.FindNearest<T>(fromPosition, visionRange);
    }

    public string SerializeWorldSnapshot()
    {
        var items = AllOrganisms.Select(o => $"{o.GetType().Name}@{o.Position.X},{o.Position.Y}");
        return $"Tick={Tick} | {string.Join(";", items)}";
    }

    public IReadOnlyDictionary<Point2, Organism> GridSnapshot() 
        => _worldGrid.GridSnapshot();
}
