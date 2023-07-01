namespace Endo
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Drawing;
	using System.IO;
	using System.Xml;
	using System.Xml.Serialization;

	public class ClockColor
	{
		public string HourHand;
		public string MinuteHand;
		public string SecondHand;
        public string Indicator;
		public string Face;
		public string HourTick;
		public string MinuteTick;
		public string Text;
	}

	public class ClockData
	{
		public string Label;
		public string TimeZoneId;
		public ClockColor Colors;
		public string FaceFont;
		public bool ShowMinuteTicks;
		public bool ShowHourTicks;
		public bool ShowLabel;
        public bool ShowSecondHand;
	}

	public class ClockDataCollection
	{
		[XmlElement]
		public ClockData[] ClockData;
        public FillStyleMode FillStyle;
        public int ClockSize;
	}

	static class ClockDataManager
	{
		static string[] defaultPaths = 
		{
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			"endoClock",
			"endoClockConfig.xml",
		};

		public static readonly string DefaultPath = Path.Combine(defaultPaths);

		public static ClockDataCollection GetConfig()
		{ 
			var config = new FileInfo(DefaultPath);

			if (!config.Exists)
			{
				try 
				{
					Save();
				}
				catch (DirectoryNotFoundException ex)
				{
					try 
					{
						Directory.CreateDirectory(config.Directory.FullName);
						Save();
					}
					catch (Exception innerEx)
					{
						Console.Error.WriteLine(innerEx);				
					}
				}
			}

			return Load();
		}
			

		static ClockData MakeClockData(string label, string tz)
		{
			var clockData = new ClockData();
			var defaultColors = new ClockColor
			{
				HourHand = ColorTranslator.ToHtml(Color.Magenta),
				MinuteHand = ColorTranslator.ToHtml(Color.Magenta),
				SecondHand = ColorTranslator.ToHtml(Color.Cyan),
                Indicator = ColorTranslator.ToHtml(Color.Magenta),
				Face = ColorTranslator.ToHtml(Color.Black),
				HourTick = ColorTranslator.ToHtml(Color.Cyan),
				MinuteTick = ColorTranslator.ToHtml(Color.Magenta),
				Text = ColorTranslator.ToHtml(Color.Cyan),
			};
			var faceFont = "Consolas";
			
			clockData.Label = label;
			clockData.TimeZoneId = tz;
			clockData.Colors = defaultColors;
			clockData.FaceFont = faceFont;
			clockData.ShowHourTicks = true;
			clockData.ShowMinuteTicks = true;
			clockData.ShowLabel = true;
            clockData.ShowSecondHand = false;

			return clockData;			
		}

		static ClockDataCollection Default()
		{
			var localTz = TimeZoneInfo.Local;
			var utcTz = TimeZoneInfo.Utc;
			var pstTz = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
			var estTz = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            var collection = new ClockDataCollection();

            collection.FillStyle = FillStyleMode.TopToBottom;
            collection.ClockSize = 113;
			collection.ClockData = new ClockData[]
			{
				MakeClockData("Local", localTz.Id),
				MakeClockData("West", pstTz.Id),
				MakeClockData("East", estTz.Id),
				MakeClockData("Zulu", utcTz.Id)
			};

			return collection;
		}

		static void Save(string path, ClockDataCollection data)
		{
			var serializer = new XmlSerializer(typeof(ClockDataCollection));
			var writer = new StreamWriter(path);

			serializer.Serialize(writer, data);
			writer.Close();			
		} 

		static void Save()
		{
			Save(DefaultPath, Default());
		}

        public static void Save(ClockContainerForm x)
        {
            var y = new ClockDataCollection();

            y.ClockSize = x.ClockRadius;
            y.FillStyle = x.FillStyle;
            y.ClockData = new ClockData[x.Clocks.Length];

            for (var i = 0; i < x.Clocks.Length; i++)
            {
                var c = x.Clocks[i];

                y.ClockData[i] = new ClockData()
                { 
                    FaceFont = c.FaceFont.Name,
                    Label = c.ClockName,
                    ShowLabel = c.CanDrawName,
                    ShowHourTicks = c.CanDrawHourTicks,
                    ShowMinuteTicks = c.CanDrawMinuteTicks,
                    ShowSecondHand = c.CanDrawSecondHand, 
                    TimeZoneId = c.TimeZone.Id, 
                    Colors = new ClockColor()
                    { 
                        HourHand = ColorTranslator.ToHtml(c.ColorHourHand),
                        MinuteHand = ColorTranslator.ToHtml(c.ColorMinuteHand),
                        SecondHand = ColorTranslator.ToHtml(c.ColorSecondHand),
                        Indicator = ColorTranslator.ToHtml(c.ColorIndicator),
                        Face = ColorTranslator.ToHtml(c.ColorFace),
                        HourTick = ColorTranslator.ToHtml(c.ColorHourTick),
                        MinuteTick = ColorTranslator.ToHtml(c.ColorMinuteTick),
                        Text = ColorTranslator.ToHtml(c.ColorText),
                    },
                };
            }

            Save(DefaultPath, y); 
        }

		static ClockDataCollection Load(string path)
		{
			var serializer = new XmlSerializer(typeof(ClockDataCollection));

			using (var fs = new FileStream(path, FileMode.Open))
			{
				var data = (ClockDataCollection) serializer.Deserialize(fs);

				return data;
			}
		}

		static ClockDataCollection Load()
		{
			return Load(DefaultPath);
		}		
	}
}