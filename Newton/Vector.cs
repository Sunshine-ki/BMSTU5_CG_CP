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
		public double X
		{
			get { return _x; }
			set { _x = value; }
		}
		public double Y
		{
			get { return _y; }
			set { _y = value; }
		}
		public double Z
		{
			get { return _z; }
			set { _z = value; }
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
	}
}