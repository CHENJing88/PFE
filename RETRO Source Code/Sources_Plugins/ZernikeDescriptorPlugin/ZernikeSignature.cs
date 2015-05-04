using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Polytech.Clustering.Plugin
{
    [Serializable]
    class ZernikeSignature : ASignature
    {
        /// <summary>
        /// Liste de nombres complexes correspondant aux moments de zernke calculés
        /// </summary>
        private List<Complex> m_moments = new List<Complex>();

        //private List<double> m_normalisedValue = null;

        public ZernikeSignature()
        {
        }
        
        public override void SignatureSum(ASignature sign2)
        {
            for (int i = 0; i < m_moments.Count; i++)
            {
                m_moments[i] += ((ZernikeSignature)sign2).m_moments[i];
            }
        }

        public override double EuclidianDistance(ASignature signature2)
        {
           // double distanceSum = 0.0;
            double diffSum = 0;
          //  double diffSum2 = 0;
            //calcul de la différence entre chaque feature
            for (int i = 0; i < m_moments.Count; i++)
            {
                //Calcul du carré de la différence des magnitudes
               diffSum += Math.Pow(m_moments[i].Magnitude - ((ZernikeSignature) signature2).m_moments[i].Magnitude , 2.0);
               //diffSum2 += Math.Pow( (((ZernikeSignature) signature2).m_moments[i].Phase /  ((ZernikeSignature) signature2).m_moments[i].Magnitude )  - (m_moments[i].Phase/ m_moments[i].Magnitude), 2);
            }
            return Math.Sqrt(diffSum);
        }

 
        public override object Clone()
        {
            // Copie de chaque moment
            ZernikeSignature newSign = new ZernikeSignature();
            foreach (Complex feature in m_moments)
            {
                Complex newComplex = new Complex(feature.Real, feature.Imaginary);
                newSign.m_moments.Add(newComplex);
            }
            return newSign;
        }


        public override void AddFeature(object featureToAdd)
        {
            m_moments.Add((Complex)featureToAdd);
        }

        protected override ASignature Divide(int divisor)
        {   
            //Instanciation de la nouvelle signature
            ZernikeSignature dividedSignature = new ZernikeSignature();
            foreach(Complex feature in m_moments)
            {
                dividedSignature.AddFeature( (feature / divisor) );
            }
            return dividedSignature;
        }

        public override List<double> GetNormalisedFeatures()
        {
            //on teste si la signature a été normalisée
            if (m_normalisedFeatures == null)
            {
                //Si non normalisé - On ne renvoie que les magnitudes
                List<double> listToReturn = new List<double>();
                foreach (Complex feature in m_moments)
                {
                    listToReturn.Add(feature.Magnitude);
                }

                return listToReturn;
            }
            return m_normalisedFeatures;
        }

        public override void Dispose()
        {
            // Suppression des données de la signature
            m_moments = null;
        }

        public override string GetName()
        {
            return "Zernike";
        }

        public override List<String> ToStringList()
        {
            List<String> strList = new List<String>();
            for (int i = 0; i < m_moments.Count; i++)
            {
                strList.Add(m_moments[i].ToString());
            }
            return strList;
        }

        public override List<object> GetFeatures()
        {
            return null;
        }

        protected override ASignature Add(ASignature v1)
        {
            //Instanciation de la nouvelle signature
            ZernikeSignature newSignature= new ZernikeSignature();
            for (int i = 0; i < m_moments.Count;i++ )//Complex feature in m_moments)
            {
                newSignature.AddFeature(m_moments[i] + ((ZernikeSignature)v1).m_moments[i]);
                //dividedSignature.AddFeature((feature / divisor));
            }
            return newSignature;
        }
    }
}
