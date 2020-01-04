using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Numerics;

namespace FastPoisson.CLI
{
    public class Program
    {
        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Required]
        [Option(Description = "The width, in pixels, of the noise image to create.")]
        public int Width { get; }

        [Required]
        [Option(Description = "The height, in pixels, of the noise image to create.")]
        public int Height { get; }

        [Required]
        [Option(Description = "The radius, in pixels, of the minimum distance between any two generated Poisson points. Smaller values increase runtime.")]
        public float Radius { get; }

        private void OnExecute()
        {
            System.Console.WriteLine($"Creating a Poisson noise image that is {Width}x{Height} pixels in size with a minimum distance of {Radius} pixels between any two generated noise samples.");

            IEnumerable<Vector2> generatedPoints = FastPoisson.Poisson.GenerateSamples(Width, Height, Radius);

            System.Console.WriteLine("Writing noise image to 'noise.png'.");

            // note: pixels on the border will look oblong - unsure why
            Bitmap bitmap = new Bitmap(Width, Height); 

            // take the floor of the points to define the sample locations
            foreach(var point in generatedPoints)
            {
                bitmap.SetPixel((int)Math.Floor(point.X), (int)Math.Floor(point.Y), Color.Black);
            }

            bitmap.Save("noise.png");
        }
    }
}
