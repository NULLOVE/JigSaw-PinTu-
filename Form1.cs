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
    /// Form1 ��ժҪ˵����
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

        private static int[,] A;    //��������

        private static AButton[] mButton;      //ͼƬ����
       
        private string FName;              //��Ҫ�ڹ����д��ݵ�ͼƬ���ơ�

        private static int TotalStep;
        //�ؼ��� ����ķ���
    public class AButton : PictureBox    //�̳���PictureBox
    {
        private int Pn,W,Num;    //ͼƬ��˳��ţ������ң����ϵ��£�����ȣ����أ���N*N�ľ���
        public AButton(int p,int n,Image img)
        {     //�ڻ��е�˳��ţ�ά�ȣ�ͼ��
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
            int r, c, d;  //�У���   200*200����
            int dd=this.Num;    //ά��;
            int ww = this.W;  //���
            r= this.Top /ww;     //��Ӧ�����е�������
            c = this.Left /ww;     //��Ӧ�����е�������
            d = Direction(r, c,dd);    //ȷ�������ĸ������ƶ�
            switch (d)
            {
                case 0:   //left
                    this.Left = this.Left - ww;    //�����ƶ�
                    SetA(r, c - 1, GetA(r, c,dd),dd);      //��������
                    SetA(r, c, 0,dd);                        //��������
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
                    MessageBox.Show("ƴͼ�ɹ���!��\n"+"�ܼƣ�"+TotalStep.ToString()+"��","��ϲ��");
                          //�����һ�鲹��ȥ
                    mButton[dd * dd - 1].Location = new Point(600 - ww, 600 - ww);
                    SetA(dd - 1, dd - 1, dd * dd, dd);
                }

            }

        }
    

        private void ShowPic()    //����һ��ʼ����ʾ������ͼ��
        {
            string DefaultPath = Application.StartupPath + @"\images\";
            DirectoryInfo root = new DirectoryInfo(DefaultPath);
            FileInfo[] rf = root.GetFiles();
            Random rd = new Random();
            int m = rd.Next(rf.Length) ;
            FileInfo f = rf[m];                    //�����һ��ͼƬ
            FName = f.FullName;
          
            Image img = Image.FromFile(FName);
            PictureBox p = new PictureBox();
            p.Size = new Size(600, 600);
            p.Location = new Point(0, 0);
            p.Image = img;
            this.Controls.Add(p);

        }
    //��ʼ������
   

    private void MakeBox(string fn,int N)     //��������,N=3,5,8
        {
            mButton = new AButton[N * N];
            int i,r,c;
            int w;
            Image img;
                       
            this.Controls.Clear();   //�Ƴ�֮ǰ�����пؼ�
           
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
                if (MessageBox.Show(e.ToString() + "\n   ����!    �Ƿ����(Y/N)?", "����", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    Application.Exit();
                }
            }
         }

        public void InitA(int n)   //��ʼ������ �� ͼƬ ���һ�������յ���������Ϊ0
        {
            A = new int[n, n];
            int[] b = new int[n*n];      //�������һά���顣
            Random   rand = new Random();
            int i, j,k;
            int r, c;
            bool TF;
            
            //�������ֱ�ʾ��ͼƬ�������š�Ҳ���������е�λ�á�

            r = 0;c = 0;
           //���ɲ��ظ��������  1~n*n
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
                    if (A[i, j] == n*n)         //�������һ�����λ��
                    {
                        r = i;
                        c = j;
                    }
                }

            //��ʼ��AButton��λ��

            for (i = 0; i < n; i++)
                for (j = 0; j < n; j++)
                    mButton[A[i, j] - 1].Location=new Point( j * 600/n,i*600/n);

            mButton[n * n - 1].Location = new Point(6000, 6000);   //�����һ����˦��ԶԶ�أ�����
            
            SetA(r, c, 0,n);             //���һ�����λ����0

            TotalStep = 0;
        }

        public static void SetA(int i, int j, int n,int d)  //��,��,ֵ,ά��
        {
            i = i % d;
            j = j % d;
            A[i, j] = n;
        }

        public static int GetA(int i, int j,int d)  //��,��,ά��
        {
            if (i > d-1 || i < 0) return 999;
            if (j > d-1 || j < 0) return 999;
            return A[i, j];
        }
        
        public static int Direction(int i, int j,int d)  //����ֵ-1��0��1��2��3 =�����ƶ������ϣ��ң��� �п�λ��i �кţ�j �к�
        {
            if (GetA(i - 1, j,d) == 0) return 1;
            if (GetA(i + 1, j,d) == 0) return 3;
            if (GetA(i, j - 1,d) == 0) return 0;
            if (GetA(i, j + 1,d) == 0) return 2;
            return -1;
        }
                     
        public static bool Success(int d)   //�ж��Ƿ�ɹ�,dά��
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
            // Windows ���������֧���������
            //                     
            
            InitializeComponent();

            //�̶����ڴ�С
            this.Size = new Size(605, 635);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.Gray;

            ShowPic();  //����ʾ������
        }

		/// <summary>
		/// ������������ʹ�õ���Դ��
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

		#region Windows ������������ɵĴ���
		/// <summary>
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
		/// �˷��������ݡ�
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
            this.menuItem3.Text = "��ʼ��Ϸ";
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 0;
            this.menuItem4.Text = "��3*3";
            this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 1;
            this.menuItem7.Text = "�е�4*4";
            this.menuItem7.Click += new System.EventHandler(this.menuItem7_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 2;
            this.menuItem8.Text = "����5*5";
            this.menuItem8.Click += new System.EventHandler(this.menuItem8_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 3;
            this.menuItem5.Text = "�˳�";
            this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 1;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem2,
            this.menuItem6});
            this.menuItem1.Text = "����";
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
            this.Text = "ƴͼ��Ϸ2020";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

		}
        #endregion
        
        /// <summary>
        /// Ӧ�ó��������ڵ㡣
        /// </summary>
        [STAThread]
		static void Main() 
		{
            Application.Run(new Form1());
		}
                        		 
		private void menuItem2_Click(object sender, System.EventArgs e)
		{
		   MessageBox.Show("JCƴͼ��Ϸ���人2020\n COPYRIGHT(c)JCSOLUTIONS", "Help",MessageBoxButtons.OK,MessageBoxIcon.Information);
		}

		private void menuItem5_Click(object sender, System.EventArgs e)
		{
            System.Environment.Exit(0);
		}

		
		private void menuItem6_Click(object sender, System.EventArgs e)
		{
            MessageBox.Show(" �����ͼ��ʹ֮��հ״��ƶ���С��ʾ������ƴ�ɵ�һ��!\n ����imagesĿ¼��ͼƬ������600*600���صĳߴ磬�������","����");
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
            //��ͼ������600*600����,�����ϵĴ��벻��Ҫ��

            //������ͼλͼ 
            Bitmap bitmap = new Bitmap(width, height);
            //������ͼ���� 
            Graphics graphic = Graphics.FromImage(bitmap);
            //��ȡԭͼ��Ӧ����д����ͼ�� 
            graphic.DrawImage(fromImage, 0,0,new Rectangle(spaceX, spaceY, width, height), GraphicsUnit.Pixel);
            //����ͼ��������ͼ 
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
	
	
