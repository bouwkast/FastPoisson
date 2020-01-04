using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Xunit;

namespace FastPoisson.Tests
{
    public class PoissonTest
    {
        // TODO: this is not guaranteed to work - we should probably extract the distance creation and validation logic and test those instead
        [Theory]
        [InlineData(10, 10, 2.0f)]
        [InlineData(50, 50, 5.0f)]
        [InlineData(50, 100, 5.0f)]
        [InlineData(100, 50, 5.0f)]
        public void GenerateSamples_ShouldCreatePoints_MinimumDistanceApart(int width, int height, float radius)
        {
            var generatedPoints = Poisson.GenerateSamples(width, height, radius).ToList();
            List<float> invalidDistances = new List<float>();
            for(int i = 0; i < generatedPoints.Count; i++)
            {
                for (int j = 0; j < generatedPoints.Count; j++)
                {
                    if(i == j)
                    {
                        continue;
                    }

                    float actualDistance = Vector2.Distance(generatedPoints[i], generatedPoints[j]);

                    if(actualDistance < radius)
                    {
                        invalidDistances.Add(actualDistance);
                    }
                }
            }

            Assert.Empty(invalidDistances);
        }
    }
}
