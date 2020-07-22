using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Newton
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			// Применяем стили операционной системы к приложению.
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			// Создаем экземпляр формы и запускаем его.
			Application.Run(new Form1(CreateScene("data/scene.txt")));
		}
		static List<Shape> CreateScene(string path)
		{
			List<Shape> scene = new List<Shape>();
			float xCenter, yCenter, r;

			// scene[0].print();
			foreach (string line in File.ReadLines(path))
			{
				string[] param = line.Split(' ');
				if (param.Length != 3)
					continue;
				xCenter = Convert.ToSingle(param[0]);
				yCenter = Convert.ToSingle(param[1]);
				r = Convert.ToSingle(param[2]);
				scene.Add(new Sphere(new PointF(xCenter, yCenter), r, Color.Black));
			}

			return scene;
		}
	}
}