using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geometry;
using AttributeTable;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.NetworkAnalyst;

namespace DynamicSchedulingofEmergencyResourceSystem
{
    public class Variable
    {
        private static IEnvelope pIEnvelop;
        public static IEnvelope pEnvelop
        {
            get { return pIEnvelop; }
            set { pIEnvelop = value; }
        }
        //鹰眼同步
        private static EagleEyeMapFrm pIEagleEyeMapFrm;
        public static EagleEyeMapFrm pEagleEyeMapFrm
        {
            get { return pIEagleEyeMapFrm; }
            set { pIEagleEyeMapFrm = value; }
        }

        private static MainFrm pIMainFrm;
        public static MainFrm pMainFrm
        {
            get { return pIMainFrm; }
            set { pIMainFrm = value; }
        }

        private static MapFrm pIMapFrm;
        public static MapFrm pMapFrm
        {
            get { return pIMapFrm; }
            set { pIMapFrm = value; }
        }

        private static TOCFrm pITOCFrm;
        public static TOCFrm pTOCFrm
        {
            get { return pITOCFrm; }
            set { pITOCFrm = value; }
        }

        private static bool bICanDrag;
        public static bool bCanDrag
        {
            get { return bICanDrag; }
            set { bICanDrag = value; }
        }

        private static IPoint pIMoveRectPoint;
        public static IPoint pMoveRectPoint
        {
            get { return pIMoveRectPoint; }
            set { pIMoveRectPoint = value; }
        }

        private static AttributeTableFrm pIAttributeTableFrm;
        public static AttributeTableFrm pAttributeTableFrm
        {
            get { return pIAttributeTableFrm; }
            set { pIAttributeTableFrm = value; }
        }

        private static IFeatureLayer pAttributeTableIFeatureLayer;
        public static IFeatureLayer pAttributeTableFeatureLayer
        {
            get { return pAttributeTableIFeatureLayer; }
            set { pAttributeTableIFeatureLayer = value; }
        }

        private static IFeatureWorkspace pIFeatureWorkspace;
        public static IFeatureWorkspace pFeatureWorkspace
        {
            get { return pIFeatureWorkspace; }
            set { pIFeatureWorkspace = value; }
        }

        private static INetworkDataset pINetworkDataset;
        public static INetworkDataset pNetworkDataset
        {
            get { return pINetworkDataset; }
            set { pINetworkDataset = value; }
        }


        private static INAContext pINAContext;
        public static INAContext pNAContext
        {
            get { return pINAContext; }
            set { pINAContext = value; }
        }
       
        private static double[,] pmaterialArrivetime;
        public static double[,] materialArrivetime
        {
            get { return pmaterialArrivetime; }
            set { pmaterialArrivetime = value; }
        }

        private static IElement pElement = new LineElementClass();
        public static IElement PElement
        {
            get { return Variable.pElement; }
            set { Variable.pElement = value; }
        }
    }
}
