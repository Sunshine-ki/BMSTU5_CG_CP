using System;
using System.Drawing;
using System.Windows;

namespace Newton
{
	public class Vector
	{
		private double _x, _y, _z;
		public Vector() { _x = 0; _y = 0; _z = 0; }
		public Vector(double x, double y, double z) { _x = x; _y = y; _z = z; }
		public double X { get => _x; set => _x = value; }
		public double Y { get => _y; set => _y = value; }
		public double Z { get => _z; set => _z = value; }
		public double Length => Math.Sqrt(DotProduct(this));

		public static Vector operator +(Vector a, Colors b) => new Vector(a.X + b.R, a.Y + b.G, a.Z + b.B);

		public static Vector operator +(Vector a, Vector b) => new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		public static Vector operator +(Vector a, double num) => new Vector(a.X + num, a.Y + num, a.Z + num);

		public static Vector operator -(Vector a, Vector b) => new Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		public static Vector operator -(Vector a, double num) => new Vector(a.X - num, a.Y - num, a.Z - num);

		public static Vector operator *(Vector a, Vector b) => new Vector(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
		public static Vector operator *(Vector a, double num) => new Vector(a.X * num, a.Y * num, a.Z * num);

		public static Vector operator /(Vector a, Vector b) => new Vector(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
		public static Vector operator /(Vector a, double num) => new Vector(a.X / num, a.Y / num, a.Z / num);


		// public static double DotProduct(Vector vectorFirst, Vector vectorSecond)
		// {
		// 	return vectorFirst.X * vectorSecond.X + vectorFirst.Y * vectorSecond.Y + vectorFirst.Z * vectorSecond.Z;
		// }

		public double DotProduct(Vector vector) => _x * vector.X + _y * vector.Y + _z * vector.Z;

		public Vector CrossProduct(Vector v) // Векторное произведение.
		{
			return new Vector(_y * v.Z - _z * v.Y,
							  _z * v.X - _x * v.Z, // -(_x * v.Z - _z * v.X),
							  _x * v.Y - _y * v.X);
		}

		public Vector Sign()
		{
			return new Vector(Math.Sign(_x), Math.Sign(_y), Math.Sign(_z));
		}

		// Проверка на сонаправленность 
		public bool CoDirectional(Vector v) 
		{
			Vector sign1 = this.Sign();
			Vector sign2 = v.Sign();

			Vector res = sign1 * sign2;

			if (res.X >= 0 & res.Y >= 0 & res.Z >= 0)
				return true;

			return false;
		}

		// Получить единичный вектор.
		public Vector Normalize()
		{
			return this / Length;
		}

		public void Reverse()
		{
			// (0 == 0) = 1
			// (1 == 0) = 0
			_x = Convert.ToDouble(_x == 0);
			_y = Convert.ToDouble(_y == 0);
			_z = Convert.ToDouble(_z == 0);
		}

		// Поворот по часовой стрелке.
		public void RotatePositive(double xCenter, double yCenter, double angle)
		{
			double x = _x;
			_x = (double)(xCenter + (_x - xCenter) * Math.Cos(angle) + (_y - yCenter) * Math.Sin(angle));
			_y = (double)(yCenter + (_y - yCenter) * Math.Cos(angle) - (x - xCenter) * Math.Sin(angle));
		}
		// Поворот против часовой стрелке.
		public void RotateNegative(double xCenter, double yCenter, double angle)
		{
			double x = _x;
			_x = (double)(xCenter + (_x - xCenter) * Math.Cos(angle) - (_y - yCenter) * Math.Sin(angle));
			_y = (double)(yCenter + (_y - yCenter) * Math.Cos(angle) + (x - xCenter) * Math.Sin(angle));
		}

		public override string ToString() => "X = " + _x + " Y = " + _y + " Z = " + _z;
	}
}