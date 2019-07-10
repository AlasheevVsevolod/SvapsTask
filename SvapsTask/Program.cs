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

            if (!File.Exists(assetFilePath))
            {
                Console.WriteLine("File doesn't exists");
                Console.Read();
                return;
            }

            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(assetFilePath);

            XmlNode rootNode = xmlFile.SelectSingleNode(XmlNameConsts.FOLDING_NAME);
            imgHeight = Convert.ToInt32(rootNode.Attributes.GetNamedItem(XmlNameConsts.IMG_HEIGHT_NAME).Value);
            imgWidth = Convert.ToInt32(rootNode.Attributes.GetNamedItem(XmlNameConsts.IMG_WIDTH_NAME).Value);

            using (Bitmap image = new Bitmap(imgWidth, imgHeight))
            {
                using (Graphics imgGraphics = Graphics.FromImage(image))
                {
                    imgGraphics.FillRectangle(Brushes.White, 0, 0, imgWidth, imgHeight);

                    panelX = GetIntValueFromXmlAttr(rootNode, XmlNameConsts.ROOT_X_COORDINATE_NAME);
                    panelY = GetIntValueFromXmlAttr(rootNode, XmlNameConsts.ROOT_Y_COORDINATE_NAME);

                    rootNode = rootNode.SelectSingleNode(XmlNameConsts.PANELS_NAME).SelectSingleNode(XmlNameConsts.ITEM_NAME);
                    panelHeight = GetIntValueFromXmlAttr(rootNode, XmlNameConsts.PANEL_HEIGHT_NAME);
                    panelWidth = GetIntValueFromXmlAttr(rootNode, XmlNameConsts.PANEL_WIDTH_NAME);
                    panelOffset = GetIntValueFromXmlAttr(rootNode, XmlNameConsts.HINGE_OFFSET_NAME);

                    DrawPanel(imgGraphics, panelX, panelY, panelHeight, panelWidth, panelOffset);

                    CreateChildPanels(imgGraphics, rootNode, panelX, panelY, panelHeight, panelWidth, rotation);
                }



                if (File.Exists($"{representationsPath}/{assertFileName}.jpg"))
                {
                    File.Delete($"{representationsPath}/{assertFileName}.jpg");
                }
                image.Save($"{representationsPath}/{assertFileName}.jpg");
            }

            Console.WriteLine("Representation created");
            Console.Read();
        }


        public static int GetIntValueFromXmlAttr(XmlNode node, string attrName)
        {
            string attrStrValue = node.Attributes.GetNamedItem(attrName).Value.Split(new char[] { '.' }, 2).First();
            return Convert.ToInt32(attrStrValue);
        }


        public static void CreateChildPanels(Graphics imgGraphics, XmlNode parent, int parentX, int parentY, int parentHeight, int parentWidth, int rotation)
        {
            int panelX = 0, panelY = 0, panelWidth, panelHeight, panelOffset, sideToAttach, unrotatedSide;

            foreach (XmlNode childNode in parent.SelectSingleNode(XmlNameConsts.ATTACHED_PANELS_NAME).ChildNodes)
            {
                sideToAttach = GetIntValueFromXmlAttr(childNode, XmlNameConsts.ATTACHED_TO_SIDE_NAME);
                unrotatedSide = AddRotationToSide(sideToAttach, rotation);
                switch (unrotatedSide)
                {
                    case 0:
                        panelHeight = GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_HEIGHT_NAME);
                        panelWidth = GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_WIDTH_NAME);
                        panelOffset = -GetIntValueFromXmlAttr(childNode, XmlNameConsts.HINGE_OFFSET_NAME);

                        panelX = parentX;
                        panelY = parentY + panelHeight;

                        DrawPanel(imgGraphics, panelX, panelY, panelHeight, panelWidth, panelOffset);
                        break;
                    case 1:
                        if (unrotatedSide == 2)
                        {
                            panelHeight = GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_HEIGHT_NAME);
                            panelWidth = GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_WIDTH_NAME);
                        }
                        else
                        {
                            panelHeight = GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_WIDTH_NAME);
                            panelWidth = GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_HEIGHT_NAME);
                        }
                        panelOffset = GetIntValueFromXmlAttr(childNode, XmlNameConsts.HINGE_OFFSET_NAME);

                        panelX = parentX + (parentWidth / 2) + (panelWidth / 2);
                        panelY = parentY + panelOffset;

                        DrawPanel(imgGraphics, panelX, panelY, panelHeight, panelWidth, 0);
                        break;
                    case 2:
                        panelHeight = GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_HEIGHT_NAME);
                        panelWidth = GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_WIDTH_NAME);
                        panelOffset = GetIntValueFromXmlAttr(childNode, XmlNameConsts.HINGE_OFFSET_NAME);

                        panelX = parentX;
                        panelY = parentY - parentHeight;

                        DrawPanel(imgGraphics, panelX, panelY, panelHeight, panelWidth, panelOffset);
                        break;
                    default:
                        if (unrotatedSide == 2)
                        {
                            panelHeight = GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_HEIGHT_NAME);
                            panelWidth = GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_WIDTH_NAME);
                        }
                        else
                        {
                            panelHeight = GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_WIDTH_NAME);
                            panelWidth = GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_HEIGHT_NAME);
                        }
                        panelOffset = GetIntValueFromXmlAttr(childNode, XmlNameConsts.HINGE_OFFSET_NAME);

                        panelX = parentX - (parentWidth / 2) - (panelWidth / 2);
                        panelY = parentY - panelOffset;

                        DrawPanel(imgGraphics, panelX, panelY, panelHeight, panelWidth, 0);
                        break;
                }

                if (childNode.SelectSingleNode(XmlNameConsts.ATTACHED_PANELS_NAME) != null)
                {
                    int tmpRotation = 0;
                    switch (sideToAttach)
                    {
                        case 0:
                            tmpRotation += 2;
                            break;
                        case 1:
                            tmpRotation++;
                            break;
                        case 2:
                            break;
                        default:
                            tmpRotation--;
                            break;
                    }

                    CreateChildPanels(imgGraphics, childNode, panelX, panelY, panelHeight, panelWidth, rotation + tmpRotation);
                }
            }
        }


        public static int AddRotationToSide(int side, int rotation)
        {
            if (rotation < 0)
            {
                for (int i = 0; i > rotation; i--)
                {
                    side = ++side % 4;
                }
            }
            else if (rotation > 0)
            {
                for (int i = 0; i < rotation; i++)
                {
                    if (side == 0)
                    {
                        side = 4;
                    }

                    side--;
                }
            }

            return side;
        }


        public static void DrawPanel(Graphics imgGraphics, int x, int y, int height, int width, int offset)
        {
            imgGraphics.DrawRectangle(new Pen(Brushes.Black, 3), (x - (width / 2)) + offset, y - height, width, height);
        }
    }
}
