using System;
using System.Collections.Generic;
using System.Numerics;
using Utils;
using Array = Utils.Array;
using Math = Utils.Math;

namespace NoiseMixer
{
    /// <summary>
    /// This class was written by Jay Van Schaick to quickly and efficiently mix 2d noise with a few lines of code.
    /// The section of the class to hold methods
    /// </summary>
    public partial class NoiseMixer
    {
        //the current layer
        public NoiseMixerActions CurrentLayer;

        //is the mixer in the middle of a multi-thread calculation?
        private bool MixerMiddleOfCalculation;

        //has there been a calculation of any type yet
        private bool CalculationAtleastOnce;

        //multi-thread value to see if all jobs have returned.
        private bool[] returnedThreads;

        const string ConflictErrMes = "Conflicting Operations: trying to run a main thread computation while also running" +
                              " one on background threads. Noise mixer can not take this action and must wait until background threads are finished.";

        HydraulicErosion hydraulicErosion;
        int iterations;

        /// <summary>
        /// A list to hold each step of the Noise Mixer actions as outlined by the user.
        /// </summary>
        List<NoiseMixerActions> Actions = new List<NoiseMixerActions>();

        /// <summary>
        /// List of the current layer value to mix the next layer with.
        /// </summary>
        private double[,] currentLayerValues;

        /// <summary>
        /// The Constructor which takes a fill value.
        /// </summary>
        /// <param name="XResolution">The width of the noise amount.</param>
        /// <param name="YResolution">The high of the noise amount.</param>
        /// <param name="FillValue">The value to populate the first noise width</param>
        public NoiseMixer(uint XResolution, uint YResolution, double FillValue)
        {

            currentLayerValues = new double[XResolution, YResolution];

            Actions.Add(new FillValue(FillValue, this));

        }

        /// <summary>
        /// The Constructor which takes an other noise mixer and assigns its computed values to itself. If the noise maker used in initializing has not been calculated (Apply() or something like it called),
        /// this new instance will only have the basic values of the first.
        /// </summary>
        /// <param name="NoiseMixer">The noise maker to be used in initializing this class.</param>
        public NoiseMixer(NoiseMixer NoiseMixer)
        {

            currentLayerValues = NoiseMixer.currentLayerValues.Clone() as double[,];

        }


        /// <summary>
        /// The Constructor, will initialize the first layer with a value of zero.
        /// </summary>
        /// <param name="XResolution">The width of the noise amount.</param>
        /// <param name="YResolution">The high of the noise amount.</param>
        /// <param name="FillValue">The value to populate the first noise width</param>
        public NoiseMixer(uint XResolution, uint YResolution) : this(XResolution, YResolution, 0)
        { }

        /// <summary>
        /// Combined the average of current mixer layer with a new layer. 
        /// </summary>
        /// <param name="Noise">The noise to be used in the mixer. Must be between (-1,1).</param>
        /// <param name="ScaleNoise">The scale of the noise to be used in the mixer</param>
        /// <param name="LayerMask">The value to be used to create a layer mask. Must be between (0,1). </param>
        /// <param name="ScaleLayerMask">The scale of the noise to be used in the layer mask</param>
        /// <returns></returns>
        public LayerBase NewCombineLayer(INoise Noise, double ScaleNoise)
        {
            CurrentLayer = new CombineLayer(Noise, ScaleNoise, this);

            Actions.Add(CurrentLayer);

            return (LayerBase)CurrentLayer;

        }

        /// <summary>
        /// Add a new layer to the current mixer layer.  
        /// </summary>
        /// <param name="Noise">The noise to be used in the mixer. Must be between (-1,1).</param>
        /// <param name="ScaleNoise">The scale of the noise to be used in the mixer</param>
        /// <param name="LayerMask">The value to be used to create a layer mask. Must be between (0,1). </param>
        /// <param name="ScaleLayerMask">The scale of the noise to be used in the layer mask</param>
        /// <returns></returns>
        public LayerBase NewAddLayer(INoise Noise, double ScaleNoise)
        {
            CurrentLayer = new AddLayer(Noise, ScaleNoise, this);

            Actions.Add(CurrentLayer);

            return (LayerBase)CurrentLayer;

        }

        /// <summary>
        /// Subtract a new layer from the current mixer layer.  
        /// </summary>
        /// <param name="Noise">The noise to be used in the mixer. Must be between (-1,1).</param>
        /// <param name="ScaleNoise">The scale of the noise to be used in the mixer</param>
        /// <param name="LayerMask">The value to be used to create a layer mask. Must be between (0,1). </param>
        /// <param name="ScaleLayerMask">The scale of the noise to be used in the layer mask</param>
        /// <returns></returns>
        public LayerBase NewSubtractLayer(INoise Noise, double ScaleNoise)
        {
            CurrentLayer = new SubtractLayer(Noise, ScaleNoise, this);

            Actions.Add(CurrentLayer);

            return (LayerBase)CurrentLayer;

        }

        /// <summary>
        /// Multiply a new layer by the current mixer layer.  
        /// </summary>
        /// <param name="Noise">The noise to be used in the mixer. Must be between (-1,1).</param>
        /// <param name="ScaleNoise">The scale of the noise to be used in the mixer</param>
        /// <returns></returns>
        public LayerBase NewMultiplyLayer(INoise Noise, double ScaleNoise)
        {
            CurrentLayer = new MultiplyLayer(Noise, ScaleNoise, this);

            Actions.Add(CurrentLayer);

            return (LayerBase)CurrentLayer;

        }

        /// <summary>
        /// Divide the current mixer layer by a new layer.  
        /// </summary>
        /// <param name="Noise">The noise to be used in the mixer. Must be between (-1,1).</param>
        /// <param name="ScaleNoise">The scale of the noise to be used in the mixer</param>
        /// <returns></returns>
        public LayerBase NewDivideLayer(INoise Noise, double ScaleNoise)
        {
            CurrentLayer = new DivideLayer(Noise, ScaleNoise, this);

            Actions.Add(CurrentLayer);

            return (LayerBase)CurrentLayer;

        }

        /// <summary>
        /// Only add to the current mixer layer where a new layer is higher.  
        /// </summary>
        /// <param name="Noise">The noise to be used in the mixer. Must be between (-1,1).</param>
        /// <param name="ScaleNoise">The scale of the noise to be used in the mixer</param>
        /// <returns></returns>
        public LayerBase NewOnlyHigherLayer(INoise Noise, double ScaleNoise)
        {
            CurrentLayer = new OnlyHigherLayer(Noise, ScaleNoise, this);

            Actions.Add(CurrentLayer);

            return (LayerBase)CurrentLayer;

        }

        /// <summary>
        /// Only add to the current mixer layer where the new layer is lower.  
        /// </summary>
        /// <param name="Noise">The noise to be used in the mixer. Must be between (-1,1).</param>
        /// <param name="ScaleNoise">The scale of the noise to be used in the mixer</param>
        /// <returns></returns>
        public LayerBase NewOnlyLowerLayer(INoise Noise, double ScaleNoise)
        {
            CurrentLayer = new OnlyLowerLayer(Noise, ScaleNoise, this);

            Actions.Add(CurrentLayer);

            return (LayerBase)CurrentLayer;

        }

        /// <summary>
        /// Tier the current noise layer to the nearest tier. The Amount of tiers will be equally spread from (-1,1).
        /// </summary>
        /// <param name="TierAmounts">The amount of tiers to create, the more tiers the smaller they will be.</param>
        /// <param name="Mask">The amount of effect tiering the noise should have on the noise being tiered. 0 is full effect through 1 which is no effect</param>
        public void Tier(int TierAmounts, double EffectAmount)
        {
            Actions.Add(new TierNoise((uint)TierAmounts, Math.Clamp(EffectAmount, 0, 1), this));
        }

        /// <summary>
        /// Shift the whole noise mixer in a direction, the result will always between (-1,1).
        /// </summary>
        /// <param name="ShiftAmount">The amount to shift in one direction.</param>
        public void Shift(double ShiftAmount)
        {
            Actions.Add(new ShiftMixer(ShiftAmount, this));
        }

        /// <summary>
        /// Scale the whole noise mixer, pushing higher values higher, and lower values lower. The result will always between (-1,1).
        /// </summary>
        /// <param name="ScaleAmount">The amount to scale, a value 1 will not effect the mixer at all.</param>
        public void Scale(double ScaleAmount)
        {
            Actions.Add(new ScaleMixer(ScaleAmount, this));
        }

        /// <summary>
        /// Inverse the whole noise mixer, values will be opposite what they where. The result will always between (-1,1).
        /// </summary>
        public void Invert()
        {
            Actions.Add(new InvertMixer(this));
        }

        /// <summary>
        /// Single Thread compute calculations. If background thread computation is needed use ApplyOnOtherThreads().
        /// </summary>
        /// <param name="NormalizeReturn">The return data values be between (0,1) if true, or between (-1,1) if false.</param> 
        /// <returns></returns>
        public double[,] Apply(bool NormalizeReturn)
        {

            if (MixerMiddleOfCalculation == true)
            {
                throw new Exception(ConflictErrMes);
            }

            for (int action = 0; action < Actions.Count; action++)
            {
                for (int x = 0; x < currentLayerValues.GetLength(0); x++)
                {
                    for (int y = 0; y < currentLayerValues.GetLength(1); y++)
                    {
                        Actions[action].ExecuteCommand((uint)x, (uint)y);
                    }
                }
            }

            if (hydraulicErosion != null)
                DoHydraulicErosion();


            if (NormalizeReturn)
                return NormalizeCurrentLayerValues();
            else
                return currentLayerValues.Clone() as double[,];
        }

        /// <summary>
        /// Single Thread compute calculations that return a float. If background thread computation is needed use ApplyOnOtherThreads().
        /// </summary>
        /// <param name="NormalizeReturn">The return data values be between (0,1) if true, or between (-1,1) if false.</param> 
        /// <returns></returns>
        public float[,] ApplyF(bool NormalizeReturn)
        {
            if (MixerMiddleOfCalculation == true)
            {
                throw new Exception(ConflictErrMes);
            }

            for (int action = 0; action < Actions.Count; action++)
            {
                for (int x = 0; x < currentLayerValues.GetLength(0); x++)
                {
                    for (int y = 0; y < currentLayerValues.GetLength(1); y++)
                    {
                        Actions[action].ExecuteCommand((uint)x, (uint)y);
                    }
                }
            }

            if (hydraulicErosion != null)
                DoHydraulicErosion();

            return DoubleToFloat(NormalizeReturn);
            
        }

        /// <summary>
        /// Returns the value of the noise in the mixer, if any calculations has been done, such as Apply() or ApplyOnOtherThreads().
        /// </summary>
        /// <param name="results">The output of results, if there are any in the form of a double[,].</param>
        /// <param name="NormalizeReturn">The return data values be between (0,1) if true, or between (-1,1) if false.</param> 
        /// <returns>Returns true if theres is a value to return, otherwise returns false</returns>
        public bool GetCalculations(out double[,] results, bool NormalizeReturn)
        {

            if (MixerMiddleOfCalculation == true || CalculationAtleastOnce == false)
            { results = null; return false; }
            else
            {
                if (NormalizeReturn)
                    results = NormalizeCurrentLayerValues();
                else
                    results = currentLayerValues.Clone() as double[,];
                return true;
            }

        }

        /// <summary>
        /// Returns the value of the noise in the mixer in float form, that is if any calculations have been done, such as Apply() or ApplyOnOtherThreads().
        /// </summary>
        /// <param name="results">The output of results, if there are any in the form of a float[,].</param>
        /// <returns>Returns true if theres is a value to return, otherwise returns false</returns>
        public bool GetCalculationsF(out float[,] results, bool NormalizeReturn)
        {

            if (MixerMiddleOfCalculation == true || CalculationAtleastOnce == false)
            { results = null; return false; }
            else
            {
                results = DoubleToFloat(NormalizeReturn);
                return true;
            }

        }

        /// <summary>
        /// Set up mixer to do its calculations on background threads. If main thread computation is needed use Apply() or ApplyF().
        /// </summary>
        /// <param name="ThreadAmount">The Amount of background threads to use.</param>
        /// <returns>Returns true if background threads can be started, returns false in it can not be. </returns>
        public bool ApplyOnOtherThreads(uint ThreadAmount)
        {
            if (MixerMiddleOfCalculation == true)
            { return false; }
            MixerMiddleOfCalculation = true;
            CalculationAtleastOnce = true;

            MultithreadingJobsHandler.AddJob(new Action<int,uint>(SetUpMutiThreadCal), new object[] { ThreadAmount });

            MultithreadingJobsHandler.Update();

            return true;
        }

        /// <summary>
        /// Add hydraulic erosion to be applied after all of the noise layers are computed. 
        /// </summary>
        public void HydraulicErosion(int Iterations)
        {
            iterations = Iterations;
            hydraulicErosion = new HydraulicErosion((uint)currentLayerValues.GetLength(0), (uint)currentLayerValues.GetLength(1));

        }

        /// <summary>
        /// Add hydraulic erosion to be applied after all of the noise layers are computed.
        /// </summary>
        /// <param name="Seed">The Seed to be used for the hydraulic erosion randomness</param>
        public void HydraulicErosion(int Iterations, int Seed)
        {
            iterations = Iterations;
            hydraulicErosion = new HydraulicErosion((uint)currentLayerValues.GetLength(0), (uint)currentLayerValues.GetLength(1), Seed);

        }

        /// <summary>
        /// Full control of the hydraulic erosion system, the erosion will be applied after all of the noise layers are computed.
        /// </summary>
        /// <param name="Seed">The Seed to be used for the hydraulic erosion randomness</param>
        /// <param name="ErosionRadius"></param>
        /// <param name="Inertia"></param>
        /// <param name="SedimentCapacityFactor"></param>
        /// <param name="MinSedimentCapacity"></param>
        /// <param name="ErodeSpeed"></param>
        /// <param name="DepositSpeed"></param>
        /// <param name="EvaporateSpeed"></param>
        /// <param name="Gravity"></param>
        /// <param name="MaxDropletLifetime"></param>
        /// <param name="InitialWaterVolume"></param>
        /// <param name="InitialSpeed"></param>
        public void HydraulicErosion(int Iterations, int Seed, int ErosionRadius = 3, double Inertia = 0.05f, double SedimentCapacityFactor = 4,
                                double MinSedimentCapacity = 0.01f, double ErodeSpeed = 0.3f, double DepositSpeed = 0.3f, double EvaporateSpeed = 0.01f,
                                double Gravity = 4, double MaxDropletLifetime = 30, double InitialWaterVolume = 1, double InitialSpeed = 1)
        {
            iterations = Iterations;
            hydraulicErosion = new HydraulicErosion((uint)currentLayerValues.GetLength(0), (uint)currentLayerValues.GetLength(1), Seed, ErosionRadius, Inertia, SedimentCapacityFactor,
                                                    MinSedimentCapacity, ErodeSpeed, DepositSpeed, EvaporateSpeed, Gravity, MaxDropletLifetime, InitialWaterVolume, InitialSpeed);

        }

        /// <summary>
        /// Remove the hydraulic erosion effect from the mixer.
        /// </summary>
        public void HydraulicErosionRemove()
        {
            hydraulicErosion = null;
        }

        //set up Multi-Threaded calculations. 
        void SetUpMutiThreadCal(int ID, uint ThreadAmount)
        {
            int JobAmount = MultithreadingJobsHandler.GetMaxCurrentThreads;

            if (ThreadAmount < JobAmount)
            {
                JobAmount = (int)ThreadAmount;
            }

            if (ThreadAmount == 0)
            {
                JobAmount = 1;
            }

            returnedThreads = new bool[JobAmount];


            SpliInToChunks(JobAmount, out Vector2[] startPoints, out Vector2[] endPoints);

           

            for (int i = 0; i < JobAmount - 1; i++)
            {
                MultithreadingJobsHandler.AddJob(new Action<int,uint, Vector2,Vector2>(CalculateMixerPart), new object[] { (uint)i, startPoints[i], endPoints[i] });
            }

            MultithreadingJobsHandler.Update();

            CalculateMixerPart(0, (uint)(JobAmount - 1), startPoints[JobAmount - 1], endPoints[JobAmount - 1]);


        }

        //split the 2d array in to chunk for each background thread. 
        void SpliInToChunks(int amountOfChunks, out Vector2[] StartPos, out Vector2[] EndPos)
        {
            Vector2[] startPos = new Vector2[amountOfChunks];
            Vector2[] endPos = new Vector2[amountOfChunks];

            int counterVertor = 0;

            int counter = 0;


            int chuckSize = (int)(currentLayerValues.GetLength(0) * currentLayerValues.GetLength(1) / amountOfChunks);

            //UnityEngine.Debug.Log(chuckSize);
            //UnityEngine.Debug.Log(currentLayerValues.GetLength(0) * currentLayerValues.GetLength(1));

            for (int x = 0; x < currentLayerValues.GetLength(0); x++)
            {

                for (int y = 0; y < currentLayerValues.GetLength(1); y++)
                {
                    
                    if (counter == 0 && counterVertor < amountOfChunks) //should not be greater....but just in case
                    {
                        startPos[counterVertor] = new Vector2(x, y);

                    }

                    counter++;

                    if (counter >= chuckSize && counterVertor < amountOfChunks) //should not be greater....but just in case
                    {
                        counter = 0;

                        endPos[counterVertor++] = new Vector2(x, y);

                    }

                   

                }
            }

            endPos[endPos.Length - 1].X = currentLayerValues.GetLength(0) - 1;
            endPos[endPos.Length - 1].Y = currentLayerValues.GetLength(1) - 1;

            StartPos = startPos;
            EndPos = endPos;
        }

        //the calculations to be run on multi-thread
        void CalculateMixerPart(int Id, uint part, Vector2 startPos, Vector2 endPos)
        {
           
            for (int action = 0; action < Actions.Count; action++)
            {

                for (int x = (int)startPos.X; x <= (int)endPos.X; x++)
                {
                    

                    for (int y = 0; y < currentLayerValues.GetLength(1); y++)
                    {

                        if (x == (int)startPos.X && y < startPos.Y)
                            continue;                       

                        if (x == (int)endPos.X && y > endPos.Y)
                            break;

                        Actions[action].ExecuteCommand((uint)x, (uint)y);
                    }
                }
            }

            returnedThreads[part] = true;
            CheckIfFinished();
        }

        //are all the background threads done computing.
        void CheckIfFinished()
        {
            bool isFinished = true;

            foreach (bool threadFinished in returnedThreads)
            {
                if (threadFinished == false)
                {
                    isFinished = false;
                    break;
                }
            }

            if (isFinished)
            {
                if (hydraulicErosion != null)
                    DoHydraulicErosion();

                MixerMiddleOfCalculation = false;
            }
        }

        private double[,] NormalizeCurrentLayerValues()
        {
            double[,] NormalizeArray = new double[currentLayerValues.GetLength(0), currentLayerValues.GetLength(1)];

            for (int x = 0; x < NormalizeArray.GetLength(0); x++)
            {
                for (int y = 0; y < NormalizeArray.GetLength(1); y++)
                {
                    NormalizeArray[x, y] = (currentLayerValues[x, y] + 1) / 2;
                }
            }

            return NormalizeArray;
        }

        private double[,] AddOneCurrentLayerValues( double[,] ArrayToChange )
        {
            double[,] AddOne = new double[ArrayToChange.GetLength(0), ArrayToChange.GetLength(1)];

            for (int x = 0; x < AddOne.GetLength(0); x++)
            {
                for (int y = 0; y < AddOne.GetLength(1); y++)
                {
                    AddOne[x, y] = (ArrayToChange[x, y] + 1);
                }
            }

            return AddOne;
        }

        private double[,] MinusOneCurrentLayerValues(double[,] ArrayToChange)
        {
            double[,] AddOne = new double[ArrayToChange.GetLength(0), ArrayToChange.GetLength(1)];

            for (int x = 0; x < AddOne.GetLength(0); x++)
            {
                for (int y = 0; y < AddOne.GetLength(1); y++)
                {
                    AddOne[x, y] = (ArrayToChange[x, y] - 1);
                }
            }

            return AddOne;
        }

        private float[,] DoubleToFloat(bool NormalizeReturn)
        {
            float[,] NormalizeArray = new float[currentLayerValues.GetLength(0), currentLayerValues.GetLength(1)];

            for (int x = 0; x < NormalizeArray.GetLength(0); x++)
            {
                for (int y = 0; y < NormalizeArray.GetLength(1); y++)
                {
                    if (NormalizeReturn)
                        NormalizeArray[x, y] = (float)(currentLayerValues[x, y] + 1) / 2;
                    else
                    { NormalizeArray[x, y] = (float)currentLayerValues[x, y]; }
                }
            }

            return NormalizeArray;
        }



        private void DoHydraulicErosion()
        {
            //1D  array for Erosion, must be Positive value. Add one to keep values scaled
            double[] OneDCurrentLayer = Array.Make1DArray(AddOneCurrentLayerValues(currentLayerValues));

            hydraulicErosion.Erode(OneDCurrentLayer, (uint)iterations);

            // Minus one to keep values to original scale;
            double[,] scaledBack = MinusOneCurrentLayerValues(Array.Make2DArray(OneDCurrentLayer, currentLayerValues.GetLength(0), currentLayerValues.GetLength(1)));


            for (int x = 0; x < currentLayerValues.GetLength(0); x++)
            {
                for (int y = 0; y < currentLayerValues.GetLength(1); y++)
                {
                    scaledBack[x, y] = Math.Clamp(scaledBack[x, y], -1, 1);
                }
            }

            currentLayerValues = scaledBack;

        }

    }

    /// <summary>
    /// The section of the class to hold internal classes
    /// </summary>
    public partial class NoiseMixer
    {


        /// <summary>
        /// A Class to invert the current values in the mixer.
        /// </summary>
        private class InvertMixer : NoiseMixerActions
        {
            NoiseMixer mixer;

            public InvertMixer(NoiseMixer Mixer)
            {
                mixer = Mixer;
            }

            public override void ExecuteCommand(uint XPos, uint YPos)
            {
                mixer.currentLayerValues[XPos, YPos] = Math.Clamp(mixer.currentLayerValues[XPos, YPos] * -1, -1, 1);
            }
        }

        /// <summary>
        /// A Class to scale the current values in the mixer.
        /// </summary>
        private class ScaleMixer : NoiseMixerActions
        {
            NoiseMixer mixer;
            double scaleAmount;

            public ScaleMixer(double ScaleAmount, NoiseMixer Mixer)
            {
                mixer = Mixer;
                scaleAmount = ScaleAmount;
            }

            public override void ExecuteCommand(uint XPos, uint YPos)
            {
                mixer.currentLayerValues[XPos, YPos] = Math.Clamp(mixer.currentLayerValues[XPos, YPos] * scaleAmount, -1, 1);
            }
        }


        /// <summary>
        /// A Class to shift the current values in the mixer.
        /// </summary>
        private class ShiftMixer : NoiseMixerActions
        {
            NoiseMixer mixer;
            double shiftAmount;

            public ShiftMixer(double ShiftAmount, NoiseMixer Mixer)
            {
                mixer = Mixer;
                shiftAmount = ShiftAmount;
            }

            public override void ExecuteCommand(uint XPos, uint YPos)
            {
                mixer.currentLayerValues[XPos, YPos] = Math.Clamp(mixer.currentLayerValues[XPos, YPos] + shiftAmount, -1, 1);
            }
        }


        /// <summary>
        /// A Class to tier the current values in the mixer to the nearest value.
        /// </summary>
        private class TierNoise : NoiseMixerActions
        {
            NoiseMixer mixer;

            double[] tierValues;

            double mask;


            public TierNoise(uint TierAmount, double Mask, NoiseMixer Mixer)
            {
                mixer = Mixer;

                mask = Mask;

                if (TierAmount == 0)
                    TierAmount++;

                double spaceBetweenTiers = 2.0 / TierAmount;

                tierValues = new double[TierAmount + 1];

                for (int i = 0; i <= TierAmount; i++)
                {
                    tierValues[i] = -1 + (spaceBetweenTiers * i);
                    
                }
            }

            public static double ClosestTo(double[] collection, double target)
            {
                // NB Method will return int.MaxValue for a sequence containing no elements.
                // Apply any defensive coding here as necessary.
                double closest = double.MaxValue;
                double minDifference = double.MaxValue;
                foreach (double element in collection)
                {
                    var difference = System.Math.Abs(element - target);
                    if (minDifference > difference)
                    {
                        minDifference = difference;
                        closest = element;
                    }
                }

                return closest;
            }

            public override void ExecuteCommand(uint XPos, uint YPos)
            {


                mixer.currentLayerValues[XPos, YPos] = Math.Lerp(ClosestTo(tierValues, mixer.currentLayerValues[XPos, YPos]), mixer.currentLayerValues[XPos, YPos], mask);


            }
        }

        /// <summary>
        /// A Class to only add noise with the current values in the mixer if it is lower then current value.
        /// </summary>
        public class OnlyLowerLayer : LayerBase
        {
            public OnlyLowerLayer(INoise Noise, double ScaleNoise, NoiseMixer Mixer) : base(Noise, ScaleNoise, Mixer)
            {
            }

            public override void ExecuteCommand(uint XPos, uint YPos)
            {
                double value = noise.GetValue(XPos * scaleNoise, YPos * scaleNoise);

                //do Base Manipulations effects
                value = BaseManipulations(value);

                //add the mask effect
                value = Math.Lerp(value, mixer.currentLayerValues[XPos, YPos], GetLayerMask(XPos, YPos));

                if (value < mixer.currentLayerValues[XPos, YPos])
                    mixer.currentLayerValues[XPos, YPos] = Math.Clamp(value, -1, 1);

            }
        }

        /// <summary>
        /// A Class to only add noise with the current values in the mixer if it is higher then current value.
        /// </summary>
        public class OnlyHigherLayer : LayerBase
        {
            public OnlyHigherLayer(INoise Noise, double ScaleNoise, NoiseMixer Mixer) : base(Noise, ScaleNoise, Mixer)
            {
            }

            public override void ExecuteCommand(uint XPos, uint YPos)
            {

                double value = noise.GetValue(XPos * scaleNoise, YPos * scaleNoise);

                //do Base Manipulations effects
                value = BaseManipulations(value);

                //add the mask effect
                value = Math.Lerp(value, mixer.currentLayerValues[XPos,YPos], GetLayerMask(XPos, YPos));

                if (XPos == 1 && YPos == 1)
                {
                    UnityEngine.Debug.Log(GetLayerMask(XPos, YPos));
                }

                if (value > mixer.currentLayerValues[XPos, YPos])
                    mixer.currentLayerValues[XPos, YPos] = Math.Clamp(value, -1, 1);
            }
        }

        /// <summary>
        /// A Class to divide noise the current values in the mixer by a new noise.
        /// </summary>
        public class DivideLayer : LayerBase
        {
            public DivideLayer(INoise Noise, double ScaleNoise, NoiseMixer Mixer) : base(Noise, ScaleNoise, Mixer)
            {
            }

            public override void ExecuteCommand(uint XPos, uint YPos)
            {

                double value = noise.GetValue(XPos * scaleNoise, YPos * scaleNoise);

                //do Base Manipulations effects
                value = BaseManipulations(value);

                //add the mask effect
                value = Math.Lerp(value, 1, GetLayerMask(XPos, YPos));


                if (value == 0)
                    value = double.Epsilon;


                mixer.currentLayerValues[XPos, YPos] = Math.Clamp((mixer.currentLayerValues[XPos, YPos] / value), -1, 1);


            }

        }

        /// <summary>
        /// A Class to multiply noise the current values in the mixer by a new noise.
        /// </summary>
        public class MultiplyLayer : LayerBase
        {
            public MultiplyLayer(INoise Noise, double ScaleNoise, NoiseMixer Mixer) : base(Noise, ScaleNoise, Mixer)
            {
            }

            public override void ExecuteCommand(uint XPos, uint YPos)
            {

                double value = noise.GetValue(XPos * scaleNoise, YPos * scaleNoise);

                //do Base Manipulations effects
                value = BaseManipulations(value);

                //add the mask effect
                value = Math.Lerp(value, 1, GetLayerMask(XPos, YPos));

                mixer.currentLayerValues[XPos, YPos] = Math.Clamp((mixer.currentLayerValues[XPos, YPos] * value), -1, 1);

            }

        }

        /// <summary>
        /// A Class to Subtract a new noise from noise the current values in the mixer.
        /// </summary>
        public class SubtractLayer : LayerBase
        {
            public SubtractLayer(INoise Noise, double ScaleNoise, NoiseMixer Mixer) : base(Noise, ScaleNoise, Mixer)
            {
            }

            public override void ExecuteCommand(uint XPos, uint YPos)
            {
                double value = noise.GetValue(XPos * scaleNoise, YPos * scaleNoise);

                //do Base Manipulations effects
                value = BaseManipulations(value);

                //add the mask effect
                value = Math.Lerp(value, 0, GetLayerMask(XPos, YPos));

                mixer.currentLayerValues[XPos, YPos] = Math.Clamp((mixer.currentLayerValues[XPos, YPos] - value), -1, 1);
            }
        }

        /// <summary>
        /// A Class to Add a new noise from noise the current values in the mixer.
        /// </summary>
        public class AddLayer : LayerBase
        {
            public AddLayer(INoise Noise, double ScaleNoise, NoiseMixer Mixer) : base(Noise, ScaleNoise, Mixer)
            {  
            }

            public override void ExecuteCommand(uint XPos, uint YPos)
            {
                double value = noise.GetValue(XPos * scaleNoise, YPos * scaleNoise);

                //do Base Manipulations effects
                value = BaseManipulations(value);

                //add the mask effect
                value = Math.Lerp(value, 0, GetLayerMask(XPos, YPos));

                mixer.currentLayerValues[XPos, YPos] = Math.Clamp((mixer.currentLayerValues[XPos, YPos] + value), -1, 1);

                /*if (XPos == 1 && YPos == 1)
                    UnityEngine.Debug.Log("Value is " + mixer.currentLayerValues[XPos, YPos]);*/

            }
        }

        /// <summary>
        /// A command to Mix a new noise with the current values in the mixer.
        /// </summary>
        public class CombineLayer : LayerBase
        {
            public CombineLayer(INoise Noise, double ScaleNoise, NoiseMixer Mixer) : base(Noise, ScaleNoise, Mixer)
            {

            }

            public override void ExecuteCommand(uint XPos, uint YPos)
            {
                double value = noise.GetValue(XPos * scaleNoise, YPos * scaleNoise);

                value = BaseManipulations(value);

                //add the mask effect
                value = Math.Lerp(value , mixer.currentLayerValues[XPos, YPos], GetLayerMask(XPos, YPos));

                mixer.currentLayerValues[XPos, YPos]  = Math.Clamp((mixer.currentLayerValues[XPos, YPos] + value) / 2, -1 , 1);
            }

        }

        /// <summary>
        /// A base Layer class to handle generic method between layer types
        /// </summary>
        public class LayerBase : NoiseMixerActions
        {
            protected NoiseMixer mixer;

            protected INoise noise;
            protected readonly double scaleNoise;
            protected dynamic layerMask = 0.0;
            protected double? scaleLayerMask;


            bool inverse = false;
            bool scale = false;
            bool shift = false;

            double scaleAmount = 0;
            double shiftAmount = 0;

            public LayerBase(INoise Noise, double ScaleNoise, NoiseMixer Mixer)
            {
                noise = Noise;
                scaleNoise = ScaleNoise;
                mixer = Mixer;
            }


            /// <summary>
            /// A Mask to be applied to dampen the results of the layer.
            /// </summary>
            /// <param name="LayerMask">The value to be used to create a layer mask. Must be between (0,1). </param>
            /// <param name="ScaleLayerMask">The scale of the noise to be used in the layer mask</param>
            /// <returns></returns>
            public LayerBase LayerMask(INoise LayerMask, double ScaleLayerMask)
            {

                layerMask = LayerMask;
                this.scaleLayerMask = ScaleLayerMask;
                return this;
            }

            /// <summary>
            /// A Mask to be applied to dampen the results of the layer.
            /// </summary>
            /// <param name="LayerMask">The value to be used to create a layer mask. Must be between (0,1). </param>
            public LayerBase LayerMask(double LayerMask)
            {
                layerMask = LayerMask;
                return this;
            }

            /// <summary>
            /// A Mask to be applied to dampen the results of the layer.
            /// </summary>
            /// <param name="LayerMask">The value to be used to create a layer mask. Must be between (0,1). </param>
            public LayerBase LayerMask(double[,] LayerMask)
            {

                if (LayerMask.GetLength(0) == mixer.currentLayerValues.GetLength(0) && LayerMask.GetLength(1) == mixer.currentLayerValues.GetLength(1))
                {
                    layerMask = LayerMask;
                }
                else
                {
                    double xScale = (double)mixer.currentLayerValues.GetLength(0) / (double)LayerMask.GetLength(0);
                    double YScale = (double)mixer.currentLayerValues.GetLength(1) / (double)LayerMask.GetLength(1);

                    layerMask = Array.BilinearInterpolation(LayerMask, (float)xScale, (float)YScale);

                }
                return this;
            }

            protected double GetLayerMask(uint XPos, uint YPos)
            {

                double layerMaskValue = 0;

                if (layerMask is INoise)
                {
                    layerMaskValue = layerMask.GetValue((double)(XPos * scaleLayerMask), (double)(YPos * scaleLayerMask));
                }
                else if (layerMask.GetType() == typeof(double))
                {
                    layerMaskValue = layerMask;
                }
                else if (layerMask.GetType() == typeof(double[,]))
                {
                    layerMaskValue = layerMask[XPos, YPos];
                }

                return Math.Clamp(layerMaskValue, 0, 1);


            }

            //DO NOT USE THIS BASE LAYER METHOD --- MUST BE OVER RIDDEN ID DERIVED CLASS!!!
            public override void ExecuteCommand(uint XPos, uint YPos)
            {
            }

            /// <summary>
            /// The manipulations (shift, inverse, scale, etc.) needed to be done to the original value.
            /// </summary>
            /// <param name="value">The value to check to manipulate</param>
            /// <returns></returns>
            protected double BaseManipulations(double value)
            {

                if (inverse)
                {
                    value *= -1;
                }
                if (scale)
                {
                    value *= scaleAmount;
                }
                if (shift)
                {
                    value += shiftAmount;
                }


                return value;

            }

            /// <summary>
            /// Inverse the layer, values will be opposite what they where. The result will always between (-1,1).
            /// </summary>
            public LayerBase Inverse()
            {
                inverse = true;
                return this;
            }

            /// <summary>
            /// Shift the whole noise mixer in a direction, the result will always between (-1,1).
            /// </summary>
            /// <param name="ShiftAmount">The amount to shift in one direction.</param>
            public LayerBase Shift(double ShiftAmount)
            {
                shift = true;
                shiftAmount = ShiftAmount;
                return this;
            }

            /// <summary>
            /// Scale the layer, pushing higher values higher, and lower value lower. The result will always between (-1,1).
            /// </summary>
            /// <param name="ScaleAmount">The amount to scale, a value 1 will not effect the mixer at all.</param>
            public LayerBase Scale(double ScaleAmount)
            {
                scale = true;
                scaleAmount = ScaleAmount;
                return this;
            }
        }

        /// <summary>
        /// A command to overwrite the current values in the mixer.
        /// </summary>
        public class FillValue : NoiseMixerActions
        {
            NoiseMixer mixer;
            double value;

            /// <summary>
            /// The Constructor.
            /// </summary>
            /// <param name="value">The value to overwrite the current values in the mixer.</param>
            public FillValue(double value, NoiseMixer Mixer)
            {
                mixer = Mixer;
                this.value = Math.Clamp(value, -1, 1);
            }

            public override void ExecuteCommand(uint XPos, uint YPos)
            {
                mixer.currentLayerValues[XPos, YPos] = value;
            }

        }

        public abstract class NoiseMixerActions
        {
            /// <summary>
            /// A command to Execute what ever command is needed.
            /// </summary>
            /// <param name="XPos"></param>
            /// <param name="YPos"></param>
            /// <param name="mixer"></param>
            public abstract void ExecuteCommand(uint XPos, uint YPos);

        }

    }

}
