﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PieChart.ViitorCloud
{
    public class PieChart : MonoBehaviour
    {
        [Tooltip("Object of PieChartMeshController")]
        public PieChartMeshController pieChartMeshController;

        [Tooltip("Each of the parts into which the will be divided")]
        public int segments;

        [Tooltip("The data for the pie\n" +
                 "The size of this list must exact the value of Segment.")]
        public decimal[] Data;

        [Tooltip("Main Material that the mesh of the pie will use to rander")]
        public Material mainMaterial;

        [Tooltip("The colors that will be applied on the pie\n" +
                 "The size of this list must exact the value of Segment.")]
        public Color[] customColors;

        [SerializeField]
        [Tooltip("Pie chart with not information and title")]
        public bool justCreateThePie;

        [Tooltip("The list of description of the pie.")]
        public List<string> dataDescription = new List<string>();

        [Tooltip("Type of animation which will the pie have.")]
        public PieChartMeshController.AnimationType animationType;

        void Start()
        {
            //pieChartMeshController.onPointerEnter.AddListener(onPointerClick);

            if (mainMaterial != null)
                pieChartMeshController.SetMatrialOfPie(mainMaterial);
        }

        public void GenerateChart()
        {
            ClearChart();
            pieChartMeshController.SetData(Data);
            pieChartMeshController.SetColor(customColors);
            pieChartMeshController.SetDescription(dataDescription.ToArray());
            pieChartMeshController.GenerateChart(segments, animationType, justCreateThePie);
        }

        public void ClearChart()
        {
            pieChartMeshController.ClearChart();
        }

        [ContextMenu("Take SS")]
        void TakeSS()
        {
            ScreenCapture.CaptureScreenshot($"{Application.productName} {GetTimeString()}.png");

            string GetTimeString()
            {
                return System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            }
        }

        public void ReverseAnimation(Animation anim, string AnimationName)
        {
            anim[AnimationName].speed = -1;
            anim[AnimationName].time = anim[AnimationName].length;
            anim.CrossFade(AnimationName);
        }
        public void ForwardAnimation(Animation anim, string AnimationName)
        {
            anim[AnimationName].speed = 1;
            anim[AnimationName].time = 0;
            anim.CrossFade(AnimationName);
        }
    }
}