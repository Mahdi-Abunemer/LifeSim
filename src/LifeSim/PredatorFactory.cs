using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeSim
{
    public class PredatorFactory : IOrganismFactory
    {
        public Organism Create(World world, Point2 position)
        {
            var predator = new Predator(world, position);
            return predator;
        }
    }
}
