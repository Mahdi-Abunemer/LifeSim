namespace LifeSim;

public class Predator : Animal
{
    public Predator(World world, Point2 position, Gender? gender = null)
        : base(world, position, gender)
    {
    }

    protected override int Vision => 12;

    protected override int MoveCost => 3;

    protected override int BiteGain => 28;

    protected override int ReproduceThreshold => 80;

    protected override int InitialEnergy => 40;

    protected override char SelfGlyph => 'W';

    public override System.ConsoleColor? Color => System.ConsoleColor.Red;

    protected override Organism? FindPrey() => World.FindNearest<Herbivore>(Position, Vision);

    protected override Animal MakeChild(Point2 p) => new Predator(World, p);
}
