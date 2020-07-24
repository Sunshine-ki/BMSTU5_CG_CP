using System;
using System.Timers;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Newton
{
	public class Form1 : Form
	{
		List<Shape> _scene;
		System.Timers.Timer aTimer; // frame frequency
		private PictureBox _imgBox;
		Mode _mode;
		State _state;
		Bitmap _img; // Содержит растровое изображение.

		public Form1(List<Shape> scene)
		{
			SettingsWindows();
			InitializeComponent();
			_imgBox = new PictureBox();
			_imgBox.Size = new Size((int)SizeObjects.WidthCanvas, (int)SizeObjects.HeightCanvas);
			this.Controls.Add(_imgBox);
			_img = new Bitmap((int)SizeObjects.WidthCanvas, (int)SizeObjects.HeightCanvas);
			// _imgBox.Image = _img;
			_mode = Mode.Off;
			_scene = scene;
		}

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
			Button buttonOffOn = new Button();
			buttonOffOn.Text = "off/on";
			buttonOffOn.Location = new Point(550, 800);
			this.AcceptButton = buttonOffOn;
			buttonOffOn.Click += new EventHandler(ButtonOffOnClick);
			this.Controls.Add(buttonOffOn);
		}

		private void ButtonOffOnClick(object sender, System.EventArgs e)
		{
			// Если выключен - включить.
			if (_mode == Mode.Off)
			{
				_mode = Mode.On;
				aTimer = new System.Timers.Timer(2000);
				aTimer.Elapsed += OnTimedEvent;
				aTimer.AutoReset = true;
				aTimer.Enabled = true;
			}
			else // Если включен - выключить.
			{
				_mode = Mode.Off;
				aTimer.Stop();
			}

			Console.WriteLine("Yes!");
		}

		private void OnTimedEvent(Object source, ElapsedEventArgs e)
		{
			float x, y, xTrans, yTrans;
			float xCenter = 800.0f;
			float yCenter = 200.0f;
			double angle;
			switch (_state)
			{
				case State.Start:
					// Console.WriteLine("Начинаем движение.");
					// Преобразования (Можно тут с самого начала начинать движение).

					// 
					_state = State.MovingUpRight;
					break;
				case State.MovingUpRight:
					// Console.WriteLine("Двигаемся правым шаром вверх.");
					angle = 0.03d;
					x = _scene[_scene.Count - 1].Center.X;
					y = _scene[_scene.Count - 1].Center.Y;
					xTrans = (float)(xCenter + (x - xCenter) * Math.Cos(angle) + (y - yCenter) * Math.Sin(angle));
					yTrans = (float)(yCenter + (y - yCenter) * Math.Cos(angle) - (x - xCenter) * Math.Sin(angle));
					_scene[_scene.Count - 1].Center = new PointF(xTrans, yTrans);
					if (yTrans < 250)
						_state = State.MovingDownRight;
					break;
				case State.MovingDownRight:
					// Console.WriteLine("Двигаемся правым шаром вниз.");

					angle = -0.03d;
					x = _scene[_scene.Count - 1].Center.X;
					y = _scene[_scene.Count - 1].Center.Y;
					// Console.WriteLine(x + " " + (x * Math.Cos(angle)));
					xTrans = (float)(xCenter + (x - xCenter) * Math.Cos(angle) + (y - yCenter) * Math.Sin(angle));
					yTrans = (float)(yCenter + (y - yCenter) * Math.Cos(angle) - (x - xCenter) * Math.Sin(angle));

					if (xTrans < 800)
					{
						// Начальное значение.
						_scene[_scene.Count - 1].Center = new PointF(800, 400);
						_state = State.MovingUpLeft;
						break;
					}
					_scene[_scene.Count - 1].Center = new PointF(xTrans, yTrans);
					// Console.WriteLine(_scene[_scene.Count - 1].Center);
					break;
				case State.MovingUpLeft:
					// Console.WriteLine("Двигаемся левым шаром вверх.");
					xCenter = 400.0f;
					yCenter = 200.0f;

					angle = 0.03d;
					x = _scene[0].Center.X;
					y = _scene[0].Center.Y;
					xTrans = (float)(xCenter + (x - xCenter) * Math.Cos(angle) - (y - yCenter) * Math.Sin(angle));
					yTrans = (float)(yCenter + (y - yCenter) * Math.Cos(angle) + (x - xCenter) * Math.Sin(angle));
					_scene[0].Center = new PointF(xTrans, yTrans);

					if (yTrans < 250)
						_state = State.MovingDownLeft;
					break;
				case State.MovingDownLeft:
					// Console.WriteLine("Двигаемся левым шаром вниз.");

					xCenter = 400.0f;
					yCenter = 200.0f;
					angle = -0.03d;
					x = _scene[0].Center.X;
					y = _scene[0].Center.Y;
					xTrans = (float)(xCenter + (x - xCenter) * Math.Cos(angle) - (y - yCenter) * Math.Sin(angle));
					yTrans = (float)(yCenter + (y - yCenter) * Math.Cos(angle) + (x - xCenter) * Math.Sin(angle));

					if (xTrans > 400)
					{
						// Начальное значение.
						_scene[0].Center = new PointF(400, 400);
						_state = State.Start;
						break;
					}
					_scene[0].Center = new PointF(xTrans, yTrans);

					break;
				default:
					return;
			}

			_imgBox.Image = RayTracing();
		}

		private Bitmap RayTracing()
		{
			// _imgBox.Image.Dispose();
			Graphics graphics = Graphics.FromImage(_img);
			graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, (int)SizeObjects.WidthCanvas, (int)SizeObjects.HeightCanvas));

			float radius;
			double distance;
			for (int i = 0; i < (int)SizeObjects.WidthCanvas; i++)
			{
				for (int j = 0; j < (int)SizeObjects.HeightCanvas; j++)
				{
					foreach (var elem in _scene)
					{
						// TODO: Вынести это в функцию, которая просто будет возвращать
						// true or false в зависимости от того, находится ли пиксель
						// Внутри области или нет. 
						// Также избавиться от точечного синтаксиса.
						radius = (elem as Sphere).Radius;
						distance = Math.Sqrt(Math.Pow(elem.Center.X - i, 2.0d) + Math.Pow(elem.Center.Y - j, 2.0d));
						if (distance <= radius)
							_img.SetPixel(i, j, Color.Red); //Color.FromArgb(255, 255, 255));


					}
					// Color curPixColor = img.GetPixel(i, j);
				}
			}

			// bool IsVisible()
			// {

			// }

			// FIXME: Потом graphics не будет.
			// Graphics graphics = Graphics.FromImage(_img);
			// RectangleF rectangle;
			// float radius;


			// graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, (int)SizeObjects.WidthCanvas, (int)SizeObjects.HeightCanvas));

			// foreach (var elem in _scene)
			// {
			// 	radius = (elem as Sphere).Radius;
			// 	rectangle = new RectangleF(new PointF(elem.Center.X - radius, elem.Center.Y - radius), new SizeF(2 * radius, 2 * radius));
			// 	// Объект PointF, представляющий левый верхний угол прямоугольной области.
			// 	// Объект SizeF, представляющий ширину и высоту прямоугольной области.
			// 	graphics.FillEllipse(new SolidBrush(Color.Red), rectangle);
			// }

			// TODO: Нужно ли возвращать ? Привязать вначале и все...

			return _img;

		}
	}
}