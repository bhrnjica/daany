using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using OxyPlot.WindowsForms;

namespace Daany.Plot
{
    public class ChartComponent
    {
       
        public static void Show(string wndTitlle, PlotModel model)
        {
            // export using the instance methods
            using (var stream = new MemoryStream())
            {
                var strPath = $"{Guid.NewGuid().ToString()}.svg";
                var strFull = Path.Combine(Directory.GetCurrentDirectory(), strPath);
                var jpegExporter = new OxyPlot.SvgExporter();
                jpegExporter.Export(model, stream);
                System.IO.File.WriteAllBytes(strFull, stream.ToArray());
                Process.Start(@"cmd.exe ", @"/c " + strFull);
            }
        }
        public static Task ShowPlots(string wndTitlle, params PlotModel[] models)
        {
            Task task = Task.Run(() =>
            {
                //
                var frm = new Form();
                // 
                // tableLayoutPanel1
                // 
                var tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
                tableLayoutPanel1.SuspendLayout();
                frm.SuspendLayout();

                tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
                tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
                tableLayoutPanel1.Name = "tableLayoutPanel1";


                //column/row
                tableLayoutPanel1.ColumnCount = 1;
                tableLayoutPanel1.ColumnStyles.Add(
                    new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));

                tableLayoutPanel1.RowCount = models.Length;
                for (int i = 0; i < models.Length; i++)
                    tableLayoutPanel1.RowStyles.Add(
                        new System.Windows.Forms.RowStyle(SizeType.Percent, (float)(1.0 / (float)models.Length)));


                for (int i = 0; i < models.Length; i++)
                    tableLayoutPanel1.Controls.Add(getplotView(models[i]), 0, i);

                tableLayoutPanel1.Size = new System.Drawing.Size(0, 0);
                tableLayoutPanel1.TabIndex = 0;


                frm.Size = new System.Drawing.Size(950, 600);
                frm.WindowState = FormWindowState.Normal;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Text = wndTitlle;
                tableLayoutPanel1.ResumeLayout(false);
                frm.Controls.Add(tableLayoutPanel1);
                frm.ResumeLayout(false);
                frm.ShowDialog();


            });
            return task;

            PlotView getplotView(PlotModel model)
            {
                var plot1 = new PlotView();
                // 
                // plot1
                // 
                plot1.Dock = System.Windows.Forms.DockStyle.Fill;
                plot1.Location = new System.Drawing.Point(0, 0);
                plot1.Name = "plot1";
                plot1.PanCursor = System.Windows.Forms.Cursors.Hand;
                //plot1.Size = new System.Drawing.Size(1219, 688);
                plot1.TabIndex = 1;
                plot1.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
                plot1.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
                plot1.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
                plot1.Model = model;
                //plot1.Show();
                return plot1;
            }
        }
    }
}
