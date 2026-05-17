using System.Linq;
using LifeSim;
using Xunit;

namespace LifeSim.Tests;

public class WorldTests
{
    [Fact]
    public void Wrap_ReturnsToroidalCoordinates()
    {
        var world = new World(5, 4);

        var wrapped = world.Wrap(new Point2(-1, 5));

        Assert.Equal(new Point2(4, 1), wrapped);
    }

    [Fact]
    public void Add_DoesNotAddSecondOrganismToSameCell()
    {
        var world = new World(5, 5);
        var first = new Plant(world, new Point2(1, 1));
        var second = new Plant(world, new Point2(1, 1));

        world.Add(first);
        world.Add(second);

        Assert.Single(world.All);
    }

    [Fact]
    public void MoveTo_MovesAndWrapsWhenTargetIsEmpty()
    {
        var world = new World(5, 5);
        var plant = new Plant(world, new Point2(4, 4));
        world.Add(plant);

        world.MoveTo(plant, new Point2(5, 4));

        Assert.Equal(new Point2(0, 4), plant.Pos);
    }

    [Fact]
    public void MoveTo_DoesNotMoveToOccupiedCell()
    {
        var world = new World(5, 5);
        var first = new Plant(world, new Point2(0, 0));
        var second = new Plant(world, new Point2(1, 0));
        world.Add(first);
        world.Add(second);

        world.MoveTo(first, second.Pos);

        Assert.Equal(new Point2(0, 0), first.Pos);
    }

    [Fact]
    public void Seed_DoesNotExceedWorldCapacity()
    {
        var world = new World(2, 2);

        world.Seed<Plant>(100);

        Assert.Equal(4, world.All.Count());
    }

    [Fact]
    public void FindNearest_UsesToroidalDistanceAndVision()
    {
        var world = new World(10, 10);
        var seekerPoint = new Point2(0, 0);
        var nearAcrossBorder = new Plant(world, new Point2(9, 0));
        var farTarget = new Plant(world, new Point2(5, 5));
        world.Add(nearAcrossBorder);
        world.Add(farTarget);

        var found = world.FindNearest<Plant>(seekerPoint, 2);

        Assert.Same(nearAcrossBorder, found);
    }

    [Fact]
    public void RandomEmptyCell_ReturnsNullWhenWorldIsFull()
    {
        var world = new World(1, 1);
        world.Add(new Plant(world, new Point2(0, 0)));

        var empty = world.RandomEmptyCell();

        Assert.Null(empty);
    }
}
