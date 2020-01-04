using System;
using System.Collections.Generic;
using System.Numerics;

namespace FastPoisson
{
    /// <summary>
    ///     Implementation of the Fast Poisson Disk Sampling in Arbitrary Dimensions by Dr Robert Bridson.
    ///     Paper accessed via - https://www.cct.lsu.edu/~fharhad/ganbatte/siggraph2007/CD2/content/sketches/0250.pdf
    /// </summary>
    /// <remarks>
    ///     This is a port of an implementation that I made in C++.
    /// </remarks>
    public static class Poisson
    {
        private static int _k = 30; // recommended value from the paper TODO provide a means for configuring this value

        /// <summary>
        ///     Generates a Poisson distribution of <see cref="Vector2"/> within some rectangular shape defined by <paramref name="height"/> * <paramref name="width"/>.
        /// </summary>
        /// <param name="width">The width of the plane.</param>
        /// <param name="height">The height of the plane.</param>
        /// <param name="radius">The minimum distance between any two points.</param>
        /// <returns>Enumeration of <see cref="Vector2"/> elements where no element is within <paramref name="radius"/> distance to any other element.</returns>
        public static IEnumerable<Vector2> GenerateSamples(float width, float height, float radius)
        {
            List<Vector2> samples = new List<Vector2>();
            Random random = new Random(); // TODO evaluate whether this Random can generate uniformly random numbers

            // cell size to guarantee that each cell within the accelerator grid can have at most one sample
            float cellSize = radius / (float)Math.Sqrt(radius);

            // dimensions of our accelerator grid
            int acceleratorWidth = (int)Math.Ceiling(width / cellSize);
            int acceleratorHeight = (int)Math.Ceiling(height / cellSize);

            // backing accelerator grid to speed up rejection of generated samples
            int[,] accelerator = new int[acceleratorHeight, acceleratorWidth];

            // initializer point right at the center
            Vector2 initializer = new Vector2(width / 2, height / 2);

            // keep track of our active samples
            List<Vector2> activeSamples = new List<Vector2>();

            activeSamples.Add(initializer);

            // begin sample generation
            while (activeSamples.Count != 0)
            {
                // pop off the most recently added samples and begin generating addtional samples around it
                int index = random.Next(0, activeSamples.Count);
                Vector2 currentOrigin = activeSamples[index];
                bool isValid = false; // need to keep track whether or not the sample we have meets our criteria

                // attempt to randomly place a point near our current origin up to _k rejections
                for (int i = 0; i < _k; i++)
                {
                    // create a random direction to place a new sample at
                    float angle = (float)(random.NextDouble() * Math.PI * 2);
                    Vector2 direction;
                    direction.X = (float)Math.Sin(angle);
                    direction.Y = (float)Math.Cos(angle);

                    // create a random distance between r and 2r away for that direction
                    float distance = random.NextFloat(radius, 2 * radius);
                    direction.X *= distance;
                    direction.Y *= distance;

                    // create our generated sample from our currentOrigin plus our new direction vector
                    Vector2 generatedSample;
                    generatedSample.X = currentOrigin.X + direction.X;
                    generatedSample.Y = currentOrigin.Y + direction.Y;

                    isValid = IsGeneratedSampleValid(generatedSample, width, height, radius, cellSize, samples, accelerator);

                    if (isValid)
                    {
                        activeSamples.Add(generatedSample); // we may be able to add more samples around this valid generated sample later
                        samples.Add(generatedSample);

                        // mark the generated sample as "taken" on our accelerator
                        accelerator[(int)(generatedSample.X / cellSize), (int)(generatedSample.Y / cellSize)] = samples.Count;

                        break; // restart since we successfully generated a point
                    }
                }

                if (!isValid)
                {
                    activeSamples.RemoveAt(index);
                }
            }
            return samples;
        }

        private static bool IsGeneratedSampleValid(Vector2 generatedSample, float width, float height, float radius, float cellSize, List<Vector2> samples, int[,] accelerator)
        {
            // is our generated sample within our boundaries?
            if (generatedSample.X < 0 || generatedSample.X >= height || generatedSample.Y < 0 || generatedSample.Y >= width)
            {
                return false; // out of bounds
            }

            int acceleratorX = (int)(generatedSample.X / cellSize);
            int acceleratorY = (int)(generatedSample.Y / cellSize);

            // TODO - for some reason my math for initially have +/- 2 for the area bounds causes some points to just slip
            //        through with a distance just below the radis - bumping this up to +/- 3 solves it at the cost of additional compute
            // create our search area bounds
            int startX = Math.Max(0, acceleratorX - 3);
            int endX = Math.Min(acceleratorX + 3, accelerator.GetLength(0) - 1);

            int startY = Math.Max(0, acceleratorY - 3);
            int endY = Math.Min(acceleratorY + 3, accelerator.GetLength(1) - 1);

            // search within our boundaries for another sample
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    int index = accelerator[x, y] - 1; // index of sample at this point (if there is one)

                    if (index >= 0) // in each point for the accelerator where we have a sample we put the current size of the number of samples
                    {
                        // compute Euclidean distance squared (more performant as there is no square root)
                        float distance = Vector2.DistanceSquared(generatedSample, samples[index]);
                        if (distance < radius * radius)
                        {
                            return false; // too close to another point
                        }
                    }
                }
            }
            return true; // this is a valid generated sample as there are no other samples too close to it
        }
    }
}
