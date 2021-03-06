﻿using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Tasks.Geoprocessing;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ArcGISRuntimeSDKDotNet_DesktopSamples.Samples
{
    /// <summary>
    /// This sample demonstrates use of the Geoprocessor to call a MessageInABottle geoprocessing service. To use the sample, specify the number of days and click a point in the ocean. The path of a bottle dropped at the click point over the specified number of days will be drawn on the map.
    /// </summary>
    /// <title>Message in a Bottle</title>
	/// <category>Tasks</category>
	/// <subcategory>Geoprocessing</subcategory>
	public partial class MessageInABottle : UserControl
    {
        /// <summary>Construct Message In A Bottle sample control</summary>
        public MessageInABottle()
        {
            InitializeComponent();
        }

        // Begin geoprocessing with a user tap on the map
        private async void mapView_MapViewTapped(object sender, Esri.ArcGISRuntime.Controls.MapViewInputEventArgs e)
        {
            try
            {
                Progress.Visibility = Visibility.Visible;

                InputLayer.Graphics.Clear();
                InputLayer.Graphics.Add(new Graphic() { Geometry = e.Location });

                Geoprocessor geoprocessorTask = new Geoprocessor(
                    new Uri("http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Specialty/ESRI_Currents_World/GPServer/MessageInABottle"));

                var parameter = new GPInputParameter() { OutSpatialReference = mapView.SpatialReference };
                var ptNorm = GeometryEngine.NormalizeCentralMeridianOfGeometry(e.Location);
                var ptGeographic = GeometryEngine.Project(ptNorm, SpatialReferences.Wgs84) as MapPoint;

                parameter.GPParameters.Add(new GPFeatureRecordSetLayer("Input_Point", ptGeographic));
                parameter.GPParameters.Add(new GPDouble("Days", Convert.ToDouble(DaysTextBox.Text)));

                var result = await geoprocessorTask.ExecuteAsync(parameter);

                ResultLayer.Graphics.Clear();
                foreach (GPParameter gpParameter in result.OutParameters)
                {
                    if (gpParameter is GPFeatureRecordSetLayer)
                    {
                        GPFeatureRecordSetLayer gpLayer = gpParameter as GPFeatureRecordSetLayer;
                        ResultLayer.Graphics.AddRange(gpLayer.FeatureSet.Features);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Geoprocessor service failed: " + ex.Message, "Sample Error");
            }
            finally
            {
                Progress.Visibility = Visibility.Collapsed;
            }
        }
    }
}
