# Fast Poisson

An implementation of the _Fast Poisson Disk Sampling in Arbitrary Dimensions_ paper in two dimensions. Paper: https://www.cct.lsu.edu/~fharhad/ganbatte/siggraph2007/CD2/content/sketches/0250.pdf

This project can create a blue noise sample of seeds of some spacing on some image.

## Use

Currently, it is recommended to clone this repository down and run the .NET Core 3.1 console app FastPoisson.CLI.

There are a few options that must be supplied (in the debug project options) prior to running: width of the image, height of the image, and minimum radius between any two points.

Sample generated image (500x500 with a r of 15):

> Note: this is an image with a transparent background - best viewed with a white background

![poisson image](noise.png)

## Future Work

This is a rough proof-of-concept and still needs some work:

- the Random generator used may not be uniformly random
- the test project is lacking and brute forces its tests
- the implementation could do with some general refactoring to reduce code complexity
- I haven't done any real performance optimizations
- the commandline project is also lacking in customization