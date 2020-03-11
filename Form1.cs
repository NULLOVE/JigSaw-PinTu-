using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

namespace PinTu
{
    /// <summary>
    /// Form1 的摘要说明。
    /// </summary>
    /// 
   
	public class Form1 : System.Windows.Forms.Form
	{		
		private IContainer components;
        private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem6;

        private MenuItem menuItem4;
        private MenuItem menuItem7;
        private MenuItem menuItem8;

        private static int[,] A;    //棋盘数组

        private static AButton[] mButton;      //图片盒子
       
        private string FName;              //需要在过程中传递的图片名称。

        private static int TotalStep;
        //关键类 点击的方块
    public class AButton : PictureBox    //继承自PictureBox
    {
        private int Pn,W,Num;    //图片的顺序号（从左到右，从上到下），宽度（像素），N*N的矩阵
        public AButton(int p,int n,Image img)
        {     //在画中的顺序号，维度，图像
            this.Pn = p;
            this.W = 600 / n;
            this.Num = n;
            this.Size = new Size(this.W,this.W);
            this.Image = img;          
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Click += new EventHandler(AButton_Click);
        }
      
        private    int GetN()
            {
                return this.Num ;
            }
        private void AButton_Click(object sender, System.EventArgs e)
        {
            int r, c, d;  //行，列   200*200像素
            int dd=this.Num;    //维度;
            int ww = this.W;  //宽度
            r= this.Top /ww;     //对应数组中的行索引
            c = this.Left /ww;     //对应数组中的列索引
            d = Direction(r, c,dd);    //确定能往哪个方向移动
            switch (d)
            {
                case 0:   //left
                    this.Left = this.Left - ww;    //向左移动
                    SetA(r, c - 1, GetA(r, c,dd),dd);      //更新数组
                    SetA(r, c, 0,dd);                        //更新数组
                        TotalStep += 1;
                    break;
                case 1:   //up
                    this.Top = this.Top - ww;
                    SetA(r - 1, c, GetA(r, c,dd),dd);
                    SetA(r, c, 0,dd);
                        TotalStep += 1;
                        break;
                case 2:  //right
                    this.Left = this.Left + ww;
                    SetA(r, c + 1, GetA(r, c,dd),dd);
                    SetA(r, c, 0,dd);
                        TotalStep += 1;
                        break;
                case 3:  //down
                    this.Top = this.Top + ww;
                    SetA(r + 1, c, GetA(r, c,dd),dd);
                    SetA(r, c, 0,dd);
                        TotalStep += 1;
                        break;
            }

                if (Success(dd))
                {
                    MessageBox.Show("拼图成功！!！\n"+"总计："+TotalStep.ToString()+"步","恭喜！");
                          //把最后一块补上去
                    mButton[dd * dd - 1].Location = new Point(600 - ww, 600 - ww);
                    SetA(dd - 1, dd - 1, dd * dd, dd);
                }

            }

        }
    

        private void ShowPic()    //程序一开始，显示完整的图像
        {
            string DefaultPath = Application.StartupPath + @"\images\";
            DirectoryInfo root = new DirectoryInfo(DefaultPath);
            FileInfo[] rf = root.GetFiles();
            Random rd = new Random();
            int m = rd.Next(rf.Length) ;
            FileInfo f = rf[m];                    //随机挑一张图片
            FName = f.FullName;
          
            Image img = Image.FromFile(FName);
            PictureBox p = new PictureBox();
            p.Size = new Size(600, 600);
            p.Location = new Point(0, 0);
            p.Image = img;
            this.Controls.Add(p);

        }
    //初始化棋盘
   

    private void MakeBox(string fn,int N)     //产生盒子,N=3,5,8
        {
            mButton = new AButton[N * N];
            int i,r,c;
            int w;
            Image img;
                       
            this.Controls.Clear();   //移除之前的所有控件
           
            w = 600 / N;

            try
            {
                img = Image.FromFile(fn);
                for (i = 0; i <N*N; i++)
                {
                    r = i / N; c = i % N;
                    mButton[i] = new AButton(i+1, N, CaptureImage(img, w, w, c * w, r * w));                         
                    this.Controls.Add(mButton[i]);
                }
            }
            catch (Exception e)
            {
                if (MessageBox.Show(e.ToString() + "\n   错误!    是否继续(Y/N)?", "错误", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    Application.Exit();
                }
            }
         }

        public void InitA(int n)   //初始化数组 和 图片 最后一个块必须空掉。数组置为0
        {
            A = new int[n, n];
            int[] b = new int[n*n];      //随机化的一维数组。
            Random   rand = new Random();
            int i, j,k;
            int r, c;
            bool TF;
            
            //以下数字表示了图片的索引号。也是在棋盘中的位置。

            r = 0;c = 0;
           //生成不重复的随机数  1~n*n
            b[0] = rand.Next(n*n) + 1;   
            i = 1;
            while (i<n*n)            
            {
                do
                {
                    k = rand.Next(n*n) + 1;
                    j = 0;TF = false;
                    while (j < i & b[j] != k) j++;
                    if (j == i) TF = true;
                 } while (!TF);

                b[i] = k;
                i = i + 1;
            }

            for (i = 0; i < n; i++)
                for (j = 0; j < n; j++)
                {
                    A[i, j] = b[i * n + j ];
                    if (A[i, j] == n*n)         //记下最后一个块的位置
                    {
                        r = i;
                        c = j;
                    }
                }

            //初始化AButton的位置

            for (i = 0; i < n; i++)
                for (j = 0; j < n; j++)
                    mButton[A[i, j] - 1].Location=new Point( j * 600/n,i*600/n);

            mButton[n * n - 1].Location = new Point(6000, 6000);   //把最后一个块甩得远远地！：）
            
            SetA(r, c, 0,n);             //最后一个块的位置清0

            TotalStep = 0;
        }

        public static void SetA(int i, int j, int n,int d)  //行,列,值,维度
        {
            i = i % d;
            j = j % d;
            A[i, j] = n;
        }

        public static int GetA(int i, int j,int d)  //行,列,维度
        {
            if (i > d-1 || i < 0) return 999;
            if (j > d-1 || j < 0) return 999;
            return A[i, j];
        }
        
        public static int Direction(int i, int j,int d)  //返回值-1，0，1，2，3 =不能移动，左，上，右，下 有空位。i 行号，j 列号
        {
            if (GetA(i - 1, j,d) == 0) return 1;
            if (GetA(i + 1, j,d) == 0) return 3;
            if (GetA(i, j - 1,d) == 0) return 0;
            if (GetA(i, j + 1,d) == 0) return 2;
            return -1;
        }
                     
        public static bool Success(int d)   //判断是否成功,d维度
        {
            bool TF;
            int i, m,n;

            TF = true;      i = 0;

            while(i<d*d-1)
            {
                m = i / d;n = i % d;
                if (A[m, n] != (i + 1))
                { 
                    TF = false;
                    break;                
                }
                i = i + 1;
            }

            return TF;
        }

        public Form1()
		{
            //
            // Windows 窗体设计器支持所必需的
            //                     
            
            InitializeComponent();

            //固定窗口大小
            this.Size = new Size(605, 635);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.Gray;

            ShowPic();  //先显示完整的
        }

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem3,
            this.menuItem1});
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 0;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem4,
            this.menuItem7,
            this.menuItem8,
            this.menuItem5});
            this.menuItem3.Text = "开始游戏";
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 0;
            this.menuItem4.Text = "简单3*3";
            this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 1;
            this.menuItem7.Text = "中等4*4";
            this.menuItem7.Click += new System.EventHandler(this.menuItem7_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 2;
            this.menuItem8.Text = "复杂5*5";
            this.menuItem8.Click += new System.EventHandler(this.menuItem8_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 3;
            this.menuItem5.Text = "退出";
            this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 1;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem2,
            this.menuItem6});
            this.menuItem1.Text = "帮助";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 0;
            this.menuItem2.Text = "About PinTu";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 1;
            this.menuItem6.Text = "Help";
            this.menuItem6.Click += new System.EventHandler(this.menuItem6_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(856, 307);
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "拼图游戏2020";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

		}
        #endregion
        
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
		static void Main() 
		{
            Application.Run(new Form1());
		}
                        		 
		private void menuItem2_Click(object sender, System.EventArgs e)
		{
		   MessageBox.Show("JC拼图游戏，武汉2020\n COPYRIGHT(c)JCSOLUTIONS", "Help",MessageBoxButtons.OK,MessageBoxIcon.Information);
		}

		private void menuItem5_Click(object sender, System.EventArgs e)
		{
            System.Environment.Exit(0);
		}

		
		private void menuItem6_Click(object sender, System.EventArgs e)
		{
            MessageBox.Show(" 鼠标点击图块使之向空白处移动，小提示：）先拼成第一行!\n 放入images目录的图片必须是600*600像素的尺寸，否则出错。","帮助");
		}

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public Image CaptureImage(Image fromImage, int width, int height, int spaceX, int spaceY)
        {
           /* int x = 0;
            int y = 0;
            int sX = fromImage.Width - width;
            int sY = fromImage.Height - height;
            if (sX > 0)
            {
                x = sX > spaceX ? spaceX : sX;
            }
            else
            {
                width = fromImage.Width;
            }
            if (sY > 0)
            {
                y = sY > spaceY ? spaceY : sY;
            }
            else
            {
                height = fromImage.Height;
            }
            */   
            //若图形满足600*600以上,则以上的代码不需要。

            //创建新图位图 
            Bitmap bitmap = new Bitmap(width, height);
            //创建作图区域 
            Graphics graphic = Graphics.FromImage(bitmap);
            //截取原图相应区域写入作图区 
            graphic.DrawImage(fromImage, 0,0,new Rectangle(spaceX, spaceY, width, height), GraphicsUnit.Pixel);
            //从作图区生成新图 
            Image saveImage = Image.FromHbitmap(bitmap.GetHbitmap());
            return saveImage;
        }

     

        private void menuItem4_Click(object sender, EventArgs e)
        {
            
            MakeBox(FName, 3);
            InitA(3);
        }

        private void menuItem7_Click(object sender, EventArgs e)
        {            
            MakeBox(FName, 4);
            InitA(4);
        }

        private void menuItem8_Click(object sender, EventArgs e)
        {
            MakeBox(FName, 5);
            InitA(5);
        }
                                 
    }
    

}
	
	
