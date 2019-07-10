using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SvapsTask.Consts;


namespace SvapsTask
{
    class Program
    {
        /// <summary>
        /// Parse XML file and create 2d representation of given asset
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string rootDirPath = $"{Environment.CurrentDirectory}/../../";
            string assetsPath = $"{rootDirPath}/assets/";
            string representationsPath = $"{rootDirPath}/representations/";
            string assertFileName;

            int imgHeight, imgWidth, panelX, panelY, panelWidth, panelHeight, panelOffset, rotation = 0;

            Console.Write("Input name of XML file you need to process: ");
            assertFileName = Console.ReadLine();
            string assetFilePath = $"{assetsPath}/{assertFileName}.xml";

            //Check if file exists
            if (!File.Exists(assetFilePath))
            {
                Console.WriteLine("File doesn't exists");
                Console.Read();
                return;
            }

            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(assetFilePath);

            //Set image size based on attributes of "folding" node
            XmlNode rootNode = xmlFile.SelectSingleNode(XmlNameConsts.FOLDING_NAME);
            imgHeight = Convert.ToInt32(rootNode.Attributes.GetNamedItem(XmlNameConsts.IMG_HEIGHT_NAME).Value);
            imgWidth = Convert.ToInt32(rootNode.Attributes.GetNamedItem(XmlNameConsts.IMG_WIDTH_NAME).Value);

            using (Bitmap image = new Bitmap(imgWidth, imgHeight))
            {
                using (Graphics imgGraphics = Graphics.FromImage(image))
                {
                    //Fill image with default pixels
                    imgGraphics.FillRectangle(Brushes.White, 0, 0, imgWidth, imgHeight);

                    //Get values to draw root node
                    panelX = XmlConverter.GetIntValueFromXmlAttr(rootNode, XmlNameConsts.ROOT_X_COORDINATE_NAME);
                    panelY = XmlConverter.GetIntValueFromXmlAttr(rootNode, XmlNameConsts.ROOT_Y_COORDINATE_NAME);

                    rootNode = rootNode.SelectSingleNode(XmlNameConsts.PANELS_NAME).SelectSingleNode(XmlNameConsts.ITEM_NAME);
                    panelHeight = XmlConverter.GetIntValueFromXmlAttr(rootNode, XmlNameConsts.PANEL_HEIGHT_NAME);
                    panelWidth = XmlConverter.GetIntValueFromXmlAttr(rootNode, XmlNameConsts.PANEL_WIDTH_NAME);
                    panelOffset = XmlConverter.GetIntValueFromXmlAttr(rootNode, XmlNameConsts.HINGE_OFFSET_NAME);

                    //Draw root node
                    PanelManager.DrawPanel(imgGraphics, panelX, panelY, panelHeight, panelWidth, panelOffset);

                    //Create all other nodes recursively
                    PanelManager.CreateChildPanels(imgGraphics, rootNode, panelX, panelY, panelHeight, panelWidth, rotation);
                }

                //If image with asset name already exists - delete and create new
                if (File.Exists($"{representationsPath}/{assertFileName}.jpg"))
                {
                    File.Delete($"{representationsPath}/{assertFileName}.jpg");
                }
                image.Save($"{representationsPath}/{assertFileName}.jpg");
            }

            Console.WriteLine("Representation created");
            Console.Read();
        }
    }
}
