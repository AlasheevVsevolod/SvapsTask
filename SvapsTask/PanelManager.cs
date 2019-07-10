using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SvapsTask.Consts;

namespace SvapsTask
{
    public static class PanelManager
    {
        /// <summary>
        /// Recursively creating all children of given parent node
        /// </summary>
        /// <param name="imgGraphics">Graphic tool used for drawing figures (rectangles)</param>
        /// <param name="parent">Parent node, whose children we'll draw</param>
        /// <param name="parentX">X coordinate of parent panel (Is used to calculate X coordinate of children)</param>
        /// <param name="parentY">Y coordinate of parent panel (Is used to calculate Y coordinate of children)</param>
        /// <param name="parentHeight">Height of parent panel (Is used to calculate Y coordinate of children)</param>
        /// <param name="parentWidth">Width of parent panel (Is used to calculate X coordinate of children)</param>
        /// <param name="rotation">Value, describes if parent panel is rotated</param>
        public static void CreateChildPanels(Graphics imgGraphics, XmlNode parent, int parentX, int parentY, int parentHeight, int parentWidth, int rotation)
        {
            int panelX, panelY, panelWidth, panelHeight, panelOffset, sideToAttach, unrotatedSide;

            foreach (XmlNode childNode in parent.SelectSingleNode(XmlNameConsts.ATTACHED_PANELS_NAME).ChildNodes)
            {
                //Side of parent panel, where child is attached
                sideToAttach = XmlConverter.GetIntValueFromXmlAttr(childNode, XmlNameConsts.ATTACHED_TO_SIDE_NAME);

                //Normalized side number name
                unrotatedSide = AddRotationToSide(sideToAttach, rotation);
                switch (unrotatedSide)
                {
                    //Child is attached to the bottom of parent
                    case 0:
                        panelHeight = XmlConverter.GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_HEIGHT_NAME);
                        panelWidth = XmlConverter.GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_WIDTH_NAME);
                        panelOffset = -XmlConverter.GetIntValueFromXmlAttr(childNode, XmlNameConsts.HINGE_OFFSET_NAME);

                        panelX = parentX;
                        panelY = parentY + panelHeight;

                        DrawPanel(imgGraphics, panelX, panelY, panelHeight, panelWidth, panelOffset);
                        break;

                    //Child is attached to the right side of parent
                    case 1:
                        if (unrotatedSide == 2)
                        {
                            panelHeight = XmlConverter.GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_HEIGHT_NAME);
                            panelWidth = XmlConverter.GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_WIDTH_NAME);
                        }
                        else
                        {
                            panelHeight = XmlConverter.GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_WIDTH_NAME);
                            panelWidth = XmlConverter.GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_HEIGHT_NAME);
                        }
                        panelOffset = XmlConverter.GetIntValueFromXmlAttr(childNode, XmlNameConsts.HINGE_OFFSET_NAME);

                        panelX = parentX + (parentWidth / 2) + (panelWidth / 2);
                        panelY = parentY + panelOffset;

                        DrawPanel(imgGraphics, panelX, panelY, panelHeight, panelWidth, 0);
                        break;

                    //Child is attached to the top of parent
                    case 2:
                        panelHeight = XmlConverter.GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_HEIGHT_NAME);
                        panelWidth = XmlConverter.GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_WIDTH_NAME);
                        panelOffset = XmlConverter.GetIntValueFromXmlAttr(childNode, XmlNameConsts.HINGE_OFFSET_NAME);

                        panelX = parentX;
                        panelY = parentY - parentHeight;

                        DrawPanel(imgGraphics, panelX, panelY, panelHeight, panelWidth, panelOffset);
                        break;

                    //Child is attached to the left side of parent
                    default:
                        if (unrotatedSide == 2)
                        {
                            panelHeight = XmlConverter.GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_HEIGHT_NAME);
                            panelWidth = XmlConverter.GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_WIDTH_NAME);
                        }
                        else
                        {
                            panelHeight = XmlConverter.GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_WIDTH_NAME);
                            panelWidth = XmlConverter.GetIntValueFromXmlAttr(childNode, XmlNameConsts.PANEL_HEIGHT_NAME);
                        }
                        panelOffset = XmlConverter.GetIntValueFromXmlAttr(childNode, XmlNameConsts.HINGE_OFFSET_NAME);

                        panelX = parentX - (parentWidth / 2) - (panelWidth / 2);
                        panelY = parentY - panelOffset;

                        DrawPanel(imgGraphics, panelX, panelY, panelHeight, panelWidth, 0);
                        break;
                }

                //If current child has its own children, we recursively going through all of them
                //(according to this child rotation)
                if (childNode.SelectSingleNode(XmlNameConsts.ATTACHED_PANELS_NAME) != null)
                {
                    int newRotation = 0;
                    switch (sideToAttach)
                    {
                        case 0:
                            newRotation += 2;
                            break;
                        case 1:
                            newRotation++;
                            break;
                        case 2:
                            break;
                        default:
                            newRotation--;
                            break;
                    }

                    //(rotation + newRotation) is used to track rotated child of rotated parent
                    CreateChildPanels(imgGraphics, childNode, panelX, panelY, panelHeight, panelWidth, rotation + newRotation);
                }
            }
        }

        /// <summary>
        /// Change side number name according to rotation value
        /// </summary>
        /// <param name="side">Side number name</param>
        /// <param name="rotation">Rotation (>0 - clockwise, less than 0 - counter-clockwise, 0 - unchanged)</param>
        /// <returns>Normalized side number</returns>
        public static int AddRotationToSide(int side, int rotation)
        {
            /*I have decided to "normalize" panels. What do I mean?
             *By default number name of sides is, like, "rotating". Lets assume we have root panel.
             * It'll have sides (from bottom counter-clockwise)
             *
             * parent
             * bot/right/top/left
             * 0 1 2 3
             *
             * When we attaching new panel to the right side of root panel, it'll have sides
             *
             * child
             * bot/right/top/left
             * 1 2 3 0
             *
             * If we'll attach another panel to the child on the right, brand new panel will have
             * "attachedSide" tag == 2, which is kind of strange for me.
             *
             * So I think it'll be easier, if sides wouldn't "rotate". In this case all new panels with
             * "attachedSide" tag == 2 would be placed on the top of their parents. So we need to
             * "normalize" side names.
             * Procedure is simple: we take in account rotating value. If new panel is attached to the
             * right side (rotation += 1) of its parent, we need to think of this child's sides as
             * "decreased" version of what they really are. So child from upper example will have following sides
             *
             * parent
             * bot/right/top/left
             * 0 1 2 3
             *
             * child on the right side of parent
             * bot/right/top/left
             * 0 1 2 3
             *
             * Same logic is for left sides, we get (rotation -= 1) so sides need to be increased
             */
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

        /// <summary>
        /// Draw panel based on given values
        /// </summary>
        /// <param name="imgGraphics">Graphic tool used for drawing figures (rectangles)</param>
        /// <param name="x">X coordinate of panel (middle of bottom side)</param>
        /// <param name="y">Y coordinate of panel (bottom side)</param>
        /// <param name="height">Height of rectangle</param>
        /// <param name="width">Width of rectangle</param>
        /// <param name="offset">Offset to change base x coordinate</param>
        public static void DrawPanel(Graphics imgGraphics, int x, int y, int height, int width, int offset)
        {
            //(x - (width / 2)) is used because default X coordinate is located on the bottom of a panel
            //(y - height) minus is used because by default image has reversed Y axis and Y coordinate
            //also located on the bottom of a panel
            //We draw rectangles from the top left side
            imgGraphics.DrawRectangle(new Pen(Brushes.Black, 3), (x - (width / 2)) + offset, y - height, width, height);
        }
    }
}
