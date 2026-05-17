using System;
using System.Linq;
using System.Threading;

namespace LifeSim;

public static class Program
{
    public static void Main()
    {
        ConfigureConsole();

        var world = CreateWorld();

        var paused = false;
        const int delayMs = 120;

        while (true)
        {
            bool flowControl = HandleInput(ref paused);
            if (!flowControl)
            {
                return;
            }

            if (!paused)
            {
                world.Step();
                RenderWorld(world);
            }

            Thread.Sleep(delayMs);
        }
    }

    private static bool HandleInput(ref bool paused)
    {
        while (!Console.IsInputRedirected && Console.KeyAvailable)
        {
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Q || key == ConsoleKey.Escape)
            {
                Console.ResetColor();
                Console.CursorVisible = true;
                return false;
            }

            if (key == ConsoleKey.Spacebar || key == ConsoleKey.P)
            {
                paused = !paused;
            }
        }

        return true;
    }

    private static World CreateWorld()
    {
        const int width = 50;
        const int height = 22;
        var initialPlants = (int)(width * height * 0.22);
        const int initialHerbivores = 28;
        const int initialPredators = 10;

        var worldGrid = new WorldGrid(width, height);
        var world = new World(worldGrid);
        world.Seed(initialPlants, new PlantFactory());
        world.Seed(initialHerbivores, new HerbivoreFactory());
        world.Seed(initialPredators, new PredatorFactory());
        return world;
    }

    private static void ConfigureConsole()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;
    }

    private static void RenderWorld(World world)
    {
        Console.SetCursorPosition(0, 0);

        RenderHeader(world);

        RenderGrid(world);
    }

    private static void RenderGrid(World world)
    {
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

    private static void RenderHeader(World world)
    {
        var plants = world.AllOrganisms.OfType<Plant>().Count();
        var herbs = world.AllOrganisms.OfType<Herbivore>().Count();
        var preds = world.AllOrganisms.OfType<Predator>().Count();

        Console.ResetColor();
        Console.WriteLine(
            $"Tick: {world.Tick,-8}  " +
            $"Plants: {plants,-5}  " +
            $"Herbivores: {herbs,-5}  " +
            $"Predators: {preds,-5}   " +
            $"[Space/P] pause, [Q/Esc] quit");
    }
}
