using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polytech.Clustering.Plugin
{
    class DirectionnalSignature : ASignature
    {

        /// <summary>
        /// Va définir le nombre de pixels noirs dans une direction données
        /// </summary>
        List<Double> m_blackPixels = new List<double>();

        /// <summary>
        /// Liste des caractéristiques de la signature normalisées
        /// </summary>
        //List<double> m_normalisedValue = null;

        public override void SignatureSum(ASignature sign2)
        {
            for(int i =0;i<m_blackPixels.Count;i++)
            {
                m_blackPixels[i] += ((DirectionnalSignature) sign2).m_blackPixels[i];
            }
        }

        /// <summary>
        /// "featureToAdd" doit être de type double
        /// </summary>
        /// <param name="featureToAdd"></param>
        public override void AddFeature(object featureToAdd)
        {
            m_blackPixels.Add((double) featureToAdd);
        }

        public override double EuclidianDistance(ASignature signature2)
        {
            double diffSum = 0;
            //calcul de la différence entre chaque feature
            for (int i = 0; i < m_blackPixels.Count; i++)
            {
                //Calcul du carré de la différence des magnitudes
                diffSum += Math.Pow(m_blackPixels[i] - ((DirectionnalSignature) signature2).m_blackPixels[i], 2.0);

            }
            return Math.Sqrt(diffSum);
        }

        public override object Clone()
        {
            //Copie des valeurs de "blackPixels"
            DirectionnalSignature newSign = new DirectionnalSignature();
            foreach (double feature in m_blackPixels)
            {
                newSign.m_blackPixels.Add(feature);
            }
            return newSign;
        }

        protected override ASignature Divide(int divisor)
        {
            //Instanciation de la nouvelle signature
            DirectionnalSignature dividedSignature = new DirectionnalSignature();
            foreach (double feature in m_blackPixels)
            {
                dividedSignature.AddFeature((feature / divisor));
            }
            return dividedSignature;
        }

        public override List<Double> GetNormalisedFeatures()
        {
            if (m_normalisedFeatures == null)
                return m_blackPixels;
            else
                return m_normalisedFeatures;
        }

        public override void Dispose()
        {
            
        }

        public override string GetName()
        {
            return "Caractéristique directionnelle";
        }

        public override List<object> GetFeatures()
        {
            return null;
        }

        public override List<string> ToStringList()
        {
            List<String> strList = new List<String>();
            for (int i = 0; i < this.m_blackPixels.Count; i++)
            {
                string direction = null;
                switch (i)
                {
                    case 0:
                        direction = "Nord :";
                        break;
                    case 1:
                         direction = "Sud :";
                        break;
                    case 2:
                      direction = "Est :";
                         break;
                    case 3:
                      direction = "Ouest :";
                      break;
                    case 4:
                      direction = "Nord-Est :";
                      break;
                    case 5:
                      direction = "Nord-Ouest :";
                      break;
                    case 6:
                      direction = "Sud-Est";
                      break;
                    case 7:
                      direction = "Sud-Ouest";
                      break;
                }
                strList.Add( direction + m_blackPixels[i].ToString());
            }
            return strList;
        }

        protected override ASignature Add(ASignature v1)
        {
            //Instanciation de la nouvelle signature
            DirectionnalSignature newSignature = new DirectionnalSignature();
            for (int i = 0; i < m_blackPixels.Count; i++)//Complex feature in m_moments)
            {
                newSignature.AddFeature(m_blackPixels[i] + ((DirectionnalSignature)v1).m_blackPixels[i]);
            }
            return newSignature;
        }
    }
}