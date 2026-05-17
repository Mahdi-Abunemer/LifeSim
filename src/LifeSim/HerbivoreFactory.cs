using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeSim
{
    public class HerbivoreFactory : IOrganismFactory
    {
        public Organism Create(World world, Point2 position)
        {
            var herbivore = new Herbivore(world, position);
            return herbivore;
        }
    }
}
