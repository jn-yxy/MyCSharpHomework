﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace Game_2
{
    public partial class 连连看 : Form
    {
        public 连连看()
        {
            InitializeComponent();
        }

        private void 连连看_Load(object sender, EventArgs e)
        {

        }
        static int[] arr1 = new int[98];
        private void InitControl()//生成随机块
        {
            long tick = DateTime.Now.Ticks;
            Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
            //生成成对的数，49对
            for (int i = 0; i < 98; i += 2)
            {
                arr1[i] = ran.Next(1, 20);
                arr1[i + 1] = arr1[i];
            }
            int[] arr2 = new int[98];
            ArrayList list = new ArrayList();
            for (int i = 0; i < 98; i++)
            {
                int number = ran.Next(0, 98);
                while (list.Contains(number))
                {
                    number = ran.Next(0, 98);
                }
                list.Add(number);
            }

            int geshu = 0;
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    
                    Label lb = new Label();
                    lb.Click += new System.EventHandler(Label_Click);
                    lb.Location = new System.Drawing.Point(40 + 50 * j, 80 + 50 * i);
                    lb.Size = new Size(49, 49);//方格大小
                    lb.Text = arr1[(int)list[geshu++]].ToString();
                    lb.Font = new Font("Consolas", 16, FontStyle.Bold);
                    lb.BackColor = Color.White;
                    lb.TextAlign = ContentAlignment.MiddleCenter;
                    this.Controls.Add(lb);
                }
            }
        }


        /// <summary>
        /// 点击相关变量
        /// </summary>
        static private int i = 0;
        static private Label One, Two;
        static int[,] Maze = new int[9, 16];
        static MyPoint KaiShi, JieShu;
        static int jishu = 49;
        private void Label_Click(object sender, EventArgs e)
        {
            Label lb = (Label)sender;
            if (i == 0)//第一次点击
            {
                One = lb;
                lb.BackColor = Color.Yellow;
                KaiShi = new MyPoint((One.Location.X - 40) / 50+1, (One.Location.Y - 80) / 50+1);
                Maze[KaiShi.Y, KaiShi.X] = 0;
                i = 1;
            }
            else if (i == 1)//第二次点击
            {
                Two = lb;
                lb.BackColor = Color.Yellow;
                if (One.Location!=Two.Location)//判断两次不是一个地方
                {
                    if (One.Text.Equals(Two.Text))//判断内容相同
                    {
                        JieShu = new MyPoint((Two.Location.X - 40) / 50 +1, (Two.Location.Y - 80) / 50+1);
                        MyPoint p = new MyPoint(KaiShi.X, KaiShi.Y);
                        p.parent = null;
                        Maze[JieShu.Y, JieShu.X] = 0;
                        if (BFS(p, Maze, JieShu))
                        {
                            One.Visible = false;
                            Two.Visible = false;
                            jishu--;
                        }
                        else
                        {
                            Maze[KaiShi.Y, KaiShi.X] = 1;
                            Maze[JieShu.Y, JieShu.X] = 1;
                            One.BackColor = Color.White;
                            Two.BackColor = Color.White;
                        }
                        
                        for (int i = 0; i < 9; i++)//恢复数组中上次执行改变的值
                        {
                            for (int j = 0; j < 16; j++)
                            {
                                if (Maze[i, j] == -1)
                                {
                                    Maze[i, j] = 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        One.BackColor = Color.White;
                        Two.BackColor = Color.White;
                    }
                }
                else
                {
                    Maze[KaiShi.Y, KaiShi.X] = 1;
                    One.BackColor = Color.White;
                    i = 0;
                }
                i = 0;

                //textBox1.Clear();
                //for (int i = 0; i < 9; i++)
                //{
                //    for (int j = 0; j < 16; j++)
                //    {
                //        textBox1.Text += Maze[i, j].ToString() + "\t";
                //    }
                //    textBox1.Text += "\r\n";
                //}
            }
            
            //lb.Visible = false;
        }

        private void Start_Click(object sender, EventArgs e)
        {
            InitControl();
            One = new Label();
            Two = new Label();
            timer1.Enabled = true;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    Maze[i, j] = 0;
                }
            }
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    Maze[i + 1, j + 1] = 1;
                }
            }
        }
        private void Restart_Click(object sender, EventArgs e)
        {
            foreach (Label lab in this.Controls.OfType<Label>())
            {
                lab.Visible = false;
            }
            label1.Visible = true;
            label2.Visible = true;
            timer1.Enabled = false;
            label2.Text = "time:";
            time = 0;
            jishu = 49;
        }
        int time = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            time++;
            //label2.Text = "time:" + time / 10.0;
            if (jishu==0)
            {
                timer1.Enabled = false;
                MessageBox.Show("你赢了，用时：" + time / 10.0 + "s");
            }
        }

        #region 寻找通路
        /// <summary>
        /// 迷宫程序移植，运用BFS，计算两点是否存在通路
        /// </summary>
        public class MyPoint
        {
            public MyPoint parent { get; set; }
            public int X { get; set; }
            public int Y { get; set; }

            public MyPoint(int a, int b)
            {
                this.X = a;
                this.Y = b;
            }
        }

        static bool BFS(MyPoint p, int[,] data,MyPoint JieShu)
        {
            Queue<MyPoint> q = new Queue<MyPoint>();
            data[0, 0] = -1;
            q.Enqueue(p);
            while (q.Count > 0)
            {
                MyPoint qp = (MyPoint)q.Dequeue();
                for (int i = -1; i < 2; i++) //遍历可以到达的节点
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if ((qp.X + i >= 0) && (qp.X + i < 16) && (qp.Y + j >= 0) && (qp.Y + j < 9) && (qp.X + i == qp.X || qp.Y == qp.Y + j)) //是否越界 只遍历上下左右
                        {
                            if (data[qp.Y + j, qp.X + i] == 0)
                            {
                                if (qp.X + i == JieShu.X && qp.Y + j == JieShu.Y)  //是否为终点
                                {
                                    return true;
                                }
                                else
                                {
                                    MyPoint temp = new MyPoint(qp.X + i, qp.Y + j);   //加入队列
                                    data[qp.Y + j, qp.X + i] = -1;
                                    temp.parent = qp;
                                    q.Enqueue(temp);
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        #endregion
    }
}
