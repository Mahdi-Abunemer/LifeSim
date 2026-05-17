using System;
using LifeSim;
using Xunit;

namespace LifeSim.Tests;

public class GenderTests
{
    [Fact]
    public void Plant_HasDefinedRandomGender()
    {
        var world = new World(2, 2);
        var plant = new Plant(world, new Point2(0, 0));

        Assert.True(Enum.IsDefined(typeof(Gender), plant.Gender));
    }

    [Fact]
    public void Animal_CanBeCreatedWithSpecificGender()
    {
        var world = new World(2, 2);
        var herbivore = new Herbivore(world, new Point2(1, 1), Gender.Female);

        Assert.Equal(Gender.Female, herbivore.Gender);
    }
}
