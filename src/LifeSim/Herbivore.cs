namespace LifeSim;

public class Herbivore : Animal
{
    public Herbivore(World world, Point2 position, Gender? gender = null)
        : base(world, position, gender)
    {
    }

    protected override int Vision => 8;

    protected override int MoveCost => 2;

    protected override int BiteGain => 18;

    protected override int ReproduceThreshold => 60;

    protected override int InitialEnergy => 30;

    protected override char SelfGlyph => 'h';

    public override System.ConsoleColor? Color => System.ConsoleColor.Yellow;

    protected override Organism? FindPrey() => World.FindNearest<Plant>(Position, Vision);

    protected override Animal MakeChild(Point2 p) => new Herbivore(World, p);
}
