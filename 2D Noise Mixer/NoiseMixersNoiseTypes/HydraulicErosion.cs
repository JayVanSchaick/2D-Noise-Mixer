using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Random = System.Random;

namespace NoiseMixer
{

    public class HydraulicErosion
    {
        /// <summary>
        /// Taken from https://github.com/arthursimas1/Hydraulic-Erosion/blob/master/src/Erosion.cpp
        /// </summary>

        Random random;

        uint mapSizeX;
        uint mapSizeY;

         int erosionRadius = 3;
        double inertia = 0.05f;
        double sedimentCapacityFactor = 4;
        double minSedimentCapacity = 0.01f;
        double erodeSpeed = 0.3f;
        double depositSpeed = 0.3f;
        double evaporateSpeed = 0.01f;
        double gravity = 4;
        double maxDropletLifetime = 30;

        double initialWaterVolume = 1;
        double initialSpeed = 1;

        List<List<uint>> erosionBrushIndices;
        List<List<double>> erosionBrushWeights;

        public int ErosionRadius { get => erosionRadius; set => erosionRadius = value; }
        public double Inertia { get => inertia; set => inertia = value; }
        public double SedimentCapacityFactor { get => sedimentCapacityFactor; set => sedimentCapacityFactor = value; }
        public double MinSedimentCapacity { get => minSedimentCapacity; set => minSedimentCapacity = value; }
        public double ErodeSpeed { get => erodeSpeed; set => erodeSpeed = value; }
        public double DepositSpeed { get => depositSpeed; set => depositSpeed = value; }
        public double EvaporateSpeed { get => evaporateSpeed; set => evaporateSpeed = value; }
        public double Gravity { get => gravity; set => gravity = value; }
        public double MaxDropletLifetime { get => maxDropletLifetime; set => maxDropletLifetime = value; }
        public double InitialWaterVolume { get => initialWaterVolume; set => initialWaterVolume = value; }
        public double InitialSpeed { get => initialSpeed; set => initialSpeed = value; }

        struct HeightAndGradient
        {
            public double height;
            public double gradientX;
            public double gradientY;
        };


        public HydraulicErosion(uint MapSizeX, uint MapSizeY)
        {
            mapSizeX = MapSizeX;

            mapSizeY = MapSizeY;

            random = new Random();
            InitializeBrushIndices();
        }

        public HydraulicErosion(uint MapSizeX, uint MapSizeY, int Seed)
        {
            mapSizeX = MapSizeX;
            mapSizeY = MapSizeY;

            random = new Random(Seed);
            InitializeBrushIndices();
        }

        /// <summary>
        /// A Constructor to give total control over the Hydraulic Erosion System.
        /// </summary>
        public HydraulicErosion(uint MapSizeX, uint MapSizeY, int Seed, int erosionRadius = 3, double inertia = 0.05f, double sedimentCapacityFactor = 4,
                                double minSedimentCapacity = 0.01f, double erodeSpeed = 0.3f, double depositSpeed = 0.3f, double evaporateSpeed = 0.01f,
                                double gravity = 4, double maxDropletLifetime = 30, double initialWaterVolume = 1, double initialSpeed = 1)
        {

            mapSizeX = MapSizeX;
       
            mapSizeY = MapSizeY;

            this.erosionRadius = erosionRadius;
            this.inertia = inertia;
            this.sedimentCapacityFactor = sedimentCapacityFactor;
            this.minSedimentCapacity = minSedimentCapacity;
            this.erodeSpeed = erodeSpeed;
            this.depositSpeed = depositSpeed;
            this.evaporateSpeed = evaporateSpeed;
            this.gravity = gravity;
            this.maxDropletLifetime = maxDropletLifetime;
            this.initialWaterVolume = initialWaterVolume;
            this.initialSpeed = initialSpeed;
        }

        public void SetSeed(int newSeed)
        {
            // seed = newSeed;
            random = new Random(newSeed);
        }


        public void Erode(double[] map, uint numIterations)
        {
            Parallel.For(0, numIterations, iterate =>
           {
               // Creates the droplet at a random X and Y on the map
               double posX = (double)random.NextDouble() * mapSizeX - 1;
               double posY = (double)random.NextDouble() * mapSizeY - 1;
               double dirX = 0;
               double dirY = 0;
               double speed = initialSpeed;
               double water = initialWaterVolume;
               double sediment = 0;

               // Simulates the droplet only up to it's max lifetime, prevents an infite loop
               for (uint lifetime = 0; lifetime < maxDropletLifetime; lifetime++)
               {
                   int nodeX = (int)posX;
                   int nodeY = (int)posY;
                   int dropletIndex = (int)(nodeY * mapSizeX + nodeX);

                   // Calculates the droplet offset inside the cell
                   double cellOffsetX = posX - nodeX;
                   double cellOffsetY = posY - nodeY;

                   // Calculate droplet's height and direction of flow with bilinear interpolation of surrounding heights
                   HeightAndGradient heightAndGradient = calculateHeightAndGradient(map, posX, posY);

                   // Update the droplet's direction and position (move position 1 unit regardless of speed)
                   dirX = (dirX * inertia - heightAndGradient.gradientX * (1 - inertia));
                   dirY = (dirY * inertia - heightAndGradient.gradientY * (1 - inertia));

                   // Normalize direction
                   double len = (double)Math.Sqrt(dirX * dirX + dirY * dirY);
                   if (len != 0)
                   {
                       dirX /= len;
                       dirY /= len;
                   }

                   posX += dirX;
                   posY += dirY;

                   // Stop simulating droplet if it's not moving or has flowed over edge of map
                   if ((dirX == 0 && dirY == 0) || posX < 0 || posX >= mapSizeX - 1 || posY < 0 || posY >= mapSizeY - 1)
                       break;

                   // Find the droplet's new height and calculate the deltaHeight
                   double deltaHeight = calculateHeightAndGradient(map, posX, posY).height - heightAndGradient.height;

                   // Calculate the droplet's sediment capacity (higher when moving fast down a slope and contains lots of water)
                   double sedimentCapacity = Math.Max(-deltaHeight * speed * water * sedimentCapacityFactor, minSedimentCapacity);

                   // If carrying more sediment than capacity, or if flowing uphill:
                   if (sediment > sedimentCapacity || deltaHeight > 0)
                   {
                       // If moving uphill (deltaHeight > 0) try fill up to the current height, otherwise deposit a fraction of the excess sediment
                       double amountToDeposit = (deltaHeight > 0) ? Math.Min(deltaHeight, sediment) : (sediment - sedimentCapacity) * depositSpeed;
                       sediment -= amountToDeposit;

                       // Add the sediment to the four nodes of the current cell using bilinear interpolation
                       // Deposition is not distributed over a radius (like erosion) so that it can fill small pits
                       map[dropletIndex] += amountToDeposit * (1 - cellOffsetX) * (1 - cellOffsetY);
                       map[dropletIndex + 1] += amountToDeposit * cellOffsetX * (1 - cellOffsetY);
                       map[(int)(dropletIndex + mapSizeX)] += amountToDeposit * (1 - cellOffsetX) * cellOffsetY;
                       map[(int)(dropletIndex + mapSizeX + 1)] += amountToDeposit * cellOffsetX * cellOffsetY;
                   }
                   else
                   {
                       // Erode a fraction of the droplet's current carry capacity.
                       // Clamp the erosion to the change in height so that it doesn't dig a hole in the terrain behind the droplet
                       double amountToErode = Math.Min((sedimentCapacity - sediment) * erodeSpeed, -deltaHeight);

                       // Use erosion brush to erode from all nodes inside the droplet's erosion radius
                       for (uint brushPointIndex = 0; brushPointIndex < erosionBrushIndices[dropletIndex].Count; brushPointIndex++)
                       {
                           uint nodeIndex = (erosionBrushIndices[dropletIndex])[(int)brushPointIndex];
                           if (nodeIndex >= map.Length) continue;
                           double weighedErodeAmount = amountToErode * (erosionBrushWeights[dropletIndex])[(int)brushPointIndex];
                           double deltaSediment = (map[(int)nodeIndex] < weighedErodeAmount) ? map[(int)nodeIndex] : weighedErodeAmount;
                           map[(int)nodeIndex] -= deltaSediment;
                           sediment += deltaSediment;
                       }
                   }

                   speed = Math.Sqrt(speed * speed + Math.Abs(deltaHeight) * gravity);
                   water *= (1 - evaporateSpeed);
               }
           });
        }


        HeightAndGradient calculateHeightAndGradient(double[] nodes, double posX, double posY)
        {
            int coordX = (int)posX;
            int coordY = (int)posY;

            // Calculate droplet's offset inside the cell
            double x = posX - coordX;
            double y = posY - coordY;

            // Calculate heights of the nodes
            int nodeIndexNW = (int)(coordY * mapSizeX + coordX);
            double heightNW = nodes[nodeIndexNW];
            double heightNE = nodes[nodeIndexNW + 1];
            double heightSW = nodes[(int)(nodeIndexNW + mapSizeX)];
            double heightSE = nodes[(int)(nodeIndexNW + mapSizeX + 1)];

            // Calculate droplet's direction of flow with bilinear interpolation of height difference along the edges
            double gradientX = (heightNE - heightNW) * (1 - y) + (heightSE - heightSW) * y;
            double gradientY = (heightSW - heightNW) * (1 - x) + (heightSE - heightNE) * x;

            // Calculate height with bilinear interpolation of the heights of the nodes of the cell
            double height = heightNW * (1 - x) * (1 - y) + heightNE * x * (1 - y) + heightSW * (1 - x) * y + heightSE * x * y;

            return new HeightAndGradient { height = height, gradientX = gradientX, gradientY = gradientY };
        }


        void InitializeBrushIndices()
        {
            erosionBrushIndices = new List<List<uint>>(new List<uint>[(int)(mapSizeX * mapSizeY)]);
            erosionBrushWeights = new List<List<double>>(new List<double>[(int)(mapSizeX * mapSizeY)]);

            List<int> xOffsets = new List<int>(new int[(erosionRadius + erosionRadius + 1) * (erosionRadius + erosionRadius + 1)]);
            List<int> yOffsets = new List<int>(new int[(erosionRadius + erosionRadius + 1) * (erosionRadius + erosionRadius + 1)]);
            List<double> weights = new List<double>(new double[(erosionRadius + erosionRadius + 1) * (erosionRadius + erosionRadius + 1)]);

            Parallel.For( 0, (mapSizeX * mapSizeY) - 1, i =>
            {
                double weightSum = 0;
                 uint addIndex = 0;
                 uint centerX = (uint)i % mapSizeX;
                 uint centerY = (uint)i / mapSizeX;

                for (int y = -erosionRadius; y <= (int)erosionRadius; y++)
                {
                    for (int x = -erosionRadius; x <= (int)erosionRadius; x++)
                    {
                        double sqrDst = x * x + y * y;
                        if (sqrDst < erosionRadius * erosionRadius)
                        {
                            int coordX = (int)centerX + x;
                            int coordY = (int)centerY + y;

                            if (0 <= coordX && coordX < (int)mapSizeX && 0 <= coordY && coordY < (int)mapSizeY)
                            {
                                double weight = 1 - Math.Sqrt(sqrDst) / erosionRadius;
                                weightSum += weight;
                               
                                weights[(int)addIndex] = weight;
                                xOffsets[(int)addIndex] = x;
                                yOffsets[(int)addIndex] = y;
                                addIndex++;
                            }
                        }
                    }
                }


                uint numEntries = addIndex;
                erosionBrushIndices[(int)i] = new List<uint>(new uint [(int)numEntries]);
                 erosionBrushWeights[(int)i] = new List<double>(new double[(int)numEntries]);

                 for (int j = 0; j < numEntries; j++)
                 {
                     erosionBrushIndices[(int)i][j] = (uint)((yOffsets[j] + centerY) * mapSizeX + xOffsets[j] + centerX);
                     erosionBrushWeights[(int)i][j] = weights[j] / weightSum;
                 }
            });
        }


    }

}


