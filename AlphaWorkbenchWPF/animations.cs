using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace AlphaWorkbenchWPF
{
    class animations : Window
    {
        public void anim_width(ListBox temp, int width)
        {
            DoubleAnimation animate = new DoubleAnimation(0, width, new Duration(TimeSpan.FromSeconds(0.2)));
            temp.BeginAnimation(WidthProperty, animate);
            return;
        }
        public void anim_width(StackPanel temp, int width)
        {
            DoubleAnimation animate = new DoubleAnimation(0, width, new Duration(TimeSpan.FromSeconds(0.2)));
            temp.BeginAnimation(WidthProperty, animate);
            return;
        }
        public void anim_width(StackPanel temp, int width,double time)
        {
            DoubleAnimation animate = new DoubleAnimation(0, width, new Duration(TimeSpan.FromSeconds(time)));
            temp.BeginAnimation(WidthProperty, animate);
            return;
        }

        public void anim_width(ListBox temp, int initial, int final)
        {
            DoubleAnimation animate = new DoubleAnimation(initial, final, new Duration(TimeSpan.FromSeconds(0.2)));
            temp.BeginAnimation(WidthProperty, animate);
            return;
        }
        public void anim_width(StackPanel temp, int initial, int final)
        {
            DoubleAnimation animate = new DoubleAnimation(initial, final, new Duration(TimeSpan.FromSeconds(0.2)));
            temp.BeginAnimation(WidthProperty, animate);
            return;
        }
        public void anim_width(StackPanel temp, int initial, int final, double time)
        {
            DoubleAnimation animate = new DoubleAnimation(initial, final, new Duration(TimeSpan.FromSeconds(time)));
            temp.BeginAnimation(WidthProperty, animate);
            return;
        }
        public void anim_width(Grid temp, double initial, double final, double time)
        {
            DoubleAnimation animate = new DoubleAnimation(initial, final, new Duration(TimeSpan.FromSeconds(time)));
            temp.BeginAnimation(WidthProperty, animate);
            return;
        }
        async public void anim_height(ListBox temp, int height, double time)
        {
            DoubleAnimation animate = new DoubleAnimation(height, new Duration(TimeSpan.FromSeconds(time)));
            temp.BeginAnimation(HeightProperty, animate);
        }
    }
}
