using System;
using System.Drawing;
using System.Windows;

namespace Newton
{
	public class Shape
	{
		Vector _centerVec;
		protected PointF _center; // { get; set; } // = new PointF(0, 0);
		protected Color _clr;
		public PointF Center
		{
			get { return _center; }
			// TODO: Сделать проверку, выходит ли значение 
			// за пределы экрана. 
			set { _center = value; }
		}
		public Shape(PointF center, Color color) { _center = center; _clr = color; }
		public Shape(Vector centerVec, Color color) { _centerVec = centerVec; _clr = color; }



		// FIXME: Убрать этот метод.
		public virtual void print() { }
	}

	public class Sphere : Shape
	{
		private float _radius;
		public float Radius
		{
			get { return _radius; }
		}

		public Sphere(PointF center, float radius, Color color) : base(center, color) => _radius = radius;

		public override void print()
		{
			Console.WriteLine("R = " + _radius + " " + _center + " ");
		}
	}
}


