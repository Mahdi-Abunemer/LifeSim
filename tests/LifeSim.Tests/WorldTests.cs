using System.Linq;
using LifeSim;
using Xunit;

namespace LifeSim.Tests;

public class WorldTests
{
    [Fact]
    public void Wrap_ReturnsToroidalCoordinates()
    {
        var worldGrid = new WorldGrid(5, 4);
        var world = new World(worldGrid);

        var wrapped = world.Wrap(new Point2(-1, 5));

        Assert.Equal(new Point2(4, 1), wrapped);
    }

    [Fact]
    public void Add_DoesNotAddSecondOrganismToSameCell()
    {
        var worldGrid = new WorldGrid(5, 5);
        var world = new World(worldGrid);
        var first = new Plant(world, new Point2(1, 1));
        var second = new Plant(world, new Point2(1, 1));

        world.Add(first);
        world.Add(second);

        Assert.Single(world.All);
    }

    [Fact]
    public void MoveTo_MovesAndWrapsWhenTargetIsEmpty()
    {
        var worldGrid = new WorldGrid(5, 5);
        var world = new World(worldGrid);
        var plant = new Plant(world, new Point2(4, 4));
        world.Add(plant);

        world.MoveTo(plant, new Point2(5, 4));

        Assert.Equal(new Point2(0, 4), plant.Position);
    }

    [Fact]
    public void MoveTo_DoesNotMoveToOccupiedCell()
    {
        var worldGrid = new WorldGrid(5, 5);
        var world = new World(worldGrid);
        var first = new Plant(world, new Point2(0, 0));
        var second = new Plant(world, new Point2(1, 0));
        world.Add(first);
        world.Add(second);

        world.MoveTo(first, second.Position);

        Assert.Equal(new Point2(0, 0), first.Position);
    }

    [Fact]
    public void Seed_DoesNotExceedWorldCapacity()
    {
        var worldGrid = new WorldGrid(2, 2);
        var world = new World(worldGrid);
        var plantFactory = new PlantFactory();
        world.Seed(100, plantFactory);

        Assert.Equal(4, world.All.Count());
    }

    [Fact]
    public void FindNearest_UsesToroidalDistanceAndVision()
    {
        var worldGrid = new WorldGrid(10, 10);
        var world = new World(worldGrid);
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
        var worldGrid = new WorldGrid(1, 1);
        var world = new World(worldGrid);
        world.Add(new Plant(world, new Point2(0, 0)));

        var empty = world.RandomEmptyCell();

        Assert.Null(empty);
    }
}
