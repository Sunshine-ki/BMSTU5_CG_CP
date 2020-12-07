using System;
using System.Timers;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using System.Threading;

namespace Newton
{
	// Вспомогательный класс для распараллеливания.
	public class Limit
	{
		public Int32 begin;
		public Int32 end;
		public Limit(Int32 begin, Int32 end) { this.begin = begin; this.end = end;}
	}

	public class MainForm : Form
	{
		private List<Shape> _scene;
		private Bitmap _img; // Содержит растровое изображение.
		private PictureBox _imgBox; // Будет сожержать само изображение.
		
		private int _currImgIndex = 0;
		private	List<Bitmap> _arrayBitmap = new List<Bitmap>();
		 

		private Mode _mode; // Off/On.
		private State _state;

		private Vector _cameraPosition;
		private List<Light> _lights = new List<Light>();

		private System.Timers.Timer _timer; // Частота кадров.

		public MainForm(List<Shape> scene)
		{
			SettingsWindows();
			InitializeComponent();
			_scene = scene;
			
			_state = State.Start;
			_mode = Mode.Off;

			_cameraPosition = new Vector(0, 0, 0);

			_lights.Add(new Light(null, 0.2, LightType.Ambient));
			_lights.Add(new Light(new Vector(2, 1, 0), 0.6, LightType.Point));
			_lights.Add(new Light(new Vector(1, 4, 4), 0.2, LightType.Directional));

			createArrayImgBox();
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

		private void createArrayImgBox()
		{
			double angle = 0.04d;
			double h = 0.23;
			DrawScene();
			
			// Правый шар поднимается.
			while (_scene[2].Center.Y < h) 
			{
				Console.WriteLine(_scene[2].Center.Y);
				_scene[2].Center.RotateNegative(1.6, 5, angle);	
				DrawScene();
			}
			// Правый шар опускается.
			while (_scene[2].Center.Y - 0.2 > 0.001) 
			{
				Console.WriteLine(_scene[2].Center.Y);
				_scene[2].Center.RotatePositive(1.6, 5, angle);	
				DrawScene();
			}
			// Левый шар поднимается. 
			while (_scene[0].Center.Y < h) 
			{
				Console.WriteLine(_scene[0].Center.Y);
				_scene[0].Center.RotatePositive(-1.6, 5, angle);	
				DrawScene();
			}
			// Левый шар опускается. 
			while (_scene[0].Center.Y - 0.2 > 0.001) 
			{
				Console.WriteLine(_scene[0].Center.Y);
				_scene[0].Center.RotateNegative(-1.6, 5, angle);	
				DrawScene();
			}
		}

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
			if (_currImgIndex >= _arrayBitmap.Count)
				_currImgIndex = 0;

			Console.WriteLine(_currImgIndex + "  " + _arrayBitmap.Count);
			_imgBox.Image = _arrayBitmap[_currImgIndex];

			_currImgIndex++;
		}

		private void CanvasToViewport(ref Vector point)
		{
			point.X = point.X * 2f / (float)SizeObjects.WidthCanvas;
			// point.X = point.X / (float)SizeObjects.WidthCanvas;
			point.Y = point.Y / (float)SizeObjects.HeightCanvas;
		}

		private void PutPixel(float i, float j, Color color, Bitmap img)
		{
			float x = (float)SizeObjects.WidthCanvas / 2 + i;
			float y = (float)SizeObjects.HeightCanvas / 2 - j - 1;
			if (x < 0 || y < 0 || x >= (float)SizeObjects.WidthCanvas || y >= (float)SizeObjects.HeightCanvas)
				return;
			img.SetPixel((int)x, (int)y, color);
		}

		public void FuncVertically(object obj)
		{
			Limit limit = (Limit)obj;

			for (float i = -(float)SizeObjects.WidthCanvas / 2; i < (float)SizeObjects.WidthCanvas / 2; i++)
				for (Int32 j = limit.begin; j < limit.end ; j++)
				// for (float j = -(float)SizeObjects.HeightCanvas / 2; j < (float)SizeObjects.HeightCanvas / 2; j++)
				{
					Vector direction = new Vector(i, j, 1);
					CanvasToViewport(ref direction);
					Colors c = TraceRay(_cameraPosition, direction, 1, Double.PositiveInfinity);
					// PutPixel(i, j, Color.FromArgb(c.R, c.G, c.B), img); // TODO: img добавить в limit.
				}
		}

		private void DrawScene(int step = 150) // 8 потоков.
		{
			// List<Thread> listThread = new List<Thread>();

			// for (int i =  -(int)SizeObjects.HeightCanvas / 2; i < (int)SizeObjects.WidthCanvas / 2; i += step)
			// {
			// 	listThread.Add(new Thread(new ParameterizedThreadStart(FuncVertically)));
			// 	listThread[listThread.Count - 1].Start(new Limit(i, i + step));
			// }

			// // Join — Это метод синхронизации, который блокирует вызывающий поток (то есть поток, который вызывает метод).
			// // Используйте этот метод, чтобы убедиться, что поток был завершен.
			// // То есть мы не пойдем далее по коду, пока что не выполнятся потоки, вызванные ранее 
			// // (то есть те потоки, которые мы джоиним.).
			// foreach (var elem in listThread)
			// {
			// 	elem.Join();
			// }

			Bitmap img = new Bitmap((int)SizeObjects.WidthCanvas, (int)SizeObjects.HeightCanvas);


			for (float i = -(float)SizeObjects.WidthCanvas / 2; i < (float)SizeObjects.WidthCanvas / 2; i++)
				for (float j = -(float)SizeObjects.HeightCanvas / 2; j < (float)SizeObjects.HeightCanvas / 2; j++)
				{

					Vector direction = new Vector(i, j, 1);
					CanvasToViewport(ref direction);
					Colors c = TraceRay(_cameraPosition, direction, 1, Double.PositiveInfinity);
					PutPixel(i, j, Color.FromArgb(c.R, c.G, c.B), img);
				}

			_arrayBitmap.Add(img);
			// _imgBox.Image = _img;
		}

		private Vector ReflectRay(Vector v1, Vector n)
		{
			// Возвращает отраженный к v1 луч, относительно нормали n.
			return n * (2 * v1.DotProduct(n)) - v1;
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

					blocker = FindNearestObject(point, vec_l, 0.001d, t_max, ref temp);
					// Если есть объект, который загораживает освещение, то пропускам вычисление освещения от этого источника.
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
			double closest_t = Double.PositiveInfinity;
			// Находим ближайший объект, который пересекает луч.
			Shape ClosestObject = FindNearestObject(origin, direction, min_t, max_t, ref closest_t);
			// Если такого объекта нет, возвращаем цвет фона.
			if (ClosestObject == null)
				return new Colors(0, 0, 0);

			// Light.
			// Вычисляем точку пересечения, по найденному параметру t.
			Vector point = origin + direction * closest_t;

			// Вычисляем нормаль.
			Vector tmp = null;

			if (ClosestObject.Type == TypeShape.Sphere)
			{
				tmp = ClosestObject.Center;
			}
			else if (ClosestObject.Type == TypeShape.Cylinder)
			{
				Cylinder cylinder = ClosestObject as Cylinder;

				Vector axesValue = cylinder.GetAxesValue();    // (0,1,1)
				Vector axesValueRev = cylinder.GetAxesValue(); // (1,0,0) 		
				axesValueRev.Reverse();

				axesValue = axesValue * cylinder.Center; // (0,Center.Y, Center.Z)
				axesValueRev = axesValueRev * point; // (Point.X,0,0)
				tmp = axesValue + axesValueRev; // (Point.X, Center.Y, Center.Z)

				// // Смещаем одну координату центра до точки пересечения.
				// switch (cylinder.GetAxis)
				// {
				// 	case Axis.X:
				// 		tmp = new Vector(point.X, cylinder.Center.Y, cylinder.Center.Z);
				// 		normal = point - tmp;
				// 		break;

				// 	case Axis.Y:
				// 		tmp = new Vector(cylinder.Center.X, point.Y, cylinder.Center.Z);
				// 		normal = point - tmp;
				// 		break;
					
				// 	case Axis.Z:
				// 		tmp = new Vector(cylinder.Center.X, cylinder.Center.Y, point.Z);
				// 		normal = point - tmp;
				// 		break;
				// }
			}

			Vector normal = point - tmp;
			normal = normal * (1.0d / normal.Length);
			Vector view = direction * -1.0d;
			// Вычисляем освещение.
			double lighting = ComputeLighting(point, normal, view);

			Colors result = ClosestObject.Clr * lighting;

			// Отражение.
			if (depth <= 0)
				return result;

			Vector reflected_ray = ReflectRay(view, normal);
			Colors reflected_color = TraceRay(point, reflected_ray, 0.001d, Double.PositiveInfinity, depth - 1);

			// Насколько он отражающий.
			double reflective = ClosestObject.Reflective;

			return result * (1 - reflective) + reflected_color * reflective;
		}

		private Shape FindNearestObject(Vector origin, Vector direction, double min_t, double max_t, ref double closest_t, bool print_flag = false)
		{
			// Возвращает ближайший объект, который пересекает луч.
			closest_t = Double.PositiveInfinity;
			// tmp - Содержит найденные пересечения (параметры t).
			// В случает сферы и цилиндра их м.б. 0, 1 или 2.
			Vector tmp = new Vector(0, 0, 0);
			// Ближайший объект,
			Shape closestObject = null;
			// Проходимся по всем элементам сцены
			foreach (var elem in _scene)
			{
				// И ищем пересечение.
				// Далее определяем наименьшее t.
				IsVisible(origin, ref tmp, direction, elem);
				if (tmp.X < closest_t && min_t < tmp.X && tmp.X < max_t)
				{
					closestObject = elem;
					closest_t = tmp.X;
				}
				if (tmp.Y < closest_t && min_t < tmp.Y && tmp.Y < max_t)
				{
					closestObject = elem;
					closest_t = tmp.Y;
				}
			}

			return closestObject;
		}

		private void IsVisible(Vector origin, ref Vector tmp, Vector direction, Shape shape)
		{
			// Функция, которая записывает в tmp
			// Найденные параметры t.
			// oc - Вектор из точки просмотра в центр объекта.
			Vector oc = origin - shape.Center;
			// Коэффициенты для решения квадратного уравнения
			double a = 0, b = 0, c = 0;

			switch (shape.Type)
			{
				case TypeShape.Cylinder:
					Cylinder cylinder = shape as Cylinder;
					// Возвращает вектор (X,Y,Z), x,y,z = 0,1, i-ая компонента которого
					// Равна нулю, если образующая параллельная данной оси.
					Vector axesValue = cylinder.GetAxesValue();

					// Мы не учитываем вклад той компоненты (x или y или z), образующая которой параллельна этой компаненте.
					a = axesValue.X * direction.X * direction.X + axesValue.Y * direction.Y * direction.Y + axesValue.Z * direction.Z * direction.Z;
					b = axesValue.X * 2 * oc.X * direction.X + axesValue.Y * 2 * oc.Y * direction.Y + axesValue.Z * 2 * oc.Z * direction.Z;
					c = axesValue.X * oc.X * oc.X + axesValue.Y * oc.Y * oc.Y + axesValue.Z * oc.Z * oc.Z - cylinder.Radius * cylinder.Radius;


					// switch (cylinder.GetAxis)
					// {
					// 	case Axis.X:
					// 		a = direction.Y * direction.Y + direction.Z * direction.Z;
					// 		b = 2 * oc.Y * direction.Y + 2 * oc.Z * direction.Z;
					// 		c = oc.Y * oc.Y + oc.Z * oc.Z - cylinder.Radius * cylinder.Radius;
					// 		break;

					// 	case Axis.Y:
					// 		a = direction.X * direction.X + direction.Z * direction.Z;
					// 		b = 2 * oc.X * direction.X + 2 * oc.Z * direction.Z;
					// 		c = oc.X * oc.X + oc.Z * oc.Z - cylinder.Radius * cylinder.Radius;
					// 		break;

					// 	case Axis.Z:
					// 		a = direction.X * direction.X + direction.Y * direction.Y;
					// 		b = 2 * oc.X * direction.X + 2 * oc.Y * direction.Y;
					// 		c = oc.X * oc.X + oc.Y * oc.Y - cylinder.Radius * cylinder.Radius;
					// 		break;
					// }

					break;
				case TypeShape.Sphere:
					Sphere sphere = shape as Sphere;

					a = direction.DotProduct(direction);
					b = 2 * oc.DotProduct(direction);
					c = oc.DotProduct(oc) - sphere.Radius * sphere.Radius;

					// Аналогично, только расписали векторные произведения.
					// double a = direction.X * direction.X + direction.Y * direction.Y + direction.Z * direction.Z;
					// double b = 2 * oc.X * direction.X + 2 * oc.Y * direction.Y + 2 * oc.Z * direction.Z;
					// double c = oc.X * oc.X + oc.Y * oc.Y + oc.Z * oc.Z - sphere.Radius * sphere.Radius;
					break;
			}

			double discriminant = b * b - 4 * a * c;

			// Нет решений (пересечений).
			if (discriminant < 0)
			{
				tmp.X = Double.PositiveInfinity;
				tmp.Y = Double.PositiveInfinity;
				return;
			}

			tmp.X = (-b + Math.Sqrt(discriminant)) / (2 * a);
			tmp.Y = (-b - Math.Sqrt(discriminant)) / (2 * a);
			
			// Чтобы цилиндр был конечным, нужно проверить, 
			// Что найденная точка не выходит за допустимые значения.
			if (shape.Type == TypeShape.Cylinder)
			{
				Cylinder cylinder = shape as Cylinder;
				Vector axesValue = cylinder.GetAxesValue();
				axesValue.Reverse();
				double coordinate1 = 0, coordinate2 = 0;
				
				// Находим точку координату точки пересечения луча и цилиндра.
				coordinate1 = axesValue.X * (oc.X + tmp.X * direction.X) + axesValue.Y * (oc.Y + tmp.X * direction.Y) + axesValue.Z * (oc.Z + tmp.X * direction.Z);
				coordinate2 = axesValue.X * (oc.X + tmp.Y * direction.X) + axesValue.Y * (oc.Y + tmp.Y * direction.Y) + axesValue.Z * (oc.Z + tmp.Y * direction.Z);

				// switch (cylinder.GetAxis)
				// {
				// 	case Axis.X:
				// 		t1 = oc.X + tmp.X * direction.X;
				// 		t2 = oc.X + tmp.Y * direction.X;
				// 		break;

				// 	case Axis.Y:
				// 		// Конечный цилиндр удолетворяет условию Ymin < y1,y2 < Ymax.
				// 		t1 = oc.Y + tmp.X * direction.Y;
				// 		t2 = oc.Y + tmp.Y * direction.Y;
				// 		break;

				// 	case Axis.Z:
				// 		t1 = oc.Z + tmp.X * direction.Z;
				// 		t2 = oc.Z + tmp.Y * direction.Z;
				// 		break;
				// }

				if (!((cylinder.Min < coordinate1 && coordinate1 < cylinder.Max) && (cylinder.Min < coordinate2 && coordinate2 < cylinder.Max)))
				{
					tmp.X = Double.PositiveInfinity;
					tmp.Y = Double.PositiveInfinity;
				}
			}
		}

	}
}

// private void IsVisible(Vector origin, ref Vector ts, Vector direction, Triangle triangle)
// {
// 	ts.X = Double.PositiveInfinity;

// 	Vector n = triangle.GetNormal();

// 	Double div = n.DotProduct(direction);

// 	if (Math.Abs(div) < 0.0001d)
// 	{
// 		return;
// 	}	

// 	// if (div == 0)
// 	// 	return;
	
// 	ts.X =  n.DotProduct(triangle.P1 - origin) / div;
// 	if (ts.X < 0)
// 	{
// 		ts.X = Double.PositiveInfinity;
// 		return;
// 	}

// 	// Console.WriteLine(ts.X);
// 	Vector point = origin + direction * ts.X;


// 	Vector v1 = triangle.L1.CrossProduct(point - triangle.P1);  
// 	Vector v2 = triangle.L2.CrossProduct(point - triangle.P2);  
// 	Vector v3 = triangle.L3.CrossProduct(point - triangle.P3);  
	
// 	v1 = v1.Sign();
// 	v2 = v2.Sign();
// 	v3 = v3.Sign();

// 	// if ((v1.X * v2.X) >= 0 && (v2.X * v3.X) >= 0 && (v3.X * v1.X) >= 0 &&
// 	// 	(v1.Y * v2.Y) >= 0 && (v2.Y * v3.Y) >= 0 && (v3.Y * v1.Y) >= 0 &&
// 	// 	(v1.Z * v2.Z) >= 0 && (v2.Z * v3.Z) >= 0 && (v3.Z * v1.Z) >= 0)

// 	if (v1.CoDirectional(v2) && v2.CoDirectional(v3) && v3.CoDirectional(v1))
// 	{
// 		return;
// 	}
// 	ts.X = Double.PositiveInfinity;
// }


// double kf = 0.1;
// double a = direction.X * direction.X + direction.Z * direction.Z + kf * (direction.Y * direction.Y);
// double b = 2 * oc.X * direction.X + 2 * oc.Z * direction.Z + 2 * kf * (oc.Y * direction.Y);
// double c = oc.X * oc.X + oc.Z * oc.Z- sphere.Radius * sphere.Radius + kf * (oc.Y * oc.Y) ;

// Крышечка. Ymax = 2 
// Для Ymin уравнение аналогично.
// double Ymax = 2; 
// ts.Z = (Ymax - oc.Y) / direction.Y;
// if (ts.X > ts.Z)
// 	ts.X = ts.Z;
// if (ts.Y > ts.Z)
// 	ts.Y = ts.Z;