using System.Linq;

namespace LifeSim;

public class Plant : Organism
{
    private const int MatureAge = 6;
    private const double SpreadChance = 0.18;
    private const int MaxAge = 250;

    public Plant(World world, Point2 position, Gender? gender = null)
        : base(world, position, gender)
    {
    }

    public override char Glyph => '♣';

    public override System.ConsoleColor? Color => System.ConsoleColor.Green;

    public override void Tick()
    {
        base.Tick();

        TryExtend();

        TryDie();
    }

    private void TryDie()
    {
        if (Age > MaxAge && Rand.Chance(0.01))
        {
            World.Remove(this);
        }
    }

    private void TryExtend()
    {
        if (Age >= MatureAge && Rand.Chance(SpreadChance))
        {
            var spots = World.EmptyNeighbors8(Position).ToList();
            if (spots.Count > 0)
            {
                World.Add(new Plant(World, spots.Pick()!));
            }
        }
    }
}
