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

    public IEnumerable<Organism> All => _worldGrid.All;

    public void Add(Organism org)
    {
        _worldGrid.Add(org);
    }

    public void Remove(Organism org)
    {
        _worldGrid.Remove(org);
    }

    public void MoveTo(Organism org, Point2 newPos)
    {
        _worldGrid.MoveTo(org, newPos);
    }

    public bool IsEmpty(Point2 p) => _worldGrid.IsEmpty(p);

    public Point2 Wrap(Point2 p)
    {
        return _worldGrid.Wrap(p);
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

        _worldGrid.RemoveAllDeadOrganisms();
    }

    public IEnumerable<Point2> Neighbors8(Point2 p)
    {
        return _worldGrid.Neighbors8(p);
    }

    public IEnumerable<Point2> EmptyNeighbors8(Point2 p)
    {
        return _worldGrid.EmptyNeighbors8(p);
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
        return _worldGrid.RandomEmptyCell();
    }

    public Organism? FindNearest<T>(Point2 from, int visionRange)
        where T : Organism
    {
        return _worldGrid.FindNearest<T>(from, visionRange);
    }

    public string SerializeWorldSnapshot()
    {
        var items = All.Select(o => $"{o.GetType().Name}@{o.Position.X},{o.Position.Y}");
        return $"Tick={Tick} | {string.Join(";", items)}";
    }

    public IReadOnlyDictionary<Point2, Organism> GridSnapshot() 
        => _worldGrid.GridSnapshot();
}
