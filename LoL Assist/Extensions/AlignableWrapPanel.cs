﻿using System.Windows.Controls;
using System.Windows;
using System;

namespace LoL_Assist_WAPP.Extensions
{ 
    // Source: https://stackoverflow.com/questions/806777/wpf-how-can-i-center-all-items-in-a-wrappanel
    public class AlignableWrapPanel: Panel
    {
        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        public static readonly DependencyProperty HorizontalContentAlignmentProperty =
            DependencyProperty.Register("HorizontalContentAlignment", typeof(HorizontalAlignment), typeof(AlignableWrapPanel), new FrameworkPropertyMetadata(HorizontalAlignment.Left, FrameworkPropertyMetadataOptions.AffectsArrange));

        protected override Size MeasureOverride(Size constraint)
        {
            Size curLineSize = new Size();
            Size panelSize = new Size();

            UIElementCollection children = base.InternalChildren;

            for (int i = 0; i < children.Count; i++)
            {
                UIElement child = children[i] as UIElement;

                // Flow passes its own constraint to children
                child.Measure(constraint);
                Size sz = child.DesiredSize;

                if (curLineSize.Width + sz.Width > constraint.Width) //need to switch to another line
                {
                    panelSize.Width = Math.Max(curLineSize.Width, panelSize.Width);
                    panelSize.Height += curLineSize.Height;
                    curLineSize = sz;

                    if (sz.Width > constraint.Width) // if the element is wider then the constraint - give it a separate line                    
                    {
                        panelSize.Width = Math.Max(sz.Width, panelSize.Width);
                        panelSize.Height += sz.Height;
                        curLineSize = new Size();
                    }
                }
                else //continue to accumulate a line
                {
                    curLineSize.Width += sz.Width;
                    curLineSize.Height = Math.Max(sz.Height, curLineSize.Height);
                }
            }

            // the last line size, if any need to be added
            panelSize.Width = Math.Max(curLineSize.Width, panelSize.Width);
            panelSize.Height += curLineSize.Height;

            return panelSize;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            int firstInLine = 0;
            Size curLineSize = new Size();
            double accumulatedHeight = 0;
            UIElementCollection children = this.InternalChildren;

            for (int i = 0; i < children.Count; i++)
            {
                Size sz = children[i].DesiredSize;

                if (curLineSize.Width + sz.Width > arrangeBounds.Width) //need to switch to another line
                {
                    ArrangeLine(accumulatedHeight, curLineSize, arrangeBounds.Width, firstInLine, i);

                    accumulatedHeight += curLineSize.Height;
                    curLineSize = sz;

                    if (sz.Width > arrangeBounds.Width) //the element is wider then the constraint - give it a separate line                    
                    {
                        ArrangeLine(accumulatedHeight, sz, arrangeBounds.Width, i, ++i);
                        accumulatedHeight += sz.Height;
                        curLineSize = new Size();
                    }
                    firstInLine = i;
                }
                else //continue to accumulate a line
                {
                    curLineSize.Width += sz.Width;
                    curLineSize.Height = Math.Max(sz.Height, curLineSize.Height);
                }
            }

            if (firstInLine < children.Count)
                ArrangeLine(accumulatedHeight, curLineSize, arrangeBounds.Width, firstInLine, children.Count);

            return arrangeBounds;
        }

        private void ArrangeLine(double y, Size lineSize, double boundsWidth, int start, int end)
        {
            double x = 0;
            if (this.HorizontalContentAlignment == HorizontalAlignment.Center)
            {
                x = (boundsWidth - lineSize.Width) / 2;
            }
            else if (this.HorizontalContentAlignment == HorizontalAlignment.Right)
            {
                x = (boundsWidth - lineSize.Width);
            }

            UIElementCollection children = InternalChildren;
            for (int i = start; i < end; i++)
            {
                UIElement child = children[i];
                child.Arrange(new Rect(x, y, child.DesiredSize.Width, lineSize.Height));
                x += child.DesiredSize.Width;
            }
        }
    }
}
