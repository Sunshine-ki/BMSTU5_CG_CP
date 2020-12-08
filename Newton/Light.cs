using System;
using System.Drawing;
using System.Windows;

namespace Newton
{
	public class Light
	{
		private Vector _position;
		private double _intensity;
		private LightType _type;
		public LightType Type { get => _type; }
		public Vector Position { get => _position; }
		public double Intensity { get => _intensity; }
		public Light(Vector position, double intensity, LightType type) { _position = position; _intensity = intensity; _type = type; }

		public override string ToString() => "Position = " + _position + " Intensity = " + _intensity + "\n\n";
	}
}