using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeSim;

public abstract class Animal : Organism
{
    private int Energy { get; set; }

    private int MaxAge { get; set; } = 1000;

    protected abstract int Vision { get; }

    protected abstract int MoveCost { get; }

    protected abstract int BiteGain { get; }

    protected abstract int ReproduceThreshold { get; }

    protected abstract int InitialEnergy { get; }

    protected abstract char SelfGlyph { get; }

    public override char Glyph => SelfGlyph;

    public override ConsoleColor? Color => ConsoleColor.White;

    protected Animal(World world, Point2 position, Gender? gender = null)
    : base(world, position, gender)
    {
    }

    public override void Tick()
    {
        base.Tick();

        InitializeEnergy();

        HuntOrWander();

        ExpendEnergy();

        TryReproduce();

        TryDie();
    }

    protected abstract Organism? FindPrey();

    protected abstract Animal MakeChild(Point2 p);

    private static bool AreNeighborsOrSame(Point2 a, Point2 b) =>
        Math.Abs(a.X - b.X) <= 1 && Math.Abs(a.Y - b.Y) <= 1;

    private void StepToward(Point2 target)
    {
        var xDirectionStep = BestToroidalStep(Position.X, target.X, World.Width);
        var yxDirectionStep = BestToroidalStep(Position.Y, target.Y, World.Height);

        var candidates = new List<Point2>();
        if (xDirectionStep != 0)
        {
            candidates.Add(World.Wrap(new Point2(Position.X + xDirectionStep, Position.Y)));
        }

        if (yxDirectionStep != 0)
        {
            candidates.Add(World.Wrap(new Point2(Position.X, Position.Y + yxDirectionStep)));
        }

        if (xDirectionStep != 0 && yxDirectionStep != 0)
        {
            candidates.Add(World.Wrap(new Point2(Position.X + xDirectionStep, Position.Y + yxDirectionStep)));
        }

        var freePositions = candidates.Where(World.IsEmpty).ToList();
        if (freePositions.Count == 0)
        {
            Wander();
            return;
        }

        World.MoveTo(this, freePositions.Pick()!);
    }

    private void Wander()
    {
        var positionOptions = World.EmptyNeighbors8(Position).ToList();
        if (positionOptions.Count > 0)
        {
            World.MoveTo(this, positionOptions.Pick()!);
        }
    }

    private static int BestToroidalStep(int from, int to, int size)
    {
        var direct = to - from;
        var wrapA = (to + size) - from;
        var wrapB = to - (from + size);

        var best =
            Math.Abs(direct) <= Math.Abs(wrapA) && Math.Abs(direct) <= Math.Abs(wrapB)
                ? direct
                : Math.Abs(wrapA) < Math.Abs(wrapB)
                    ? wrapA
                    : wrapB;

        return Math.Sign(best);
    }

    private void TryDie()
    {
        if (Energy <= 0 || IsOldAndNoChance())
        {
            World.Remove(this);
        }
    }

    private bool IsOldAndNoChance()
    {
        return (Age > MaxAge && Rand.Chance(0.02));
    }

    private void TryReproduce()
    {
        if (Energy >= ReproduceThreshold)
        {
            var emptyNeighbors = World.EmptyNeighbors8(Position).ToList();
            if (emptyNeighbors.Count > 0)
            {
                var child = MakeChild(emptyNeighbors.Pick()!);
                Energy /= 2;
                World.Add(child);
            }
        }
    }

    private void ExpendEnergy()
    {
        Energy -= MoveCost;
    }

    private void HuntOrWander()
    {
        var prey = FindPrey();
        if (prey != null)
        {
            Hunt(prey);
        }
        else
        {
            Wander();
        }
    }

    private void Hunt(Organism prey)
    {
        StepToward(prey.Position);
        if (AreNeighborsOrSame(Position, prey.Position) && prey.IsAlive)
        {
            World.Remove(prey);
            Energy += BiteGain;
        }
    }

    private void InitializeEnergy()
    {
        if (Age == 1 && Energy == 0)
        {
            Energy = InitialEnergy;
        }
    }
}
