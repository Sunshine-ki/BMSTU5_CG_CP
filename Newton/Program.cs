using System;
using System.Windows.Forms;

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
			Application.Run(new Form1());
		}
	}
}