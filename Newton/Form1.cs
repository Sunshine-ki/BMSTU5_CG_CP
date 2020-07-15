using System;
using System.Drawing;
using System.Windows.Forms;

namespace Newton
{
	public class Form1 : Form
	{
		private PictureBox imgBox;
		private Bitmap img; // Содержит растровое изображение.
		public Form1()
		{
			SettingsWindows();
			InitializeComponent();
			imgBox = new PictureBox();
			imgBox.Size = new Size((int)SizeObjects.WidthCanvas, (int)SizeObjects.HeightCanvas);
			// flag = new Bitmap(600, 400);
			this.Controls.Add(imgBox);
			imgBox.Image = img;
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
			Console.WriteLine("Yes!");
			this.img = new Bitmap((int)SizeObjects.WidthCanvas, (int)SizeObjects.HeightCanvas);

			for (int i = 0; i < (int)SizeObjects.WidthCanvas; i++)
			{
				for (int j = 0; j < (int)SizeObjects.HeightCanvas; j++)
				{
					img.SetPixel(i, j, Color.Black);
					// Color curPixColor = img.GetPixel(i, j);
				}
			}

			imgBox.Image = img;
		}
	}

}