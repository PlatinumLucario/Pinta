﻿// 
// ArrowedEditEngine.cs
//  
// Author:
//	   Andrew Davis <andrew.3.1415@gmail.com>
// 
// Copyright (c) 2014 Andrew Davis, GSoC 2014
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using Cairo;
using Pinta.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pinta.Tools
{
	public abstract class ArrowedEditEngine : BaseEditEngine
	{
		// NRT - These are all set by HandleBuildToolBar
		private Gtk.SeparatorToolItem arrowSep = null!;
		private ToolBarLabel arrowLabel = null!;
		private ToolBarWidget<Gtk.CheckButton> showArrowOneBox = null!, showArrowTwoBox = null!;
		private bool showOtherArrowOptions;

		private ToolBarComboBox arrowSize = null!;
		private ToolBarLabel arrowSizeLabel = null!;
		private ToolBarButton arrowSizeMinus = null!, arrowSizePlus = null!;

		private ToolBarComboBox arrowAngleOffset = null!;
		private ToolBarLabel arrowAngleOffsetLabel = null!;
		private ToolBarButton arrowAngleOffsetMinus = null!, arrowAngleOffsetPlus = null!;

		private ToolBarComboBox arrowLengthOffset = null!;
		private ToolBarLabel arrowLengthOffsetLabel = null!;
		private ToolBarButton arrowLengthOffsetMinus = null!, arrowLengthOffsetPlus = null!;

		private Arrow previousSettings1 = new Arrow();
		private Arrow previousSettings2 = new Arrow();


		#region ToolbarEventHandlers

		void arrowSizeMinus_Clicked(object? sender, EventArgs e)
		{
			double newSize = 10d;

			if (Double.TryParse(arrowSize.ComboBox.ActiveText, out newSize))
			{
				--newSize;

				if (newSize < 1d)
				{
					newSize = 1d;
				}
			}
			else
			{
				newSize = 10d;
			}

			arrowSize.ComboBox.Entry.Text = newSize.ToString();
		}

		void arrowSizePlus_Clicked(object? sender, EventArgs e)
		{
			double newSize = 10d;

			if (Double.TryParse(arrowSize.ComboBox.ActiveText, out newSize))
			{
				++newSize;

				if (newSize > 100d)
				{
					newSize = 100d;
				}
			}
			else
			{
				newSize = 10d;
			}

			arrowSize.ComboBox.Entry.Text = newSize.ToString();
		}

		void arrowAngleOffsetMinus_Clicked(object? sender, EventArgs e)
		{
			double newAngle = 0d;

			if (Double.TryParse(arrowAngleOffset.ComboBox.ActiveText, out newAngle))
			{
				--newAngle;

				if (newAngle < -89d)
				{
					newAngle = -89d;
				}
			}
			else
			{
				newAngle = 0d;
			}

			arrowAngleOffset.ComboBox.Entry.Text = newAngle.ToString();
		}

		void arrowAngleOffsetPlus_Clicked(object? sender, EventArgs e)
		{
			double newAngle = 0d;

			if (Double.TryParse(arrowAngleOffset.ComboBox.ActiveText, out newAngle))
			{
				++newAngle;

				if (newAngle > 89d)
				{
					newAngle = 89d;
				}
			}
			else
			{
				newAngle = 0d;
			}

			arrowAngleOffset.ComboBox.Entry.Text = newAngle.ToString();
		}

		void arrowLengthOffsetMinus_Clicked(object? sender, EventArgs e)
		{
			double newLength = 10d;

			if (Double.TryParse(arrowLengthOffset.ComboBox.ActiveText, out newLength))
			{
				--newLength;

				if (newLength < -100d)
				{
					newLength = -100d;
				}
			}
			else
			{
				newLength = 10d;
			}

			arrowLengthOffset.ComboBox.Entry.Text = newLength.ToString();
		}

		void arrowLengthOffsetPlus_Clicked(object? sender, EventArgs e)
		{
			double newLength = 10d;

			if (Double.TryParse(arrowLengthOffset.ComboBox.ActiveText, out newLength))
			{
				++newLength;

				if (newLength > 100d)
				{
					newLength = 100d;
				}
			}
			else
			{
				newLength = 10d;
			}

			arrowLengthOffset.ComboBox.Entry.Text = newLength.ToString();
		}

		#endregion ToolbarEventHandlers


		public override void HandleBuildToolBar(Gtk.Toolbar tb)
		{
			base.HandleBuildToolBar(tb);


			#region Show Arrows

			//Arrow separator.

			if (arrowSep == null)
			{
				arrowSep = new Gtk.SeparatorToolItem();

				showOtherArrowOptions = false;
			}

			tb.AppendItem(arrowSep);


			if (arrowLabel == null)
			{
				arrowLabel = new ToolBarLabel(string.Format(" {0}: ", Translations.GetString("Arrow")));
			}

			tb.AppendItem(arrowLabel);


			//Show arrow 1.

			if (showArrowOneBox == null) {
				showArrowOneBox = new (new Gtk.CheckButton ("1"));
				showArrowOneBox.Widget.Active = previousSettings1.Show;

				showArrowOneBox.Widget.Toggled += (o, e) => {
					//Determine whether to change the visibility of Arrow options in the toolbar based on the updated Arrow showing/hiding.
					if (!showArrowOneBox.Widget.Active && !showArrowTwoBox.Widget.Active) {
						if (showOtherArrowOptions) {
							tb.Remove (arrowSizeLabel);
							tb.Remove (arrowSizeMinus);
							tb.Remove (arrowSize);
							tb.Remove (arrowSizePlus);
							tb.Remove (arrowAngleOffsetLabel);
							tb.Remove (arrowAngleOffsetMinus);
							tb.Remove (arrowAngleOffset);
							tb.Remove (arrowAngleOffsetPlus);
							tb.Remove (arrowLengthOffsetLabel);
							tb.Remove (arrowLengthOffsetMinus);
							tb.Remove (arrowLengthOffset);
							tb.Remove (arrowLengthOffsetPlus);

							showOtherArrowOptions = false;
						}
					} else {
						if (!showOtherArrowOptions) {
							tb.Add (arrowSizeLabel);
							tb.Add (arrowSizeMinus);
							tb.Add (arrowSize);
							tb.Add (arrowSizePlus);
							tb.Add (arrowAngleOffsetLabel);
							tb.Add (arrowAngleOffsetMinus);
							tb.Add (arrowAngleOffset);
							tb.Add (arrowAngleOffsetPlus);
							tb.Add (arrowLengthOffsetLabel);
							tb.Add (arrowLengthOffsetMinus);
							tb.Add (arrowLengthOffset);
							tb.Add (arrowLengthOffsetPlus);

							showOtherArrowOptions = true;
						}
					}

					LineCurveSeriesEngine? activeEngine = (LineCurveSeriesEngine?) ActiveShapeEngine;

					if (activeEngine != null) {
						activeEngine.Arrow1.Show = showArrowOneBox.Widget.Active;

						DrawActiveShape (false, false, true, false, false);

						StorePreviousSettings ();
					}
				};
			}

			tb.AppendItem(showArrowOneBox);


			//Show arrow 2.
			if (showArrowTwoBox == null) {
				showArrowTwoBox = new (new Gtk.CheckButton ("2"));
				showArrowTwoBox.Widget.Active = previousSettings2.Show;

				showArrowTwoBox.Widget.Toggled += (o, e) => {
					//Determine whether to change the visibility of Arrow options in the toolbar based on the updated Arrow showing/hiding.
					if (!showArrowOneBox.Widget.Active && !showArrowTwoBox.Widget.Active) {
						if (showOtherArrowOptions) {
							tb.Remove (arrowSizeLabel);
							tb.Remove (arrowSizeMinus);
							tb.Remove (arrowSize);
							tb.Remove (arrowSizePlus);
							tb.Remove (arrowAngleOffsetLabel);
							tb.Remove (arrowAngleOffsetMinus);
							tb.Remove (arrowAngleOffset);
							tb.Remove (arrowAngleOffsetPlus);
							tb.Remove (arrowLengthOffsetLabel);
							tb.Remove (arrowLengthOffsetMinus);
							tb.Remove (arrowLengthOffset);
							tb.Remove (arrowLengthOffsetPlus);

							showOtherArrowOptions = false;
						}
					} else {
						if (!showOtherArrowOptions) {
							tb.Add (arrowSizeLabel);
							tb.Add (arrowSizeMinus);
							tb.Add (arrowSize);
							tb.Add (arrowSizePlus);
							tb.Add (arrowAngleOffsetLabel);
							tb.Add (arrowAngleOffsetMinus);
							tb.Add (arrowAngleOffset);
							tb.Add (arrowAngleOffsetPlus);
							tb.Add (arrowLengthOffsetLabel);
							tb.Add (arrowLengthOffsetMinus);
							tb.Add (arrowLengthOffset);
							tb.Add (arrowLengthOffsetPlus);

							showOtherArrowOptions = true;
						}
					}

					LineCurveSeriesEngine? activeEngine = (LineCurveSeriesEngine?) ActiveShapeEngine;

					if (activeEngine != null) {
						activeEngine.Arrow2.Show = showArrowTwoBox.Widget.Active;

						DrawActiveShape (false, false, true, false, false);

						StorePreviousSettings ();
					}
				};
			}

			tb.AppendItem (showArrowTwoBox);

			#endregion Show Arrows


			#region Arrow Size

			if (arrowSizeLabel == null)
			{
				arrowSizeLabel = new ToolBarLabel(string.Format(" {0}: ", Translations.GetString("Size")));
			}

			if (arrowSizeMinus == null)
			{
				arrowSizeMinus = new ToolBarButton("Toolbar.MinusButton.png", "", Translations.GetString("Decrease arrow size"));
				arrowSizeMinus.Clicked += new EventHandler(arrowSizeMinus_Clicked);
			}

			if (arrowSize == null)
			{
				arrowSize = new ToolBarComboBox(65, 7, true,
					"3", "4", "5", "6", "7", "8", "9", "10", "12", "15", "18",
					"20", "25", "30", "40", "50", "60", "70", "80", "90", "100");

				arrowSize.ComboBox.Changed += (o, e) =>
				{
					if (arrowSize.ComboBox.ActiveText.Length < 1)
					{
						//Ignore the change until the user enters something.
						return;
					}
					else
					{
						double newSize = 10d;

						if (arrowSize.ComboBox.ActiveText == "-")
						{
							//The user is trying to enter a negative value: change it to 1.
							newSize = 1d;
						}
						else
						{
							if (Double.TryParse(arrowSize.ComboBox.ActiveText, out newSize))
							{
								if (newSize < 1d)
								{
									//Less than 1: change it to 1.
									newSize = 1d;
								}
								else if (newSize > 100d)
								{
									//Greater than 100: change it to 100.
									newSize = 100d;
								}
							}
							else
							{
								//Not a number: wait until the user enters something.
								return;
							}
						}

						arrowSize.ComboBox.Entry.Text = newSize.ToString();

						LineCurveSeriesEngine? activeEngine = (LineCurveSeriesEngine?)ActiveShapeEngine;

						if (activeEngine != null)
						{
							activeEngine.Arrow1.ArrowSize = newSize;
							activeEngine.Arrow2.ArrowSize = newSize;

							DrawActiveShape(false, false, true, false, false);

							StorePreviousSettings();
						}
					}
				};
			}

			if (arrowSizePlus == null)
			{
				arrowSizePlus = new ToolBarButton("Toolbar.PlusButton.png", "", Translations.GetString("Increase arrow size"));
				arrowSizePlus.Clicked += new EventHandler(arrowSizePlus_Clicked);
			}

			#endregion Arrow Size


			#region Angle Offset

			if (arrowAngleOffsetLabel == null)
			{
				arrowAngleOffsetLabel = new ToolBarLabel(string.Format(" {0}: ", Translations.GetString("Angle")));
			}

			if (arrowAngleOffsetMinus == null)
			{
				arrowAngleOffsetMinus = new ToolBarButton("Toolbar.MinusButton.png", "", Translations.GetString("Decrease angle offset"));
				arrowAngleOffsetMinus.Clicked += new EventHandler(arrowAngleOffsetMinus_Clicked);
			}

			if (arrowAngleOffset == null)
			{
				arrowAngleOffset = new ToolBarComboBox(65, 9, true,
					"-30", "-25", "-20", "-15", "-10", "-5", "0", "5", "10", "15", "20", "25", "30");

				arrowAngleOffset.ComboBox.Changed += (o, e) =>
				{
					if (arrowAngleOffset.ComboBox.ActiveText.Length < 1)
					{
						//Ignore the change until the user enters something.
						return;
					}
					else if (arrowAngleOffset.ComboBox.ActiveText == "-")
					{
						//The user is trying to enter a negative value: ignore the change until the user enters more.
						return;
					}
					else
					{
						double newAngle = 15d;

						if (Double.TryParse(arrowAngleOffset.ComboBox.ActiveText, out newAngle))
						{
							if (newAngle < -89d)
							{
								//Less than -89: change it to -89.
								newAngle = -89d;
							}
							else if (newAngle > 89d)
							{
								//Greater than 89: change it to 89.
								newAngle = 89d;
							}
						}
						else
						{
							//Not a number: wait until the user enters something.
							return;
						}

						arrowAngleOffset.ComboBox.Entry.Text = newAngle.ToString();

						LineCurveSeriesEngine? activeEngine = (LineCurveSeriesEngine?)ActiveShapeEngine;

						if (activeEngine != null)
						{
							activeEngine.Arrow1.AngleOffset = newAngle;
							activeEngine.Arrow2.AngleOffset = newAngle;

							DrawActiveShape(false, false, true, false, false);

							StorePreviousSettings();
						}
					}
				};
			}

			if (arrowAngleOffsetPlus == null)
			{
				arrowAngleOffsetPlus = new ToolBarButton("Toolbar.PlusButton.png", "", Translations.GetString("Increase angle offset"));
				arrowAngleOffsetPlus.Clicked += new EventHandler(arrowAngleOffsetPlus_Clicked);
			}

			#endregion Angle Offset


			#region Length Offset

			if (arrowLengthOffsetLabel == null)
			{
				arrowLengthOffsetLabel = new ToolBarLabel(string.Format(" {0}: ", Translations.GetString("Length")));
			}

			if (arrowLengthOffsetMinus == null)
			{
				arrowLengthOffsetMinus = new ToolBarButton("Toolbar.MinusButton.png", "", Translations.GetString("Decrease length offset"));
				arrowLengthOffsetMinus.Clicked += new EventHandler(arrowLengthOffsetMinus_Clicked);
			}

			if (arrowLengthOffset == null)
			{
				arrowLengthOffset = new ToolBarComboBox(65, 8, true,
					"-30", "-25", "-20", "-15", "-10", "-5", "0", "5", "10", "15", "20", "25", "30");

				arrowLengthOffset.ComboBox.Changed += (o, e) =>
				{
					if (arrowLengthOffset.ComboBox.ActiveText.Length < 1)
					{
						//Ignore the change until the user enters something.
						return;
					}
					else if (arrowLengthOffset.ComboBox.ActiveText == "-")
					{
						//The user is trying to enter a negative value: ignore the change until the user enters more.
						return;
					}
					else
					{
						double newLength = 10d;

						if (Double.TryParse(arrowLengthOffset.ComboBox.ActiveText, out newLength))
						{
							if (newLength < -100d)
							{
								//Less than -100: change it to -100.
								newLength = -100d;
							}
							else if (newLength > 100d)
							{
								//Greater than 100: change it to 100.
								newLength = 100d;
							}
						}
						else
						{
							//Not a number: wait until the user enters something.
							return;
						}

						arrowLengthOffset.ComboBox.Entry.Text = newLength.ToString();

						LineCurveSeriesEngine? activeEngine = (LineCurveSeriesEngine?)ActiveShapeEngine;

						if (activeEngine != null)
						{
							activeEngine.Arrow1.LengthOffset = newLength;
							activeEngine.Arrow2.LengthOffset = newLength;

							DrawActiveShape(false, false, true, false, false);

							StorePreviousSettings();
						}
					}
				};
			}

			if (arrowLengthOffsetPlus == null)
			{
				arrowLengthOffsetPlus = new ToolBarButton("Toolbar.PlusButton.png", "", Translations.GetString("Increase length offset"));
				arrowLengthOffsetPlus.Clicked += new EventHandler(arrowLengthOffsetPlus_Clicked);
			}

			#endregion Length Offset

			
			if (showOtherArrowOptions)
			{
				tb.Add(arrowSizeLabel);
				tb.Add(arrowSizeMinus);
				tb.Add(arrowSize);
				tb.Add(arrowSizePlus);
				tb.Add(arrowAngleOffsetLabel);
				tb.Add(arrowAngleOffsetMinus);
				tb.Add(arrowAngleOffset);
				tb.Add(arrowAngleOffsetPlus);
				tb.Add(arrowLengthOffsetLabel);
				tb.Add(arrowLengthOffsetMinus);
				tb.Add(arrowLengthOffset);
				tb.Add(arrowLengthOffsetPlus);
			}
		}


		public ArrowedEditEngine(ShapeTool passedOwner): base(passedOwner)
		{
			
		}


		/// <summary>
		/// Set the new arrow's settings to be the same as what's in the toolbar settings.
		/// </summary>
		protected void setNewArrowSettings(LineCurveSeriesEngine newEngine)
		{
			if (showArrowOneBox != null)
			{
				newEngine.Arrow1.Show = showArrowOneBox.Widget.Active;
				newEngine.Arrow2.Show = showArrowTwoBox.Widget.Active;

				Double.TryParse(arrowSize.ComboBox.Entry.Text, out newEngine.Arrow1.ArrowSize);
				Double.TryParse(arrowAngleOffset.ComboBox.Entry.Text, out newEngine.Arrow1.AngleOffset);
				Double.TryParse(arrowLengthOffset.ComboBox.Entry.Text, out newEngine.Arrow1.LengthOffset);

				newEngine.Arrow1.ArrowSize = Utility.Clamp(newEngine.Arrow1.ArrowSize, 1d, 100d);
				newEngine.Arrow2.ArrowSize = newEngine.Arrow1.ArrowSize;
				newEngine.Arrow1.AngleOffset = Utility.Clamp(newEngine.Arrow1.AngleOffset, -89d, 89d);
				newEngine.Arrow2.AngleOffset = newEngine.Arrow1.AngleOffset;
				newEngine.Arrow1.LengthOffset = Utility.Clamp(newEngine.Arrow1.LengthOffset, -100d, 100d);
				newEngine.Arrow2.LengthOffset = newEngine.Arrow1.LengthOffset;
			}
		}


		public override void UpdateToolbarSettings(ShapeEngine engine)
		{
			if (engine != null && engine.ShapeType == ShapeTypes.OpenLineCurveSeries)
			{
				if (showArrowOneBox != null)
				{
					LineCurveSeriesEngine lCSEngine = (LineCurveSeriesEngine)engine;

					showArrowOneBox.Widget.Active = lCSEngine.Arrow1.Show;
					showArrowTwoBox.Widget.Active = lCSEngine.Arrow2.Show;
					
					if (showOtherArrowOptions)
					{
						arrowSize.ComboBox.Entry.Text = lCSEngine.Arrow1.ArrowSize.ToString();
						arrowAngleOffset.ComboBox.Entry.Text = lCSEngine.Arrow1.AngleOffset.ToString();
						arrowLengthOffset.ComboBox.Entry.Text = lCSEngine.Arrow1.LengthOffset.ToString();
					}
				}

				base.UpdateToolbarSettings(engine);
			}
		}

		protected override void RecallPreviousSettings()
		{
			if (showArrowOneBox != null)
			{
				showArrowOneBox.Widget.Active = previousSettings1.Show;
				showArrowTwoBox.Widget.Active = previousSettings2.Show;

				if (showOtherArrowOptions)
				{
					arrowSize.ComboBox.Entry.Text = previousSettings1.ArrowSize.ToString();
					arrowAngleOffset.ComboBox.Entry.Text = previousSettings1.AngleOffset.ToString();
					arrowLengthOffset.ComboBox.Entry.Text = previousSettings1.LengthOffset.ToString();
				}
			}

			base.RecallPreviousSettings();
		}

		protected override void StorePreviousSettings()
		{
			if (showArrowOneBox != null)
			{
				previousSettings1.Show = showArrowOneBox.Widget.Active;
				previousSettings2.Show = showArrowTwoBox.Widget.Active;

				Double.TryParse(arrowSize.ComboBox.Entry.Text, out previousSettings1.ArrowSize);
				Double.TryParse(arrowAngleOffset.ComboBox.Entry.Text, out previousSettings1.AngleOffset);
				Double.TryParse(arrowLengthOffset.ComboBox.Entry.Text, out previousSettings1.LengthOffset);

				//Other Arrow2 settings are unnecessary since they are the same as Arrow1's.
			}

			base.StorePreviousSettings();
		}


		protected override void DrawExtras(ref Rectangle? dirty, Context g, ShapeEngine engine)
		{
            LineCurveSeriesEngine? lCSEngine = engine as LineCurveSeriesEngine;
			if (lCSEngine != null && engine.ControlPoints.Count > 0)
			{
				// Draw the arrows for the currently active shape.
				GeneratedPoint[] genPoints = engine.GeneratedPoints;

                if (lCSEngine.Arrow1.Show)
                {
                    if (genPoints.Length > 1)
                    {
                        dirty = dirty.UnionRectangles(lCSEngine.Arrow1.Draw(g, lCSEngine.OutlineColor,
                            genPoints[0].Position, genPoints[1].Position));
                    }
                }

                if (lCSEngine.Arrow2.Show)
                {
                    if (genPoints.Length > 1)
                    {
                        dirty = dirty.UnionRectangles(lCSEngine.Arrow2.Draw(g, lCSEngine.OutlineColor,
                            genPoints[genPoints.Length - 1].Position, genPoints[genPoints.Length - 2].Position));
                    }
                }
			}

			base.DrawExtras(ref dirty, g, engine);
		}
	}
}
