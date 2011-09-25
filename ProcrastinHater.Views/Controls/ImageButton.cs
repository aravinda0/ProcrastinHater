using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProcrastinHater.Views.Controls
{
	/// <summary>
	/// Description of ImageButton.
	/// </summary>
	public class ImageButton : Button
	{
		static ImageButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));

		}
		
		#region Dependency properties
		
		public static readonly DependencyProperty ImageSourceProperty =
			DependencyProperty.Register("ImageSource", typeof(string), typeof(ImageButton),
			                            new FrameworkPropertyMetadata());
		
		public string ImageSource {
			get { return (string)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}
		
		
	    public Visibility BorderVisibility
	    {
	      get { return (Visibility)GetValue(BorderVisibilityProperty); }
	      set { SetValue(BorderVisibilityProperty, value); }
	    }
	
	    public static readonly DependencyProperty BorderVisibilityProperty =
	        DependencyProperty.Register("BorderVisibility", typeof(Visibility), typeof(ImageButton),
	        new FrameworkPropertyMetadata(Visibility.Hidden, FrameworkPropertyMetadataOptions.AffectsRender));

		
	    public double ImageSize
	    {
	      get { return (double)GetValue(ImageSizeProperty); }
	      set { SetValue(ImageSizeProperty, value); }
	    }
	
	    public static readonly DependencyProperty ImageSizeProperty =
	        DependencyProperty.Register("ImageSize", typeof(double), typeof(ImageButton),
	        new FrameworkPropertyMetadata(50.0, FrameworkPropertyMetadataOptions.AffectsRender));


	    public static readonly DependencyProperty OrientationProperty =
	    	DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ImageButton),
	    	                            new FrameworkPropertyMetadata());
	    
	    public Orientation Orientation {
	    	get { return (Orientation)GetValue(OrientationProperty); }
	    	set { SetValue(OrientationProperty, value); }
	    }
	    
	    public static readonly DependencyProperty ImageStretchProperty =
	    	DependencyProperty.Register("ImageStretch", typeof(Stretch), typeof(ImageButton),
	    	                            new FrameworkPropertyMetadata());
	    
	    public Stretch ImageStretch {
	    	get { return (Stretch)GetValue(ImageStretchProperty); }
	    	set { SetValue(ImageStretchProperty, value); }
	    }
	    
	    public static readonly DependencyProperty ScaleCenterXProperty =
	    	DependencyProperty.Register("ScaleCenterX", typeof(double), typeof(ImageButton),
	    	                            new FrameworkPropertyMetadata());
	    
	    public double ScaleCenterX {
	    	get { return (double)GetValue(ScaleCenterXProperty); }
	    	set { SetValue(ScaleCenterXProperty, value); }
	    }
	    
	    public static readonly DependencyProperty ScaleCenterYProperty =
	    	DependencyProperty.Register("ScaleCenterY", typeof(double), typeof(ImageButton),
	    	                            new FrameworkPropertyMetadata());
	    
	    public double ScaleCenterY {
	    	get { return (double)GetValue(ScaleCenterYProperty); }
	    	set { SetValue(ScaleCenterYProperty, value); }
	    }
		
		#endregion
	}
}
