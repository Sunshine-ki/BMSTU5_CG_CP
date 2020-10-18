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
			_lights.Add(new Light(new Vector(0, 0, 0), 0.6, LightType.Point));
			_lights.Add(new Light(new Vector(0, 0, 0), 0.2, LightType.Ambient));
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
			point.Y = point.Y / (float)SizeObjects.HeightCanvas;
		}

		private void PutPixel(float i, float j, Color color)
		{
			float x = (float)SizeObjects.WidthCanvas / 2 + i;
			float y = (float)SizeObjects.HeightCanvas / 2 - j - 1;
			if (x < 0 || y < 0) //|| x >= (float)SizeObjects.WidthCanvas || y >= (float)SizeObjects.HeightCanvas)
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
					Color c = TraceRay(direction);
					PutPixel(i, j, c);
				}
			_imgBox.Image = _img;
		}

		// origin, direction, min_t, max_t
		private Shape ClosestIntersection(ref double closest_t, Vector origin, Vector direction, double min_t, double max_t)
		{
			Shape ClosestObject = null;
			Vector ts = new Vector(0, 0, 0);


			foreach (var elem in _scene)
			{
				IsVisible(ref ts, direction, elem as Sphere);

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

		private Color TraceRay(Vector direction)
		{
			double min_t = 1, max_t = Double.PositiveInfinity;
			double closest_t = Double.PositiveInfinity, temp = Double.PositiveInfinity;
			Shape ClosestObject = ClosestIntersection(ref closest_t, _cameraPosition, direction, min_t, max_t);

			if (ClosestObject == null)
				return Color.Black;
			// return Color.White;

			Colors result = ClosestObject.Clr;

			// Light.
			Vector point = _cameraPosition + direction * closest_t;
			Vector normal = point - ClosestObject.Center;
			normal = normal * (1.0f / normal.Length);

			double intensity = 0;
			double length_n = normal.Length;

			Vector view = direction * -1f;
			double length_v = view.Length;

			double specular = 1000; // Больше/меньше.

			foreach (var elem in _lights)
			{
				if (elem.Type == LightType.Ambient)
				{
					intensity += elem.Intensity;
				}
				else
				{

					Vector vec_l = elem.Position - point;

					// Тени.
					// Shape blocker = ClosestIntersection(ref temp, point, vec_l, 0.001f, 1f);
					// if (blocker != null)
					// {
					// 	Console.WriteLine("Block");
					// 	continue;
					// }

					// Интенсивность.
					double n_dot_l = normal.DotProduct(vec_l);
					if (n_dot_l > 0)
					{
						intensity += elem.Intensity * n_dot_l / (length_n * vec_l.Length);
					}

					// Отражение от гладкой поверхности.
					Vector vec_r = normal * (2f * normal.DotProduct(vec_l)) - vec_l;
					double r_dot_v = vec_r.DotProduct(view);
					if (r_dot_v > 0)
					{
						intensity += elem.Intensity * Math.Pow(r_dot_v / (vec_r.Length * length_v), specular);
					}
				}
			}

			result = result * intensity;

			return Color.FromArgb(result.R, result.G, result.B);
		}

		private void IsVisible(ref Vector ts, Vector direction, Sphere sphere)
		{
			Vector oc = _cameraPosition - sphere.Center;

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