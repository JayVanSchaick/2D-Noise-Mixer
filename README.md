# 2D-Noise-Mixer

The 2D noise mixer is designed to make mixing different types noise quick with only a few lines of code. Highly commented and easy to use, this system types to be non-platform specific. 

## Description

This 2D noise mixer will let the user have a high degree of control over the different noise layers, and over the mixer itself. A list of noises are provided as well as a list of manipulations including masking between layers, shifting, scaling, and inverting. The noise is calculated on the CPU and can be off loaded onto a background thread is necessary. New noises are easy to implement with the INoise interface, and can be up and running in short while. A list of already added noises are Perlin, Simplex, Voronoi, Worley, OpenSimplex, and OpenSimplex2S.

<p align="center">
 <img style="display: flex; flex-wrap: wrap;" src = "zRead%20Me%20Images/Capture%201.png" width="40">
 <img style="display: flex; flex-wrap: wrap;" src = "zRead%20Me%20Images/Capture%202.png" width="40">
 <img style="display: flex; flex-wrap: wrap;" src = "zRead%20Me%20Images/Capture%203.png" width="40">
 <img style="display: flex; flex-wrap: wrap;" src = "zRead%20Me%20Images/Capture%204.png" width="40">
 <img style="display: flex; flex-wrap: wrap;" src = "zRead%20Me%20Images/Capture%205.png" width="40">
 <img style="display: flex; flex-wrap: wrap;" src = "zRead%20Me%20Images/Capture%206.png" width="40">
</p>



## Getting Started

### Dependencies

* Bacic C# libraries such as System.
* My Utilities Repo.

### Installing

* Download and add files to your project.
* If a library is not available on your platform it probable can be remove from being necessary with minimal code change.    

### Documentation

##### Please note: the classes themselves have more Documentation in their XML Docs.
 

* How to use the class

*The following creates a new Noise Mixer class.
```C#
using NoiseMixer;
NoiseMixer noiseMixer = new NoiseMixer.NoiseMixer((uint)resolution, (uint)resolution);
```
* To mix a new layer (in this case Perlin noise) with the current layer the following is shown. A new Perlin noise class is added in the INoise parameter, and float is added to scale it. 

```C#
noiseMixer.NewCombineLayer(new PerlinNoise(seed), ScaleNoise);
```

* A list of different types of noise layer include:

```C#
noiseMixer.NewCombineLayer(INoise Noise, double NoiseScale);
noiseMixer.NewAddLayer(INoise Noise, double NoiseScale);
noiseMixer.NewDivideLayer(INoise Noise, double NoiseScale);
noiseMixer.NewMultiplyLayer(INoise Noise, double NoiseScale);
noiseMixer.NewOnlyHigherLayer(INoise Noise, double NoiseScale);
noiseMixer.NewOnlyLowerLayer(INoise Noise, double NoiseScale);
noiseMixer.NewSubtractLayer(INoise Noise, double NoiseScale);
```

* A list of effects can be added to each layer such as: 

```C#
noiseMixer.NewCombineLayer(new PerlinNoise(seed), ScaleNoise).LayerMask(float MaskAmount);
noiseMixer.NewCombineLayer(new PerlinNoise(seed), ScaleNoise).Inverse();
noiseMixer.NewCombineLayer(new PerlinNoise(seed), ScaleNoise).Scale(float ScaleAmount);
noiseMixer.NewCombineLayer(new PerlinNoise(seed), ScaleNoise).Shift(float ShiftAmount);
```

*Or strung together: 
```C#
noiseMixer.NewCombineLayer(new PerlinNoise(seed), ScaleNoise).LayerMask(float MaskAmount).Shift(float ShiftAmount).Scale(float ScaleAmount).Inverse();
```
* The Mixer class itself can be manipulated, which in turn manipulates every value in the mixer.

```C#
noiseMixer.Inverse();
noiseMixer.Scale(float ScaleAmount);
noiseMixer.Shift(float ScaleAmount);
noiseMixer.Tier(int AmountOfTiers);
```

* The Erosion effect can be added as well:

```C#
noiseMixer.HydraulicErosion(int Iterations);
```

* Once all of the setup has been setup Apply() must be called to do the calculation (or ApplyF() if an array of floats is preferred). Both methods are main thread.

```C#
noiseMixer.Apply(bool normalize); //For double array
//or
noiseMixer.ApplyF(bool normalize); //For float array

```

* If doing the calculation no a background thread is preferred, use  

```C#
noiseMixer.ApplyOnOtherThreads(uint threadsAmount);

```

* If the calculations where done on a background thread, or was computed and the user want to retrieve the values without doing the calculations again GetCalculations() can be called to retrieve the results.  

```C#
noiseMixer. GetCalculations(out double[,] Results, bool normalize); // for double array
//or
noiseMixer. GetCalculationsF(out float[,] Results, bool normalize); // for float array

```



## Help

Need help ask in the issues section. 

## Authors

Jay Van Schaick

## Version History

* 0.1
    * Initial Release

## License

This project is licensed under the MIT License - see https://opensource.org/licenses/MIT for details

## Acknowledgments

Inspiration, code snippets, etc are in the classes themselves with a link to the originals.
