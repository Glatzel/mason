using Mason.Geometry;
using static System.Math;

namespace Geometry.Test;

public class TestVec3
{
    [Fact]
    public void TesOne()
    {
        Vec3 v = Vec3.One;
        Assert.Equal(1, v.X);
        Assert.Equal(1, v.Y);
        Assert.Equal(1, v.Z);
    }

    [Fact]
    public void TestAdd()
    {
        {
            Vec3 u = new(1, 2, 3);
            Vec3 v = new(1, 2, 3);
            Vec3 result = Vec3.Add(in u, in v);
            Assert.Equal(2, result.X);
            Assert.Equal(4, result.Y);
            Assert.Equal(6, result.Z);
        }
        {
            Vec3 u = new(1, 2, 3);
            Vec3 result = Vec3.Add(in u, 4);
            Assert.Equal(5, result.X);
            Assert.Equal(6, result.Y);
            Assert.Equal(7, result.Z);
        }
        {
            Vec3 u = new(1, 2, 3);
            Vec3 v = new(1, 2, 3);
            Vec3 result = new();
            Vec3.Add(in u, in v, ref result);
            Assert.Equal(2, result.X);
            Assert.Equal(4, result.Y);
            Assert.Equal(6, result.Z);
        }
        {
            Vec3 u = new(1, 2, 3);
            Vec3.Add(in u, 4, ref u);
            Assert.Equal(5, u.X);
            Assert.Equal(6, u.Y);
            Assert.Equal(7, u.Z);
        }
        {
            Vec3 u = new(1, 2, 3);
            u.Add(4);
            Assert.Equal(5, u.X);
            Assert.Equal(6, u.Y);
            Assert.Equal(7, u.Z);
        }
        {
            Vec3 u = new(1, 2, 3);
            Vec3 v = new(1, 2, 3);
            u.Add(in v);
            Assert.Equal(2, u.X);
            Assert.Equal(4, u.Y);
            Assert.Equal(6, u.Z);
        }
    }

    [Fact]
    public void TestConstructor()
    {
        {
            Vec3 v = new();
            Assert.Equal(0, v.X);
            Assert.Equal(0, v.Y);
            Assert.Equal(0, v.Z);
        }
        {
            Vec3 v = new(3);
            Assert.Equal(3, v.X);
            Assert.Equal(3, v.Y);
            Assert.Equal(3, v.Z);
        }
        {
            Vec3 v = new(1, 2, 3);
            Assert.Equal(1, v.X);
            Assert.Equal(2, v.Y);
            Assert.Equal(3, v.Z);
        }
        {
            Vec3 v = new([1, 2, 3]);
            Assert.Equal(1, v.X);
            Assert.Equal(2, v.Y);
            Assert.Equal(3, v.Z);
        }
        {
            Assert.ThrowsAny<Exception>(() => new Vec3([1, 2]));
        }
    }

    [Fact]
    public void TestCross()
    {
        Vec3 u = new(1, 2, 3);
        Vec3 v = new(2, 4, 7);
        Vec3 result = Vec3.Cross(in u, in v);
        Assert.Equal(2, result.X);
        Assert.Equal(-1, result.Y);
        Assert.Equal(0, result.Z);
    }

    [Fact]
    public void TestDivide()
    {
        {
            Vec3 u = new(4, 6, 15);
            Vec3 v = new(1, 2, 3);
            Vec3 result = Vec3.Divide(in u, in v);
            Assert.Equal(4, result.X);
            Assert.Equal(3, result.Y);
            Assert.Equal(5, result.Z);
        }
        {
            Vec3 u = new(3, 9, 15);
            Vec3 result = Vec3.Divide(in u, 3);
            Assert.Equal(1, result.X);
            Assert.Equal(3, result.Y);
            Assert.Equal(5, result.Z);
        }
        {
            Vec3 u = new(4, 6, 15);
            Vec3 v = new(1, 2, 3);
            Vec3 result = new();
            Vec3.Divide(in u, in v, ref result);
            Assert.Equal(4, result.X);
            Assert.Equal(3, result.Y);
            Assert.Equal(5, result.Z);
        }
        {
            Vec3 u = new(3, 9, 15);
            Vec3.Divide(in u, 3, ref u);
            Assert.Equal(1, u.X);
            Assert.Equal(3, u.Y);
            Assert.Equal(5, u.Z);
        }
        {
            Vec3 u = new(3, 9, 15);
            u.Divide(3);
            Assert.Equal(1, u.X);
            Assert.Equal(3, u.Y);
            Assert.Equal(5, u.Z);
        }
        {
            Vec3 u = new(4, 6, 15);
            Vec3 v = new(1, 2, 3);
            u.Divide(in v);
            Assert.Equal(4, u.X);
            Assert.Equal(3, u.Y);
            Assert.Equal(5, u.Z);
        }
    }

    [Fact]
    public void TestDot()
    {
        Vec3 u = new(1, 2, 3);
        Vec3 v = new(3, 4, 5);
        double result = Vec3.Dot(in u, in v);
        Assert.Equal(26, result);
    }

    [Fact]
    public void TestEquals()
    {
        Vec3 u = new(1, 2, 3);
        Vec3 v = new(3, 4, 5);
        Vec3 w = new(1, 2, 3);
        double[] x = [12, 13, 14];
        object y = new Vec3(1, 2, 3);

        {
            Assert.False(u.Equals(v));
            Assert.True(u.Equals(w));
            Assert.False(u.Equals(x));
            Assert.True(u.Equals(y));
        }
    }

    [Fact]
    public void TestGetHashCode()
    {
        Vec3 u = new(1, 2, 3);
        Assert.Equal(HashCode.Combine(1.0, 2.0, 3.0), u.GetHashCode());
    }

    [Fact]
    public void TestIndex()
    {
        {
            Vec3 v = new(1, 2, 3);
            Assert.Equal(1, v[0]);
            Assert.Equal(2, v[1]);
            Assert.Equal(3, v[2]);
            Assert.ThrowsAny<Exception>(() => v[3]);
        }
        {
            Vec3 v = new(1, 1, 1);
            v[0] = 2;
            v[1] = 3;
            v[2] = 4;
            Assert.Equal(2, v[0]);
            Assert.Equal(3, v[1]);
            Assert.Equal(4, v[2]);
            Assert.ThrowsAny<Exception>(() => v[3] = 5);
        }
    }

    [Fact]
    public void TestLength()
    {
        Vec3 u = new(1, 2, 3);
        Assert.Equal(Sqrt(14), u.Length(), 6);
        Assert.Equal(Sqrt(14), Vec3.Length(in u), 6);
    }

    [Fact]
    public void TestLength2()
    {
        Vec3 u = new(1, 2, 3);
        Assert.Equal(14, u.Length2(), 6);
        Assert.Equal(14, Vec3.Length2(in u), 6);
    }

    [Fact]
    public void TestMultiply()
    {
        {
            Vec3 u = new(2, 3, 4);
            Vec3 v = new(1, 2, 3);
            Vec3 result = Vec3.Multiply(in u, in v);
            Assert.Equal(2, result.X);
            Assert.Equal(6, result.Y);
            Assert.Equal(12, result.Z);
        }
        {
            Vec3 u = new(3, 9, 15);
            Vec3 result = Vec3.Multiply(in u, 3);
            Assert.Equal(9, result.X);
            Assert.Equal(27, result.Y);
            Assert.Equal(45, result.Z);
        }
        {
            Vec3 u = new(4, 6, 15);
            Vec3 v = new(1, 2, 3);
            Vec3 result = new();
            Vec3.Multiply(in u, in v, ref result);
            Assert.Equal(4, result.X);
            Assert.Equal(12, result.Y);
            Assert.Equal(45, result.Z);
        }
        {
            Vec3 u = new(3, 9, 15);
            Vec3.Multiply(in u, 3, ref u);
            Assert.Equal(9, u.X);
            Assert.Equal(27, u.Y);
            Assert.Equal(45, u.Z);
        }
        {
            Vec3 u = new(3, 9, 15);
            u.Multiply(3);
            Assert.Equal(9, u.X);
            Assert.Equal(27, u.Y);
            Assert.Equal(45, u.Z);
        }
        {
            Vec3 u = new(4, 6, 15);
            Vec3 v = new(1, 2, 3);
            u.Multiply(in v);
            Assert.Equal(4, u.X);
            Assert.Equal(12, u.Y);
            Assert.Equal(45, u.Z);
        }
    }

    [Fact]
    public void TestNormalize()
    {
        Vec3 v = new(3, 4, 5);
        Vec3 u = Vec3.Normalize(in v);
        v.Normalize();
        Vec3 expected = new(3.0 / 5.0 / Sqrt(2), 4.0 / 5.0 / Sqrt(2), 1.0 / Sqrt(2));
        Assert.Equal(expected.X, u.X, 6);
        Assert.Equal(expected.Y, u.Y, 6);
        Assert.Equal(expected.Z, u.Z, 6);
        Assert.Equal(expected.X, v.X, 6);
        Assert.Equal(expected.Y, v.Y, 6);
        Assert.Equal(expected.Z, v.Z, 6);
    }

    [Fact]
    public void TestSubtract()
    {
        {
            Vec3 u = new(1, 2, 3);
            Vec3 v = new(1, 2, 3);
            Vec3 result = Vec3.Subtract(in u, in v);
            Assert.Equal(0, result.X);
            Assert.Equal(0, result.Y);
            Assert.Equal(0, result.Z);
        }
        {
            Vec3 u = new(1, 2, 3);
            Vec3 result = Vec3.Subtract(in u, 4);
            Assert.Equal(-3, result.X);
            Assert.Equal(-2, result.Y);
            Assert.Equal(-1, result.Z);
        }
        {
            Vec3 u = new(1, 2, 3);
            Vec3 v = new(1, 2, 3);
            Vec3 result = new();
            Vec3.Subtract(in u, in v, ref result);
            Assert.Equal(0, result.X);
            Assert.Equal(0, result.Y);
            Assert.Equal(0, result.Z);
        }
        {
            Vec3 u = new(1, 2, 3);
            Vec3.Subtract(in u, 4, ref u);
            Assert.Equal(-3, u.X);
            Assert.Equal(-2, u.Y);
            Assert.Equal(-1, u.Z);
        }
        {
            Vec3 u = new(1, 2, 3);
            u.Subtract(4);
            Assert.Equal(-3, u.X);
            Assert.Equal(-2, u.Y);
            Assert.Equal(-1, u.Z);
        }
        {
            Vec3 u = new(1, 2, 3);
            Vec3 v = new(1, 2, 3);
            u.Subtract(in v);
            Assert.Equal(0, u.X);
            Assert.Equal(0, u.Y);
            Assert.Equal(0, u.Z);
        }
    }

    [Fact]
    public void TestUnitX()
    {
        Vec3 v = Vec3.UnitX;
        Assert.Equal(1, v.X);
        Assert.Equal(0, v.Y);
        Assert.Equal(0, v.Z);
    }

    [Fact]
    public void TestUnitY()
    {
        Vec3 v = Vec3.UnitY;
        Assert.Equal(0, v.X);
        Assert.Equal(1, v.Y);
        Assert.Equal(0, v.Z);
    }

    [Fact]
    public void TestUnitZ()
    {
        Vec3 v = Vec3.UnitZ;
        Assert.Equal(0, v.X);
        Assert.Equal(0, v.Y);
        Assert.Equal(1, v.Z);
    }

    [Fact]
    public void TestZero()
    {
        Vec3 v = Vec3.Zero;
        Assert.Equal(0, v.X);
        Assert.Equal(0, v.Y);
        Assert.Equal(0, v.Z);
    }

    [Fact]
    public void TestOperator()
    {
        //minus
        {
            Vec3 u = new(1, 2, 3);
            Vec3 v = -u;
            Assert.Equal(-1, v.X);
            Assert.Equal(-2, v.Y);
            Assert.Equal(-3, v.Z);
        }
        //subtract
        {
            Vec3 u = new(1, 2, 3);
            Vec3 v = new(1, 2, 3);
            Vec3 w = u - v;
            Assert.Equal(0, w.X);
            Assert.Equal(0, w.Y);
            Assert.Equal(0, w.Z);
        }
        {
            Vec3 u = new(1, 2, 3);
            Vec3 v = 4 - u;
            Assert.Equal(3, v.X);
            Assert.Equal(2, v.Y);
            Assert.Equal(1, v.Z);
        }
        {
            Vec3 u = new(1, 2, 3);
            Vec3 v = u - 4;
            Assert.Equal(-3, v.X);
            Assert.Equal(-2, v.Y);
            Assert.Equal(-1, v.Z);
        }
        // !=,==
        {
            Vec3 u = new(1, 2, 3);
            Vec3 v = new(1, 2, 3);
            Vec3 w = new(4, 5, 6);
            Assert.True(u == v);
            Assert.False(u == w);
            Assert.False(u != v);
            Assert.True(u != w);
        }
        // multiply
        {
            Vec3 u = new(1, 2, 3);
            Vec3 v = new(1, 2, 3);
            Vec3 w = u * v;
            Assert.Equal(1, w.X);
            Assert.Equal(4, w.Y);
            Assert.Equal(9, w.Z);
        }
        {
            Vec3 u = new(1, 2, 3);
            Vec3 v = 4 * u;
            Assert.Equal(4, v.X);
            Assert.Equal(8, v.Y);
            Assert.Equal(12, v.Z);
        }
        {
            Vec3 u = new(1, 2, 3);
            Vec3 v = u * 4;
            Assert.Equal(4, v.X);
            Assert.Equal(8, v.Y);
            Assert.Equal(12, v.Z);
        }
        //divide
        {
            Vec3 u = new(1, 2, 3);
            Vec3 v = new(1, 2, 3);
            Vec3 w = u / v;
            Assert.Equal(1, w.X);
            Assert.Equal(1, w.Y);
            Assert.Equal(1, w.Z);
        }
        {
            Vec3 u = new(1, 2, 3);
            Vec3 v = 6 / u;
            Assert.Equal(6, v.X);
            Assert.Equal(3, v.Y);
            Assert.Equal(2, v.Z);
        }
        {
            Vec3 u = new(8, 4, 2);
            Vec3 v = u / 2;
            Assert.Equal(4, v.X);
            Assert.Equal(2, v.Y);
            Assert.Equal(1, v.Z);
        }
        //add
        {
            {
                Vec3 u = new(1, 2, 3);
                Vec3 v = new(1, 2, 3);
                Vec3 w = u + v;
                Assert.Equal(2, w.X);
                Assert.Equal(4, w.Y);
                Assert.Equal(6, w.Z);
            }
            {
                Vec3 u = new(1, 2, 3);
                Vec3 v = 4 + u;
                Assert.Equal(5, v.X);
                Assert.Equal(6, v.Y);
                Assert.Equal(7, v.Z);
            }
            {
                Vec3 u = new(1, 2, 3);
                Vec3 v = u + 4;
                Assert.Equal(5, v.X);
                Assert.Equal(6, v.Y);
                Assert.Equal(7, v.Z);
            }
        }
    }

    [Fact]
    public void TestToString()
    {
        Vec3 u = new(1, 2, 3);
        Assert.Equal("Vec3<1, 2, 3>", u.ToString());
#pragma warning disable CA1305 // 指定 IFormatProvider
        Assert.Equal("Vec3<1, 2, 3>", u.ToString("G"));
#pragma warning restore CA1305 // 指定 IFormatProvider
    }
}
