using System.Linq;

namespace LifeSim;

public class Plant : Organism
{
    private const int MatureAge = 6;
    private const double SpreadChance = 0.18;
    private const int MaxAge = 250;

    public Plant(World world, Point2 pos, Gender? gender = null)
        : base(world, pos, gender)
    {
    }

    public override char Glyph => '♣';

    public override System.ConsoleColor? Color => System.ConsoleColor.Green;

    public override void Tick()
    {
        base.Tick();

        if (Age >= MatureAge && Rand.Chance(SpreadChance))
        {
            var spots = World.EmptyNeighbors8(Pos).ToList();
            if (spots.Count > 0)
            {
                World.Add(new Plant(World, spots.Pick()!));
            }
        }

        if (Age > MaxAge && Rand.Chance(0.01))
        {
            World.Remove(this);
        }
    }
}
