using AForge.Imaging.Filters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Polytech.Clustering.Plugin
{
    public class AltoReaderPlugin : IDocumentReaderPlugin
    {
        Database m_database = null;

        /// <summary>
        /// Référence vers la classe encapsulant la configuration du plugin (NOn implementé pour l'instant)
        /// </summary>
        IConfig m_config = null;


        public Database LoadDatabase(List<IDescriptorPlugin> descriptors, string path, Form mainWindow, System.Delegate changeState)
        {
            //CAlcul de perf
            Stopwatch newWatch = new Stopwatch();
            newWatch.Start();

            m_database = new Database();

            //lecture des fichiers Agora comprenant les informations sur les documents à charger
            //récupération des fichiers xml contenus dans le dossier pointé par "path"
            string[] xmlPaths = Directory.GetFiles(path + "\\alto", "*.xml");
            string fileName = null; //permet de stocker le nom du fichier

            //list de fichiers xml (représente chaque document)
            List<XmlDocument> xmlList = new List<XmlDocument>();
            //pour chaque fichier, on charge les infos relatives à chaque caractère
            bool parallelExecution = true;
            if (parallelExecution)
            {
                //Instanciation de la liste des document chargés (BlockingCollection est thread safe)
                BlockingCollection<Document> documents = new BlockingCollection<Document>();
                Task loadDbTask = Task.Factory.StartNew(() =>
                {
                    Parallel.ForEach(xmlPaths, xmlPath =>
                    {
                        //récupération du nom du fichier
                        string fName = Path.GetFileName(xmlPath);
                        fName = Path.ChangeExtension(fName, null); //nom sans extension

                        //création d'un nouveau document
                        Document newDoc = new Document(xmlPath);
                        documents.Add(newDoc); //ajout du document à la liste thread safe

                        XmlDocument xmlDocument = new XmlDocument();
                        xmlList.Add(xmlDocument);

                        //récupération du nom du document
                        fileName = Path.GetFileNameWithoutExtension(xmlPath);

                        xmlDocument.Load(xmlPath);//chargement du fichier xml

                        //récupération des entrées de type string
                        XmlNodeList stringList = xmlDocument.GetElementsByTagName("alto:String");

                        string[] imgPaths = Directory.GetFiles(path + "\\images\\", fName + ".*");
                        //ouverture de l'image du document en bitmap
                        Bitmap imageSource = (Bitmap)Bitmap.FromFile(imgPaths[0]);

                        //Rognage de l'image afin de récupérer chaque vignette
                        Crop filterCrop;
                        int hpos, vpos, width, height;
                        string imageName;
                        Bitmap imageThumbnail; //référence vers les images rognées

                        int index = 0;
                        foreach (XmlNode node in stringList) //pour chaque TAG string
                        {
                            // Rognage de l'image
                            //récupération des rectangles englobants
                            hpos = Convert.ToInt32(node.Attributes["HPOS"].Value);
                            vpos = Convert.ToInt32(node.Attributes["VPOS"].Value);
                            width = Convert.ToInt32(node.Attributes["WIDTH"].Value);
                            height = Convert.ToInt32(node.Attributes["HEIGHT"].Value);
                            imageName = node.Attributes["ID"].Value;

                            //application du rognage
                            filterCrop = new Crop(new Rectangle(hpos, vpos, width, height));

                            imageThumbnail = filterCrop.Apply(imageSource); //récupération du morceau d'image correspondnant au caractère considéré
                            //  imageThumbnail.Save(@"F:\workspace\visual\thumbail" + imageName);
                            //Création d'un nouveau pattern
                            ShapeEoC newPattern = null;
                            newPattern = new ShapeEoC(imageName, imageName, hpos, vpos, width, height, imgPaths[0]);

                            //Calcul des différentes signatures en fonction des plugins disponibles
                            foreach (IDescriptorPlugin plugin in descriptors)
                            {
                                plugin.CalculateSignature(newPattern);
                            }

                            // ajout du pattern à la base de données + màj des infos de normalisation
                            newDoc.AddPattern(newPattern);
                           // m_database.UpdateNormalizationData(newPattern);

                            newPattern.Dispose(); //libération de la mémoire occupée par l'image

                            index++;
                            mainWindow.Invoke(changeState/*new SetToolStripStateDelegate(m_mainWindow.ChangeToolstripState) */, new object[] { true, "Calcul des signatures - A traiter : " + (xmlPaths.Length - documents.Count) });
                        }
                    });
                    //complétion de l'ajout des documents à la liste
                    documents.CompleteAdding();
                });

                loadDbTask.Wait();

                //on ajoute les documents à la base de données
                foreach (Document doc in documents)
                {
                    m_database.AddDocument(doc);
                }
            }
            else
            {
                //TODO, version non parallélisée
            }
            newWatch.Stop();

            System.IO.StreamWriter file = new System.IO.StreamWriter(path+"\\time.txt");
            file.WriteLine("DATABASE : " + newWatch.Elapsed + "--");
            file.Close();


            return m_database;
        }

        public TimeSpan GetProcessingTime()
        {
            throw new NotImplementedException();
        }

        public Database LoadDatabase(List<IDescriptorPlugin> descriptors, string path)
        {
            return null;
        }

        public Database GetLoadedDatabase()
        {
            return m_database;
        }

        public string GetAuthor()
        {
            return "Ludo";
        }

        Form IDocumentReaderPlugin.GetConfigWindow()
        {
            return null;
        }

        public IConfig GetConfig()
        {
            return m_config;
        }

        public string GetName()
        {
            return "Alto + Images";
        }
    }
}
