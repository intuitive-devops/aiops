using System;

public class Noise : NoiseBase<Noise.Vector3>
{
    private static Vector3[] gradients = {new Vector3(1,1,0), new Vector3(-1,1,-0),
        new Vector3(1,-1,0), new Vector3(-1,-1,0), new Vector3(1,0,1),
        new Vector3(-1,0,1), new Vector3(1,0,-1), new Vector3(-1,0,-1),
        new Vector3(0,1,1), new Vector3(0,-1,1), new Vector3(0,1,-1),
        new Vector3(0,-1,-1)};
    /// <summary>
    /// Initializes a new instance of the <see cref="Noise"/> class.
    /// </summary>
    /// <param name="smoothingFunction">The smoothing function.</param>
    public Noise(Func<double, double> smoothingFunction) : base(gradients, Dot, smoothingFunction) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="Noise"/> class.
    /// </summary>
    public Noise() : this(SmoothToSCurve) { }

    private static double Dot(Vector3 gradient, double x, double y, double z)
    {
        return gradient.x * x + gradient.y * y + gradient.z * z;
    }
    /// <summary>
    /// A structure for a vector of three-dimensions.
    /// </summary>
    public struct Vector3
    {
        public double x, y, z;

        public Vector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
