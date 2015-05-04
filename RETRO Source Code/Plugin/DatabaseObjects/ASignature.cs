using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polytech.Clustering.Plugin
{
    /// <summary>
    /// Abstract Class that allows to store a vector of features represanting a Pattern (for exemple Zernike coefficiants). 
    /// </summary>
    [Serializable] 
    public abstract class ASignature : ICloneable, IDisposable
    {
        /// <summary>
        /// Surcharge of "/". Method should be implemented in the final signature class 
        /// </summary>
        /// <param name="divisor"></param>
        /// <returns></returns>
        protected abstract ASignature Divide(int divisor);
        /// <summary>
        /// Surcharge of operator "diviser". Division of the features by an integer
        /// </summary>
        /// <param name="v1">signature to divide</param>
        /// <param name="divisor">divisor</param>
        /// <returns>resulted signature</returns>
        public static ASignature operator /(ASignature v1, int divisor)
        {
            return v1.Divide(divisor);
        }

        /// <summary>
        /// Surcharge of  "addition". To add 2 signatures.
        /// </summary>       
        protected abstract ASignature Add(ASignature v1);
        
        /// <summary>
        /// Surcharge of  "addition". To add 2 signature.
        /// </summary>
        /// <param name="v1">Fisrt signature signature</param>
        /// <param name="toAdd">Second signature</param>
        /// <returns>resulting Signature </returns>
        public static ASignature operator +(ASignature v1, ASignature toAdd)
        {
            return v1.Add(toAdd);
        }

        /// <summary>
        /// Normalised values of the features (to be computed by the user before use)
        /// </summary>
        protected List<double> m_normalisedFeatures = null;

        /// <summary>
        /// Abstract method to get the normalized values of the features (they need to have been computed before).
        /// </summary>
        /// <returns>List of features as double</returns>
        public abstract List<double> GetNormalisedFeatures();

        /// <summary>
        /// Abstract method to Get the initial values of the features (not normalized)
        /// </summary>
        /// <returns>The list of features</returns>
        public abstract List<object> GetFeatures();

        /// <summary>
        /// Abstract method to add the features of a signature to the current values
        /// </summary>
        /// <param name="sign2">Signature to use for addition</param>
        public abstract void SignatureSum(ASignature sign2);

        /// <summary>
        /// Should be Abstract like Sum : Not implemented yet
        /// </summary>
        public void SignatureSub(ASignature aSignature)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Abstract method to add one more feature to the signature
        /// </summary>
        /// <param name="toAdd">Feature to add</param>
        public abstract void AddFeature(object toAdd);

        /// <summary>
        /// Abstract method to compute euclidian distance between 2 signatures
        /// </summary>
        /// <param name="signature2">Second signature to consider</param>
        /// <returns>Value of the distance</returns>
        public abstract double EuclidianDistance(ASignature signature2);

        /// <summary>
        /// Abstract method to get the name of the signature (Zernike" for exemple) signatures
        /// </summary>
        public abstract string GetName();

        /// <summary>
        /// Abstract method to clone a signature
        /// </summary>
        /// <returns>New signature</returns>
        public abstract object Clone();

        /// <summary>
        /// Abstract method to free memory
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Abstract method to get information about a signature
        /// </summary>
        /// <returns>List of strings providing info about the signature (values of the features for exemple)</returns>
         public abstract List<String> ToStringList();

        /// <summary>
        /// Updating the normalized values of the signature
        /// </summary>
        /// <param name="normalisedFeatures">List of normalized features for this signature</param>
         public void SetNormalisedFeatures(List<double> normalisedFeatures)
         {
             m_normalisedFeatures = normalisedFeatures;
         }

    }
}
