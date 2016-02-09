﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PSO_FCM.Logic.FCM;
using PSO_FCM.Logic.PSO;
using PSO_FCM.Utility;

namespace PSO_FCM
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var data = Loader.LoadData();
            MessageBox.Show(data[0].CluseterName);
            double w = 1;//GeneralCom.GetRandom(0.1, 0.9);
            double c1 = 2;//GeneralCom.GetRandom(0.1, 0.9);
            double c2 = 2;//GeneralCom.GetRandom(0.1, 0.9);
            double rate = Math.Pow(10, -30);
            int c = 3;
            int m = 2;
            int n = data.Count;
            double[] MaxD = new double[data[0].DataDim.Val.Length];
            for (int i = 0; i < data[0].DataDim.Val.Length; i++)
            {
                for (int j = 0; j < data.Count; j++)
                {
                    MaxD[i] = Math.Max(data[j].DataDim.Val[i], MaxD[i]);
                }
            }
            Pso ps = new Pso(c, n, m, w, c1, c2, 30, data[0].DataDim.Val.Length, data,MaxD);
            File.WriteAllText("log", "");
            for (int i = 0; i < 200; i++)
            {
                ps.Calc();
                File.AppendAllText("log",ps.GloablBestError+"\t;\t"+ps.Variance+"\t\n");
                if(ps.Variance<rate)
                    break;
            }

            File.WriteAllText("datares", "");
            for (int i = 0; i < ps.N; i++)
            {
                for (int j = 0; j < ps.C; j++)
                {
                    File.AppendAllText("datares", ps.U[i, j].ToString(CultureInfo.InvariantCulture) + ";");
                }
                File.AppendAllText("datares", "\t\n");
            }
            Fcm fc=new Fcm(c,n,m,30,data[0].DataDim.Val.Length,data,ps.U);
            
            File.WriteAllText("datafcm", "");
            for (int a = 0; a < 100; a++)
            {
                fc.CalcCenter();
                fc.CalcU();
                for (int i = 0; i < ps.N; i++)
                {
                    for (int j = 0; j < ps.C; j++)
                    {
                        File.AppendAllText("datafcm", fc.U[i, j].ToString(CultureInfo.InvariantCulture) + ";");
                    }
                    File.AppendAllText("datafcm", "\t\n");
                }
                File.AppendAllText("datafcm", "\t\n--\t\n");
            }

        }
    }
}
