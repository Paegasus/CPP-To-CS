using System.Runtime.CompilerServices;
using UI.Numerics;

using static UI.Numerics.AngleConversions;

namespace UI.GFX.Geometry;

/// <summary>
/// A 3D vector of floats, used to represent a distance in 3D space.
/// </summary>
public struct Vector3DF : IEquatable<Vector3DF>
{
    private const double Epsilon = 1.0e-6;

    private float x_, y_, z_;

    public float x { readonly get => x_; set => x_ = value; }
    public float y { readonly get => y_; set => y_ = value; }
    public float z { readonly get => z_; set => z_ = value; }

    public Vector3DF()
    {
        x_ = 0.0f;
        y_ = 0.0f;
        z_ = 0.0f;
    }

    public Vector3DF(float x, float y, float z)
    {
        x_ = x;
        y_ = y;
        z_ = z;
    }
    
    public Vector3DF(in Vector2DF vector)
    {
        x_ = vector.x;
        y_ = vector.y;
        z_ = 0;
    }

    /// <summary>
    /// Checks if all components of the vector are zero.
    /// </summary>
    public readonly bool IsZero() => x_ == 0.0f && y_ == 0.0f && z_ == 0.0f;

    /// <summary>
    /// Adds the components of the other vector to this vector.
    /// </summary>
    public void Add(in Vector3DF other)
    {
        x_ += other.x_;
        y_ += other.y_;
        z_ += other.z_;
    }

    /// <summary>
    /// Subtracts the components of the other vector from this vector.
    /// </summary>
    public void Subtract(in Vector3DF other)
    {
        x_ -= other.x_;
        y_ -= other.y_;
        z_ -= other.z_;
    }

    public void SetToMin(in Vector3DF other)
    {
        x_ = Math.Min(x_, other.x_);
        y_ = Math.Min(y_, other.y_);
        z_ = Math.Min(z_, other.z_);
    }

    public void SetToMax(in Vector3DF other)
    {
        x_ = Math.Max(x_, other.x_);
        y_ = Math.Max(y_, other.y_);
        z_ = Math.Max(z_, other.z_);
    }

    /// <summary>
    /// Returns the square of the vector's length.
    /// </summary>
    public readonly double LengthSquared() => (double)x_ * x_ + (double)y_ * y_ + (double)z_ * z_;
    
    /// <summary>
    /// Returns the vector's length.
    /// </summary>
    public readonly float Length() => (float)Math.Sqrt(LengthSquared());
    
    /// <summary>
    /// Scales all components of the vector uniformly by a single scale factor.
    /// </summary>
    public void Scale(float scale) => Scale(scale, scale, scale);

    /// <summary>
    /// Scales each component of the vector by the given scale factors.
    /// </summary>
    public void Scale(float x_scale, float y_scale, float z_scale)
    {
        x_ *= x_scale;
        y_ *= y_scale;
        z_ *= z_scale;
    }
    
    /// <summary>
    /// Divides all components of the vector by a single scale factor.
    /// </summary>
    public void InvScale(float inv_scale) => InvScale(inv_scale, inv_scale, inv_scale);

    /// <summary>
    /// Divides each component of the vector by the given scale factors.
    /// </summary>
    public void InvScale(float inv_x_scale, float inv_y_scale, float inv_z_scale)
    {
        x_ /= inv_x_scale;
        y_ /= inv_y_scale;
        z_ /= inv_z_scale;
    }
    
    /// <summary>
    /// Computes the cross product of this vector with another and updates this vector with the result.
    /// </summary>
    public void Cross(in Vector3DF other)
    {
        double dx = x_;
        double dy = y_;
        double dz = z_;
        float new_x = (float)(dy * other.z_ - dz * other.y_);
        float new_y = (float)(dz * other.x_ - dx * other.z_);
        float new_z = (float)(dx * other.y_ - dy * other.x_);
        x_ = new_x;
        y_ = new_y;
        z_ = new_z;
    }

    /// <summary>
    /// Attempts to compute a unit-length vector in the same direction.
    /// </summary>
    /// <param name="result">The resulting normalized vector.</param>
    /// <returns>True if the vector was successfully normalized, false if the vector is too short (close to zero).</returns>
    public readonly bool GetNormalized(out Vector3DF result)
    {
        result = this;
        double lengthSquared = LengthSquared();
        if (lengthSquared < Epsilon * Epsilon)
            return false;
        result.InvScale((float)Math.Sqrt(lengthSquared));
        return true;
    }
    
    public override readonly string ToString() => $"[{x_} {y_} {z_}]";

    public override readonly int GetHashCode() => HashCode.Combine(x_, y_, z_);

    public override readonly bool Equals(object? obj) => obj is Vector3DF other && Equals(other);

    public readonly bool Equals(Vector3DF other) => x_ == other.x_ && y_ == other.y_ && z_ == other.z_;

    public static bool operator ==(in Vector3DF left, in Vector3DF right) => left.Equals(right);
    public static bool operator !=(in Vector3DF left, in Vector3DF right) => !left.Equals(right);
    
    public static Vector3DF operator -(in Vector3DF v) => new Vector3DF(-v.x_, -v.y_, -v.z_);

    public static Vector3DF operator +(in Vector3DF lhs, in Vector3DF rhs)
    {
        Vector3DF result = lhs;
        result.Add(rhs);
        return result;
    }

    public static Vector3DF operator -(in Vector3DF lhs, in Vector3DF rhs)
    {
        Vector3DF result = lhs;
        result.Subtract(rhs);
        return result;
    }
    
    /// <summary>
    /// Returns the cross product of two vectors.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3DF CrossProduct(in Vector3DF lhs, in Vector3DF rhs)
    {
        Vector3DF result = lhs;
        result.Cross(rhs);
        return result;
    }

    /// <summary>
    /// Returns the dot product of two vectors.
    /// </summary>
    public static float DotProduct(in Vector3DF lhs, in Vector3DF rhs)
    {
        return lhs.x_ * rhs.x_ + lhs.y_ * rhs.y_ + lhs.z_ * rhs.z_;
    }

    /// <summary>
    /// Returns a new vector created by scaling the components of |v| by the components of |s|.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3DF ScaleVector(in Vector3DF v, in Vector3DF s)
    {
        return new Vector3DF(v.x_ * s.x_, v.y_ * s.y_, v.z_ * s.z_);
    }
    
    /// <summary>
    /// Returns a new vector created by scaling |v| by the given scale factors.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3DF ScaleVector(in Vector3DF v, float x_scale, float y_scale, float z_scale)
    {
        return new Vector3DF(v.x_ * x_scale, v.y_ * y_scale, v.z_ * z_scale);
    }

    /// <summary>
    /// Returns a new vector created by scaling |v| by a uniform scale factor.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3DF ScaleVector(in Vector3DF v, float scale)
    {
        return ScaleVector(v, scale, scale, scale);
    }
    
    /// <summary>
    /// Returns the angle between two vectors in degrees.
    /// </summary>
    public static float AngleBetweenVectorsInDegrees(in Vector3DF baseVec, in Vector3DF other)
    {
        float angle = RadToDeg(
            MathF.Acos(
                Math.Clamp(DotProduct(baseVec, other) / baseVec.Length() / other.Length(), -1.0f, 1.0f)
            )
        );
        return angle;
    }

    /// <summary>
    /// Returns the clockwise angle between two vectors, viewed from the direction of a normal vector.
    /// </summary>
    public static float ClockwiseAngleBetweenVectorsInDegrees(in Vector3DF baseVec, in Vector3DF other, in Vector3DF normal)
    {
        float angle = AngleBetweenVectorsInDegrees(baseVec, other);
        Vector3DF cross = CrossProduct(baseVec, other);

        // If the dot product of this cross product is normal, it means that the
        // shortest angle between |base| and |other| was counterclockwise and
        // this angle must be reversed.
        if (DotProduct(cross, normal) > 0.0f)
            angle = 360.0f - angle;
        return angle;
    }
}
