using System;
using System.Timers;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Newton
{
	public class MainForm : Form
	{
		private List<Shape> _scene;
		private Bitmap _img; // Содержит растровое изображение.
		private PictureBox _imgBox; // Будет сожержать само изображение.

		private Vector _cameraPosition;

		// 
		private List<Light> _lights = new List<Light>();
		//

		public MainForm(List<Shape> scene)
		{
			SettingsWindows();
			InitializeComponent();
			_scene = scene;

			_cameraPosition = new Vector(0, 0, 0);

			//
			_lights.Add(new Light(null, 0.2, LightType.Ambient));
			_lights.Add(new Light(new Vector(2, 1, 0), 0.6, LightType.Point));
			_lights.Add(new Light(new Vector(1, 4, 4), 0.2, LightType.Directional));
			//
		}

		#region SETTINGS
		private void SettingsWindows()
		{
			this.Text = "Newton\'s cradle";
			this.Width = (int)SizeObjects.WidthWindows;
			this.Height = (int)SizeObjects.HeightWindows;
			// Стиль рамки: 
			// Sizable - изменяем размер (по умолчанию).
			// None - без рамки.
			// Fixed3D - Фиксированная трехмерная граница.
			this.FormBorderStyle = FormBorderStyle.Sizable;
			// Убираем возможность развернуть окно во весь экран и сложить.
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			// Устанавливаем начальное положение окна в центре.
			this.StartPosition = FormStartPosition.CenterScreen;
		}
		private void InitializeComponent()
		{
			// Button Off/On.
			Button buttonOffOn = new Button();
			buttonOffOn.Text = "off/on";
			buttonOffOn.Location = new Point(550, 800);
			this.AcceptButton = buttonOffOn;
			buttonOffOn.Click += new EventHandler(ButtonOffOnClick);
			this.Controls.Add(buttonOffOn);

			// Image.
			_imgBox = new PictureBox();
			_imgBox.Size = new Size((int)SizeObjects.WidthCanvas, (int)SizeObjects.HeightCanvas);
			this.Controls.Add(_imgBox);
			_img = new Bitmap((int)SizeObjects.WidthCanvas, (int)SizeObjects.HeightCanvas);
			// _imgBox.Image = _img;
		}
		#endregion

		private void ButtonOffOnClick(object sender, System.EventArgs e)
		{
			DrawScene();

		}

		private void CanvasToViewport(ref Vector point)
		{
			point.X = point.X * 2f / (float)SizeObjects.WidthCanvas;
			// point.X = point.X / (float)SizeObjects.WidthCanvas;
			point.Y = point.Y / (float)SizeObjects.HeightCanvas;
		}

		private void PutPixel(float i, float j, Color color)
		{
			float x = (float)SizeObjects.WidthCanvas / 2 + i;
			float y = (float)SizeObjects.HeightCanvas / 2 - j - 1;
			if (x < 0 || y < 0 || x >= (float)SizeObjects.WidthCanvas || y >= (float)SizeObjects.HeightCanvas)
				return;
			_img.SetPixel((int)x, (int)y, color);
		}

		private void DrawScene()
		{
			for (float i = -(float)SizeObjects.WidthCanvas / 2; i < (float)SizeObjects.WidthCanvas / 2; i++)
				for (float j = -(float)SizeObjects.HeightCanvas / 2; j < (float)SizeObjects.HeightCanvas / 2; j++)
				{

					// Vector direction = new Vector(i / (float)SizeObjects.WidthCanvas, j / (float)SizeObjects.HeightCanvas, 1); // TODO: Можно сделать так.
					Vector direction = new Vector(i, j, 1);
					CanvasToViewport(ref direction);
					Colors c = TraceRay(_cameraPosition, direction, 1, Double.PositiveInfinity);
					PutPixel(i, j, Color.FromArgb(c.R, c.G, c.B));
				}
			_imgBox.Image = _img;
		}

		private Vector ReflectRay(Vector v1, Vector v2)
		{
			return v2 * (2 * v1.DotProduct(v2)) - v1;
		}

		private Shape ClosestIntersection(Vector origin, Vector direction, double min_t, double max_t, ref double closest_t, bool print_flag = false)
		{
			closest_t = Double.PositiveInfinity;
			Shape ClosestObject = null;
			Vector ts = new Vector(0, 0, 0);

			foreach (var elem in _scene)
			{
				IsVisible(origin, ref ts, direction, elem as Sphere);
				if (ts.X < closest_t && min_t < ts.X && ts.X < max_t)
				{
					ClosestObject = elem;
					closest_t = ts.X;
				}

				if (ts.Y < closest_t && min_t < ts.Y && ts.Y < max_t)
				{
					ClosestObject = elem;
					closest_t = ts.Y;
				}
			}

			return ClosestObject;
		}

		private double ComputeLighting(Vector point, Vector normal, Vector view, double specular = 1000)
		{
			double intensity = 0;
			double length_n = normal.Length;
			double length_v = view.Length;

			foreach (var light in _lights)
			{
				if (light.Type == LightType.Ambient)
				{
					intensity += light.Intensity;
				}
				else
				{
					Vector vec_l;
					double t_max;

					if (light.Type == LightType.Point)
					{
						vec_l = light.Position - point;
						t_max = 1.0d;
					}
					else // LightType.Directional
					{
						vec_l = light.Position;
						t_max = Double.PositiveInfinity;
					}
					// // Тени.
					double temp = Double.PositiveInfinity;
					Shape blocker;

					blocker = ClosestIntersection(point, vec_l, 0.001d, t_max, ref temp);

					if (blocker != null)
						continue;

					// Интенсивность.
					double n_dot_l = normal.DotProduct(vec_l);
					if (n_dot_l > 0)
					{
						// intensity += light.Intensity * n_dot_l / (length_n * vec_l.Length) / (light.Position - point).Length;
						intensity += light.Intensity * n_dot_l / (length_n * vec_l.Length);
					}

					// Отражение от гладкой поверхности.
					Vector vec_r = normal * (2d * normal.DotProduct(vec_l)) - vec_l;
					double r_dot_v = vec_r.DotProduct(view);
					if (r_dot_v > 0)
					{
						intensity += light.Intensity * Math.Pow(r_dot_v / (vec_r.Length * length_v), specular);
					}
				}
			}

			return intensity;
		}

		private Colors TraceRay(Vector origin, Vector direction, double min_t, double max_t, int depth = 3)
		{
			double closest_t = Double.PositiveInfinity, temp = Double.PositiveInfinity;

			Shape ClosestObject = ClosestIntersection(origin, direction, min_t, max_t, ref closest_t);

			if (ClosestObject == null)
				return new Colors(0, 0, 0);

			// Light.
			Vector point = origin + direction * closest_t;
			Vector normal = point - ClosestObject.Center;
			normal = normal * (1.0d / normal.Length);

			Vector view = direction * -1.0d;

			double lighting = ComputeLighting(point, normal, view);

			Colors result = ClosestObject.Clr * lighting;

			// Отражение.
			if (depth <= 0)
			{
				return result;
			}

			Vector reflected_ray = ReflectRay(view, normal);
			Colors reflected_color = TraceRay(point, reflected_ray, 0.001d, Double.PositiveInfinity, depth - 1);

			double reflective = 0.4d;

			return result * (1 - reflective) + reflected_color * reflective;
		}

		private void IsVisible(Vector origin, ref Vector ts, Vector direction, Sphere sphere)
		{
			Vector oc = origin - sphere.Center;

			double k1 = direction.DotProduct(direction);
			double k2 = 2 * oc.DotProduct(direction);
			double k3 = oc.DotProduct(oc) - sphere.Radius * sphere.Radius;

			double discriminant = k2 * k2 - 4 * k1 * k3;

			if (discriminant < 0)
			{
				ts.X = Double.PositiveInfinity;
				ts.Y = Double.PositiveInfinity;
				return;
			}

			ts.X = (-k2 + Math.Sqrt(discriminant)) / (2 * k1);
			ts.Y = (-k2 - Math.Sqrt(discriminant)) / (2 * k1);
		}

	}
}