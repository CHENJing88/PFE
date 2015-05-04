using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace RetroGUI.util
{
    /// <summary>Adds Circle element at every point of graph PCA for the lib DynamicDataDisplay</summary>
    public class ACPElementPointMarker : CircleElementPointMarker
    {
        /// <summary>
        /// mouse click marker event
        /// </summary>
        public EventHandler<MouseButtonEventArgs> MarkerClick;
        /// <summary>
        /// the list of markers
        /// </summary>
        private ObservableCollection<UIElement> m_markers;
        public ObservableCollection<UIElement> Markers
        {
            get { return m_markers; }
        }

        public override UIElement CreateMarker()
        {
            Ellipse result = new Ellipse();
            result.Width = Size;
            result.Height = Size;
            result.Stroke = Brush;
            result.Fill = Fill;
            //add the mouse event on the element point
            result.MouseLeftButtonDown += new MouseButtonEventHandler(result_MouseLeftButtonDown);
            //load the tooltiptext if it has
            if (!String.IsNullOrEmpty(ToolTipText))
            {
                ToolTip tt = new ToolTip();
                tt.Content = ToolTipText;
                result.ToolTip = tt;
            }
            return result;
        }
        /// <summary>
        /// mouse event MouseLeftButtonDown on the element point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void result_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MarkerClick != null)
            {
                MarkerClick(sender, e);
            }
        }
        /// <summary>
        /// set the position of marker in the screen
        /// </summary>
        /// <param name="marker">marker</param>
        /// <param name="screenPoint">screenPoint</param>
        public override void SetPosition(UIElement marker, Point screenPoint)
        {
            Canvas.SetLeft(marker, screenPoint.X - Size / 2);
            Canvas.SetTop(marker, screenPoint.Y - Size / 2);
        }
    }
}
