namespace Newton
{
	public enum SizeObjects
	{
		WidthWindows = 1200,
		HeightWindows = 900,
		WidthCanvas = 1200, //600,
		HeightCanvas = 600

	}

	public enum Mode
	{
		Off,
		On
	}

	public enum State
	{
		Start,
		MovingUpRight,
		MovingDownRight,
		MovingUpLeft,
		MovingDownLeft

	}

	public enum LightType
	{
		Point,
		Ambient,
		Directional
	}
}