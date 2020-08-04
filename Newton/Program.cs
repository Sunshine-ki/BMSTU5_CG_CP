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
		static void Main(string[] args)
		{
			// Применяем стили операционной системы к приложению.
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			// Создаем экземпляр формы и запускаем его.
			Application.Run(new MainForm(CreateScene(args[0])));
		}

		static List<Shape> CreateScene(string path)
		{
			// Color c = Color.Blue;
			// Console.WriteLine(Color.Multiply(c, 2));


			List<Shape> scene = new List<Shape>();
			string[] param;

			foreach (string line in File.ReadLines(path))
			{
				param = line.Split(' ');
				if (param.Length != 5)
					continue;
				scene.Add(new Sphere(new Vector(Convert.ToDouble(param[0]),
					Convert.ToDouble(param[1]), Convert.ToDouble(param[2])),
					Convert.ToDouble(param[3]), Color.FromName(param[4])));
			}

			return scene;
		}
	}
}