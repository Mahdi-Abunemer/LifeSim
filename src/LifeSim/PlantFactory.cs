using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeSim
{
    public class PlantFactory : IOrganismFactory
    {
        public Organism Create(World world, Point2 position) 
        { 
            var plant = new Plant(world, position);
            return plant;
        }
    }
}
