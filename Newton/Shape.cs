using System;
using System.Drawing;
using System.Windows;

namespace Newton
{
	public class Shape
	{
		protected Vector _center;
		protected Color _clr;
		public Vector Center
		{
			get { return _center; }
			set { _center = value; }
		}
		public Color Clr
		{
			get { return _clr; }
		}
		public Shape() { _clr = Color.Black; }
		public Shape(Vector center, Color clr) { _center = center; _clr = clr; }

		// FIXME: Убрать потом print.
		public virtual void print()
		{
			Console.WriteLine("Center = " + _center.X + " " + _center.Y + " " + _center.Z + " " + "\nColor = " + _clr);
		}

	}

	public class Sphere : Shape
	{
		private double _radius;
		public double Radius
		{
			get { return _radius; }
		}
		public Sphere(Vector center, double radius, Color color) : base(center, color) => _radius = radius;

		// FIXME: Убрать потом print.
		public override void print()
		{
			base.print();
			Console.WriteLine("Radius = " + _radius + "\n\n");
		}
	}
}


