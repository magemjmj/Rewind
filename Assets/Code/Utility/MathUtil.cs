using UnityEngine;

public class MathUtil
{
    // Linear
    public static float
    LinLerp(float a, float b, float t)
    {
        return (b - a) * t + a;
    }

    // Cosine
    public static float
    CosLerp(float a, float b, float t)
    {
        float f = (1.0f - Mathf.Cos(Mathf.PI * t)) * 0.5f;
        return (b - a) * f + a;
    }

    // Tri-linear
    public static float
    TriLerp(float a, float b, float t)
    {
        float f = t * t * (3.0f - 2.0f * t);
        return (b - a) * f + a;
    }

    // Fade Curve
    public static float
    FadeLerp(float a, float b, float t)
    {
        float f = t * t * t * (t * (t * 6.0f - 15.0f) + 10.0f);
        return (b - a) * f + a;
    }

    // TEMPLATE FUNCTION Min
    public static float
    Min(float a, float b)
	{
		// return (std::min<_T>(a, b));
		return (a <= b) ? a : b;
	}

    // TEMPLATE FUNCTION Max
    public static float
    Max(float a, float b)
	{
		// return (std::max<_Ty>(a, b));
		return (a >= b) ? a : b;
	}

    // TEMPLATE FUNCTION Clamp Min ~ Max
    public static float
    Clamp(float a, float _Min, float _Max)
	{
		// return (Max<T>(Min<T>(a, _Max), _Min));
		return a <_Min ? _Min : a <_Max ? a : _Max;
	}

    // TEMPLATE FUNCTION Align
    public static float
    Align(float a, uint Alignment)
	{
		return (float)(((uint)a + Alignment - 1) & ~(Alignment - 1));
	}

    // TEMPLATE FUNCTION Abs
    public static float
    Abs(float a)
	{
		return (a >= 0.0f) ? a : -a;
	}

    // TEMPLATE FUNCTION Sgn
    public static float
    Sgn(float a)
	{
		return (a > 0) ? 1 : ((a < 0) ? -1 : 0);
	}

    // TEMPLATE FUNCTION Square
    public static float
    Square(float a)
	{
		return a * a;
	}

    // TEMPLATE FUNCTION Step (Min ~ Max) => (0 ~ 1)
    public static float
    Step(float a, float _Min, float _Max)
	{
		return (Clamp(a, _Min, _Max) - _Min) / (_Max - _Min);
	}

    // TEMPLATE FUNCTION Cfit (Min ~ Max) => (NewMin ~ NewMax)
    public static float
    Cfit(float a, float _Min, float _Max, float _NewMin, float _NewMax)
	{
		return LinLerp(_NewMin, _NewMax, Step(a, _Min, _Max));
	}

    // Mod
    public static float
    Mod(float a, float b)
    {
        float r = a % b;
        return r >= 0.0f ? r : r + Abs(b);
    }
}
