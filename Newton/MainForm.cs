using System;
using System.Timers;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

// TODO: Добавить класс Color

namespace Newton
{
	public class MainForm : Form
	{
		private List<Shape> _scene;
		private Mode _mode; // Off/On.
		private State _state;
		private Bitmap _img; // Содержит растровое изображение.
		private PictureBox _imgBox; // Будет сожержать само изображение.
		private System.Timers.Timer _timer; // Частота кадров.

		// 
		private Light _light;
		//

		public MainForm(List<Shape> scene)
		{
			SettingsWindows();
			InitializeComponent();
			_scene = scene;
			_state = State.Start;
			_mode = Mode.Off;

			//
			_light = new Light(new Vector(2, 1, 0), 0.6);
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

			// Timer
			_timer = new System.Timers.Timer(150);
			_timer.Elapsed += OnTimedEvent;
			// ScreenRefreshTime.Enabled = true;
		}
		#endregion

		private void ButtonOffOnClick(object sender, System.EventArgs e)
		{
			if (_mode == Mode.Off)
			{
				_timer.Start();
				_mode = Mode.On;
			}
			else
			{
				_mode = Mode.Off;
				_timer.Stop();
			}
		}

		private void OnTimedEvent(Object source, ElapsedEventArgs e)
		{
			double angle = 0.06d;
			switch (_state)
			{
				case State.Start:
					_state = State.MovingUpRight;
					break;
				case State.MovingUpRight:
					_scene[_scene.Count - 1].Center.RotatePositive(800, 200, angle);
					if (_scene[_scene.Count - 1].Center.Y < 300)
						_state = State.MovingDownRight;
					break;
				case State.MovingDownRight:
					_scene[_scene.Count - 1].Center.RotatePositive(800, 200, -angle);
					if (_scene[_scene.Count - 1].Center.X < 800)
					{
						// Начальное значение.
						_scene[_scene.Count - 1].Center.X = 800;
						_scene[_scene.Count - 1].Center.Y = 400;
						_state = State.MovingUpLeft;
						break;
					}
					break;
				case State.MovingUpLeft:
					_scene[0].Center.RotateNegative(400, 200, angle);
					if (_scene[0].Center.Y < 300)
						_state = State.MovingDownLeft;
					break;
				case State.MovingDownLeft:
					_scene[0].Center.RotateNegative(400, 200, -angle);
					if (_scene[0].Center.X > 400)
					{
						// Начальное значение.
						_scene[0].Center.X = 400;
						_scene[0].Center.Y = 400;
						_state = State.Start;
					}
					break;
			}
			DrawScene();
		}

		private void DrawScene()
		{
			// // Задний фон - черный.
			// Graphics graphics = Graphics.FromImage(_img);
			// graphics.FillRectangle(new SolidBrush(Color.Black),
			// 	new Rectangle(0, 0, (int)SizeObjects.WidthCanvas,
			//  						(int)SizeObjects.HeightCanvas));
			Color clr;
			for (Int32 i = 0; i < (int)SizeObjects.WidthCanvas; i++)
				for (Int32 j = 0; j < (int)SizeObjects.HeightCanvas; j++)
				{
					clr = TraceRay(new Point(i, j));
					_img.SetPixel(i, j, clr); //Color.FromArgb(255, 255, 255));
				}

			_imgBox.Image = _img;
		}

		private Color TraceRay(Point point)
		{
			Shape ClosestObject = null;
			double MinZ = Double.NegativeInfinity;
			foreach (var elem in _scene)
			{
				if (IsVisible((elem as Sphere).Radius, elem.Center.X - point.X, elem.Center.Y - point.Y))
				{
					if (elem.Center.Z > MinZ)
					{
						MinZ = elem.Center.Z;
						ClosestObject = elem;
					}
				}
			}

			return ClosestObject == null ? Color.Black : test(ClosestObject, new Vector(point.X, point.Y, ClosestObject.Center.Z));
		}
		private bool IsVisible(double R, double dx, double dy)
		{
			if (Math.Pow(dx, 2.0d) + Math.Pow(dy, 2.0d) <= R * R)
				return true;
			return false;
		}
		private Color test(Shape Obj, Vector point)
		{
			Color result = Obj.Clr;

			Vector Normal = point - Obj.Center;
			Normal = Normal / Normal.Length;

			// Color Intensity = ComputeLighting(point, Normal) * Obj.Clr;

			return result;
		}

		// private double ComputeLighting(Vector point, Vector Normal)
		// {
		// 	return 0d;
		// }
	}
}