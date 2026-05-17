using System;
using System.Linq;
using System.Threading;

namespace LifeSim;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;

        const int width = 50;
        const int height = 22;
        var initialPlants = (int)(width * height * 0.22);
        const int initialHerbivores = 28;
        const int initialPredators = 10;

        var world = new World(width, height);
        world.Seed<Plant>(initialPlants);
        world.Seed<Herbivore>(initialHerbivores);
        world.Seed<Predator>(initialPredators);

        var paused = false;
        const int delayMs = 120;

        while (true)
        {
            while (!Console.IsInputRedirected && Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Q || key == ConsoleKey.Escape)
                {
                    Console.ResetColor();
                    Console.CursorVisible = true;
                    return;
                }

                if (key == ConsoleKey.Spacebar || key == ConsoleKey.P)
                {
                    paused = !paused;
                }
            }

            if (!paused)
            {
                world.Step();
                RenderWorld(world);
            }

            Thread.Sleep(delayMs);
        }
    }

    private static void RenderWorld(World world)
    {
        Console.SetCursorPosition(0, 0);

        var plants = world.All.OfType<Plant>().Count();
        var herbs = world.All.OfType<Herbivore>().Count();
        var preds = world.All.OfType<Predator>().Count();

        Console.ResetColor();
        Console.WriteLine($"Tick: {world.Tick,-8}  Plants: {plants,-5}  Herbivores: {herbs,-5}  Predators: {preds,-5}   [Space/P] pause, [Q/Esc] quit");

        var snapshot = world.GridSnapshot();
        for (var y = 0; y < world.Height; y++)
        {
            for (var x = 0; x < world.Width; x++)
            {
                if (snapshot.TryGetValue(new Point2(x, y), out var organism))
                {
                    organism.ApplyColor();
                    Console.Write(organism.Glyph);
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(' ');
                }
            }

            Console.WriteLine();
        }
    }
}
