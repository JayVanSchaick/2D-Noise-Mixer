# 2D-Noise-Mixer

The 2D noise mixer is designed to make mixing different types noise quick with only a few lines of code. Highly commented and easy to use, this system types to be non-platform specific. 

## Description

This 2D noise mixer will let the user have a high degree of control over the different noise layers, and over the mixer itself. A list of noises are provided as well as a list of manipulations including masking between layers, shifting, scaling, and inverting. The noise is calculated on the CPU and can be off loaded onto a background thread is necessary. New noises are easy to implement with the INoise interface, and can be up and running in short while. A list of already added noises are Perlin, Simplex, Voronoi, Worley, OpenSimplex, and OpenSimplex2S.

## Getting Started

### Dependencies

* Bacic C# libraries such as System.
* My Utilities Repo.

### Installing

* Download and add files to your project.
* If a library is not available on your platform it probable can be remove from being necessary with minimal code change.    

### Executing program

* How to use the class

*The following creates a new noiseMixer class.
```C#
using NoiseMixer;
NoiseMixer noiseMixer = new NoiseMixer.NoiseMixer((uint)resolution, (uint)resolution)
```
