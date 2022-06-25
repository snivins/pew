// Type: UnityEngine.Vector2
// Assembly: UnityEngine, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Program Files (x86)\Unity\Editor\Data\Managed\UnityEngine.dll
using System;
using Unity.Mathematics;

namespace UnityEngine
{
    public struct Vector3d
    {
        const double EPSILON_MAGNITUDE = 9.99999974737875E-06;      // ~= 1e-5
        const double EPSILON_MAGNITUDE_SQR = EPSILON_MAGNITUDE * EPSILON_MAGNITUDE;

        public const double kEpsilon = 1E-05d;		// Unused? Should be merged with EPSILON_MAGNITUDE?
        public double x;
        public double y;
        public double z;

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.x;
                    case 1:
                        return this.y;
                    case 2:
                        return this.z;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3d index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;
                    case 1:
                        this.y = value;
                        break;
                    case 2:
                        this.z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3d index!");
                }
            }
        }

        public Vector3d normalized
        {
            get
            {
                Vector3d vector3d = new Vector3d(this.x, this.y, this.z);
                vector3d.Normalize();
                return vector3d;
            }
        }

        public double magnitude
        {
            get
            {
                return Mathd.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
            }
        }
        public double magnitudems
        {
            get
            {
                return Mathd.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z) *1000d;
            }
        }

        public double sqrMagnitude
        {
            get
            {
                return this.x * this.x + this.y * this.y + this.z * this.z;
            }
        }

        public static Vector3d zero
        {
            get
            {
                return new Vector3d(0.0d, 0.0d, 0.0d);
            }
        }

        public static Vector3d one
        {
            get
            {
                return new Vector3d(1d, 1d, 1d);
            }
        }

        public static Vector3d up
        {
            get
            {
                return new Vector3d(0.0d, 1d, 0.0d);
            }
        }

        public static Vector3d right
        {
            get
            {
                return new Vector3d(1d, 0.0d, 0.0d);
            }
        }
        public static Vector3d forward
        {
            get
            {
                return new Vector3d(0.0d, 0.0d, 1d);
            }
        }

        public Vector3d(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Vector3d(Vector3 coord)
        {
            this.x = (double)coord.x;
            this.y = (double)coord.y;
            this.z = (double)coord.z;
        }
        public Vector3d(float3 coord)
        {
            this.x = (double)coord.x;
            this.y = (double)coord.y;
            this.z = (double)coord.z;
        }

        public static implicit operator Vector2d(Vector3d v)
        {
            return new Vector2d(v.x, v.y);
        }

        public static implicit operator Vector3d(Vector2d v)
        {
            return new Vector3d(v.x, v.y, 0.0d);
        }

        public static Vector3d operator +(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3d operator -(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3d operator -(Vector3d a)
        {
            return new Vector3d(-a.x, -a.y, -a.z);
        }

        public static Vector3d operator *(Vector3d a, double d)
        {
            return new Vector3d(a.x * d, a.y * d, a.z*d);
        }

        public static Vector3d operator *(double d, Vector3d a)
        {
            return new Vector3d(a.x * d, a.y * d, a.z*d);
        }

        public static Vector3d operator /(Vector3d a, double d)
        {
            return new Vector3d(a.x / d, a.y / d, a.z / d);
        }

        public static bool operator ==(Vector3d lhs, Vector3d rhs)
        {
            // Implementation similar to Vector3
            return Vector3d.SqrMagnitude(lhs - rhs) < EPSILON_MAGNITUDE_SQR;
        }

        public static bool operator !=(Vector3d lhs, Vector3d rhs)
        {
            return !(lhs == rhs);
        }

        public void Set(double new_x, double new_y)
        {
            this.x = new_x;
            this.y = new_y;
        }
        public static Vector3d Cross(Vector3d lhs, Vector3d rhs)
        {
            return new Vector3d(
                lhs.y * rhs.z - lhs.z * rhs.y,
                lhs.z * rhs.x - lhs.x * rhs.z,
                lhs.x * rhs.y - lhs.y * rhs.x);
        }
        public static Vector3d Lerp(Vector3d from, Vector3d to, double t)
        {
            t = Mathd.Clamp01(t);
            return new Vector3d(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t);
        }

        public static Vector3d MoveTowards(Vector3d current, Vector3d target, double maxDistanceDelta)
        {          // avoid vector ops because current scripting backends are terrible at inlining
            double toVector_x = target.x - current.x;
            double toVector_y = target.y - current.y;
            double toVector_z = target.z - current.z;

            double sqdist = toVector_x * toVector_x + toVector_y * toVector_y + toVector_z * toVector_z;

            if (sqdist == 0 || (maxDistanceDelta >= 0 && sqdist <= maxDistanceDelta * maxDistanceDelta))
                return target;
            double dist = (double)Math.Sqrt(sqdist);

            return new Vector3d(current.x + toVector_x / dist * maxDistanceDelta,
                current.y + toVector_y / dist * maxDistanceDelta,
                current.z + toVector_z / dist * maxDistanceDelta);
        }

        public static Vector3d Scale(Vector3d a, Vector3d b, Vector3d c)
        {
            return new Vector3d(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3d Tove3(float3 floa)
        {
            return new Vector3d((double)floa.x, (double)floa.y, (double)floa.z);
        }
        public static float3 Tofl3(Vector3d v3)
        {
            return new float3((float)v3.x, (float)v3.y, (float)v3.z);
        }

        public void Scale(Vector2d scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
        }

        public void Normalize()
        {
            double magnitude = this.magnitude;
            if (magnitude > EPSILON_MAGNITUDE)
                this = this / magnitude;
            else
                this = Vector3d.zero;
        }

        public Vector3 ToFloat()
        {
            return new Vector3((float)this.x, (float)this.y, (float)this.z);
        }
        public static Vector3 ToFloaterino(Vector3d v3)
        {
            return new Vector3((float)v3.x, (float)v3.y, (float)v3.z);
        }
        public override string ToString()
        {
            /*
      string fmt = "({0:D1}, {1:D1})";
      object[] objArray = new object[2];
      int index1 = 0;
      // ISSUE: variable of a boxed type
      __Boxed<double> local1 = (ValueType) this.x;
      objArray[index1] = (object) local1;
      int index2 = 1;
      // ISSUE: variable of a boxed type
      __Boxed<double> local2 = (ValueType) this.y;
      objArray[index2] = (object) local2;
      */
            return new Vector3((float)this.x, (float)this.y, (float)this.z).ToString();
        }

        public string ToString(string format)
        {
            /* TODO:
      string fmt = "({0}, {1})";
      object[] objArray = new object[2];
      int index1 = 0;
      string str1 = this.x.ToString(format);
      objArray[index1] = (object) str1;
      int index2 = 1;
      string str2 = this.y.ToString(format);
      objArray[index2] = (object) str2;
      */
            return "not implemented";
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() << 2;
        }

        public override bool Equals(object other)
        {
            if (!(other is Vector2d))
                return false;
            Vector2d vector2d = (Vector2d)other;
            if (this.x.Equals(vector2d.x))
                return this.y.Equals(vector2d.y);
            else
                return false;
        }

        public static double Dot(Vector2d lhs, Vector2d rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y;
        }

        public static double Angle(Vector2d from, Vector2d to)
        {
            return Mathd.Acos(Mathd.Clamp(Vector2d.Dot(from.normalized, to.normalized), -1d, 1d)) * 57.29578d;
        }

        public static double Distance(Vector3d a, Vector3d b)
        {
            return (a - b).magnitude;
        }

        public static Vector3d ClampMagnitude(Vector3d vector, double maxLength)
        {
            if (vector.sqrMagnitude > maxLength * maxLength)
                return vector.normalized * maxLength;
            else
                return vector;
        }

        public static double SqrMagnitude(Vector3d a)
        {
            return (a.x * a.x + a.y * a.y + a.z * a.z);
        }

        public double SqrMagnitude()
        {
            return (this.x * this.x + this.y * this.y + this.z * this.z);
        }

        public static Vector3d Min(Vector3d lhs, Vector3d rhs)
        {
            return new Vector3d(Mathd.Min(lhs.x, rhs.x), Mathd.Min(lhs.y, rhs.y), Mathd.Min(lhs.z, rhs.z));
        }

        public static Vector2d Max(Vector2d lhs, Vector2d rhs)
        {
            return new Vector2d(Mathd.Max(lhs.x, rhs.x), Mathd.Max(lhs.y, rhs.y));
        }
    }
}
