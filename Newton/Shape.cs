using System;
using System.Drawing;
using System.Windows;

namespace Newton
{
	public class Shape
	{
		protected Vector _center;
		protected Colors _clr;
		protected double _reflective;
		protected TypeShape _type = TypeShape.Shape;

		public double Reflective { get => _reflective; }
		public Vector Center { get => _center; set => _center = value; }
		public Colors Clr { get => _clr; }
		public TypeShape Type { get => _type; }


		public Shape() { _clr = new Colors(); }
		public Shape(Vector Center, Colors clr, double reflective) { _center = Center; _clr = clr; _reflective = reflective; }

		public override string ToString() => $"Center = {_center}\nColor = {_clr}\nReflective={_reflective}";
	}
	
	public class Sphere : Shape
	{
		private double _radius;

		public double Radius { get => _radius; }
		
		public Sphere(Vector Center, double Radius, Colors clr, double reflective, TypeShape type) : base(Center, clr, reflective) 
		{
			_radius = Radius;
			_type = type;
		}

		public override string ToString() => base.ToString() + $"Radius = {_radius}\n\n";
	}

	public class Cylinder : Shape
	{
		private double _radius;
		private double _max, _min;
		private Axis _axis;

		public double Radius { get => _radius; }
		public double Min { get => _max; }
		public double Max { get => _min; }
		public Axis GetAxis { get => _axis; }

		public Cylinder(Vector Center, double Radius, Colors clr, double reflective, double max, double min, Axis axis, TypeShape type) : base(Center, clr, reflective) 
		{
			_radius = Radius;
			_min = min;
			_max = max;
			_axis = axis;
			_type = type;
		}

		public Vector GetAxesValue()
		{
			Vector axesValue = new Vector(1, 1, 1);

			if (_axis == Axis.X)
				axesValue.X = 0;
			else if (_axis == Axis.Y)
				axesValue.Y = 0;
			else 
				axesValue.Z = 0;
				
			return axesValue;
		} 					


		public override string ToString() => base.ToString() + $"Radius = {_radius}\n\nMin = {_min}\nMax = {_max}\nAxis = {_axis}";
	}

	public class Triangle : Shape
	{
		private Vector _p1, _p2, _p3; // Координаты вершин треугольника.
		private Vector _l1, _l2, _l3; // Ребра.

		public Vector P1 { get => _p1; set => _p1 = value; }
		public Vector P2 { get => _p2; set => _p2 = value; }
		public Vector P3 { get => _p3; set => _p3 = value; }

		public Vector L1 { get => _l1; }
		public Vector L2 { get => _l2; }
		public Vector L3 { get => _l3; }

		public Vector GetNormal()
		{
			// Векторное произведение
			// Даст нам вектор, который будет перпендикулярен 
			// Плоскости в кторой лежит треугольник
			Vector res = _l1.CrossProduct(_l2); 
			// Нормируем его.
			return res.Normalize();
		}

		public Triangle(Vector p1, Vector p2, Vector p3, Vector Center, Colors clr, double reflective) : base(Center, clr, reflective) 
		{ 
			_p1 = p1; _p2 = p2; _p3 = p3;

			_l1 = _p2 - _p1;
			_l2 = _p3 - _p2;  
			_l3 = _p1 - _p3;

			_type = TypeShape.Triangle;
		}

		public override string ToString() => base.ToString() + "P1: " + _p1 + " P2: " + _p2  + " P3: " + _p3 + "\n\n";
	}
}


