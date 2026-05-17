using System.Linq;
using LifeSim;
using Xunit;

namespace LifeSim.Tests;

public class StepTests
{
    [Fact]
    public void Step_IncrementsTickAndOrganismAge()
    {
        var world = new World(3, 3);
        var organism = new AgingOrganism(world, new Point2(0, 0));
        world.Add(organism);

        world.Step();

        Assert.Equal(1, world.Tick);
        Assert.Equal(1, organism.Age);
    }

    [Fact]
    public void Step_RemovesDeadOrganismsFromAliveCollection()
    {
        var world = new World(3, 3);
        var organism = new SelfRemovingOrganism(world, new Point2(0, 0));
        world.Add(organism);

        world.Step();

        Assert.Empty(world.All);
    }

    [Fact]
    public void SerializeWorldSnapshot_ContainsTickAndOrganismCoordinates()
    {
        var world = new World(3, 3);
        world.Add(new Plant(world, new Point2(1, 2)));
        world.Step();

        var snapshot = world.SerializeWorldSnapshot();

        Assert.Contains("Tick=1", snapshot);
        Assert.Contains("Plant@1,2", snapshot);
    }

    private sealed class AgingOrganism : Organism
    {
        public AgingOrganism(World world, Point2 pos) : base(world, pos)
        {
        }

        public override char Glyph => 'a';
    }

    private sealed class SelfRemovingOrganism : Organism
    {
        public SelfRemovingOrganism(World world, Point2 pos) : base(world, pos)
        {
        }

        public override char Glyph => 'x';

        public override void Tick()
        {
            base.Tick();
            World.Remove(this);
        }
    }
}
